using MarioGame.src._Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MarioGame.src._Entities.Base
{
public abstract class MovableObj : GameObj
{
    public Vector2 Velocity;
    public Vector2 Acceleration;

    protected float Gravity = 0.5f;       // Trọng lực kéo xuống
    protected float Friction = 0.9f;      // Ma sát trượt (tùy chọn)
    public bool IsOnGround = false;       // Cờ kiểm tra đang đứng trên đất
    public float Speed = 0f;              // Tốc độ di chuyển cơ bản

    protected void ApplyPhysics()
    {
        // Áp dụng trọng lực nếu không đứng trên đất
        if (!IsOnGround)
        {
            Velocity.Y += Gravity;
        }
        else
        {
            Velocity.Y = 0; // Reset vận tốc rơi khi chạm đất
        }

        // Áp dụng gia tốc (nếu có)
        Velocity += Acceleration;

        // Cập nhật vị trí
        Position += Velocity;

        // Reset gia tốc sau mỗi frame
        Acceleration = Vector2.Zero;
    }

    // Hàm hỗ trợ va chạm (sẽ được gọi từ Game/Level Manager)
    public void OnCollision(Rectangle intersect, bool isVerticalCollision)
    {
        if (isVerticalCollision)
        {
            // Va chạm theo chiều dọc (đứng lên block hoặc cụng đầu)
            if (Velocity.Y > 0) // Đang rơi xuống
            {
                Position.Y -= intersect.Height;
                IsOnGround = true;
            }
            else if (Velocity.Y < 0) // Đang nhảy lên cụng đầu
            {
                Position.Y += intersect.Height;
                Velocity.Y = 0;
            }
        }
        else
        {
            // Va chạm ngang (đi vào tường)
            Position.X -= intersect.Width * (Velocity.X > 0 ? 1 : -1);
            Velocity.X = 0;
        }
    }
}
}
