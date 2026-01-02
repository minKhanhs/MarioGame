using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarioGame.Managers;
using MarioGame.Level;
using MarioGame.UI;
using System;

namespace MarioGame.Core
{
    public class GameManager : Game
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameState CurrentState { get; private set; }
        public Level.Level CurrentLevel { get; private set; }

        private UIManager _uiManager;
        private LevelLoader _levelLoader;

        public int Score { get; set; }
        public int Lives { get; set; }
        public int Coins { get; set; }
        public float TimeRemaining { get; set; }

        public GameManager()
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            // Subscribe to events
            GameEvents.CoinCollected += OnCoinCollected;
        }

        private void OnCoinCollected(Entities.Player.Player player)
        {
            // Basic handling for coin collection
            AddCoin();
        }

        protected override void Initialize()
        {
            CurrentState = GameState.StartScreen;
            Lives = 3;
            Score = 0;
            Coins = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize Singletons
            SoundManager.Instance.Initialize(Content);
            DataManager.Instance.Initialize();

            _uiManager = new UIManager(Content, _spriteBatch);
            _levelLoader = new LevelLoader(Content);

            // Load first level
            LoadLevel(1);
        }

        public void LoadLevel(int levelId)
        {
            CurrentLevel = _levelLoader.LoadLevel(levelId);
            if (CurrentLevel != null)
            {
                CurrentLevel.Initialize(Content, GraphicsDevice);
                TimeRemaining = CurrentLevel.TimeLimit;
                CurrentState = GameState.Playing;
            }
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;

            if (newState == GameState.GameOver)
            {
                Lives = 3;
                Score = 0;
                Coins = 0;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputManager.Instance.Update();

            switch (CurrentState)
            {
                case GameState.StartScreen:
                    UpdateStartScreen();
                    break;

                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;

                case GameState.Paused:
                    UpdatePaused();
                    break;

                case GameState.LevelComplete:
                    UpdateLevelComplete();
                    break;

                case GameState.GameOver:
                    UpdateGameOver();
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateStartScreen()
        {
            if (InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                LoadLevel(1);
            }
        }

        private void UpdatePlaying(float deltaTime)
        {
            if (InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                CurrentState = GameState.Paused;
                return;
            }

            TimeRemaining -= deltaTime;
            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                PlayerDied();
            }

            CurrentLevel?.Update(deltaTime);

            // Check win condition
            if (CurrentLevel?.IsCompleted == true)
            {
                CurrentState = GameState.LevelComplete;
            }
        }

        private void UpdatePaused()
        {
            if (InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                CurrentState = GameState.Playing;
            }
        }

        private void UpdateLevelComplete()
        {
            if (InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                int nextLevel = CurrentLevel.LevelId + 1;
                if (nextLevel <= 10)
                {
                    LoadLevel(nextLevel);
                }
                else
                {
                    // Win game
                    CurrentState = GameState.StartScreen;
                }
            }
        }

        private void UpdateGameOver()
        {
            if (InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                LoadLevel(1);
            }
        }

        public void PlayerDied()
        {
            Lives--;
            if (Lives <= 0)
            {
                ChangeState(GameState.GameOver);
            }
            else
            {
                // Restart level
                LoadLevel(CurrentLevel.LevelId);
            }
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void AddCoin()
        {
            Coins++;
            AddScore(100);
            if (Coins >= 100)
            {
                Coins -= 100;
                Lives++;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            switch (CurrentState)
            {
                case GameState.StartScreen:
                    _uiManager.DrawStartScreen();
                    break;

                case GameState.Playing:
                case GameState.Paused:
                    CurrentLevel?.Draw(_spriteBatch);
                    _uiManager.DrawHUD(Score, Lives, Coins, (int)TimeRemaining);

                    if (CurrentState == GameState.Paused)
                    {
                        _uiManager.DrawPauseScreen();
                    }
                    break;

                case GameState.LevelComplete:
                    CurrentLevel?.Draw(_spriteBatch);
                    _uiManager.DrawLevelComplete();
                    break;

                case GameState.GameOver:
                    _uiManager.DrawGameOver();
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}