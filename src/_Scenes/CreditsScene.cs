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
    public class CreditsScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

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
            int buttonHeight = 45;

            int centerX = 640;
            int startX = centerX - buttonWidth / 2;
            int startY = 670;

            // Back button
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
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

            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Handle button click or ESC
            if (_buttons[0].WasPressed || (currentKeyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)))
            {
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
                // Draw title
                string title = "CREDITS";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 20), Color.Gold);

                // Draw separator
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(50, 65, 1180, 2), Color.Gold);
                }

                // Draw credits content
                int contentY = 100;
                int spacing = 50;

                // Development Team
                DrawSection(spriteBatch, "DEVELOPMENT TEAM", contentY, Color.Cyan);
                contentY += 35;
                DrawCredit(spriteBatch, "Lead Developer", "Design & Programming", contentY);
                contentY += spacing;
                DrawCredit(spriteBatch, "Graphics Artist", "Sprite & Asset Design", contentY);
                contentY += spacing;
                DrawCredit(spriteBatch, "Audio Designer", "Music & Sound Effects", contentY);
                contentY += spacing;

                // Game Design
                DrawSection(spriteBatch, "GAME DESIGN", contentY, Color.LimeGreen);
                contentY += 35;
                DrawCredit(spriteBatch, "Level Designer", "Map Creation & Gameplay", contentY);
                contentY += spacing;
                DrawCredit(spriteBatch, "Game Designer", "Mechanics & Balance", contentY);
                contentY += spacing;

                // Quality Assurance
                DrawSection(spriteBatch, "QUALITY ASSURANCE", contentY, Color.Yellow);
                contentY += 35;
                DrawCredit(spriteBatch, "QA Lead", "Testing & Bug Reports", contentY);
                contentY += spacing;

                // Footer
                contentY += 30;
                string footer = "Special Thanks to Nintendo for the original Mario Bros inspiration";
                Vector2 footerSize = _font.MeasureString(footer);
                spriteBatch.DrawString(_font, footer,
                    new Vector2(640 - footerSize.X / 2, contentY), Color.Gray);

                contentY += 40;
                string footer2 = "Built with MonoGame & .NET 8";
                Vector2 footer2Size = _font.MeasureString(footer2);
                spriteBatch.DrawString(_font, footer2,
                    new Vector2(640 - footer2Size.X / 2, contentY), Color.Gray);

                // Hint
                string hint = "Press ESC or click MAIN MENU to return";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, 620), Color.DarkGray);
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

        private void DrawSection(SpriteBatch spriteBatch, string sectionName, int y, Color color)
        {
            Vector2 sectionSize = _font.MeasureString(sectionName);
            spriteBatch.DrawString(_font, sectionName,
                new Vector2(640 - sectionSize.X / 2, y), color);
        }

        private void DrawCredit(SpriteBatch spriteBatch, string role, string description, int y)
        {
            spriteBatch.DrawString(_font, role, new Vector2(200, y), Color.White);
            spriteBatch.DrawString(_font, description, new Vector2(220, y + 20), Color.LightGray);
        }
    }
}
