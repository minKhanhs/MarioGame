using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.items
{
    public abstract class Item : MovableObj
    {
        // Hàm trừu tượng: Xử lý khi Mario ăn vật phẩm
        public abstract void OnCollect(Player player);
    }
}
