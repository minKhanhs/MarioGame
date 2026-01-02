using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MarioGame.Core;
using MarioGame.Managers;
using MarioGame.Level;
using MarioGame.UI;
using MarioGame.Systems.Physics;

namespace MarioGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game State
        private GameState _currentState;
        private Level.Level _currentLevel;

        // Managers (Singleton)
        private InputManager _inputManager;
        private SoundManager _soundManager;
        private DataManager _dataManager;

        // Managers (Normal)
        private CameraManager _cameraManager;
        private UIManager _uiManager;
        private LevelLoader _levelLoader;
        private PhysicsEngine _physicsEngine;

        // Game Data
        private int _score;
        private int _lives;
        private int _coins;
        private float _timeRemaining;

        // Graphics
        private SpriteFont _gameFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _currentState = GameState.StartScreen;
            _score = 0;
            _coins = 0;
            _lives = 3;

            // Singleton managers
            _inputManager = InputManager.Instance;
            _soundManager = SoundManager.Instance;
            _dataManager = DataManager.Instance;

            // Normal managers
            _cameraManager = new CameraManager();
            _physicsEngine = new PhysicsEngine();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameFont = Content.Load<SpriteFont>("Fonts/GameFont");

            _soundManager.Initialize(Content);
            _dataManager.Initialize();

            _uiManager = new UIManager(Content, _spriteBatch);
            _levelLoader = new LevelLoader(Content);

            LoadLevel(1);
        }

        private void LoadLevel(int levelId)
        {
            _currentLevel = _levelLoader.LoadLevel(levelId);

            if (_currentLevel == null)
                return;

            _currentLevel.Initialize(Content, GraphicsDevice);
            _timeRemaining = _currentLevel.TimeLimit;
            _currentState = GameState.Playing;

            _soundManager.PlayMusic("main_theme");
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _inputManager.Update();

            if (_inputManager.IsKeyPressed(Keys.Escape))
            {
                _currentState = _currentState == GameState.Playing
                    ? GameState.Paused
                    : GameState.Playing;
            }

            switch (_currentState)
            {
                case GameState.StartScreen:
                    if (_inputManager.IsKeyPressed(Keys.Enter))
                        LoadLevel(1);
                    break;

                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;

                case GameState.Paused:
                    break;

                case GameState.LevelComplete:
                    if (_inputManager.IsKeyPressed(Keys.Enter))
                        LoadLevel(_currentLevel.LevelId + 1);
                    break;

                case GameState.GameOver:
                    if (_inputManager.IsKeyPressed(Keys.Enter))
                        LoadLevel(1);
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdatePlaying(float deltaTime)
        {
            if (_currentLevel == null)
                return;

            _timeRemaining -= deltaTime;
            if (_timeRemaining <= 0)
            {
                PlayerDied();
                return;
            }

            _currentLevel.Update(deltaTime);

            if (_currentLevel.Players.Count > 0)
            {
                _cameraManager.Update(deltaTime,
                    _currentLevel.Players[0].Position);
            }

            if (_currentLevel.IsCompleted)
            {
                _currentState = GameState.LevelComplete;
                _soundManager.PlaySound("levelcomplete");
            }
        }

        private void PlayerDied()
        {
            _lives--;
            _soundManager.PlaySound("gameover");

            if (_lives <= 0)
            {
                _currentState = GameState.GameOver;
            }
            else
            {
                LoadLevel(_currentLevel.LevelId);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp
            );

            if (_currentState == GameState.Playing ||
                _currentState == GameState.Paused ||
                _currentState == GameState.LevelComplete)
            {
                _currentLevel?.Draw(_spriteBatch);
                _uiManager.DrawHUD(_score, _lives, _coins, (int)_timeRemaining);
            }
            else if (_currentState == GameState.StartScreen)
            {
                _uiManager.DrawStartScreen();
            }
            else if (_currentState == GameState.GameOver)
            {
                _uiManager.DrawGameOver();
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _dataManager.SaveCurrentState(
                _currentLevel?.LevelId ?? 1,
                _lives,
                _score,
                _coins,
                "Small"
            );

            base.UnloadContent();
        }
    }
}
