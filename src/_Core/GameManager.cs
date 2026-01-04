using MarioGame.src._Scenes;
using MarioGame.src._Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core
{
    /// <summary>
    /// Saves and restores game state for pause/resume functionality
    /// </summary>
    public class GameState
    {
        public int LevelIndex { get; set; }
        public Vector2 PlayerPosition { get; set; }
        public Vector2 PlayerVelocity { get; set; }
        public int PlayerLives { get; set; }
        public int PlayerCoins { get; set; }
        public int PlayerScore { get; set; }
        public List<GameObj> GameObjects { get; set; }
        public bool IsValid { get; set; }

        public GameState()
        {
            IsValid = false;
            GameObjects = new List<GameObj>();
        }

        public void Clear()
        {
            IsValid = false;
            GameObjects.Clear();
        }
    }

    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        // Scene hiện tại đang chạy
        public IScene CurrentScene { get; private set; }

        // Để load content khi chuyển scene
        public Microsoft.Xna.Framework.Content.ContentManager Content { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        // Game state for pause/resume functionality
        private GameState _savedGameState;

        public GameManager()
        {
            _savedGameState = new GameState();
        }

        public void ChangeScene(IScene newScene)
        {
            CurrentScene = newScene;
            CurrentScene.LoadContent(); // Tự động load tài nguyên cho màn mới
        }

        public void SaveGameState(GameState state)
        {
            _savedGameState = state;
        }

        public GameState GetSavedGameState()
        {
            return _savedGameState;
        }

        public void ClearSavedGameState()
        {
            _savedGameState.Clear();
        }

        public void Update(GameTime gameTime)
        {
            CurrentScene?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentScene?.Draw(spriteBatch);
        }
    }
}
