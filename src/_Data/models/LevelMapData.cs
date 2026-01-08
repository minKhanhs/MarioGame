using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Data.models
{
    // Class này ánh xạ trực tiếp cấu trúc file JSON
    public class LevelMapData
    {
        public int LevelId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; } // Kích thước 1 ô (thường là 32 hoặc 16)
        public List<string> Layout { get; set; } // Mảng các chuỗi ký tự biểu diễn bản đồ
        public bool HasBulletBill { get; set; } = false;
    }
}
