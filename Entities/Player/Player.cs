using MarioGame.Core;
using MarioGame.Entities.Base;
using MarioGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Player
{
    public class Player : Entity
    {
        public PowerUpState PowerUp { get; private set; } = PowerUpState.Small;
        public float MoveSpeed { get; set; } = 150f;
        public float JumpForce { get; set; } = -400f;
        public bool IsDead { get; private set; }

        private IPlayerState _currentState;
        private PlayerController _controller;
        private int _playerIndex;

        private float _shootCooldown = 0f;
        private const float SHOOT_COOLDOWN_TIME = 0.3f;

        public Player(Vector2 position, int playerIndex = 1)
            : base(position, new Vector2(16, 16))
        {
            _playerIndex = playerIndex;
            _controller = new PlayerController(this, playerIndex);
            _currentState = new IdleState();
            _currentState.Enter(this);
        }

        public void ChangeState(IPlayerState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        public override void Update(float deltaTime)
        {
            if (IsDead)
            {
                _currentState?.Update(this, deltaTime);
                base.Update(deltaTime);
                return;
            }

            _controller.HandleInput(deltaTime);
            _currentState?.Update(this, deltaTime);

            if (_shootCooldown > 0)
                _shootCooldown -= deltaTime;

            // Apply friction when on ground
            if (IsGrounded)
            {
                ApplyFriction(0.85f);
            }

            base.Update(deltaTime);

            // Update size based on power up
            UpdateSize();
        }

        private void UpdateSize()
        {
            switch (PowerUp)
            {
                case PowerUpState.Small:
                    Size = new Vector2(16, 16);
                    break;
                case PowerUpState.Big:
                case PowerUpState.Fire:
                    Size = new Vector2(16, 32);
                    break;
            }
        }

        public void Jump()
        {
            Velocity = new Vector2(Velocity.X, JumpForce);
            IsGrounded = false;
            ChangeState(new JumpingState());
            SoundManager.Instance.PlaySound("jump");
        }

        public void Shoot()
        {
            if (_shootCooldown > 0) return;

            _shootCooldown = SHOOT_COOLDOWN_TIME;
            // TODO: Create fireball projectile
            SoundManager.Instance.PlaySound("shoot");
        }

        public void PowerUpTo(PowerUpState newPowerUp)
        {
            if (newPowerUp > PowerUp)
            {
                PowerUp = newPowerUp;
                SoundManager.Instance.PlaySound("powerup");
            }
        }

        public void TakeDamage()
        {
            if (PowerUp == PowerUpState.Small)
            {
                Die();
            }
            else
            {
                PowerUp = PowerUpState.Small;
                // Invincibility frames
            }
        }

        public void Die()
        {
            IsDead = true;
            ChangeState(new DeadState());
            SoundManager.Instance.PlaySound("gameover");
        }

        public override void OnCollision(ICollidable other, CollisionSide side)
        {
            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
                Velocity = new Vector2(Velocity.X, 0);
            }
            else if (side == CollisionSide.Top)
            {
                Velocity = new Vector2(Velocity.X, 0);
            }
            else if (side == CollisionSide.Left || side == CollisionSide.Right)
            {
                Velocity = new Vector2(0, Velocity.Y);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsVisible) return;

            Color drawColor = Color.White;
            Rectangle destRect = new Rectangle(
                (int)(Position.X - cameraOffset.X),
                (int)(Position.Y - cameraOffset.Y),
                (int)Size.X,
                (int)Size.Y
            );

            // TODO: Draw actual sprite with animation
            // For now, draw colored rectangle
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { drawColor });

            spriteBatch.Draw(pixel, destRect, Color.Red);
        }
    }
}