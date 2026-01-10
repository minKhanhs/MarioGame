using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MarioGame.src._UI
{
    public class Button
    {
        public Rectangle Bounds { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public Color BorderColor { get; set; }
        public Color HoverColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color HoverBackgroundColor { get; set; }
        public SpriteFont Font { get; set; }
        public float TextScale { get; set; } = 1.0f; // Add text scale property
        public bool IsHovered { get; set; }
        public bool WasPressed { get; set; }
        public object Tag { get; set; }

        private MouseState _previousMouseState;
        private const int BorderWidth = 4;
        private const int ShadowOffset = 4;

        public Button(Rectangle bounds, string text, SpriteFont font)
        {
            Bounds = bounds;
            Text = text;
            Font = font;
            TextColor = Color.White;
            BorderColor = Color.White;
            HoverColor = Color.Yellow;
            BackgroundColor = new Color(32, 32, 32); // #202020
            HoverBackgroundColor = Color.Yellow;
            IsHovered = false;
            WasPressed = false;
        }

        public void Update(GameTime gameTime)
        {
            MouseState currentMouse = Mouse.GetState();
            IsHovered = Bounds.Contains(currentMouse.Position);

            // Detect click (mouse button released after being pressed)
            WasPressed = false;
            if (IsHovered && currentMouse.LeftButton == ButtonState.Released && 
                _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                WasPressed = true;
            }

            _previousMouseState = currentMouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.WhitePixel == null)
                return;

            // Draw shadow (retro style offset)
            if (!IsHovered)
            {
                spriteBatch.Draw(
                    Game1.WhitePixel,
                    new Rectangle(Bounds.X + ShadowOffset, Bounds.Y + ShadowOffset, Bounds.Width, Bounds.Height),
                    Color.Black * 0.5f
                );
            }

            // Draw background
            Color bgColor = IsHovered ? HoverBackgroundColor : BackgroundColor;
            spriteBatch.Draw(
                Game1.WhitePixel,
                Bounds,
                bgColor
            );

            // Draw border with hover color
            Color borderColor = IsHovered ? Color.White : BorderColor;
            DrawRectangleOutline(spriteBatch, Bounds, borderColor, BorderWidth);

            // Draw text centered in button with scale support
            if (Font != null)
            {
                Vector2 textSize = Font.MeasureString(Text) * TextScale;
                Vector2 textPosition = new Vector2(
                    Bounds.Center.X - textSize.X / 2,
                    Bounds.Center.Y - textSize.Y / 2
                );

                // Change text color on hover
                Color currentTextColor = IsHovered ? Color.Black : TextColor;
                spriteBatch.DrawString(Font, Text, textPosition, currentTextColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
            }
        }

        private void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
        {
            // Top line
            spriteBatch.Draw(
                Game1.WhitePixel,
                new Rectangle(rect.X, rect.Y, rect.Width, thickness),
                color
            );

            // Bottom line
            spriteBatch.Draw(
                Game1.WhitePixel,
                new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness),
                color
            );

            // Left line
            spriteBatch.Draw(
                Game1.WhitePixel,
                new Rectangle(rect.X, rect.Y, thickness, rect.Height),
                color
            );

            // Right line
            spriteBatch.Draw(
                Game1.WhitePixel,
                new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height),
                color
            );
        }
    }
}
