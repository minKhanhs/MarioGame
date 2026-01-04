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
    public class PlayerNameInputScene : IScene
    {
        private SpriteFont _font;
        private string _playerName = "";
        private int _finalScore;
        private int _finalCoins;
        private int _enemiesDefeated;
        private int _levelsCompleted;
        private float _totalTime;
        private int _maxLevel;
        private List<Button> _buttons;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        public PlayerNameInputScene(int score, int coins, int enemies, int levels, float time, int maxLevel)
        {
            _finalScore = score;
            _finalCoins = coins;
            _enemiesDefeated = enemies;
            _levelsCompleted = levels;
            _totalTime = time;
            _maxLevel = maxLevel;
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

            int buttonWidth = 150;
            int buttonHeight = 50;
            int spacing = 20;

            int centerX = 640;
            int startX = centerX - buttonWidth / 2;
            int startY = 500;

            // Submit button
            _buttons.Add(new Button(
                new Rectangle(startX - 100, startY, buttonWidth, buttonHeight),
                "SUBMIT",
                _font
            ));

            // Clear button
            _buttons.Add(new Button(
                new Rectangle(startX + 100, startY, buttonWidth, buttonHeight),
                "CLEAR",
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

            // Text input
            foreach (Keys key in currentKeyboardState.GetPressedKeys())
            {
                if (!_previousKeyboardState.IsKeyDown(key))
                {
                    // Key was just pressed
                    if (key == Keys.Back && _playerName.Length > 0)
                    {
                        _playerName = _playerName.Substring(0, _playerName.Length - 1);
                    }
                    else if (key == Keys.Enter)
                    {
                        // Submit name
                        if (!string.IsNullOrWhiteSpace(_playerName))
                        {
                            SaveAndGoToHistory();
                        }
                    }
                    else if (key >= Keys.A && key <= Keys.Z && _playerName.Length < 20)
                    {
                        // Add letter
                        char letter = char.ToLower((char)('A' + (key - Keys.A)));
                        _playerName += letter;
                    }
                    else if (key == Keys.Space && _playerName.Length < 20)
                    {
                        _playerName += " ";
                    }
                }
            }

            // Handle button clicks
            if (_buttons[0].WasPressed) // Submit
            {
                if (!string.IsNullOrWhiteSpace(_playerName))
                {
                    SaveAndGoToHistory();
                }
            }
            else if (_buttons[1].WasPressed) // Clear
            {
                _playerName = "";
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
                // Draw title
                string title = "ENTER YOUR NAME";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 80), Color.Gold);

                // Draw input field background
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(350, 180, 580, 60), Color.DarkGray);
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(350, 180, 580, 60), Color.White * 0.3f);
                }

                // Draw player name
                spriteBatch.DrawString(_font, _playerName.Length > 0 ? _playerName : "Type your name...",
                    new Vector2(370, 195), _playerName.Length > 0 ? Color.White : Color.Gray);

                // Draw cursor
                if ((_playerName.Length > 0))
                {
                    Vector2 nameSize = _font.MeasureString(_playerName);
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle((int)(370 + nameSize.X), 195, 2, 30), Color.White);
                }

                // Draw stats
                string statsTitle = "Final Statistics:";
                Vector2 statsTitleSize = _font.MeasureString(statsTitle);
                spriteBatch.DrawString(_font, statsTitle,
                    new Vector2(640 - statsTitleSize.X / 2, 280), Color.Cyan);

                int statY = 330;
                string scoreText = $"Total Score: {_finalScore}";
                Vector2 scoreSize = _font.MeasureString(scoreText);
                spriteBatch.DrawString(_font, scoreText,
                    new Vector2(640 - scoreSize.X / 2, statY), Color.Yellow);

                string coinsText = $"Total Coins: {_finalCoins}";
                Vector2 coinsSize = _font.MeasureString(coinsText);
                spriteBatch.DrawString(_font, coinsText,
                    new Vector2(640 - coinsSize.X / 2, statY + 40), Color.Gold);

                string enemiesText = $"Enemies Defeated: {_enemiesDefeated}";
                Vector2 enemiesSize = _font.MeasureString(enemiesText);
                spriteBatch.DrawString(_font, enemiesText,
                    new Vector2(640 - enemiesSize.X / 2, statY + 80), Color.Red);

                string levelText = $"Max Level Reached: {_maxLevel}";
                Vector2 levelSize = _font.MeasureString(levelText);
                spriteBatch.DrawString(_font, levelText,
                    new Vector2(640 - levelSize.X / 2, statY + 120), Color.LimeGreen);

                // Draw hint
                string hint = "Press ENTER or click SUBMIT to save";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, 580), Color.Gray);
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

        private void SaveAndGoToHistory()
        {
            // Save record with game mode
            GameRecord record = new GameRecord(
                _playerName,
                _finalScore,
                _finalCoins,
                _enemiesDefeated,
                _levelsCompleted,
                _totalTime,
                _maxLevel,
                GameManager.Instance.GameMode  // Include game mode
            );
            GameRecordManager.Instance.AddRecord(record);

            // Go to history scene
            GameManager.Instance.ClearSavedGameState();
            GameManager.Instance.ChangeScene(new PlayHistoryScene());
        }
    }
}
