using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core.Camera
{
    public class AutoScrollStrategy : ICameraStrategy
    {
        public float ScrollSpeed { get; set; } = 100f; // Tốc độ trôi pixel/giây

        // Chúng ta cần biến thời gian trôi qua, tạm thời giả định gọi liên tục
        // Trong thực tế Strategy pattern thuần túy ít khi chứa state, 
        // nhưng để đơn giản ta sẽ cộng dồn dựa trên vị trí cũ.

        public Vector2 CalculatePosition(Vector2 currentCamPos, Vector2 targetPos, Rectangle viewport, Rectangle mapBounds)
        {
            // Chỉ đơn giản là cộng thêm vào trục X
            // Lưu ý: Để mượt mà cần nhân với DeltaTime, nhưng Interface đang không có tham số Time.
            // Để fix nhanh: Ta giả định hàm Update của Camera sẽ lo việc lerp, 
            // hoặc ta chỉ trả về mục tiêu phía trước.

            float newX = currentCamPos.X + 2.0f; // Tốc độ cứng mỗi frame (cần cải thiện sau)

            // Vẫn cần Clamp để không chạy quá Map
            float clampedX = MathHelper.Clamp(newX, mapBounds.Left, mapBounds.Right - viewport.Width);

            return new Vector2(clampedX, currentCamPos.Y);
        }
    }
}
