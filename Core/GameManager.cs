using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MarioGame.Core;
using MarioGame.Level;
using MarioGame.Entities.Player;
using MarioGame.Systems.Physics;
using MarioGame.Systems.Rendering;
using System.Collections.Generic;

namespace MarioGame.Managers
{
    public class GameManager
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameManager();
                return instance;
            }
        }

        public GameState CurrentState { get; private set; } = GameState.StartMenu;
        public Level.Level CurrentLevel { get; private set; }
        public int CurrentLevelId { get; private set; } = 1;

        private List<Level.Level> levels = new List<Level.Level>();
        private PhysicsEngine physicsEngine;

        // Game settings
        public bool IsMusicOn { get; set; } = true;
        public bool IsSfxOn { get; set; } = true;
        public float MasterVolume { get; set; } = 0.8f;

        private GameManager()
        {
            physicsEngine = new PhysicsEngine();
        }

        public void Initialize()
        {
            // Load levels data
            LoadLevels();

            // Initialize managers
            CameraManager.Instance.Initialize(0, 0);

            CurrentState = GameState.StartMenu;
        }

        private void LoadLevels()
        {
            // TODO: Load from JSON file
            // For now, create test levels programmatically

            // Level 1
            var level1 = CreateTestLevel(1);
            levels.Add(level1);
        }

        private Level.Level CreateTestLevel(int levelId)
        {
            var level = new Level.Level(levelId, $"World 1-{levelId}", 300, 200, 15);

            // Create simple ground
            for (int x = 0; x < level.Map.Width; x++)
            {
                level.Map.SetTile(x, 13, TileType.Ground);
                level.Map.SetTile(x, 14, TileType.Ground);
            }

            // Add some platforms
            for (int x = 10; x < 15; x++)
            {
                level.Map.SetTile(x, 10, TileType.Brick);
            }

            // Add question blocks
            level.Map.SetTile(15, 9, TileType.QuestionBlock);
            level.Map.SetTile(20, 9, TileType.QuestionBlock);

            // Add pipes
            for (int y = 11; y < 14; y++)
            {
                level.Map.SetTile(25, y, TileType.Pipe);
                level.Map.SetTile(26, y, TileType.Pipe);
            }

            return level;
        }

        public void StartGame(int numberOfPlayers)
        {
            CurrentLevelId = 1;
            LoadLevel(CurrentLevelId);

            // Create players
            Vector2 spawnPos = new Vector2(100, 400);

            var player1 = new Player(spawnPos, 1);
            CurrentLevel.AddPlayer(player1);

            if (numberOfPlayers == 2)
            {
                var player2 = new Player(spawnPos + new Vector2(50, 0), 2);
                CurrentLevel.AddPlayer(player2);
            }

            CurrentState = GameState.Playing;
        }

        public void LoadLevel(int levelId)
        {
            CurrentLevelId = levelId;

            if (levelId > levels.Count)
            {
                // Game completed!
                WinGame();
                return;
            }

            CurrentLevel = levels[levelId - 1];
            CurrentLevel.Initialize();

            // Setup physics
            physicsEngine.Clear();

            // Add tiles to physics
            foreach (var tile in CurrentLevel.Map.GetSolidTiles())
            {
                physicsEngine.AddStaticObject(tile);
            }

            // Add players to physics
            foreach (var player in CurrentLevel.Players)
            {
                physicsEngine.AddDynamicObject(player);
            }

            // Spawn enemies based on level design
            SpawnEnemiesForLevel(levelId);
        }

        private void SpawnEnemiesForLevel(int levelId)
        {
            // Example enemy spawning
            if (levelId >= 3)
            {
                var goomba1 = new Entities.Enemies.Goomba(new Vector2(400, 300));
                CurrentLevel.AddEnemy(goomba1);
                physicsEngine.AddDynamicObject(goomba1);

                var goomba2 = new Entities.Enemies.Goomba(new Vector2(600, 300));
                CurrentLevel.AddEnemy(goomba2);
                physicsEngine.AddDynamicObject(goomba2);
            }

            if (levelId >= 4)
            {
                var koopa = new Entities.Enemies.Koopa(new Vector2(800, 300));
                CurrentLevel.AddEnemy(koopa);
                physicsEngine.AddDynamicObject(koopa);
            }

            if (levelId >= 6)
            {
                var plant = new Entities.Enemies.PiranhaPlant(new Vector2(25 * 32, 11 * 32));
                CurrentLevel.AddEnemy(plant);
                physicsEngine.AddDynamicObject(plant);
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.StartMenu:
                    UpdateStartMenu(gameTime);
                    break;

                case GameState.Playing:
                    UpdatePlaying(gameTime);
                    break;

                case GameState.Paused:
                    UpdatePaused(gameTime);
                    break;

                case GameState.GameOver:
                case GameState.Victory:
                    UpdateEndScreen(gameTime);
                    break;
            }
        }

        private void UpdateStartMenu(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                StartGame(1); // Single player
            }
            else if (keyboard.IsKeyDown(Keys.Space))
            {
                StartGame(2); // Two players
            }
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            if (CurrentLevel == null) return;

            KeyboardState keyboard = Keyboard.GetState();

            // Pause
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                CurrentState = GameState.Paused;
                return;
            }

            // Update level
            CurrentLevel.Update(gameTime);

            // Update physics
            physicsEngine.Update(gameTime);

            // Update camera
            CameraManager.Instance.Update(gameTime);

            // Check level completion
            if (CurrentLevel.IsCompleted)
            {
                NextLevel();
            }
            else if (CurrentLevel.IsFailed)
            {
                GameOver();
            }
        }

        private void UpdatePaused(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                CurrentState = GameState.Playing;
            }
        }

        private void UpdateEndScreen(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                CurrentState = GameState.StartMenu;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentState)
            {
                case GameState.StartMenu:
                    DrawStartMenu(spriteBatch);
                    break;

                case GameState.Playing:
                case GameState.Paused:
                    DrawPlaying(spriteBatch);
                    break;

                case GameState.GameOver:
                    DrawGameOver(spriteBatch);
                    break;

                case GameState.Victory:
                    DrawVictory(spriteBatch);
                    break;
            }
        }

        private void DrawStartMenu(SpriteBatch spriteBatch)
        {
            // TODO: Draw title, menu options
            // Simple placeholder text
        }

        private void DrawPlaying(SpriteBatch spriteBatch)
        {
            if (CurrentLevel == null) return;

            // Draw level
            CurrentLevel.Draw(spriteBatch);

            // Draw HUD
            DrawHUD(spriteBatch);

            if (CurrentState == GameState.Paused)
            {
                // Draw pause overlay
                // TODO
            }
        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            // TODO: Draw Lives, Score, Time, Coins
            // Placeholder - needs SpriteFont
        }

        private void DrawGameOver(SpriteBatch spriteBatch)
        {
            // TODO: Game Over screen
        }

        private void DrawVictory(SpriteBatch spriteBatch)
        {
            // TODO: Victory screen
        }

        public void NextLevel()
        {
            CurrentLevelId++;
            LoadLevel(CurrentLevelId);
        }

        public void GameOver()
        {
            CurrentState = GameState.GameOver;
            // Save high scores
        }

        public void WinGame()
        {
            CurrentState = GameState.Victory;
            // Save completion data
        }

        public void RespawnPlayer(Player player)
        {
            // Reset player position
            player.Position = new Vector2(100, 400);
            player.Velocity = Vector2.Zero;
            player.IsActive = true;
        }
    }
}