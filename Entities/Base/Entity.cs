using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Base
{
    public abstract class Entity : GameObject, ICollidable
    {
        public Vector2 Velocity { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsSolid { get; protected set; } = true;
        public bool AffectedByGravity { get; protected set; } = true;

        protected bool _facingRight = true;
        protected float _gravity = 980f;
        protected float _maxFallSpeed = 600f;

        public Entity(Vector2 position, Vector2 size) : base(position, size)
        {
        }

        public override void Update(float deltaTime)
        {
            if (AffectedByGravity)
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * deltaTime);

                if (Velocity.Y > _maxFallSpeed)
                {
                    Velocity = new Vector2(Velocity.X, _maxFallSpeed);
                }
            }

            Position += Velocity * deltaTime;
        }

        public abstract void OnCollision(ICollidable other, CollisionSide side);

        protected void ApplyFriction(float friction)
        {
            Velocity = new Vector2(Velocity.X * friction, Velocity.Y);
        }
    }
}