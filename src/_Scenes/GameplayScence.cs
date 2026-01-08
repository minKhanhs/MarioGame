using MarioGame.src._Core;
using MarioGame.src._Core.Camera;
using MarioGame.src._Data;
using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.enemies;
using MarioGame.src._Entities.Enviroments;
using MarioGame.src._Entities.items;
using MarioGame.src._Entities.player;
using MarioGame.src._Entities.player.states;
using MarioGame.src._Scenes;
using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class GameplayScene : IScene
    {
        private int _levelIndex;
        private List<GameObj> _gameObjects;
        private Player _player;
        private Camera _camera;
        private Texture2D _backgroundTex;
        private GameHUD _hud;
        private int _previousPlayerLives = 3; // Track previous frame's lives to detect death

        // Trạng thái thắng thua
        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;

        // Quản lý pause
        private bool _isPaused = false;
        private KeyboardState _previousKeyboardState;
        private bool _isResumingFromPause = false;

        // Kiểm tra xem đã load content chưa
        private bool _isContentLoaded = false;


        private float _bulletSpawnTimer = 0f;
        private float _bulletSpawnInterval = 5.0f; // Cứ 5 giây bắn 1 quả (bạn có thể chỉnh)
        private Random _random = new Random();
        public GameplayScene(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void LoadContent()
        {
            // Tránh load content nhiều lần
            if (_isContentLoaded)
                return;

            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;

            _camera = new Camera(device.Viewport);
            _backgroundTex = content.Load<Texture2D>("sprites/background");

            // Load HUD font
            SpriteFont hudFont = null;
            try
            {
                hudFont = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch { }

            _hud = new GameHUD(hudFont);

            // --- LOAD TEXTURES & MAP ---
            var textures = new Dictionary<string, Texture2D>();
            Texture2D marioSheet = content.Load<Texture2D>("sprites/mario");

            var playerAnims = new Dictionary<string, SpriteAnimation>();
            playerAnims.Add("Run", new SpriteAnimation(marioSheet, 4, 36, 52, rowIndex: 0));
            playerAnims.Add("Idle", new SpriteAnimation(marioSheet, 1, 36, 52, rowIndex: 0));
            playerAnims.Add("Jump", new SpriteAnimation(marioSheet, 1, 36, 52, rowIndex: 0));

            textures.Add("mario", marioSheet);
            textures.Add("castle", content.Load<Texture2D>("sprites/castle"));
            textures.Add("ground", content.Load<Texture2D>("sprites/groundTile"));
            textures.Add("brick", content.Load<Texture2D>("sprites/blockbreak"));
            textures.Add("coin", content.Load<Texture2D>("sprites/coin"));
            textures.Add("goomba", content.Load<Texture2D>("sprites/goomba"));
            textures.Add("mushroom", content.Load<Texture2D>("sprites/mushroom"));
            textures.Add("pipe", content.Load<Texture2D>("sprites/pipe"));
            textures.Add("plant", content.Load<Texture2D>("sprites/plant"));
            textures.Add("koopa", content.Load<Texture2D>("sprites/koopa"));
            textures.Add("bullet", content.Load<Texture2D>("sprites/bullet"));

            MapLoader.Initialize(textures);

            // Check if we're resuming from pause
            GameState savedState = GameManager.Instance.GetSavedGameState();
            if (savedState.IsValid && savedState.LevelIndex == _levelIndex)
            {
                // RESTORE từ saved state (PAUSE RESUME)
                RestoreGameState(savedState, playerAnims);
                _isResumingFromPause = true;
                GameManager.Instance.ClearSavedGameState();
            }
            else
            {
                // LOAD level bình thường (NEW GAME or NEXT LEVEL)
                // If loading level 1, reset GameSession for new game
                if (_levelIndex == 1)
                {
                    GameSession.Instance.ResetSession();
                }
                LoadLevelFromFile(playerAnims);
            }

            _isContentLoaded = true;
        }

        private void RestoreGameState(GameState savedState, Dictionary<string, SpriteAnimation> playerAnims)
        {
            // Restore game objects
            _gameObjects = new List<GameObj>(savedState.GameObjects);

            // Restore player with PlayerIndex = 1 for single-player
            _player = new Player(savedState.PlayerPosition, playerAnims, 1);
            _player.Velocity = savedState.PlayerVelocity;
            _player.Lives = savedState.PlayerLives;
            _player.Coins = savedState.PlayerCoins;
            _player.Score = savedState.PlayerScore;
            _player.Scale = savedState.PlayerScale;
            _previousPlayerLives = _player.Lives;

            // Restore player state based on scale
            if (_player.Scale > 1.2f)
            {
                _player.SetState(new BigState());
            }
            else
            {
                _player.SetState(new SmallState());
            }

            // Restore HUD with all state
            _hud.LivesRemaining = savedState.PlayerLives;
            _hud.CoinsCollected = savedState.PlayerCoins;
            _hud.ElapsedTime = savedState.ElapsedTime;
            _hud.EnemiesDefeated = savedState.EnemiesDefeated;
            _hud.MushroomsCollected = savedState.MushroomsCollected;
            _hud.DeathCount = savedState.DeathCount;

            System.Diagnostics.Debug.WriteLine($"[RESUME] Restored player at {_player.Position}, Lives: {_player.Lives}, Scale: {_player.Scale}");
        }

        private void LoadLevelFromFile(Dictionary<string, SpriteAnimation> playerAnims)
        {
            string levelPath = $"Content/levels/level{_levelIndex}.json";

            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);
                // Single-player uses PlayerIndex = 1
                _player = new Player(new Vector2(50, 200), playerAnims, 1);
                _previousPlayerLives = _player.Lives;
                _hud.Reset();
                System.Diagnostics.Debug.WriteLine($"[NEW GAME] Loaded level {_levelIndex}");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load level {_levelIndex}");
                GameManager.Instance.ChangeScene(new MenuScene());
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update HUD
            _hud.Update(gameTime);

            // Skip first frame after resume to prevent ESC collision
            if (_isResumingFromPause)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                _previousKeyboardState = currentKeyboardState;
                _isResumingFromPause = false;
                return;
            }

            if (_player.Position.X > 2
        && MapLoader.CurrentLevelConfig != null
        && MapLoader.CurrentLevelConfig.HasBulletBill) // <--- KIỂM TRA TẠI ĐÂY
            {
                _bulletSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_bulletSpawnTimer > _bulletSpawnInterval)
                {
                    _bulletSpawnTimer = 0f;
                    SpawnBulletBill();
                    _bulletSpawnInterval = _random.Next(3, 7);
                }
            }

            KeyboardState currentKbState = Keyboard.GetState();

            // Check for pause input (ESC key)
            if (currentKbState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                System.Diagnostics.Debug.WriteLine("[PAUSE] Game paused");
                PauseGame();
                _previousKeyboardState = currentKbState;
                return;
            }

            _previousKeyboardState = currentKbState;

            // Don't update if paused
            if (_isPaused)
                return;

            // Check level completion
            if (_isLevelFinished)
            {
                _finishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_finishTimer > 2.0f)
                {
                    _isContentLoaded = false; // Reset flag for next level
                    System.Diagnostics.Debug.WriteLine($"[LEVEL COMPLETE] Level {_levelIndex}, Score: {_hud.CurrentScore}, Coins: {_hud.CoinsCollected}, Time: {_hud.ElapsedTime:F1}s, Enemies: {_hud.EnemiesDefeated}");
                    // Show level complete scene with HUD score (not player.Score)
                    int bonusScore = _hud.CalculateLevelBonus();
                    GameManager.Instance.ChangeScene(new LevelCompleteScene(_levelIndex, 3, _hud.CurrentScore, _hud.CoinsCollected, bonusScore, _hud.EnemiesDefeated, _hud.ElapsedTime, _hud.MushroomsCollected, _hud.DeathCount));
                }
                return;
            }

            // Update game logic
            _player.Update(gameTime);
            _player.IsOnGround = false;

            // Check if player died
            if (_player.Lives < _previousPlayerLives)
            {
                _hud.DeathCount++;
                _previousPlayerLives = _player.Lives;
            }
            else if (_player.Lives == _previousPlayerLives)
            {
                _previousPlayerLives = _player.Lives;
            }

            // Check game over
            if (_player.Lives <= 0 || _player.Position.Y > 1000)
            {
                System.Diagnostics.Debug.WriteLine($"[GAME OVER] Level {_levelIndex}, Score: {_player.Score}, Coins: {_player.Coins}");
                _isContentLoaded = false; // Reset flag for retry
                
                // DON'T update session stats here - will be handled by GameOverScene if needed
                // Just pass the current level stats
                GameManager.Instance.ChangeScene(new GameOverScene(_levelIndex, _hud.CurrentScore, _hud.CoinsCollected, _hud.EnemiesDefeated));
                return;
            }

            // Update all game objects
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var obj = _gameObjects[i];
                obj.Update(gameTime);

                // Enemy collision with blocks
                if (obj is MovableObj movableObj && !(obj is Player) && !(obj is PiranhaPlant) && !(obj is BulletBill))
                {
                    foreach (var other in _gameObjects)
                    {
                        // QUAN TRỌNG: Kiểm tra va chạm với cả Block và Pipe
                        if (other != obj)
                        {
                            if (other is Block || other is Pipe)
                            {
                                Collision.ResolveStaticCollision(movableObj, other);
                            }
                        }
                    }
                }

                if (obj.IsActive)
                {
                    // Player collision with blocks
                    if (obj is Block || obj is Pipe)
                        Collision.ResolveStaticCollision(_player, obj);
                    // Item collection
                    else if (obj is Item item && _player.Bounds.Intersects(item.Bounds))
                    {
                        if (item is Mushroom)
                        {
                            _hud.MushroomsCollected++;
                        }
                        item.OnCollect(_player);
                    }
                    // Enemy interaction
                    else if (obj is Enemy enemy && _player.Bounds.Intersects(enemy.Bounds))
                    {
                        if (enemy is PiranhaPlant)
                        {
                            _player.TakeDamage();
                        }
                        else if (Collision.IsTopCollision(_player, enemy))
                        {
                            enemy.OnStomped(); // Goomba thì chết
                            _player.Velocity.Y = -5f;
                        }
                        else
                        {
                            _player.TakeDamage();
                        }
                    }
                    // Level completion
                    else if (obj is Castle && _player.Bounds.Intersects(obj.Bounds))
                    {
                        System.Diagnostics.Debug.WriteLine("LEVEL COMPLETE!");
                        _isLevelFinished = true;
                    }
                }

                if (!obj.IsActive)
                    _gameObjects.RemoveAt(i);
            }

            // Update HUD with current player stats (coins only - score is calculated by HUD)
            _hud.LivesRemaining = _player.Lives;
            _hud.CoinsCollected = _player.Coins;
            // Don't override CurrentScore - let HUD calculate it based on formula

            // Update camera
            Rectangle mapBounds = new Rectangle(0, 0, 3200, 736);
            _camera.Update(_player.Position, mapBounds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (_backgroundTex != null)
                spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();

            // Draw game world
            spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);
            foreach (var obj in _gameObjects)
                if (_camera.IsVisible(obj.Bounds))
                    obj.Draw(spriteBatch);
            _player.Draw(spriteBatch);
            spriteBatch.End();

            // Draw HUD on top
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void PauseGame()
        {
            _isPaused = true;

            // Save complete game state including HUD
            GameState state = new GameState
            {
                LevelIndex = _levelIndex,
                PlayerPosition = _player.Position,
                PlayerVelocity = _player.Velocity,
                PlayerLives = _player.Lives,
                PlayerCoins = _player.Coins,
                PlayerScore = _player.Score,
                PlayerScale = _player.Scale,
                GameObjects = new List<GameObj>(_gameObjects),
                // Save HUD state
                ElapsedTime = _hud.ElapsedTime,
                EnemiesDefeated = _hud.EnemiesDefeated,
                MushroomsCollected = _hud.MushroomsCollected,
                DeathCount = _hud.DeathCount,
                IsValid = true
            };

            System.Diagnostics.Debug.WriteLine($"[PAUSE] Saved state - Player at {_player.Position}, Lives: {_player.Lives}, Scale: {_player.Scale}, Time: {_hud.ElapsedTime:F1}s");

            GameManager.Instance.SaveGameState(state);
            GameManager.Instance.ChangeScene(new PauseScene(_levelIndex));
        }
        private void SpawnBulletBill()
        {
            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;

            // Load lại texture từ Content (hoặc lấy từ Dictionary textures nếu bạn lưu global)
            // Tốt nhất là lưu texture vào biến _bulletTex trong LoadContent để dùng lại cho đỡ lag
            Texture2D bulletTex = content.Load<Texture2D>("sprites/bullet");

            // Tạo đối tượng Bullet Bill mới
            BulletBill bill = new BulletBill(bulletTex, _camera, device);

            // Tính độ cao ngẫu nhiên (tránh quá thấp hoặc quá cao)
            // Map cao 736. Random từ 300 đến 600.
            float randomY = _random.Next(450, 650);

            bill.Spawn(randomY);

            // Thêm vào danh sách GameObjects
            _gameObjects.Add(bill);
        }
    }
}