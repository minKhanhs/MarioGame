using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using MarioGame._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class LevelCompleteScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private int _currentLevel;
        private int _totalLevels;
        private int _bonusScore;
        private int _finalScore;
        private int _finalCoins;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        public LevelCompleteScene(int currentLevel, int totalLevels, int score, int coins, int bonusScore = 500)
        {
            _currentLevel = currentLevel;
            _totalLevels = totalLevels;
            _finalScore = score + bonusScore;
            _finalCoins = coins;
            _bonusScore = bonusScore;
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
            int startY = 500;

            // Next Level or Continue button
            if (_currentLevel < _totalLevels)
            {
                _buttons.Add(new Button(
                    new Rectangle(startX, startY, buttonWidth, buttonHeight),
                    "NEXT LEVEL",
                    _font
                ));
            }
            else
            {
                _buttons.Add(new Button(
                    new Rectangle(startX, startY, buttonWidth, buttonHeight),
                    "GAME COMPLETE",
                    _font
                ));
            }

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
            if (_buttons[0].WasPressed) // Next Level or Game Complete
            {
                if (_currentLevel < _totalLevels)
                {
                    // Go to next level
                    GameManager.Instance.ClearSavedGameState();
                    GameManager.Instance.ChangeScene(new GameplayScene(_currentLevel + 1));
                }
                else
                {
                    // Game finished - go to menu
                    GameManager.Instance.ClearSavedGameState();
                    GameManager.Instance.ChangeScene(new MenuScene());
                }
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
                // Draw LEVEL COMPLETE title
                string title = "LEVEL COMPLETE!";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 80), Color.Gold);

                // Draw level info
                string levelText = $"Level {_currentLevel} / {_totalLevels}";
                Vector2 levelSize = _font.MeasureString(levelText);
                spriteBatch.DrawString(_font, levelText,
                    new Vector2(640 - levelSize.X / 2, 170), Color.LimeGreen);

                // Draw statistics
                string coinsText = $"Coins Collected: {_finalCoins}";
                Vector2 coinsSize = _font.MeasureString(coinsText);
                spriteBatch.DrawString(_font, coinsText,
                    new Vector2(640 - coinsSize.X / 2, 240), Color.Yellow);

                string bonusText = $"Level Bonus: +{_bonusScore}";
                Vector2 bonusSize = _font.MeasureString(bonusText);
                spriteBatch.DrawString(_font, bonusText,
                    new Vector2(640 - bonusSize.X / 2, 290), Color.Cyan);

                // Draw total score
                string scoreText = $"Total Score: {_finalScore}";
                Vector2 scoreSize = _font.MeasureString(scoreText);
                spriteBatch.DrawString(_font, scoreText,
                    new Vector2(640 - scoreSize.X / 2, 360), Color.White);

                // Draw hint
                string hint = "Click buttons below to continue";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, 430), Color.Gray);
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
