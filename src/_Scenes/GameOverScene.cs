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

            // Check and unlock achievements
            CheckAchievements();
        }

        private void CheckAchievements()
        {
            // Get cumulative stats from GameSession
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

            int buttonWidth = 200;
            int buttonHeight = 50;
            int spacing = 20;

            int centerX = 640;
            int startX = centerX - buttonWidth / 2;
            int startY = 450;

            // Retry Level button
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
                "RETRY LEVEL",
                _font
            ));

            // Main Menu button
            _buttons.Add(new Button(
                new Rectangle(startX, startY + buttonHeight + spacing, buttonWidth, buttonHeight),
                "MAIN MENU",
                _font
            ));
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
                GameManager.Instance.ChangeScene(new GameplayScene(_levelIndex));
            }
            else if (_buttons[1].WasPressed) // Main Menu
            {
                GameManager.Instance.ClearSavedGameState();
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin();

            if (_font != null)
            {
                // Draw GAME OVER title
                string title = "GAME OVER";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 100), Color.Red);

                // Draw statistics
                string scoreText = $"Score: {_finalScore}";
                Vector2 scoreSize = _font.MeasureString(scoreText);
                spriteBatch.DrawString(_font, scoreText,
                    new Vector2(640 - scoreSize.X / 2, 200), Color.Yellow);

                string coinsText = $"Coins Collected: {_finalCoins}";
                Vector2 coinsSize = _font.MeasureString(coinsText);
                spriteBatch.DrawString(_font, coinsText,
                    new Vector2(640 - coinsSize.X / 2, 260), Color.Gold);

                string enemiesText = $"Enemies Defeated: {_enemiesDefeated}";
                Vector2 enemiesSize = _font.MeasureString(enemiesText);
                spriteBatch.DrawString(_font, enemiesText,
                    new Vector2(640 - enemiesSize.X / 2, 320), Color.Red);

                // Draw level info
                string levelText = $"Level: {_levelIndex}";
                Vector2 levelSize = _font.MeasureString(levelText);
                spriteBatch.DrawString(_font, levelText,
                    new Vector2(640 - levelSize.X / 2, 360), Color.White);

                // Draw hint
                string hint = "Click buttons below to continue";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, 390), Color.Gray);
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
