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
        private Button _backButton;
        private Button _saveButton;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        // Settings state
        private float _musicVolume = 0.7f;
        private float _soundFXVolume = 0.5f;
        private int _scrollOffset = 0; // For scrolling content
        private int _selectedControl = -1; // For rebinding controls
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
            // Back button - bottom left
            _backButton = new Button(
                new Rectangle(100, 660, 140, 40),
                "BACK",
                _font
            )
            {
                BackgroundColor = Color.White,
                HoverBackgroundColor = new Color(200, 200, 200),
                BorderColor = Color.Black,
                TextColor = Color.Black,
                TextScale = 0.5f
            };

            // Save button - bottom right
            _saveButton = new Button(
                new Rectangle(1040, 660, 140, 40),
                "SAVE",
                _font
            )
            {
                BackgroundColor = new Color(230, 0, 18),
                HoverBackgroundColor = new Color(200, 0, 10),
                BorderColor = Color.Black,
                TextColor = Color.White,
                TextScale = 0.5f
            };
        }

        public void Update(GameTime gameTime)
        {
            _backButton.Update(gameTime);
            _saveButton.Update(gameTime);

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
                        int playerIndex = (_selectedControl < 3) ? 1 : 2;
                        int actionIndex = _selectedControl % 3;
                        RemapKeyForPlayer(playerIndex, actionIndex, key);

                        _isWaitingForInput = false;
                        break;
                    }
                }

                _previousKeyboardState = currentKeyboardState;
                return;
            }

            // Back button
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || _backButton.WasPressed)
            {
                GameManager.Instance.ChangeScene(new MenuScene());
                _previousKeyboardState = currentKeyboardState;
                return;
            }

            // Save button
            if (_saveButton.WasPressed)
            {
                SoundManager.Instance.SetMusicVolume(_musicVolume);
                System.Diagnostics.Debug.WriteLine("[SETTINGS] Saved settings");
            }

            // Scroll up/down to navigate content
            if (currentKeyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _scrollOffset -= 30; // Scroll up
                if (_scrollOffset < 0) _scrollOffset = 0;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _scrollOffset += 30; // Scroll down
                if (_scrollOffset > 300) _scrollOffset = 300; // Max scroll
            }

            // Audio controls (Music volume)
            if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _musicVolume -= 0.1f;
                if (_musicVolume < 0) _musicVolume = 0;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _musicVolume += 0.1f;
                if (_musicVolume > 1) _musicVolume = 1;
            }

            // Controls selection
            if (currentKeyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)) // E to start selecting controls
            {
                if (_selectedControl == -1)
                    _selectedControl = 0;
                else
                    _selectedControl = -1;
            }

            // Navigate controls when in selection mode
            if (_selectedControl >= 0)
            {
                if (currentKeyboardState.IsKeyDown(Keys.W) && !_previousKeyboardState.IsKeyDown(Keys.W))
                {
                    _selectedControl--;
                    if (_selectedControl < 0) _selectedControl = 5;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.S) && !_previousKeyboardState.IsKeyDown(Keys.S))
                {
                    _selectedControl++;
                    if (_selectedControl > 5) _selectedControl = 0;
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _isWaitingForInput = true;
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
            device.Clear(new Color(240, 240, 240)); // Light background

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_font != null)
            {
                // Draw header bar (red background)
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 80), new Color(230, 0, 18));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 76, 1280, 4), Color.Black);
                }

                // Draw header content
                spriteBatch.DrawString(_font, "SETTINGS", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, "Configure Game Settings", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Draw scrollable content area with clipping
                int contentStartY = 100;
                int contentEndY = 645;

                // Store current scissor rectangle
                var previousScissor = spriteBatch.GraphicsDevice.ScissorRectangle;

                // Set scissor rectangle to content area
                spriteBatch.End();
                spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(40, contentStartY, 1200, contentEndY - contentStartY);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: new RasterizerState { ScissorTestEnable = true });

                // Draw all content (will be clipped by scissor)
                DrawAllContent(spriteBatch, contentStartY - _scrollOffset);

                spriteBatch.End();
                spriteBatch.GraphicsDevice.ScissorRectangle = previousScissor;
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                // Draw footer separator
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }

                // Draw scroll hint
                spriteBatch.DrawString(_font, "UP/DOWN: Scroll  |  LEFT/RIGHT: Volume  |  E: Select Control  |  W/S: Navigate  |  ENTER: Rebind", 
                    new Vector2(300, 660), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            // Draw buttons
            spriteBatch.Begin();
            _backButton.Draw(spriteBatch);
            _saveButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        private int DrawAllContent(SpriteBatch spriteBatch, int startY)
        {
            int y = startY;
            int sectionLabelX = 50;
            int contentX = 350;
            int spacing = 70;

            // ===== AUDIO SECTION =====
            DrawAudioSection(spriteBatch, ref y, sectionLabelX, contentX, spacing);

            y += 60; // Extra spacing between sections

            // ===== CONTROLS SECTION =====
            DrawControlsSection(spriteBatch, ref y, sectionLabelX);

            return y;
        }

        private void DrawAudioSection(SpriteBatch spriteBatch, ref int y, int sectionLabelX, int contentX, int spacing)
        {
            // Audio section card
            if (Game1.WhitePixel != null)
            {
                // Draw card background and borders
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y, 1200, 220), Color.White);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y, 1200, 2), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y + 218, 1200, 2), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y, 2, 220), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40 + 1198, y, 2, 220), Color.Black);

                // Section header tag - positioned inside the card top
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(50, y + 5, 150, 20), new Color(107, 140, 255)); // Sky blue
                spriteBatch.DrawString(_font, "AUDIO", new Vector2(60, y + 7), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }

            y += 35; // Adjusted spacing for tag inside card

            // Music Volume
            spriteBatch.DrawString(_font, "MUSIC VOLUME", new Vector2(sectionLabelX, y), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            
            string musicPercent = $"{(_musicVolume * 100):F0}%";
            spriteBatch.DrawString(_font, musicPercent, new Vector2(900, y), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            
            DrawVolumeBar(spriteBatch, contentX, y + 20, _musicVolume, 200);

            y += spacing;

            // Sound FX Volume
            spriteBatch.DrawString(_font, "SOUND FX", new Vector2(sectionLabelX, y), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            
            string sfxPercent = $"{(_soundFXVolume * 100):F0}%";
            spriteBatch.DrawString(_font, sfxPercent, new Vector2(900, y), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            
            DrawVolumeBar(spriteBatch, contentX, y + 20, _soundFXVolume, 200);

            y += 80;
        }

        private void DrawVolumeBar(SpriteBatch spriteBatch, int x, int y, float volume, int width)
        {
            if (Game1.WhitePixel == null) return;

            int height = 20;
            int fillWidth = (int)(width * volume);

            // Background
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, width, height), new Color(0, 0, 0));

            // Fill
            Color barColor = volume < 0.3f ? Color.Yellow : (volume < 0.7f ? new Color(230, 0, 18) : new Color(0, 150, 0));
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, fillWidth, height), barColor);

            // Border
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, width, 2), Color.Black);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y + height - 2, width, 2), Color.Black);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, 2, height), Color.Black);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 2, y, 2, height), Color.Black);
        }

        private void DrawControlsSection(SpriteBatch spriteBatch, ref int y, int sectionLabelX)
        {
            var p1KeyMap = InputSettings.Instance.P1_KeyMap;
            var p2KeyMap = InputSettings.Instance.P2_KeyMap;

            int leftColX = 50;
            int rightColX = 700;
            int spacing = 50;

            // Controls section card
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y, 1200, 310), Color.White);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y, 1200, 2), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y + 308, 1200, 2), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40, y, 2, 310), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(40 + 1198, y, 2, 310), Color.Black);

                // Section header tag - positioned inside the card top
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(50, y + 5, 200, 20), new Color(107, 179, 255));
                spriteBatch.DrawString(_font, "CONTROLS", new Vector2(60, y + 7), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }

            y += 35; // Adjusted spacing for tag inside card

            // Draw Player 1 controls
            DrawPlayerControls(spriteBatch, p1KeyMap, 1, leftColX, y, spacing);

            // Draw Player 2 controls
            DrawPlayerControls(spriteBatch, p2KeyMap, 2, rightColX, y, spacing);

            y += 180;
        }

        private void DrawPlayerControls(SpriteBatch spriteBatch, Dictionary<EGameAction, Keys> keyMap, int playerIndex, int x, int y, int spacing)
        {
            // Player header
            Color playerColor = playerIndex == 1 ? new Color(230, 0, 18) : new Color(0, 150, 0);
            spriteBatch.DrawString(_font, $"PLAYER {playerIndex}", new Vector2(x, y), playerColor, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

            y += 30;

            string[] actionNames = { "MOVE LEFT", "MOVE RIGHT", "JUMP" };

            for (int i = 0; i < 3; i++)
            {
                EGameAction action = (EGameAction)i;
                int controlIndex = (playerIndex == 1 ? 0 : 3) + i;
                Keys boundKey = keyMap.ContainsKey(action) ? keyMap[action] : Keys.None;
                
                bool isSelected = (_selectedControl == controlIndex);
                Color labelColor = isSelected ? new Color(230, 0, 18) : Color.Black;

                // Action name
                spriteBatch.DrawString(_font, actionNames[i], new Vector2(x, y), labelColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Key display
                string keyText = _isWaitingForInput && isSelected ? "PRESS KEY..." : boundKey.ToString();
                Color keyColor = _isWaitingForInput && isSelected ? new Color(230, 0, 18) : Color.Gray;
                
                if (Game1.WhitePixel != null && isSelected)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle((int)(x + 180), y - 2, 150, 22), new Color(230, 0, 18) * 0.2f);
                }

                spriteBatch.DrawString(_font, keyText, new Vector2(x + 185, y), keyColor, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);

                y += spacing;
            }
        }
    }
}
