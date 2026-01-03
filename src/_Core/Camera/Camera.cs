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
        // Vị trí góc trên bên trái của Camera
        public Vector2 Position { get; private set; }

        // Ma trận biến đổi để đưa vào SpriteBatch
        public Matrix ViewMatrix { get; private set; }

        // Kích thước khung nhìn (Màn hình game)
        public Rectangle Viewport { get; private set; }

        // Giới hạn bản đồ (Lấy từ MapLoader)
        public Rectangle MapBounds { get; set; }

        private ICameraStrategy _strategy;

        public Camera(Viewport viewport)
        {
            Viewport = viewport.Bounds;
            Position = Vector2.Zero;
            _strategy = new FollowTargetStrategy(); // Mặc định là bám theo
        }

        public void SetStrategy(ICameraStrategy strategy)
        {
            _strategy = strategy;
        }

        public void Update(Vector2 targetPosition, Rectangle mapBounds)
        {
            MapBounds = mapBounds;

            // 1. Tính toán vị trí mong muốn từ Strategy
            Vector2 targetCamPos = _strategy.CalculatePosition(Position, targetPosition, Viewport, MapBounds);

            // 2. Làm mượt chuyển động (Lerp - Linear Interpolation)
            // Số 0.1f là độ trễ (Smoothing). Giá trị càng nhỏ càng trễ, = 1 là dính chặt.
            Position = Vector2.Lerp(Position, targetCamPos, 0.1f);

            // 3. Làm tròn số để tránh lỗi rung hình (pixel jittering) khi vẽ pixel art
            Position = new Vector2((int)Position.X, (int)Position.Y);

            // 4. Tạo ma trận View
            // Dịch chuyển ngược lại với vị trí Camera (Camera đi sang phải = Thế giới đi sang trái)
            ViewMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
        }

        // Hàm phụ trợ để kiểm tra vật thể có nằm trong màn hình không (Culling)
        public bool IsVisible(Rectangle bounds)
        {
            Rectangle cameraView = new Rectangle((int)Position.X, (int)Position.Y, Viewport.Width, Viewport.Height);
            return cameraView.Intersects(bounds);
        }
    }
}
