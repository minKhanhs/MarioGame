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
            int startY = 530;

            // Submit button
            _buttons.Add(new Button(
                new Rectangle(startX - 100, startY, buttonWidth, buttonHeight),
                "SUBMIT",
                _font
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(230, 0, 18),
                BorderColor = Color.White,
                TextColor = Color.White
            });

            // Clear button
            _buttons.Add(new Button(
                new Rectangle(startX + 100, startY, buttonWidth, buttonHeight),
                "CLEAR",
                _font
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = Color.White,
                BorderColor = Color.White,
                TextColor = Color.White
            });
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
                spriteBatch.DrawString(_font, "GAME COMPLETE!", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, "ENTER YOUR NAME", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Content section
                int contentY = 110;
                int lineHeight = 30;

                // Input field label
                spriteBatch.DrawString(_font, "PLAYER NAME:", new Vector2(100, contentY), Color.Cyan, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;

                // Draw input field background
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(100, contentY, 1080, 50), new Color(32, 32, 32));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(100, contentY, 1080, 2), new Color(100, 100, 100));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(100, contentY + 48, 1080, 2), new Color(100, 100, 100));
                }

                // Draw player name
                spriteBatch.DrawString(_font, _playerName.Length > 0 ? _playerName : "Type your name...",
                    new Vector2(120, contentY + 12), _playerName.Length > 0 ? Color.White : Color.Gray, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

                // Separator
                contentY += 65;
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(100, contentY, 1080, 2), new Color(100, 100, 100));
                }

                // Statistics header
                contentY += 20;
                spriteBatch.DrawString(_font, "FINAL STATISTICS:", new Vector2(100, contentY), Color.Cyan, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
                contentY += 35;

                spriteBatch.DrawString(_font, $"Total Score: {_finalScore}", new Vector2(120, contentY), Color.Yellow, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Total Coins: {_finalCoins}", new Vector2(120, contentY), Color.Gold, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Enemies Defeated: {_enemiesDefeated}", new Vector2(120, contentY), Color.Red, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Levels Completed: {_levelsCompleted}", new Vector2(120, contentY), Color.LimeGreen, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

                // Footer
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }
                spriteBatch.DrawString(_font, "Click buttons below to continue",
                    new Vector2(500, 660), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
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
