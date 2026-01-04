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
    public class TwoPlayerGameplayScene : IScene
    {
        private int _levelIndex;
        private List<GameObj> _gameObjects;
        private Player _player1;
        private Player _player2;
        private Camera _camera;
        private Texture2D _backgroundTex;
        private GameHUD _hud;
        private SpriteFont _playerLabelFont;

        // Tr?ng thái th?ng thua
        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;

        // Qu?n lý pause
        private bool _isPaused = false;
        private KeyboardState _previousKeyboardState;
        private bool _isResumingFromPause = false;

        // Ki?m tra xem ?ã load content ch?a
        private bool _isContentLoaded = false;

        public TwoPlayerGameplayScene(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void LoadContent()
        {
            // Tránh load content nhi?u l?n
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
                _playerLabelFont = hudFont;
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

            MapLoader.Initialize(textures);

            // LOAD level bình th??ng
            LoadLevelFromFile(playerAnims);

            _isContentLoaded = true;
        }

        private void LoadLevelFromFile(Dictionary<string, SpriteAnimation> playerAnims)
        {
            string levelPath = $"Content/levels/level{_levelIndex}.json";

            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);
                
                // Create 2 players with different starting positions
                _player1 = new Player(new Vector2(50, 200), playerAnims, 1);
                _player2 = new Player(new Vector2(100, 200), playerAnims, 2);
                
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

            // Check level completion - BOTH players must reach goal
            if (_isLevelFinished)
            {
                _finishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_finishTimer > 2.0f)
                {
                    _isContentLoaded = false; // Reset flag for next level
                    System.Diagnostics.Debug.WriteLine($"[LEVEL COMPLETE] Level {_levelIndex} (2 Players)");
                    int bonusScore = _hud.CalculateLevelBonus();
                    GameManager.Instance.ChangeScene(new LevelCompleteScene(_levelIndex, 3, _player1.Score + _player2.Score, _player1.Coins + _player2.Coins, bonusScore, _hud.EnemiesDefeated));
                }
                return;
            }

            // Update both players
            _player1.Update(gameTime);
            _player1.IsOnGround = false;
            
            _player2.Update(gameTime);
            _player2.IsOnGround = false;

            // Check if any player died
            if (_player1.Lives <= 0 || _player1.Position.Y > 1000 || _player2.Lives <= 0 || _player2.Position.Y > 1000)
            {
                System.Diagnostics.Debug.WriteLine($"[GAME OVER] Level {_levelIndex} (2 Players)");
                _isContentLoaded = false;
                
                // Determine which player died
                string deathReason = "";
                if (_player1.Lives <= 0) deathReason = "Player 1 died";
                else if (_player1.Position.Y > 1000) deathReason = "Player 1 fell";
                else if (_player2.Lives <= 0) deathReason = "Player 2 died";
                else deathReason = "Player 2 fell";
                
                GameManager.Instance.ChangeScene(new TwoPlayerGameOverScene(_levelIndex, _player1.Score + _player2.Score, _player1.Coins + _player2.Coins, deathReason));
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
                    // Player 1 collisions
                    if (obj is Block)
                        Collision.ResolveStaticCollision(_player1, obj);
                    else if (obj is Item item && _player1.Bounds.Intersects(item.Bounds))
                        item.OnCollect(_player1);
                    else if (obj is Enemy enemy && _player1.Bounds.Intersects(enemy.Bounds))
                    {
                        if (Collision.IsTopCollision(_player1, enemy))
                        {
                            enemy.OnStomped();
                            _player1.Velocity.Y = -5f;
                            _hud.EnemiesDefeated++;
                        }
                        else
                            _player1.TakeDamage();
                    }

                    // Player 2 collisions
                    if (obj is Block)
                        Collision.ResolveStaticCollision(_player2, obj);
                    else if (obj is Item item2 && _player2.Bounds.Intersects(item2.Bounds))
                        item2.OnCollect(_player2);
                    else if (obj is Enemy enemy2 && _player2.Bounds.Intersects(enemy2.Bounds))
                    {
                        if (Collision.IsTopCollision(_player2, enemy2))
                        {
                            enemy2.OnStomped();
                            _player2.Velocity.Y = -5f;
                            _hud.EnemiesDefeated++;
                        }
                        else
                            _player2.TakeDamage();
                    }

                    // Level completion - both players must reach goal
                    if (obj is Castle)
                    {
                        if (_player1.Bounds.Intersects(obj.Bounds))
                            _player1.HasReachedGoal = true;
                        
                        if (_player2.Bounds.Intersects(obj.Bounds))
                            _player2.HasReachedGoal = true;

                        // Both reached goal
                        if (_player1.HasReachedGoal && _player2.HasReachedGoal)
                        {
                            System.Diagnostics.Debug.WriteLine("LEVEL COMPLETE (BOTH PLAYERS)!");
                            _isLevelFinished = true;
                        }
                    }
                }

                if (!obj.IsActive && !(obj is Castle && (_player1.HasReachedGoal || _player2.HasReachedGoal)))
                    _gameObjects.RemoveAt(i);
            }

            // Update HUD with combined stats
            _hud.LivesRemaining = System.Math.Min(_player1.Lives, _player2.Lives); // Show minimum lives
            _hud.CoinsCollected = _player1.Coins + _player2.Coins;
            _hud.CurrentScore = _player1.Score + _player2.Score;

            // Update camera to follow average position of both players
            Vector2 avgPos = new Vector2((_player1.Position.X + _player2.Position.X) / 2, (_player1.Position.Y + _player2.Position.Y) / 2);
            
            Rectangle mapBounds = new Rectangle(0, 0, 3200, 736);
            _camera.Update(avgPos, mapBounds);
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
            _player1.Draw(spriteBatch);
            _player2.Draw(spriteBatch);
            spriteBatch.End();

            // Draw player labels (P1, P2) on top of world
            if (_playerLabelFont != null)
            {
                spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);
                
                // P1 label (white/cyan color)
                var p1Bounds = _player1.Bounds;
                Vector2 p1LabelPos = new Vector2(p1Bounds.X + p1Bounds.Width / 2 - 8, p1Bounds.Y - 25);
                spriteBatch.DrawString(_playerLabelFont, "P1", p1LabelPos, Color.Cyan);
                
                // P2 label (magenta/red color to distinguish)
                var p2Bounds = _player2.Bounds;
                Vector2 p2LabelPos = new Vector2(p2Bounds.X + p2Bounds.Width / 2 - 8, p2Bounds.Y - 25);
                spriteBatch.DrawString(_playerLabelFont, "P2", p2LabelPos, Color.Magenta);
                
                spriteBatch.End();
            }

            // Draw HUD on top
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void PauseGame()
        {
            _isPaused = true;

            // Save complete game state
            GameState state = new GameState
            {
                LevelIndex = _levelIndex,
                PlayerPosition = _player1.Position,
                PlayerVelocity = _player1.Velocity,
                PlayerLives = _player1.Lives,
                PlayerCoins = _player1.Coins,
                PlayerScore = _player1.Score,
                GameObjects = new List<GameObj>(_gameObjects),
                IsValid = true
            };

            System.Diagnostics.Debug.WriteLine($"[PAUSE] Saved 2-player state");

            GameManager.Instance.SaveGameState(state);
            GameManager.Instance.ChangeScene(new PauseScene(_levelIndex));
        }
    }
}
