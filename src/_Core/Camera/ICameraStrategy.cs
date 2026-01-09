using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MarioGame.src._Core.Camera
{
    public interface ICameraStrategy
    {
        /// <summary>
        /// Tính toán vị trí mong muốn của Camera dựa trên mục tiêu
        /// </summary>
        /// <param name="currentCamPos">Vị trí hiện tại của Camera</param>
        /// <param name="targetPos">Vị trí của đối tượng cần theo dõi (Mario)</param>
        /// <param name="viewport">Kích thước màn hình (để căn giữa)</param>
        /// <param name="mapBounds">Giới hạn của bản đồ (để không soi ra ngoài map)</param>
        /// <returns>Vị trí mới (Vector2)</returns>
        Vector2 CalculatePosition(Vector2 currentCamPos, Vector2 targetPos, Rectangle viewport, Rectangle mapBounds, float deltaTime);

    }
}
