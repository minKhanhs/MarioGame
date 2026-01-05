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
        private Button _backButton;
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
            _backButton = new Button(
                new Rectangle(640 - 70, 680, 140, 40),
                "BACK",
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

            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Back button
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || _backButton.WasPressed)
            {
                GameManager.Instance.ChangeScene(new MenuScene());
                return;
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
                // Header bar
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 80), new Color(230, 0, 18));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 76, 1280, 4), Color.Black);
                }

                // Title
                spriteBatch.DrawString(_font, "CREDITS", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, "DEVELOPMENT TEAM", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Content sections - spread them out more
                int contentY = 110;
                int sectionSpacing = 110; // Increased from 90

                DrawCreditSection(spriteBatch, "DEVELOPMENT", contentY, new Color(230, 0, 18), new[]
                {
                    "Lead Developer",
                    "Game Design & Programming",
                    "",
                    "Graphics Artist", 
                    "Sprite & Asset Design",
                    ""
                });

                contentY += sectionSpacing;

                DrawCreditSection(spriteBatch, "LEVEL DESIGN", contentY, new Color(67, 176, 71), new[]
                {
                    "Level Designer",
                    "Map Creation & Gameplay",
                    "",
                    "Game Designer",
                    "Mechanics & Balance"
                });

                contentY += sectionSpacing;

                DrawCreditSection(spriteBatch, "QUALITY ASSURANCE", contentY, new Color(251, 208, 0), new[]
                {
                    "QA Lead",
                    "Testing & Bug Reports"
                });

                // Footer
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }

                spriteBatch.DrawString(_font, "Built with MonoGame & .NET 8  |  2024",
                    new Vector2(350, 660), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            // Draw back button
            spriteBatch.Begin();
            _backButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawCreditSection(SpriteBatch spriteBatch, string sectionTitle, int startY, Color sectionColor, string[] credits)
        {
            // Section title
            spriteBatch.DrawString(_font, sectionTitle, new Vector2(80, startY), sectionColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            int creditY = startY + 32;
            foreach (var credit in credits)
            {
                // Skip empty lines
                if (string.IsNullOrEmpty(credit))
                {
                    creditY += 8; // Small gap
                    continue;
                }

                spriteBatch.DrawString(_font, "  " + credit, new Vector2(100, creditY), new Color(200, 200, 200), 0f, Vector2.Zero, 0.32f, SpriteEffects.None, 0f);
                creditY += 20;
            }
        }
    }
}
