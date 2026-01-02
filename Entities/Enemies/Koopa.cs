using MarioGame.Core;
using MarioGame.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Enemies
{
    public class Koopa : Enemy
    {
        public Koopa(Vector2 position) : base(position, new Vector2(16, 24))
        {
            _moveSpeed = 50f;
            ScoreValue = 200;
        }

        public override void Stomp()
        {
            if (State == EnemyState.Walking)
            {
                // Enter shell state
                State = EnemyState.Shell;
                Size = new Vector2(16, 16);
                Velocity = Vector2.Zero;
            }
            else if (State == EnemyState.Shell)
            {
                // Kick shell
                State = EnemyState.Sliding;
                _direction = 1; // Kick direction based on player position
                GameManager.Instance.AddScore(ScoreValue);
            }
            else if (State == EnemyState.Sliding)
            {
                // Stop shell
                State = EnemyState.Shell;
                Velocity = Vector2.Zero;
            }
        }

        protected override void UpdateShell(float deltaTime)
        {
            base.UpdateShell(deltaTime);
            // Could add timer to come back out of shell
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

            Color color = State == EnemyState.Shell ? Color.DarkGreen : Color.Green;

            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, destRect, color);
        }
    }
}