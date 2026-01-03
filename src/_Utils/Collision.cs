using MarioGame.src._Entities.Base;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Utils
{
    public static class Collision
    {
        /// <summary>
        /// Xử lý va chạm giữa vật thể động (Player/Enemy/Item) và vật thể tĩnh (Block/Ground)
        /// Hàm này sẽ tự động đẩy vật thể động ra khỏi tường/đất
        /// </summary>
        public static void ResolveStaticCollision(MovableObj mover, GameObj target)
        {
            // 1. Kiểm tra nhanh xem có cắt nhau không
            if (!mover.Bounds.Intersects(target.Bounds)) return;

            // 2. Tính hình chữ nhật giao nhau (vùng bị lấn chiếm)
            Rectangle overlap = Rectangle.Intersect(mover.Bounds, target.Bounds);

            // Nếu vùng giao nhau bằng 0 thì bỏ qua
            if (overlap.IsEmpty) return;

            // 3. Xác định hướng va chạm dựa trên "độ nông" (Shallowest axis)
            // Nguyên tắc: Ta đẩy vật thể ra theo hướng ngắn nhất để thoát khỏi vật cản

            if (overlap.Width < overlap.Height)
            {
                // --- VA CHẠM NGANG (TRÁI/PHẢI) ---

                // Nếu Mover đang ở bên trái Target -> Đẩy sang trái
                if (mover.Position.X < target.Position.X)
                {
                    mover.Position.X -= overlap.Width;
                }
                else // Đẩy sang phải
                {
                    mover.Position.X += overlap.Width;
                }

                // Dừng vận tốc ngang
                mover.Velocity.X = 0;
            }
            else
            {
                // --- VA CHẠM DỌC (TRÊN/DƯỚI) ---

                // Nếu Mover đang ở phía trên Target -> Đứng lên đất
                if (mover.Position.Y < target.Position.Y)
                {
                    mover.Position.Y -= overlap.Height;
                    mover.Velocity.Y = 0;
                    mover.IsOnGround = true; // Quan trọng: Cho phép nhảy tiếp
                }
                else // Đang ở dưới -> Cụng đầu
                {
                    mover.Position.Y += overlap.Height;

                    // Nếu đang nhảy lên mà cụng đầu thì rơi xuống ngay
                    if (mover.Velocity.Y < 0)
                        mover.Velocity.Y = 0;
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem va chạm có phải là đạp từ trên xuống hay không (Dùng cho Mario đạp Goomba)
        /// </summary>
        public static bool IsTopCollision(MovableObj topObj, GameObj bottomObj)
        {
            Rectangle overlap = Rectangle.Intersect(topObj.Bounds, bottomObj.Bounds);

            // Điều kiện để tính là đạp:
            // 1. Va chạm theo chiều dọc (Width > Height)
            // 2. Đối tượng ở trên (Y bé hơn)
            // 3. Đối tượng đang rơi xuống (Velocity Y > 0)

            bool isVertical = overlap.Width > overlap.Height;
            bool isAbove = topObj.Position.Y < bottomObj.Position.Y;
            bool isFalling = topObj.Velocity.Y > 0;

            return isVertical && isAbove && isFalling;
        }
    }
}
