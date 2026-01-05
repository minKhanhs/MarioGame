using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class PauseScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private int _levelIndex;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        public PauseScene(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void LoadContent()
        {
            // Prevent loading content multiple times
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
            int startY = 300;

            // Resume button
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
                "RESUME",
                _font
            ));

            // Restart Level button
            _buttons.Add(new Button(
                new Rectangle(startX, startY + buttonHeight + spacing, buttonWidth, buttonHeight),
                "RESTART",
                _font
            ));

            // Main Menu button
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 2, buttonWidth, buttonHeight),
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

            // Skip first update to avoid ESC key collision
            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Resume with ESC key
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                System.Diagnostics.Debug.WriteLine("[RESUME] Returning to gameplay");
                ResumeGame();
                _previousKeyboardState = currentKeyboardState;
                return;
            }

            // Button clicks
            if (_buttons[0].WasPressed) // Resume
            {
                System.Diagnostics.Debug.WriteLine("[RESUME] Resume button clicked");
                ResumeGame();
            }
            else if (_buttons[1].WasPressed) // Restart
            {
                System.Diagnostics.Debug.WriteLine("[RESTART] Level restarting");
                GameManager.Instance.ClearSavedGameState();
                
                // Restart with correct game mode
                if (GameManager.Instance.GameMode == 2)
                {
                    GameManager.Instance.ChangeScene(new TwoPlayerGameplayScene(_levelIndex));
                }
                else
                {
                    GameManager.Instance.ChangeScene(new GameplayScene(_levelIndex));
                }
            }
            else if (_buttons[2].WasPressed) // Main Menu
            {
                System.Diagnostics.Debug.WriteLine("[MENU] Going to main menu");
                GameManager.Instance.ClearSavedGameState();
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;

            spriteBatch.Begin();

            // Draw semi-transparent overlay
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.7f);
            }

            if (_font != null)
            {
                // Draw PAUSED title
                string title = "PAUSED";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title, new Vector2(640 - titleSize.X / 2, 150), Color.Yellow);

                // Draw hint
                string hint = "Press ESC to Resume or click buttons below";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint, new Vector2(640 - hintSize.X / 2, 220), Color.White);
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

        private void ResumeGame()
        {
            // Return to gameplay with correct game mode WITHOUT reloading content
            if (GameManager.Instance.GameMode == 2)
            {
                GameManager.Instance.ChangeScene(new TwoPlayerGameplayScene(_levelIndex));
            }
            else
            {
                GameManager.Instance.ChangeScene(new GameplayScene(_levelIndex));
            }
        }
    }
}
