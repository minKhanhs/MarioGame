using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core.Camera
{
    public class Camera
    {
        public Vector2 Position { get; private set; }
        public Matrix ViewMatrix { get; private set; }
        public Rectangle Viewport { get; private set; }
        public Rectangle MapBounds { get; set; }

        private ICameraStrategy _strategy;

        public Camera(Viewport viewport)
        {
            Viewport = viewport.Bounds;
            Position = Vector2.Zero;
            // _strategy = new FollowTargetStrategy(); // Nhớ sửa cả class này theo Interface mới nhé
        }

        public void SetStrategy(ICameraStrategy strategy)
        {
            _strategy = strategy;
        }

        // SỬA: Thêm tham số GameTime
        public void Update(Vector2 targetPosition, Rectangle mapBounds, GameTime gameTime)
        {
            MapBounds = mapBounds;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 1. Tính vị trí mục tiêu
            Vector2 targetCamPos = _strategy.CalculatePosition(Position, targetPosition, Viewport, MapBounds, dt);

            // 2. Xử lý Lerp (Làm mượt)
            // Nếu đang dùng AutoScroll, ta muốn nó đi chính xác từng li, không muốn độ trễ.
            // Cách đơn giản nhất là kiểm tra kiểu Strategy
            if (_strategy is AutoScrollStrategy)
            {
                Position = targetCamPos; // Gán trực tiếp, không Lerp -> Chuyển động đều tăm tắp
            }
            else
            {
                // Với FollowTargetStrategy thì vẫn dùng Lerp cho mượt
                Position = Vector2.Lerp(Position, targetCamPos, 0.1f);
            }

            // 3. Làm tròn số (Pixel Perfect)
            Position = new Vector2((int)Position.X, (int)Position.Y);

            // 4. Tạo ma trận
            ViewMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
        }

        public bool IsVisible(Rectangle bounds)
        {
            Rectangle cameraView = new Rectangle((int)Position.X, (int)Position.Y, Viewport.Width, Viewport.Height);
            return cameraView.Intersects(bounds);
        }
    }
}
