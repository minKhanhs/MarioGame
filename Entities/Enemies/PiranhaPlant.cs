using MarioGame.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Enemies
{
    public class PiranhaPlant : Enemy
    {
        private Vector2 _hidePosition;
        private Vector2 _showPosition;
        private bool _isHidden = true;
        private float _stateTimer = 0f;
        private const float STATE_DURATION = 2f;
        private const float MOVEMENT_SPEED = 50f;

        public PiranhaPlant(Vector2 position) : base(position, new Vector2(16, 32))
        {
            AffectedByGravity = false;
            _hidePosition = position;
            _showPosition = position - new Vector2(0, 32);
            ScoreValue = 0; // Can't be stomped
        }

        protected override void UpdateWalking(float deltaTime)
        {
            _stateTimer += deltaTime;

            if (_stateTimer >= STATE_DURATION)
            {
                _stateTimer = 0;
                _isHidden = !_isHidden;
            }

            // Move up or down
            Vector2 targetPos = _isHidden ? _hidePosition : _showPosition;
            Vector2 direction = targetPos - Position;

            if (direction.Length() > 1f)
            {
                direction.Normalize();
                Position += direction * MOVEMENT_SPEED * deltaTime;
            }

            Velocity = Vector2.Zero;
        }

        public override void Stomp()
        {
            // Piranha plant cannot be stomped
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

            spriteBatch.Draw(pixel, destRect, Color.DarkGreen);
        }
    }
}