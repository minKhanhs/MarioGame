using Microsoft.Xna.Framework;
using MarioGame.Core;

namespace MarioGame.Managers
{
    public class CameraManager
    {
        public Vector2 Position { get; private set; }
        public CameraMode Mode { get; set; } = CameraMode.FollowPlayer;

        private Vector2 _targetPosition;
        private float _lerpSpeed = 5f;
        private float _minX = 0;
        private float _maxX = float.MaxValue;

        // Auto-scroll
        private float _autoScrollSpeed = 50f;

        public CameraManager()
        {
            Position = Vector2.Zero;
        }

        public void SetBounds(float minX, float maxX)
        {
            _minX = minX;
            _maxX = maxX;
        }

        public void Update(float deltaTime, Vector2 playerPosition)
        {
            switch (Mode)
            {
                case CameraMode.FollowPlayer:
                    UpdateFollowPlayer(deltaTime, playerPosition);
                    break;

                case CameraMode.AutoScroll:
                    UpdateAutoScroll(deltaTime);
                    break;

                case CameraMode.Fixed:
                    // Camera doesn't move
                    break;
            }
        }

        private void UpdateFollowPlayer(float deltaTime, Vector2 playerPosition)
        {
            // Keep player at about 1/3 from left side of screen
            _targetPosition = new Vector2(
                playerPosition.X - Constants.SCREEN_WIDTH / 3f,
                0 // Don't follow Y axis
            );

            // Smooth lerp
            Position = Vector2.Lerp(Position, _targetPosition, _lerpSpeed * deltaTime);

            // Clamp to bounds
            Position = new Vector2(
                MathHelper.Clamp(Position.X, _minX, _maxX),
                Position.Y
            );

            // Never go backwards
            if (Position.X < _minX)
                _minX = Position.X;
        }

        private void UpdateAutoScroll(float deltaTime)
        {
            Position += new Vector2(_autoScrollSpeed * deltaTime, 0);

            if (Position.X > _maxX)
                Position = new Vector2(_maxX, Position.Y);
        }

        public void SetAutoScrollSpeed(float speed)
        {
            _autoScrollSpeed = speed;
        }

        public Rectangle GetViewBounds()
        {
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Constants.SCREEN_WIDTH,
                Constants.SCREEN_HEIGHT
            );
        }
    }
}
