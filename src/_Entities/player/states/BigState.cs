using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.player.states
{
    public class BigState : IMarioState
    {
        public void Enter(Player player)
        {
            // 1. Tăng kích thước lên 1.5 lần
            player.Scale = 1.5f;

            // 2. Xử lý vị trí (QUAN TRỌNG):
            // Khi to ra, hitbox dài xuống dưới làm Mario bị kẹt vào đất.
            // Ta cần kéo Mario lên trên một đoạn bằng phần chiều cao tăng thêm.
            // Chiều cao gốc (ví dụ 32), Tăng thêm 0.5 lần = 16px.

            // Lấy chiều cao gốc từ animation hiện tại
            float baseHeight = 32f; // Hoặc player._currentAnim.FrameHeight nếu bạn public biến đó

            // Kéo lên trên
            player.Position.Y -= (baseHeight * 0.5f);
        }

        public void HandleInput(Player player)
        {
            // Logic đặc biệt nếu có (ví dụ đập gạch bằng đầu dễ hơn)
        }

        public void TakeDamage(Player player)
        {
            // Bị đánh thì biến về dạng nhỏ
            player.SetState(new SmallState());
            player.StartInvincible();
        }
    }
}
