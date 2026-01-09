using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using MarioGame._Scenes;
using MarioGame.src._Data.models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class GameOverScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private int _levelIndex;
        private int _finalScore;
        private int _finalCoins;
        private int _enemiesDefeated;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        public GameOverScene(int levelIndex, int score, int coins, int enemies = 0)
        {
            _levelIndex = levelIndex;
            _finalScore = score;
            _finalCoins = coins;
            _enemiesDefeated = enemies;

            // Check and unlock achievements (but don't add to session yet)
            CheckAchievements();
        }

        private void CheckAchievements()
        {
            // Get current session stats (before adding this level)
            GameSession session = GameSession.Instance;

            AchievementManager.Instance.CheckAndUnlockAchievements(
                _finalCoins,           // coins this level
                _enemiesDefeated,      // enemies this level
                _finalScore,           // score this level
                0,                     // level time (not available in GameOver)
                session.TotalEnemiesDefeated,  // total enemies
                session.TotalCoins,             // total coins
                session.TotalScore,             // total score
                true                            // took damage (game over = took damage)
            );

            AchievementManager.Instance.SaveAll();
        }

        public void LoadContent()
        {
            if (_isContentLoaded)
                return;

            var content = GameManager.Instance.Content;

            try
            {
                _font = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch
            {
                _font = null;
            }

            InitializeButtons();
            _isContentLoaded = true;
        }

        private void InitializeButtons()
        {
            _buttons = new List<Button>();

            int buttonWidth = 240;
            int buttonHeight = 50;
            int spacing = 20;

            int centerX = 640;
            int startX = centerX - buttonWidth / 2;
            int startY = 550;

            // Retry Level button (left)
            var retryButton = new Button(
                new Rectangle(startX - 140, startY, buttonWidth, buttonHeight),
                "RETRY LEVEL",
                _font
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(230, 0, 18),
                BorderColor = Color.White,
                TextColor = Color.White,
                TextScale = 0.7f
            };
            _buttons.Add(retryButton);

            // Main Menu button (right)
            var menuButton = new Button(
                new Rectangle(startX + 140, startY, buttonWidth, buttonHeight),
                "MAIN MENU",
                _font
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = Color.White,
                BorderColor = Color.White,
                TextColor = Color.White,
                TextScale = 0.7f
            };
            _buttons.Add(menuButton);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var button in _buttons)
            {
                button.Update(gameTime);
            }

            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Skip first update
            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Handle button clicks
            if (_buttons[0].WasPressed) // Retry
            {
                GameManager.Instance.ClearSavedGameState();
                // Don't add to GameSession - it will reset when GameplayScene loads level 1
                // (or retry same level without resetting)
                GameManager.Instance.ChangeScene(new GameplayScene(_levelIndex));
            }
            else if (_buttons[1].WasPressed) // Main Menu
            {
                GameManager.Instance.ClearSavedGameState();
                // Menu will reset GameSession when player clicks "1 PLAYER" or "2 PLAYERS"
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(new Color(18, 18, 18));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_font != null)
            {
                // Header bar (red) - matching AboutUs style
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 80), new Color(230, 0, 18));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 76, 1280, 4), Color.Black);
                }

                // Title
                spriteBatch.DrawString(_font, "GAME OVER", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, $"Level {_levelIndex}", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Content section
                int contentY = 110;
                int lineHeight = 35;

                // Statistics header
                spriteBatch.DrawString(_font, "FINAL STATISTICS:", new Vector2(100, contentY), Color.Cyan, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
                contentY += 40;

                spriteBatch.DrawString(_font, $"Score: {_finalScore}", new Vector2(120, contentY), Color.Yellow, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Coins: {_finalCoins}", new Vector2(120, contentY), Color.Gold, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Enemies Defeated: {_enemiesDefeated}", new Vector2(120, contentY), Color.Red, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

                // Footer
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }
            }

            spriteBatch.End();

            // Draw buttons
            spriteBatch.Begin();
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
