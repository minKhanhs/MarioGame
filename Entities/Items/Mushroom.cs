using MarioGame.Core;
using MarioGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Items
{
    public class Mushroom : Item
    {
        private float _moveSpeed = 60f;
        private int _direction = 1;

        public Mushroom(Vector2 position) : base(position, new Vector2(16, 16))
        {
        }

        public override void OnCollect(Player.Player player)
        {
            player.PowerUpTo(PowerUpState.Big);
            GameManager.Instance.AddScore(1000);
            SoundManager.Instance.PlaySound("powerup");
        }

        public override void Update(float deltaTime)
        {
            Velocity = new Vector2(_direction * _moveSpeed, Velocity.Y);
            base.Update(deltaTime);
        }

        public override void OnCollision(Entities.Base.ICollidable other, Entities.Base.CollisionSide side)
        {
            if (other is Player.Player)
            {
                base.OnCollision(other, side);
                return;
            }

            if (side == Entities.Base.CollisionSide.Left || side == Entities.Base.CollisionSide.Right)
            {
                _direction *= -1;
            }

            if (side == Entities.Base.CollisionSide.Bottom)
            {
                IsGrounded = true;
                Velocity = new Vector2(Velocity.X, 0);
            }
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

            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, destRect, Color.Red);
        }
    }
}