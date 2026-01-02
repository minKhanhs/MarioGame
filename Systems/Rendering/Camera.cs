using Microsoft.Xna.Framework;
using MarioGame.Core;
using MarioGame.Entities.Player;
using System;

namespace MarioGame.Systems.Rendering
{
    public enum CameraMode
    {
        FollowPlayer,
        AutoScroll,
        Fixed
    }

    // Strategy Pattern for camera behavior
    public interface ICameraStrategy
    {
        void Update(Camera camera, GameTime gameTime);
    }

    public class Camera
    {
        public Vector2 Position { get; set; }
        public Rectangle ViewBounds { get; private set; }
        public CameraMode Mode { get; private set; }

        private ICameraStrategy currentStrategy;
        private Player target;
        private float scrollSpeed = 50f;

        public int ViewWidth { get; private set; }
        public int ViewHeight { get; private set; }

        // Camera bounds (level limits)
        public int MinX { get; set; } = 0;
        public int MaxX { get; set; } = 10000;
        public int MinY { get; set; } = 0;
        public int MaxY { get; set; } = 720;

        public Camera(int viewWidth, int viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
            Position = Vector2.Zero;
            UpdateViewBounds();
            SetMode(CameraMode.FollowPlayer);
        }

        public void SetTarget(Player player)
        {
            target = player;
        }

        public void SetMode(CameraMode mode)
        {
            Mode = mode;

            switch (mode)
            {
                case CameraMode.FollowPlayer:
                    currentStrategy = new FollowPlayerStrategy();
                    break;
                case CameraMode.AutoScroll:
                    currentStrategy = new AutoScrollStrategy();
                    break;
                case CameraMode.Fixed:
                    currentStrategy = new FixedCameraStrategy();
                    break;
            }
        }

        public void SetScrollSpeed(float speed)
        {
            scrollSpeed = speed;
        }

        public void Update(GameTime gameTime)
        {
            currentStrategy?.Update(this, gameTime);
            ClampPosition();
            UpdateViewBounds();
        }

        private void ClampPosition()
        {
            Position = new Vector2(
                MathHelper.Clamp(Position.X, MinX, MaxX - ViewWidth),
                MathHelper.Clamp(Position.Y, MinY, MaxY - ViewHeight)
            );
        }

        private void UpdateViewBounds()
        {
            ViewBounds = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                ViewWidth,
                ViewHeight
            );
        }

        public Player GetTarget() => target;
        public float GetScrollSpeed() => scrollSpeed;

        // Check if an object is visible
        public bool IsVisible(Rectangle bounds)
        {
            return ViewBounds.Intersects(bounds);
        }

        // Convert world position to screen position
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return worldPosition - Position;
        }

        // Convert screen position to world position
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return screenPosition + Position;
        }
    }

    // Follow Player Strategy
    public class FollowPlayerStrategy : ICameraStrategy
    {
        public void Update(Camera camera, GameTime gameTime)
        {
            Player target = camera.GetTarget();
            if (target == null || !target.IsActive)
                return;

            // Calculate target position (player in center-left of screen)
            float targetX = target.Position.X - camera.ViewWidth / 3;
            float targetY = target.Position.Y - camera.ViewHeight / 2;

            // Smooth camera movement (lerp)
            Vector2 currentPos = camera.Position;
            Vector2 desiredPos = new Vector2(targetX, targetY);

            camera.Position = Vector2.Lerp(currentPos, desiredPos, Constants.CAMERA_LERP_SPEED);

            // Don't move camera backwards (classic Mario behavior)
            if (camera.Position.X < currentPos.X)
            {
                camera.Position = new Vector2(currentPos.X, camera.Position.Y);
            }
        }
    }

    // Auto-Scroll Strategy
    public class AutoScrollStrategy : ICameraStrategy
    {
        public void Update(Camera camera, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float scrollSpeed = camera.GetScrollSpeed();

            // Move camera automatically to the right
            camera.Position += new Vector2(scrollSpeed * deltaTime, 0);

            // Check if player is behind camera (instant death)
            Player target = camera.GetTarget();
            if (target != null && target.IsActive)
            {
                if (target.Position.X < camera.Position.X)
                {
                    target.TakeDamage();
                }
            }
        }
    }

    // Fixed Camera Strategy
    public class FixedCameraStrategy : ICameraStrategy
    {
        public void Update(Camera camera, GameTime gameTime)
        {
            // Camera doesn't move
            // Useful for boss rooms or puzzle sections
        }
    }

    // Camera Manager
    public class CameraManager
    {
        private static CameraManager instance;
        public static CameraManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new CameraManager();
                return instance;
            }
        }

        public Camera MainCamera { get; private set; }

        private CameraManager()
        {
            MainCamera = new Camera(Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT);
        }

        public void Initialize(int levelWidth, int levelHeight)
        {
            MainCamera.MaxX = levelWidth;
            MainCamera.MaxY = levelHeight;
            MainCamera.Position = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            MainCamera.Update(gameTime);
        }

        public void SetTarget(Player player)
        {
            MainCamera.SetTarget(player);
        }

        public void SetMode(CameraMode mode)
        {
            MainCamera.SetMode(mode);
        }

        public Vector2 GetOffset()
        {
            return MainCamera.Position;
        }
    }
}