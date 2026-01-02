using MarioGame.Entities.Base;
using Microsoft.Xna.Framework;

namespace MarioGame.Entities.Items
{
    public abstract class Item : Entity
    {
        public bool IsCollected { get; protected set; }

        public Item(Vector2 position, Vector2 size) : base(position, size)
        {
        }

        public abstract void OnCollect(Player.Player player);

        public override void OnCollision(ICollidable other, CollisionSide side)
        {
            if (other is Player.Player player && !IsCollected)
            {
                OnCollect(player);
                IsCollected = true;
                Destroy();
            }

            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
                Velocity = new Vector2(Velocity.X, 0);
            }
        }
    }
}
