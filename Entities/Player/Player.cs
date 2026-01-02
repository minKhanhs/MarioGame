using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MarioGame.Core;
using MarioGame.Entities.Base;
using System;

namespace MarioGame.Entities.Player
{
    public class Player : GameObject, ICollidable
    {
        // Player properties
        public int PlayerId { get; private set; }
        public int Lives { get; set; } = 3;
        public int Score { get; set; } = 0;
        public int Coins { get; set; } = 0;

        // State
        public PlayerStateType CurrentState { get; private set; } = PlayerStateType.Small;
        private Direction facingDirection = Direction.Right;

        // Movement
        private float moveSpeed = Constants.PLAYER_SPEED;
        private float jumpSpeed = Constants.PLAYER_JUMP_SPEED;
        private bool canJump = true;
        private bool isRunning = false;

        // Input keys
        private Keys leftKey, rightKey, jumpKey, shootKey;

        // Animation
        private float animationTimer = 0f;
        private int currentFrame = 0;
        private const float FRAME_TIME = 0.1f;

        // Invincibility (after taking damage)
        private float invincibilityTimer = 0f;
        private const float INVINCIBILITY_TIME = 2f;
        public bool IsInvincible => invincibilityTimer > 0;

        public Player(Vector2 position, int playerId)
            : base(position, new Vector2(32, 32))
        {
            PlayerId = playerId;
            Layer = Constants.LAYER_PLAYER;
            SetupControls();
        }

        private void SetupControls()
        {
            if (PlayerId == 1)
            {
                leftKey = Keys.A;
                rightKey = Keys.D;
                jumpKey = Keys.W;
                shootKey = Keys.J;
            }
            else
            {
                leftKey = Keys.Left;
                rightKey = Keys.Right;
                jumpKey = Keys.Up;
                shootKey = Keys.RightControl;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle input
            HandleInput();

            // Update invincibility
            if (invincibilityTimer > 0)
            {
                invincibilityTimer -= deltaTime;
            }

            // Update animation
            UpdateAnimation(deltaTime);

            // Call base update (physics)
            base.Update(gameTime);

            // Check if fell off map
            if (Position.Y > 1000)
            {
                Die();
            }
        }

        private void HandleInput()
        {
            KeyboardState keyboard = Keyboard.GetState();

            // Horizontal movement
            float horizontalInput = 0f;

            if (keyboard.IsKeyDown(leftKey))
            {
                horizontalInput = -1f;
                facingDirection = Direction.Left;
            }
            else if (keyboard.IsKeyDown(rightKey))
            {
                horizontalInput = 1f;
                facingDirection = Direction.Right;
            }

            // Run modifier (hold shift)
            isRunning = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            float speedMultiplier = isRunning ? Constants.PLAYER_RUN_MULTIPLIER : 1f;

            // Apply horizontal velocity
            Velocity = new Vector2(
                horizontalInput * moveSpeed * speedMultiplier,
                Velocity.Y
            );

            // Jump
            if (keyboard.IsKeyDown(jumpKey) && IsGrounded && canJump)
            {
                Velocity = new Vector2(Velocity.X, jumpSpeed);
                IsGrounded = false;
                canJump = false;
                // Play jump sound
                SoundManager.Instance?.PlaySound("jump");
            }

            if (keyboard.IsKeyUp(jumpKey))
            {
                canJump = true;
            }

            // Shoot (only in Fire state)
            if (keyboard.IsKeyDown(shootKey) && CurrentState == PlayerStateType.Fire)
            {
                Shoot();
            }
        }

        private void UpdateAnimation(float deltaTime)
        {
            // Simple animation logic
            if (Math.Abs(Velocity.X) > 0)
            {
                animationTimer += deltaTime;
                if (animationTimer >= FRAME_TIME)
                {
                    animationTimer = 0f;
                    currentFrame = (currentFrame + 1) % 3; // 3 frames for walking
                }
            }
            else
            {
                currentFrame = 0; // Idle frame
            }
        }

        public override void OnCollision(GameObject other, CollisionSide side)
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

        public void PowerUp(PlayerStateType newState)
        {
            if (newState > CurrentState)
            {
                CurrentState = newState;

                // Update size for Big/Fire Mario
                if (CurrentState != PlayerStateType.Small)
                {
                    Size = new Vector2(32, 64);
                }

                SoundManager.Instance?.PlaySound("powerup");
            }
        }

        public void TakeDamage()
        {
            if (IsInvincible) return;

            if (CurrentState == PlayerStateType.Small)
            {
                Die();
            }
            else
            {
                // Downgrade power state
                CurrentState = (PlayerStateType)((int)CurrentState - 1);

                if (CurrentState == PlayerStateType.Small)
                {
                    Size = new Vector2(32, 32);
                }

                invincibilityTimer = INVINCIBILITY_TIME;
                SoundManager.Instance?.PlaySound("damage");
            }
        }

        private void Die()
        {
            Lives--;
            IsActive = false;
            SoundManager.Instance?.PlaySound("death");

            if (Lives <= 0)
            {
                // Game Over
                GameManager.Instance?.GameOver();
            }
            else
            {
                // Respawn
                GameManager.Instance?.RespawnPlayer(this);
            }
        }

        private void Shoot()
        {
            // Create fireball projectile
            Vector2 fireballPos = Position + new Vector2(
                facingDirection == Direction.Right ? Size.X : 0,
                Size.Y / 2
            );

            // TODO: Create Fireball object
            // GameManager.Instance.SpawnProjectile(fireballPos, facingDirection);

            SoundManager.Instance?.PlaySound("fireball");
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void AddCoin()
        {
            Coins++;
            if (Coins >= 100)
            {
                Coins = 0;
                Lives++;
                SoundManager.Instance?.PlaySound("1up");
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsActive || texture == null) return;

            // Blink when invincible
            if (IsInvincible && ((int)(invincibilityTimer * 10) % 2 == 0))
            {
                return;
            }

            Vector2 drawPosition = Position - cameraOffset;

            // Flip sprite based on facing direction
            SpriteEffects effect = facingDirection == Direction.Left
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            spriteBatch.Draw(
                texture,
                drawPosition,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                effect,
                0f
            );
        }
    }

    // Placeholder classes - will be implemented in Managers
    public class SoundManager
    {
        public static SoundManager Instance { get; set; }
        public void PlaySound(string soundName) { }
    }

    public class GameManager
    {
        public static GameManager Instance { get; set; }
        public void GameOver() { }
        public void RespawnPlayer(Player player) { }
    }
}