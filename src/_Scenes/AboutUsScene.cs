using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class AboutUsScene : IScene
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
                spriteBatch.DrawString(_font, "ABOUT US", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, "SUPER MARIO BROS GAME", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Content section
                int contentY = 110;
                int lineHeight = 30;

                // Game Info
                spriteBatch.DrawString(_font, "Game Title: Super Mario Bros Remake", new Vector2(80, contentY), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, "Platform: MonoGame & .NET 8", new Vector2(80, contentY), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, "Genre: Platform Adventure Game", new Vector2(80, contentY), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, "Year: 2025", new Vector2(80, contentY), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

                // Separator
                contentY += 25;
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(80, contentY, 1100, 2), new Color(64, 64, 64));
                }

                // Features section
                contentY += 20;
                spriteBatch.DrawString(_font, "Features:", new Vector2(80, contentY), new Color(230, 0, 18), 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                contentY += 30;

                string[] features = new[]
                {
                    "Classic side-scrolling platformer gameplay",
                    "Multiple challenging levels with increasing difficulty",
                    "Power-up system (mushrooms for size increase)",
                    "Enemy variety with different behaviors",
                    "Collectible coins and scoring system",
                    "Pause and resume functionality",
                    "Achievement tracking system",
                    "Play history and high score records"
                };

                foreach (var feature in features)
                {
                    // Truncate long features to prevent overlap
                    string displayFeature = feature.Length > 55 ? feature.Substring(0, 52) + "..." : feature;
                    spriteBatch.DrawString(_font, "• " + displayFeature, new Vector2(100, contentY), new Color(200, 200, 200), 0f, Vector2.Zero, 0.33f, SpriteEffects.None, 0f);
                    contentY += 26;
                }

                // Footer
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }

                spriteBatch.DrawString(_font, "Special thanks to Nintendo for the original Mario Bros inspiration",
                    new Vector2(300, 660), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            // Draw back button
            spriteBatch.Begin();
            _backButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
