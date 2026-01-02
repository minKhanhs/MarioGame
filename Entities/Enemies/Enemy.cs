using MarioGame.Core;
using MarioGame.Entities.Base;
using MarioGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Enemies
{
    public abstract class Enemy : Entity
    {
        public EnemyState State { get; protected set; } = EnemyState.Walking;
        public int ScoreValue { get; protected set; } = 100;

        protected float _moveSpeed = 50f;
        protected int _direction = -1; // -1 = left, 1 = right

        public Enemy(Vector2 position, Vector2 size) : base(position, size)
        {
        }

        public override void Update(float deltaTime)
        {
            switch (State)
            {
                case EnemyState.Walking:
                    UpdateWalking(deltaTime);
                    break;
                case EnemyState.Shell:
                    UpdateShell(deltaTime);
                    break;
                case EnemyState.Sliding:
                    UpdateSliding(deltaTime);
                    break;
                case EnemyState.Dead:
                    UpdateDead(deltaTime);
                    break;
            }

            base.Update(deltaTime);
        }

        protected virtual void UpdateWalking(float deltaTime)
        {
            Velocity = new Vector2(_direction * _moveSpeed, Velocity.Y);
        }

        protected virtual void UpdateShell(float deltaTime)
        {
            Velocity = new Vector2(0, Velocity.Y);
        }

        protected virtual void UpdateSliding(float deltaTime)
        {
            Velocity = new Vector2(_direction * _moveSpeed * 3, Velocity.Y);
        }

        protected virtual void UpdateDead(float deltaTime)
        {
        }

        public virtual void Stomp()
        {
            State = EnemyState.Dead;
            GameManager.Instance.AddScore(ScoreValue);
            SoundManager.Instance.PlaySound("stomp");
            Destroy();
        }

        public virtual void HitByFireball()
        {
            State = EnemyState.Dead;
            GameManager.Instance.AddScore(ScoreValue);
            Destroy();
        }

        protected void ChangeDirection()
        {
            _direction *= -1;
        }

        public override void OnCollision(ICollidable other, CollisionSide side)
        {
            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
                Velocity = new Vector2(Velocity.X, 0);
            }
            else if (side == CollisionSide.Left || side == CollisionSide.Right)
            {
                ChangeDirection();
            }
        }
    }
}
