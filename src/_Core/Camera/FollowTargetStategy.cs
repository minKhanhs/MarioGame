using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core.Camera
{
    public class FollowTargetStrategy : ICameraStrategy
    {
        public Vector2 CalculatePosition(Vector2 currentCamPos, Vector2 targetPos, Rectangle viewport, Rectangle mapBounds, float deltaTime)
        {
            // 1. Tính toán vị trí để Mario nằm giữa màn hình
            float x = targetPos.X - (viewport.Width / 2f);
            float y = targetPos.Y - (viewport.Height / 2f);

            // 2. (Tùy chọn) Khóa trục Y nếu bạn muốn camera chỉ chạy ngang như Mario Bros cổ điển
            // y = 0; // Bỏ comment dòng này nếu muốn camera không bao giờ chạy lên xuống
            // Hoặc giới hạn trục Y để không nhìn thấy đáy vực
            if (y > 0) y = 0; // Ví dụ: không cho camera chui xuống đất

            // 3. Kẹp (Clamp) vị trí camera để không chạy ra ngoài map
            // Camera không được < 0 (bên trái)
            // Camera không được > MapWidth - ScreenWidth (bên phải)
            float clampedX = MathHelper.Clamp(x, mapBounds.Left, mapBounds.Right - viewport.Width);
            float clampedY = MathHelper.Clamp(y, mapBounds.Top, mapBounds.Bottom - viewport.Height);

            return new Vector2(clampedX, clampedY);
        }
    }
}
