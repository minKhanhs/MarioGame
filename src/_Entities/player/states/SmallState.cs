using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.player.states
{
    public class SmallState : IMarioState
    {
        public void Enter(Player player)
        {
            // 1. Trả về kích thước bình thường
            player.Scale = 1.0f;

            // 2. Logic bất tử
            player.IsInvincible = true;

            // Lưu ý: Khi từ to về nhỏ, Mario sẽ lơ lửng 1 chút (do hitbox thu hẹp từ dưới lên),
            // nhưng trọng lực sẽ tự kéo Mario xuống ngay lập tức nên không cần chỉnh Position.Y
        }

        public void HandleInput(Player player)
        {
            // Mario nhỏ không bắn được đạn
        }

        public void TakeDamage(Player player)
        {
            player.Die(); // Nhỏ mà bị đánh là chết
        }
    }
}
