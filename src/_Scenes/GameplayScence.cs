using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.enemies;
using MarioGame.src._Entities.Enviroments;
using MarioGame.src._Entities.items;
using MarioGame.src._Core;
using MarioGame.src._Core.Camera;
using MarioGame.src._Data;
using MarioGame.src._Entities.player;
using MarioGame.src._Scenes;
using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        // Trạng thái thắng thua
        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;

        // Quản lý pause
        private bool _isPaused = false;
        private KeyboardState _previousKeyboardState;
        private bool _isResumingFromPause = false;

        // Kiểm tra xem đã load content chưa
        private bool _isContentLoaded = false;

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
                // LOAD level bình thường (NEW GAME)
                LoadLevelFromFile(playerAnims);
            }

            _isContentLoaded = true;
        }

        private void RestoreGameState(GameState savedState, Dictionary<string, SpriteAnimation> playerAnims)
        {
            // Restore game objects
            _gameObjects = new List<GameObj>(savedState.GameObjects);

            // Restore player
            _player = new Player(savedState.PlayerPosition, playerAnims);
            _player.Velocity = savedState.PlayerVelocity;
            _player.Lives = savedState.PlayerLives;
            _player.Coins = savedState.PlayerCoins;
            _player.Score = savedState.PlayerScore;

            System.Diagnostics.Debug.WriteLine($"[RESUME] Restored player at {_player.Position}, Lives: {_player.Lives}");
        }

        private void LoadLevelFromFile(Dictionary<string, SpriteAnimation> playerAnims)
        {
            string levelPath = $"Content/levels/level{_levelIndex}.json";

            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);
                _player = new Player(new Vector2(50, 200), playerAnims);
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
            // Skip first frame after resume to prevent ESC collision
            if (_isResumingFromPause)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                _previousKeyboardState = currentKeyboardState;
                _isResumingFromPause = false;
                return;
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
                    System.Diagnostics.Debug.WriteLine($"[LEVEL COMPLETE] Level {_levelIndex}, Score: {_player.Score}, Coins: {_player.Coins}");
                    // Show level complete scene
                    GameManager.Instance.ChangeScene(new LevelCompleteScene(_levelIndex, 3, _player.Score, _player.Coins, 500));
                }
                return;
            }

            // Update game logic
            _player.Update(gameTime);
            _player.IsOnGround = false;

            // Check game over
            if (_player.Lives <= 0 || _player.Position.Y > 1000)
            {
                System.Diagnostics.Debug.WriteLine($"[GAME OVER] Level {_levelIndex}, Score: {_player.Score}, Coins: {_player.Coins}");
                _isContentLoaded = false; // Reset flag for retry
                GameManager.Instance.ChangeScene(new GameOverScene(_levelIndex, _player.Score, _player.Coins));
                return;
            }

            // Update all game objects
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var obj = _gameObjects[i];
                obj.Update(gameTime);

                // Enemy collision with blocks
                if (obj is MovableObj movableObj && !(obj is Player))
                {
                    foreach (var other in _gameObjects)
                        if (other is Block && other != obj)
                            Collision.ResolveStaticCollision(movableObj, other);
                }

                if (obj.IsActive)
                {
                    // Player collision with blocks
                    if (obj is Block)
                        Collision.ResolveStaticCollision(_player, obj);
                    // Item collection
                    else if (obj is Item item && _player.Bounds.Intersects(item.Bounds))
                        item.OnCollect(_player);
                    // Enemy interaction
                    else if (obj is Enemy enemy && _player.Bounds.Intersects(enemy.Bounds))
                    {
                        if (Collision.IsTopCollision(_player, enemy))
                        {
                            enemy.OnStomped();
                            _player.Velocity.Y = -5f;
                        }
                        else
                            _player.TakeDamage();
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
        }

        private void PauseGame()
        {
            _isPaused = true;

            // Save complete game state
            GameState state = new GameState
            {
                LevelIndex = _levelIndex,
                PlayerPosition = _player.Position,
                PlayerVelocity = _player.Velocity,
                PlayerLives = _player.Lives,
                PlayerCoins = _player.Coins,
                PlayerScore = _player.Score,
                GameObjects = new List<GameObj>(_gameObjects),
                IsValid = true
            };

            System.Diagnostics.Debug.WriteLine($"[PAUSE] Saved state - Player at {_player.Position}, Lives: {_player.Lives}");

            GameManager.Instance.SaveGameState(state);
            GameManager.Instance.ChangeScene(new PauseScene(_levelIndex));
        }
    }
}