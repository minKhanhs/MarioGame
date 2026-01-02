using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarioGame.Core;
using MarioGame.Entities.Base;
using MarioGame.Entities.Player;
using System;

namespace MarioGame.Entities.Items
{
    // Base Item class
    public abstract class Item : GameObject
    {
        protected int scoreValue = 100;
        protected bool isCollected = false;
        protected float spawnAnimationTimer = 0f;
        protected const float SPAWN_ANIMATION_TIME = 0.5f;
        protected Vector2 spawnStartPosition;

        public Item(Vector2 position, Vector2 size) : base(position, size)
        {
            Layer = Constants.LAYER_ITEMS;
            spawnStartPosition = position;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive || isCollected) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Spawn animation (rise up from block)
            if (spawnAnimationTimer < SPAWN_ANIMATION_TIME)
            {
                spawnAnimationTimer += deltaTime;
                float progress = spawnAnimationTimer / SPAWN_ANIMATION_TIME;
                Position = Vector2.Lerp(spawnStartPosition, spawnStartPosition - new Vector2(0, Size.Y), progress);
                HasGravity = false;
            }
            else
            {
                HasGravity = true;
            }

            base.Update(gameTime);
        }

        protected virtual void OnCollect(Player.Player player)
        {
            isCollected = true;
            player.AddScore(scoreValue);
            Destroy();
        }
    }

    // COIN
    public class Coin : Item
    {
        private float animationTimer = 0f;
        private int currentFrame = 0;
        private const int FRAME_COUNT = 4;
        private const float FRAME_TIME = 0.15f;

        public Coin(Vector2 position) : base(position, new Vector2(32, 32))
        {
            scoreValue = 100;
            HasGravity = false; // Coins float in the air
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Rotate animation
            animationTimer += deltaTime;
            if (animationTimer >= FRAME_TIME)
            {
                animationTimer = 0f;
                currentFrame = (currentFrame + 1) % FRAME_COUNT;
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            if (other is Player.Player player && !isCollected)
            {
                player.AddCoin();
                player.AddScore(scoreValue);
                Managers.SoundManager.Instance?.PlaySound("coin");
                Destroy();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsActive || texture == null) return;

            Vector2 drawPosition = Position - cameraOffset;
            Rectangle source = new Rectangle(currentFrame * 32, 0, 32, 32);

            spriteBatch.Draw(texture, drawPosition, source, Color.White);
        }
    }

    // MUSHROOM POWERUP
    public class Mushroom : Item
    {
        private const float MOVE_SPEED = 80f;
        private Direction moveDirection = Direction.Right;

        public Mushroom(Vector2 position) : base(position, new Vector2(32, 32))
        {
            scoreValue = 1000;
            HasGravity = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive || isCollected) return;

            // Move horizontally after spawn animation
            if (spawnAnimationTimer >= SPAWN_ANIMATION_TIME)
            {
                Velocity = new Vector2((int)moveDirection * MOVE_SPEED, Velocity.Y);
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            if (other is Player.Player player && !isCollected)
            {
                OnCollect(player);
                player.PowerUp(PlayerStateType.Big);
                Managers.SoundManager.Instance?.PlaySound("powerup");
                return;
            }

            // Bounce off walls
            if (side == CollisionSide.Left || side == CollisionSide.Right)
            {
                moveDirection = (Direction)(-(int)moveDirection);
            }

            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
                Velocity = new Vector2(Velocity.X, 0);
            }
        }
    }

    // FIRE FLOWER POWERUP
    public class FireFlower : Item
    {
        private float animationTimer = 0f;
        private float bobTimer = 0f;
        private Vector2 basePosition;
        private const float BOB_SPEED = 2f;
        private const float BOB_AMOUNT = 4f;

        public FireFlower(Vector2 position) : base(position, new Vector2(32, 32))
        {
            scoreValue = 1000;
            HasGravity = false; // Floats in place
            basePosition = position;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive || isCollected) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bobbing animation
            bobTimer += deltaTime * BOB_SPEED;
            float bobOffset = (float)Math.Sin(bobTimer) * BOB_AMOUNT;

            if (spawnAnimationTimer >= SPAWN_ANIMATION_TIME)
            {
                Position = new Vector2(basePosition.X, basePosition.Y + bobOffset);
            }
            else
            {
                basePosition = Position;
            }

            // Color cycle animation
            animationTimer += deltaTime;

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            if (other is Player.Player player && !isCollected)
            {
                OnCollect(player);
                player.PowerUp(PlayerStateType.Fire);
                Managers.SoundManager.Instance?.PlaySound("powerup");
            }
        }
    }

    // FIREBALL PROJECTILE
    public class Fireball : GameObject
    {
        private const float FIREBALL_SPEED = 300f;
        private const float BOUNCE_VELOCITY = -250f;
        private Direction direction;
        private int bounceCount = 0;
        private const int MAX_BOUNCES = 3;
        private float lifetime = 0f;
        private const float MAX_LIFETIME = 5f;

        public Fireball(Vector2 position, Direction shootDirection)
            : base(position, new Vector2(16, 16))
        {
            direction = shootDirection;
            Velocity = new Vector2((int)direction * FIREBALL_SPEED, 0);
            HasGravity = true;
            Layer = Constants.LAYER_PLAYER;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lifetime += deltaTime;

            // Destroy after max lifetime
            if (lifetime >= MAX_LIFETIME)
            {
                Destroy();
                return;
            }

            // Destroy after too many bounces
            if (bounceCount >= MAX_BOUNCES)
            {
                Destroy();
                return;
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            // Hit enemy
            if (other is Enemies.Enemy enemy)
            {
                enemy.TakeDamage();
                CreateExplosion();
                Destroy();
                return;
            }

            // Hit tile - bounce
            if (other is Level.Tile tile && tile.IsSolid)
            {
                if (side == CollisionSide.Bottom)
                {
                    // Bounce up
                    Velocity = new Vector2(Velocity.X, BOUNCE_VELOCITY);
                    bounceCount++;
                    Managers.SoundManager.Instance?.PlaySound("bump");
                }
                else if (side == CollisionSide.Left || side == CollisionSide.Right)
                {
                    // Hit wall - destroy
                    CreateExplosion();
                    Destroy();
                }
            }
        }

        private void CreateExplosion()
        {
            // TODO: Create particle effect
            Managers.SoundManager.Instance?.PlaySound("kick");
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsActive || texture == null) return;

            Vector2 drawPosition = Position - cameraOffset;

            // Rotate fireball
            float rotation = lifetime * 10f;

            spriteBatch.Draw(
                texture,
                drawPosition + Size / 2,
                sourceRect,
                Color.White,
                rotation,
                Size / 2,
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }

    // 1-UP MUSHROOM (Bonus)
    public class OneUpMushroom : Item
    {
        private const float MOVE_SPEED = 100f;
        private Direction moveDirection = Direction.Right;

        public OneUpMushroom(Vector2 position) : base(position, new Vector2(32, 32))
        {
            scoreValue = 0; // No score, just extra life
            HasGravity = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive || isCollected) return;

            if (spawnAnimationTimer >= SPAWN_ANIMATION_TIME)
            {
                Velocity = new Vector2((int)moveDirection * MOVE_SPEED, Velocity.Y);
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            if (other is Player.Player player && !isCollected)
            {
                player.Lives++;
                Managers.SoundManager.Instance?.PlaySound("1up");
                Destroy();
                return;
            }

            if (side == CollisionSide.Left || side == CollisionSide.Right)
            {
                moveDirection = (Direction)(-(int)moveDirection);
            }

            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
            }
        }
    }

    // STAR POWERUP (Invincibility - Bonus)
    public class Star : Item
    {
        private const float MOVE_SPEED = 150f;
        private const float BOUNCE_HEIGHT = -350f;
        private Direction moveDirection = Direction.Right;

        public Star(Vector2 position) : base(position, new Vector2(32, 32))
        {
            scoreValue = 1000;
            HasGravity = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive || isCollected) return;

            if (spawnAnimationTimer >= SPAWN_ANIMATION_TIME)
            {
                Velocity = new Vector2((int)moveDirection * MOVE_SPEED, Velocity.Y);

                // Auto bounce
                if (IsGrounded)
                {
                    Velocity = new Vector2(Velocity.X, BOUNCE_HEIGHT);
                }
            }

            base.Update(gameTime);
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            if (other is Player.Player player && !isCollected)
            {
                OnCollect(player);
                // TODO: Activate invincibility mode
                Managers.SoundManager.Instance?.PlaySound("powerup");
                return;
            }

            if (side == CollisionSide.Left || side == CollisionSide.Right)
            {
                moveDirection = (Direction)(-(int)moveDirection);
            }

            if (side == CollisionSide.Bottom)
            {
                IsGrounded = true;
            }
        }
    }
}