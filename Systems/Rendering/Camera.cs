using Microsoft.Xna.Framework;
using MarioGame.Core;

namespace MarioGame.Systems.Rendering
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1.0f;

        public void Update(Vector2 targetPosition, CameraMode mode)
        {
            // Camera position logic handled by CameraManager
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                       Matrix.CreateScale(Zoom);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Transform);
        }
    }
}