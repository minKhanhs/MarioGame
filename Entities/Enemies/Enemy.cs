using Microsoft.Xna.Framework;
using MarioGame.Core;
using MarioGame.Entities.Base;
using MarioGame.Entities.Player;
using System;

namespace MarioGame.Entities.Enemies
{
    public enum EnemyState
    {
        Walking,
        Stunned,
        Dead
    }

    public abstract class Enemy : GameObject, ICollidable
    {
        public EnemyState State { get; protected set; } = EnemyState.Walking;
        public int ScoreValue { get; protected set; } = 100;

        protected Direction moveDirection = Direction.Left;
        protected float moveSpeed = 50f;

        protected float stateTimer = 0f;

        public Enemy(Vector2 position, Vector2 size) : base(position, size)
        {
            Layer = Constants.LAYER_ENEMIES;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateState(deltaTime);
            base.Update(gameTime);
        }

        protected virtual void UpdateState(float deltaTime)
        {
            switch (State)
            {
                case EnemyState.Walking:
                    Velocity = new Vector2((int)moveDirection * moveSpeed, Velocity.Y);
                    break;

                case EnemyState.Stunned:
                    stateTimer -= deltaTime;
                    if (stateTimer <= 0)
                    {
                        State = EnemyState.Dead;
                        Destroy();
                    }
                    break;

                case EnemyState.Dead:
                    Destroy();
                    break;
            }
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            // Collision with player
            if (other is Player.Player player)
            {
                if (side == CollisionSide.Top)
                {
                    // Player jumped on enemy
                    OnStomped(player);
                }
                else
                {
                    // Enemy hits player from side
                    player.TakeDamage();
                }
                return;
            }

            // Collision with walls/tiles
            if (side == CollisionSide.Left || side == CollisionSide.Right)
            {
                // Turn around
                moveDirection = (Direction)(-(int)moveDirection);
            }

            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
                Velocity = new Vector2(Velocity.X, 0);
            }
        }

        protected virtual void OnStomped(Player.Player player)
        {
            State = EnemyState.Dead;
            player.AddScore(ScoreValue);
            player.Velocity = new Vector2(player.Velocity.X, Constants.PLAYER_JUMP_SPEED * 0.5f);
            SoundManager.Instance?.PlaySound("stomp");
            Destroy();
        }

        public virtual void TakeDamage()
        {
            State = EnemyState.Dead;
            Destroy();
        }
    }

    // GOOMBA - Simple walking enemy
    public class Goomba : Enemy
    {
        public Goomba(Vector2 position)
            : base(position, new Vector2(32, 32))
        {
            moveSpeed = Constants.GOOMBA_SPEED;
            ScoreValue = 100;
        }

        protected override void OnStomped(Player.Player player)
        {
            State = EnemyState.Stunned;
            stateTimer = 0.5f; // Flatten animation time
            Velocity = Vector2.Zero;
            HasCollision = false;

            player.AddScore(ScoreValue);
            player.Velocity = new Vector2(player.Velocity.X, Constants.PLAYER_JUMP_SPEED * 0.5f);
            SoundManager.Instance?.PlaySound("stomp");
        }
    }

    // KOOPA - Shell mechanic (State Pattern)
    public enum KoopaState
    {
        Walking,
        Shell,
        ShellSliding
    }

    public class Koopa : Enemy
    {
        public KoopaState KoopaCurrentState { get; private set; } = KoopaState.Walking;
        private float shellTimer = 0f;
        private const float SHELL_TIMEOUT = 5f;

        public Koopa(Vector2 position)
            : base(position, new Vector2(32, 48))
        {
            moveSpeed = Constants.KOOPA_SPEED;
            ScoreValue = 200;
        }

        protected override void UpdateState(float deltaTime)
        {
            switch (KoopaCurrentState)
            {
                case KoopaState.Walking:
                    Velocity = new Vector2((int)moveDirection * moveSpeed, Velocity.Y);
                    break;

                case KoopaState.Shell:
                    Velocity = new Vector2(0, Velocity.Y);
                    shellTimer += deltaTime;

                    if (shellTimer >= SHELL_TIMEOUT)
                    {
                        // Koopa comes back out
                        KoopaCurrentState = KoopaState.Walking;
                        Size = new Vector2(32, 48);
                        shellTimer = 0f;
                    }
                    break;

                case KoopaState.ShellSliding:
                    Velocity = new Vector2((int)moveDirection * Constants.KOOPA_SHELL_SPEED, Velocity.Y);
                    break;
            }

            base.UpdateState(deltaTime);
        }

        protected override void OnStomped(Player.Player player)
        {
            switch (KoopaCurrentState)
            {
                case KoopaState.Walking:
                    // First stomp - go into shell
                    KoopaCurrentState = KoopaState.Shell;
                    Size = new Vector2(32, 32);
                    Velocity = Vector2.Zero;
                    shellTimer = 0f;

                    player.AddScore(ScoreValue);
                    player.Velocity = new Vector2(player.Velocity.X, Constants.PLAYER_JUMP_SPEED * 0.5f);
                    SoundManager.Instance?.PlaySound("stomp");
                    break;

                case KoopaState.Shell:
                    // Kick the shell
                    KickShell(player);
                    break;

                case KoopaState.ShellSliding:
                    // Stop the shell
                    KoopaCurrentState = KoopaState.Shell;
                    Velocity = new Vector2(0, Velocity.Y);
                    shellTimer = 0f;

                    player.AddScore(ScoreValue);
                    player.Velocity = new Vector2(player.Velocity.X, Constants.PLAYER_JUMP_SPEED * 0.5f);
                    break;
            }
        }

        public void KickShell(Player.Player player)
        {
            KoopaCurrentState = KoopaState.ShellSliding;

            // Determine kick direction
            if (player.Position.X < Position.X)
            {
                moveDirection = Direction.Right;
            }
            else
            {
                moveDirection = Direction.Left;
            }

            player.AddScore(ScoreValue);
            SoundManager.Instance?.PlaySound("kick");
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            base.OnCollision(other, side);

            // Shell sliding kills other enemies
            if (KoopaCurrentState == KoopaState.ShellSliding && other is Enemy enemy)
            {
                if (side == CollisionSide.Left || side == CollisionSide.Right)
                {
                    enemy.TakeDamage();
                }
            }
        }
    }

    // PIRANHA PLANT - Up/Down movement
    public class PiranhaPlant : Enemy
    {
        private Vector2 hiddenPosition;
        private Vector2 shownPosition;
        private bool isHidden = true;
        private float moveTimer = 0f;
        private const float MOVE_INTERVAL = 2f;
        private const float DETECTION_RANGE = 64f;

        private Player.Player nearestPlayer;

        public PiranhaPlant(Vector2 pipePosition)
            : base(pipePosition, new Vector2(32, 48))
        {
            HasGravity = false; // Doesn't fall
            shownPosition = pipePosition;
            hiddenPosition = pipePosition + new Vector2(0, 48); // Hide inside pipe
            Position = hiddenPosition;

            moveSpeed = 50f;
            ScoreValue = 200;
        }

        protected override void UpdateState(float deltaTime)
        {
            moveTimer += deltaTime;

            // Check if player is too close
            bool playerNear = IsPlayerNearby();

            if (moveTimer >= MOVE_INTERVAL)
            {
                moveTimer = 0f;

                if (isHidden && !playerNear)
                {
                    // Come out
                    isHidden = false;
                }
                else if (!isHidden)
                {
                    // Go back in
                    isHidden = true;
                }
            }

            // Smooth movement
            Vector2 targetPos = isHidden ? hiddenPosition : shownPosition;
            Position = Vector2.Lerp(Position, targetPos, moveSpeed * deltaTime * 0.1f);
        }

        private bool IsPlayerNearby()
        {
            // TODO: Check distance to players
            // This would need access to GameManager to get player positions
            return false;
        }

        protected override void OnStomped(Player.Player player)
        {
            // Piranha Plant can't be stomped normally
            // Only killed by fireballs or shells
            player.TakeDamage();
        }

        public override void TakeDamage()
        {
            // Can be killed by projectiles
            State = EnemyState.Dead;
            SoundManager.Instance?.PlaySound("kick");
            Destroy();
        }
    }
}