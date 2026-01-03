using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Input
{
    public class InputFrame
    {
        // Giá trị từ -1.0f (Trái) đến 1.0f (Phải), 0 là đứng yên
        public float X_Axis { get; set; }

        public bool IsJumpPressed { get; set; }
        public bool IsAttackPressed { get; set; }
        public bool IsRunPressed { get; set; }   // Thêm vào để hỗ trợ chạy nhanh
        public bool IsPausePressed { get; set; } // Hỗ trợ menu pause
    }
}
