using MarioGame.src._Core;
using MarioGame.src._Core.Camera;
using MarioGame.src._Data;
using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.enemies;
using MarioGame.src._Entities.Enviroments;
using MarioGame.src._Entities.items;
using MarioGame.src._Entities.player;
using MarioGame.src._Input;
using MarioGame.src._Scenes;
using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class TwoPlayerGameplayScene : IScene
    {
        private int _levelIndex;
        private List<GameObj> _gameObjects;

        // Vẫn giữ biến riêng lẻ theo ý bạn
        private Player _player1;
        private Player _player2;

        private Camera _camera;
        private Texture2D _backgroundTex;
        private GameHUD _hud;
        private SpriteFont _playerLabelFont;

        private int _previousPlayer1Lives = 3;
        private int _previousPlayer2Lives = 3;

        // --- CÁC BIẾN MỚI CHO TÍNH NĂNG ---
        private Texture2D _fireballTexture;
        private float _shootCooldown = 0f;
        private float _bulletSpawnTimer = 0f;
        private float _bulletSpawnInterval = 5.0f;
        private Random _random = new Random();
        // -----------------------------------

        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;
        private bool _isPaused = false;
        private KeyboardState _previousKeyboardState;
        private bool _isResumingFromPause = false;
        private bool _isContentLoaded = false;

        public TwoPlayerGameplayScene(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void LoadContent()
        {
            if (_isContentLoaded) return;

            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;

            _camera = new Camera(device.Viewport);
            _backgroundTex = content.Load<Texture2D>("sprites/background");

            // Load thêm texture đạn
            _fireballTexture = content.Load<Texture2D>("sprites/fireball");

            SpriteFont hudFont = null;
            try
            {
                hudFont = content.Load<SpriteFont>("fonts/GameFont");
                _playerLabelFont = hudFont;
            }
            catch { }
            _hud = new GameHUD(hudFont);

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
            textures.Add("bullet", content.Load<Texture2D>("sprites/bullet")); // Load BulletBill
            textures.Add("mystery", content.Load<Texture2D>("sprites/present")); // Load Mystery

            MapLoader.Initialize(textures);

            LoadLevelFromFile(playerAnims);

            _isContentLoaded = true;
        }

        private void LoadLevelFromFile(Dictionary<string, SpriteAnimation> playerAnims)
        {
            string levelPath = $"Content/levels/level{_levelIndex}.json";
            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);

                // --- CÀI ĐẶT CAMERA (AutoScroll hoặc Follow) ---
                if (MapLoader.CurrentLevelConfig.IsAutoScroll)
                {
                    var autoScroll = new AutoScrollStrategy();
                    autoScroll.ScrollSpeed = MapLoader.CurrentLevelConfig.ScrollSpeed;
                    _camera.SetStrategy(autoScroll);
                }
                else
                {
                    // Mặc định bám theo mục tiêu (sẽ set target là trung bình cộng ở Update)
                    _camera.SetStrategy(new FollowTargetStrategy());
                }

                _player1 = new Player(new Vector2(50, 200), playerAnims, 1);
                _player2 = new Player(new Vector2(100, 200), playerAnims, 2);

                _previousPlayer1Lives = _player1.Lives;
                _previousPlayer2Lives = _player2.Lives;

                _hud.Reset();
                System.Diagnostics.Debug.WriteLine($"[TWO PLAYER] Loaded level {_levelIndex}");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load level {_levelIndex}");
                GameManager.Instance.ChangeScene(new MenuScene());
            }
        }

        public void Update(GameTime gameTime)
        {
            _hud.Update(gameTime);

            if (_isResumingFromPause)
            {
                _previousKeyboardState = Keyboard.GetState();
                _isResumingFromPause = false;
                return;
            }

            // --- 1. LOGIC BULLET BILL ---
            // Kiểm tra nếu P1 hoặc P2 đi đủ xa
            if ((_player1.Position.X > 2 || _player2.Position.X > 2)
                && MapLoader.CurrentLevelConfig != null
                && MapLoader.CurrentLevelConfig.HasBulletBill)
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
            _shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // --- 2. XỬ LÝ BẮN ĐẠN CHO P1 VÀ P2 ---
            // Chỉ cho phép bắn nếu map config cho phép và hết cooldown
            if (_shootCooldown <= 0 && MapLoader.CurrentLevelConfig != null && MapLoader.CurrentLevelConfig.CanShoot)
            {
                // Check P1 Fire
                Keys p1Attack = InputSettings.Instance.P1_KeyMap[EGameAction.Attack];
                if (currentKbState.IsKeyDown(p1Attack))
                {
                    ShootFireball(_player1);
                    _shootCooldown = 0.2f;
                }
                // Check P2 Fire
                Keys p2Attack = InputSettings.Instance.P2_KeyMap[EGameAction.Attack];
                if (currentKbState.IsKeyDown(p2Attack))
                {
                    ShootFireball(_player2);
                    _shootCooldown = 0.2f;
                }
            }

            // Pause Logic
            if (currentKbState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                PauseGame();
                _previousKeyboardState = currentKbState;
                return;
            }
            _previousKeyboardState = currentKbState;

            if (_isPaused) return;

            // Level Complete Logic
            if (_isLevelFinished)
            {
                _finishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_finishTimer > 2.0f)
                {
                    _isContentLoaded = false;
                    int bonusScore = _hud.CalculateLevelBonus();
                    GameManager.Instance.ChangeScene(new LevelCompleteScene(_levelIndex, 3, _hud.CurrentScore, _hud.CoinsCollected, bonusScore, _hud.EnemiesDefeated, _hud.ElapsedTime, _hud.MushroomsCollected, _hud.DeathCount));
                }
                return;
            }

            // Update Players
            _player1.Update(gameTime);
            _player1.IsOnGround = false;

            _player2.Update(gameTime);
            _player2.IsOnGround = false;

            // Check Lives Logic
            if (_player1.Lives < _previousPlayer1Lives) { _hud.DeathCount++; _previousPlayer1Lives = _player1.Lives; }
            else if (_player1.Lives == _previousPlayer1Lives) { _previousPlayer1Lives = _player1.Lives; }

            if (_player2.Lives < _previousPlayer2Lives) { _hud.DeathCount++; _previousPlayer2Lives = _player2.Lives; }
            else if (_player2.Lives == _previousPlayer2Lives) { _previousPlayer2Lives = _player2.Lives; }

            // --- LOGIC CHẾT KHI AUTO SCROLL ---
            float camLeft = _camera.Position.X;
            if (MapLoader.CurrentLevelConfig != null && MapLoader.CurrentLevelConfig.IsAutoScroll)
            {
                if (_player1.Position.X < camLeft - 32) _player1.Die();
                if (_player2.Position.X < camLeft - 32) _player2.Die();
            }

            // Game Over Check
            if (_player1.Lives <= 0 || _player1.Position.Y > 1000 || _player2.Lives <= 0 || _player2.Position.Y > 1000)
            {
                string deathReason = "";
                if (_player1.Lives <= 0) deathReason = "Player 1 died";
                else if (_player2.Lives <= 0) deathReason = "Player 2 died";
                else deathReason = "Player fell";

                GameManager.Instance.ChangeScene(new TwoPlayerGameOverScene(_levelIndex, _hud.CurrentScore, _hud.CoinsCollected, _hud.EnemiesDefeated, deathReason));
                return;
            }

            // --- VÒNG LẶP UPDATE GAME OBJECTS & VA CHẠM ---
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var obj = _gameObjects[i];
                obj.Update(gameTime);

                // 1. Quái/Item va chạm môi trường
                if (obj is MovableObj movableObj && !(obj is Player) && !(obj is PiranhaPlant) && !(obj is BulletBill) && !(obj is Fireball))
                {
                    foreach (var other in _gameObjects)
                        if (other != obj && (other is Block || other is Pipe || other is Castle))
                            Collision.ResolveStaticCollision(movableObj, other);
                }

                if (obj.IsActive)
                {
                    // ------------------------------------
                    // XỬ LÝ VA CHẠM CHO PLAYER 1 (P1)
                    // ------------------------------------
                    // Mystery Block
                    if (obj is MysteryBlock mystery)
                    {
                        if (_player1.Bounds.Intersects(mystery.Bounds))
                        {
                            bool isHeadHit = _player1.Velocity.Y < 0 && _player1.Bounds.Top > mystery.Bounds.Bottom - 10;
                            if (isHeadHit)
                            {
                                _player1.Velocity.Y = 0;
                                var content = GameManager.Instance.Content;
                                Item spawned = mystery.SpawnItem(content.Load<Texture2D>("sprites/coin"), content.Load<Texture2D>("sprites/mushroom"));
                                if (spawned != null) _gameObjects.Add(spawned);
                            }
                            Collision.ResolveStaticCollision(_player1, obj);
                        }
                    }
                    // Block/Pipe
                    else if (obj is Block || obj is Pipe)
                    {
                        Collision.ResolveStaticCollision(_player1, obj);
                    }
                    // Item
                    else if (obj is Item item && _player1.Bounds.Intersects(item.Bounds))
                    {
                        if (item is Mushroom) _hud.MushroomsCollected++;
                        item.OnCollect(_player1);
                    }
                    // Enemy
                    else if (obj is Enemy enemy && _player1.Bounds.Intersects(enemy.Bounds))
                    {
                        if (enemy is PiranhaPlant) _player1.TakeDamage();
                        else if (Collision.IsTopCollision(_player1, enemy)) { enemy.OnStomped(); _player1.Velocity.Y = -5f; _hud.EnemiesDefeated++; }
                        else _player1.TakeDamage();
                    }

                    // ------------------------------------
                    // XỬ LÝ VA CHẠM CHO PLAYER 2 (P2)
                    // ------------------------------------
                    // Mystery Block P2
                    if (obj is MysteryBlock mystery2)
                    {
                        if (_player2.Bounds.Intersects(mystery2.Bounds))
                        {
                            bool isHeadHit = _player2.Velocity.Y < 0 && _player2.Bounds.Top > mystery2.Bounds.Bottom - 10;
                            if (isHeadHit)
                            {
                                _player2.Velocity.Y = 0;
                                var content = GameManager.Instance.Content;
                                Item spawned = mystery2.SpawnItem(content.Load<Texture2D>("sprites/coin"), content.Load<Texture2D>("sprites/mushroom"));
                                if (spawned != null) _gameObjects.Add(spawned);
                            }
                            Collision.ResolveStaticCollision(_player2, obj);
                        }
                    }
                    else if (obj is Block || obj is Pipe) // Block check cho P2
                    {
                        Collision.ResolveStaticCollision(_player2, obj);
                    }
                    else if (obj is Item item2 && _player2.Bounds.Intersects(item2.Bounds))
                    {
                        if (item2 is Mushroom) _hud.MushroomsCollected++;
                        item2.OnCollect(_player2);
                    }
                    else if (obj is Enemy enemy2 && _player2.Bounds.Intersects(enemy2.Bounds))
                    {
                        if (enemy2 is PiranhaPlant) _player2.TakeDamage();
                        else if (Collision.IsTopCollision(_player2, enemy2)) { enemy2.OnStomped(); _player2.Velocity.Y = -5f; _hud.EnemiesDefeated++; }
                        else _player2.TakeDamage();
                    }

                    // ------------------------------------
                    // LOGIC CHUNG (Castle & Fireball)
                    // ------------------------------------
                    if (obj is Castle)
                    {
                        if (_player1.Bounds.Intersects(obj.Bounds)) _player1.HasReachedGoal = true;
                        if (_player2.Bounds.Intersects(obj.Bounds)) _player2.HasReachedGoal = true;
                        if (_player1.HasReachedGoal && _player2.HasReachedGoal) _isLevelFinished = true;
                    }

                    // Logic Đạn (Fireball)
                    if (obj is Fireball fireball)
                    {
                        foreach (var target in _gameObjects)
                        {
                            if (target != fireball && target.IsActive)
                            {
                                if (target is Enemy enemy && fireball.Bounds.Intersects(enemy.Bounds))
                                {
                                    enemy.OnStomped();
                                    fireball.IsActive = false;
                                }
                                else if ((target is Block || target is Pipe || target is Castle || target is MysteryBlock) && fireball.Bounds.Intersects(target.Bounds))
                                {
                                    fireball.IsActive = false;
                                }
                            }
                        }
                    }
                }

                if (!obj.IsActive && !(obj is Castle && (_player1.HasReachedGoal || _player2.HasReachedGoal)))
                    _gameObjects.RemoveAt(i);
            }

            _hud.LivesRemaining = System.Math.Min(_player1.Lives, _player2.Lives);
            _hud.CoinsCollected = _player1.Coins + _player2.Coins;

            // Camera Update (Trung bình cộng 2 người)
            Vector2 avgPos = new Vector2((_player1.Position.X + _player2.Position.X) / 2, (_player1.Position.Y + _player2.Position.Y) / 2);
            Rectangle mapBounds = new Rectangle(0, 0, 3200, 736);
            _camera.Update(avgPos, mapBounds, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (_backgroundTex != null) spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);
            foreach (var obj in _gameObjects)
                if (_camera.IsVisible(obj.Bounds)) obj.Draw(spriteBatch);

            _player1.Draw(spriteBatch);
            _player2.Draw(spriteBatch);
            spriteBatch.End();

            // Draw player labels (P1, P2)
            if (_playerLabelFont != null)
            {
                spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);
                Vector2 p1Pos = new Vector2(_player1.Bounds.X + 8, _player1.Bounds.Y - 25);
                spriteBatch.DrawString(_playerLabelFont, "P1", p1Pos, Color.Cyan);

                Vector2 p2Pos = new Vector2(_player2.Bounds.X + 8, _player2.Bounds.Y - 25);
                spriteBatch.DrawString(_playerLabelFont, "P2", p2Pos, Color.Magenta);
                spriteBatch.End();
            }

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void PauseGame()
        {
            _isPaused = true;
            // Lưu P1 làm đại diện (hoặc tạo Logic Save riêng cho 2P nếu cần)
            GameState state = new GameState
            {
                LevelIndex = _levelIndex,
                PlayerPosition = _player1.Position,
                PlayerVelocity = _player1.Velocity,
                PlayerLives = _player1.Lives,
                PlayerCoins = _player1.Coins,
                PlayerScore = _player1.Score,
                PlayerScale = _player1.Scale,
                GameObjects = new List<GameObj>(_gameObjects),
                ElapsedTime = _hud.ElapsedTime,
                EnemiesDefeated = _hud.EnemiesDefeated,
                MushroomsCollected = _hud.MushroomsCollected,
                DeathCount = _hud.DeathCount,
                IsValid = true
            };
            GameManager.Instance.SaveGameState(state);
            GameManager.Instance.ChangeScene(new PauseScene(_levelIndex));
        }

        private void SpawnBulletBill()
        {
            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;
            Texture2D bulletTex = content.Load<Texture2D>("sprites/bullet");
            BulletBill bill = new BulletBill(bulletTex, _camera, device);
            float randomY = _random.Next(450, 650);
            bill.Spawn(randomY);
            _gameObjects.Add(bill);
        }

        private void ShootFireball(Player shooter)
        {
            if (_fireballTexture == null) return;
            Vector2 spawnPos = new Vector2(shooter.Position.X, shooter.Position.Y + 16);
            int dir = shooter.FacingDirection;
            Fireball ball = new Fireball(_fireballTexture, spawnPos, dir);
            _gameObjects.Add(ball);
        }
    }
}
