using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Systems.Rendering
{
    public class SpriteRenderer
    {
        private SpriteBatch _spriteBatch;

        public SpriteRenderer(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRect,
            Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
        {
            _spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, effects, 0);
        }

        public void DrawRectangle(Rectangle rect, Color color)
        {
            Texture2D pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            _spriteBatch.Draw(pixel, rect, color);
        }
    }
}