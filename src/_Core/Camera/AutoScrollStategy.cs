using Microsoft.Xna.Framework;

namespace MarioGame.src._Core.Camera
{
    public class AutoScrollStrategy : ICameraStrategy
    {
        public float ScrollSpeed { get; set; } = 110f; // 100 pixel mỗi giây

        public Vector2 CalculatePosition(Vector2 currentCamPos, Vector2 targetPos, Rectangle viewport, Rectangle mapBounds, float deltaTime)
        {
            // 1. Tính vị trí X mới dựa trên thời gian
            // Công thức: Quãng đường = Vận tốc * Thời gian
            float deltaMove = ScrollSpeed * deltaTime;

            // 2. Cộng vào vị trí hiện tại
            // Lưu ý: Vì trong Camera.cs có hàm Lerp (Làm mềm), nếu ta chỉ trả về (Current + Move), 
            // Lerp 0.1 sẽ khiến nó chỉ đi được 10% quãng đường.
            // MẸO: Để thắng được Lerp 0.1f mà không sửa logic Camera phức tạp, 
            // ta nhân quãng đường cần đi lên 10 lần (1 / 0.1f).
            // Hoặc cách chuẩn hơn là sửa Camera.cs (xem Bước 3).

            // Ở đây tôi dùng cách tính vị trí tuyệt đối mong muốn:
            float desiredX = currentCamPos.X + deltaMove;

            // 3. Kẹp trong bản đồ
            float clampedX = MathHelper.Clamp(desiredX, mapBounds.Left, mapBounds.Right - viewport.Width);

            // Auto scroll thường không thay đổi Y, hoặc bám theo target Y
            return new Vector2(clampedX, currentCamPos.Y);
        }
    }
}