using MarioGame.src._Core;
using MarioGame.src._Input;
using MarioGame.src._UI;
using MarioGame._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class SettingsScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        // Settings state
        private float _musicVolume = 0.8f;
        private bool _isMuted = false;
        private int _currentTab = 0; // 0 = Audio, 1 = Controls P1, 2 = Controls P2
        private int _selectedControl = -1; // For rebinding
        private bool _isWaitingForInput = false;

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

            int buttonWidth = 140;
            int buttonHeight = 40;
            int spacing = 15;

            int centerX = 640;
            int startY = 670;

            // Tab buttons
            _buttons.Add(new Button(
                new Rectangle(centerX - 300, startY, buttonWidth, buttonHeight),
                "AUDIO",
                _font
            ));

            _buttons.Add(new Button(
                new Rectangle(centerX - 140, startY, buttonWidth, buttonHeight),
                "CTRL P1",
                _font
            ));

            _buttons.Add(new Button(
                new Rectangle(centerX + 20, startY, buttonWidth, buttonHeight),
                "CTRL P2",
                _font
            ));

            // Back button
            _buttons.Add(new Button(
                new Rectangle(centerX + 180, startY, buttonWidth, buttonHeight),
                "BACK",
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

            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Handle rebinding input
            if (_isWaitingForInput)
            {
                foreach (Keys key in currentKeyboardState.GetPressedKeys())
                {
                    if (!_previousKeyboardState.IsKeyDown(key))
                    {
                        // Key was just pressed - rebind it
                        if (_currentTab == 1) // P1 Controls
                        {
                            RemapKeyForPlayer(1, _selectedControl, key);
                        }
                        else if (_currentTab == 2) // P2 Controls
                        {
                            RemapKeyForPlayer(2, _selectedControl, key);
                        }

                        _isWaitingForInput = false;
                        _selectedControl = -1;
                        break;
                    }
                }

                _previousKeyboardState = currentKeyboardState;
                return;
            }

            // Tab navigation
            if (_buttons[0].WasPressed) // Audio
                _currentTab = 0;
            else if (_buttons[1].WasPressed) // Controls P1
                _currentTab = 1;
            else if (_buttons[2].WasPressed) // Controls P2
                _currentTab = 2;
            else if (_buttons[3].WasPressed) // Back
            {
                GameManager.Instance.ChangeScene(new MenuScene());
                _previousKeyboardState = currentKeyboardState;
                return;
            }

            // Tab-specific controls
            if (_currentTab == 0) // Audio tab
            {
                // Volume control
                if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                {
                    _musicVolume -= 0.1f;
                    if (_musicVolume < 0) _musicVolume = 0;
                    SoundManager.Instance.SetMusicVolume(_musicVolume);
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                {
                    _musicVolume += 0.1f;
                    if (_musicVolume > 1) _musicVolume = 1;
                    SoundManager.Instance.SetMusicVolume(_musicVolume);
                }

                // Mute toggle
                if (currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    _isMuted = !_isMuted;
                    if (_isMuted)
                        SoundManager.Instance.SetMusicVolume(0);
                    else
                        SoundManager.Instance.SetMusicVolume(_musicVolume);
                }
            }
            else if (_currentTab == 1 || _currentTab == 2) // Controls tabs
            {
                int playerIndex = _currentTab == 1 ? 1 : 2;

                if (currentKeyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
                {
                    _selectedControl--;
                    if (_selectedControl < 0) _selectedControl = 5; // 6 actions
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
                {
                    _selectedControl++;
                    if (_selectedControl > 5) _selectedControl = 0;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    if (_selectedControl >= 0)
                    {
                        _isWaitingForInput = true;
                    }
                }
            }

            _previousKeyboardState = currentKeyboardState;
        }

        private void RemapKeyForPlayer(int playerIndex, int actionIndex, Keys newKey)
        {
            EGameAction action = (EGameAction)actionIndex;
            InputSettings.Instance.RemapKey(playerIndex, action, newKey);
            System.Diagnostics.Debug.WriteLine($"[SETTINGS] Player {playerIndex} - {action} remapped to {newKey}");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin();

            if (_font != null)
            {
                // Draw title
                string title = "SETTINGS";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 15), Color.Gold);

                // Draw separator
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(50, 60, 1180, 2), Color.Gold);
                }

                if (_currentTab == 0)
                {
                    DrawAudioSettings(spriteBatch);
                }
                else if (_currentTab == 1)
                {
                    DrawControlSettings(spriteBatch, 1);
                }
                else if (_currentTab == 2)
                {
                    DrawControlSettings(spriteBatch, 2);
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

        private void DrawAudioSettings(SpriteBatch spriteBatch)
        {
            int y = 120;
            int spacing = 60;

            // Music Volume
            spriteBatch.DrawString(_font, "MUSIC VOLUME:", new Vector2(100, y), Color.Cyan);
            
            // Volume bar
            if (Game1.WhitePixel != null)
            {
                // Background
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(350, y + 5, 300, 30), Color.DarkGray);
                
                // Fill
                int fillWidth = (int)(300 * _musicVolume);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(350, y + 5, fillWidth, 30), Color.LimeGreen);
                
                // Border
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(350, y + 5, 300, 30), Color.White * 0.5f);
            }

            // Volume percentage
            string volumeText = $"{(_musicVolume * 100):F0}%";
            Vector2 volumeSize = _font.MeasureString(volumeText);
            spriteBatch.DrawString(_font, volumeText, new Vector2(680, y + 10), Color.Yellow);

            y += spacing;

            // Mute toggle
            spriteBatch.DrawString(_font, "MUTE:", new Vector2(100, y), Color.Cyan);
            string muteStatus = _isMuted ? "ON" : "OFF";
            Color muteColor = _isMuted ? Color.Red : Color.LimeGreen;
            spriteBatch.DrawString(_font, muteStatus, new Vector2(350, y), muteColor);

            y += spacing + 20;

            // Instructions
            spriteBatch.DrawString(_font, "LEFT/RIGHT to adjust volume", new Vector2(100, y), Color.Gray);
            y += 40;
            spriteBatch.DrawString(_font, "SPACE to toggle mute", new Vector2(100, y), Color.Gray);
        }

        private void DrawControlSettings(SpriteBatch spriteBatch, int playerIndex)
        {
            var keyMap = playerIndex == 1 ? InputSettings.Instance.P1_KeyMap : InputSettings.Instance.P2_KeyMap;

            int y = 120;
            int spacing = 50;
            string[] actionNames = { "MOVE LEFT", "MOVE RIGHT", "JUMP", "RUN", "ATTACK", "PAUSE" };

            spriteBatch.DrawString(_font, $"PLAYER {playerIndex} CONTROLS:", new Vector2(100, y), Color.Cyan);
            y += 50;

            for (int i = 0; i < 6; i++)
            {
                EGameAction action = (EGameAction)i;
                Keys boundKey = keyMap.ContainsKey(action) ? keyMap[action] : Keys.None;

                Color textColor = (i == _selectedControl) ? Color.Yellow : Color.White;
                string selectionMarker = (i == _selectedControl) ? ">> " : "   ";

                spriteBatch.DrawString(_font, selectionMarker + actionNames[i], new Vector2(120, y), textColor);
                
                string keyText = _isWaitingForInput && i == _selectedControl ? "Press a key..." : boundKey.ToString();
                Color keyColor = _isWaitingForInput && i == _selectedControl ? Color.Red : Color.Cyan;
                spriteBatch.DrawString(_font, keyText, new Vector2(450, y), keyColor);

                y += spacing;
            }

            // Instructions
            int instructY = y + 50;
            spriteBatch.DrawString(_font, "UP/DOWN to select | ENTER to rebind", new Vector2(100, instructY), Color.Gray);
        }
    }
}
