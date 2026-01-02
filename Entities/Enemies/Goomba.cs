using MarioGame.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Enemies
{
    public class Goomba : Enemy
    {
        public Goomba(Vector2 position) : base(position, new Vector2(16, 16))
        {
            _moveSpeed = 40f;
            ScoreValue = 100;
        }

        public override void Stomp()
        {
            // Goomba gets squished
            Size = new Vector2(16, 8);
            base.Stomp();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsVisible) return;

            Rectangle destRect = new Rectangle(
                (int)(Position.X - cameraOffset.X),
                (int)(Position.Y - cameraOffset.Y),
                (int)Size.X,
                (int)Size.Y
            );

            // TODO: Draw actual sprite
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, destRect, Color.Brown);
        }
    }
}