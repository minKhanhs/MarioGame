using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.player.states
{
    public interface IMarioState
    {
        void Enter(Player player);       // Gọi khi bắt đầu vào trạng thái
        void HandleInput(Player player); // Xử lý input riêng cho trạng thái (ví dụ: bắn đạn)
        void TakeDamage(Player player);  // Xử lý khi bị quái chạm
    }
}
