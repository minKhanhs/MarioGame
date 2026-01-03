using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.src._Entities.enemies
{
    public class Goomba : Enemy
    {
        // --- CÁC BIẾN CHO ANIMATION ---
        private SpriteAnimation _walkAnim;
        private SpriteEffects _spriteEffect; // Để lật hình khi đổi hướng

        // --- CÁC BIẾN CHO DI CHUYỂN TUẦN TRA ---
        private float _walkSpeed = 2f;     // Tốc độ đi bộ
        private float _startX;              // Ghi nhớ vị trí xuất phát trục X
        private float _patrolRange = 100f;  // Phạm vi đi tuần (ví dụ: đi xa tối đa 100px tính từ điểm xuất phát)
        private int _moveDirection = -1;    // -1 là đi trái, 1 là đi phải

        public Goomba(Texture2D texture, Vector2 position)
        {
            Position = position;
            _startX = position.X; // Ghi nhớ điểm bắt đầu

            // 1. KHỞI TẠO ANIMATION
            // Dựa trên ảnh bạn gửi: Có 2 khung hình ngang.
            // Giả định kích thước mỗi khung là 32x32 (chuẩn theo tilesize của bạn)
            // FrameTime 0.2f nghĩa là cứ 0.2 giây đổi hình 1 lần (tốc độ vừa phải cho Goomba)
            _walkAnim = new SpriteAnimation(texture, frameCount: 2, frameWidth: 45, frameHeight: 45)
            {
                FrameTime = 0.3f,
                IsLooping = true
            };

            // Mặc định đi sang trái
            Velocity.X = -_walkSpeed;
            _moveDirection = -1;
            _spriteEffect = SpriteEffects.None; // Ảnh gốc đang nhìn sang trái nên không cần lật

            // Cập nhật texture cho lớp cha để tính Bounds ban đầu
            Texture = _walkAnim.Texture;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 2. CẬP NHẬT ANIMATION
            _walkAnim.Update(gameTime);

            // 3. LOGIC DI CHUYỂN TUẦN TRA (PATROL)
            if (_moveDirection == -1) // Đang đi sang TRÁI
            {
                // Nếu đi quá giới hạn bên trái
                if (Position.X <= _startX - _patrolRange)
                {
                    TurnRight();
                }
            }
            else // Đang đi sang PHẢI
            {
                // Nếu đi quá giới hạn bên phải
                if (Position.X >= _startX + _patrolRange)
                {
                    TurnLeft();
                }
            }

            // Nếu bị tường chặn lại (Velocity.X bị collision set về 0), thì cũng quay đầu
            // Lưu ý: Logic này cần CollisionManager hoạt động tốt
            if (Velocity.X == 0 && IsOnGround)
            {
                if (_moveDirection == -1) TurnRight();
                else TurnLeft();
            }

            // 4. VẬT LÝ CƠ BẢN
            ApplyPhysics();

            // Nếu rơi khỏi map thì chết
            if (Position.Y > 1000) IsActive = false;
        }

        // Hàm hỗ trợ quay đầu sang phải
        private void TurnRight()
        {
            _moveDirection = 1;
            Velocity.X = _walkSpeed;
            _spriteEffect = SpriteEffects.FlipHorizontally; // Lật ngược ảnh lại
        }

        // Hàm hỗ trợ quay đầu sang trái
        private void TurnLeft()
        {
            _moveDirection = -1;
            Velocity.X = -_walkSpeed;
            _spriteEffect = SpriteEffects.None; // Ảnh gốc nhìn trái
        }

        public override void OnStomped()
        {
            // TODO: Sau này có thể thêm animation bẹp dí vào đây
            // Tạm thời biến mất luôn
            IsActive = false;
        }

        // Ghi đè Bounds để hitbox chính xác theo khung hình animation (32x32)
        // thay vì toàn bộ dải ảnh sprite sheet.
        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _walkAnim.FrameWidth, _walkAnim.FrameHeight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive && _walkAnim.Texture != null)
            {
                // Vẽ sử dụng SourceRectangle từ animation và hiệu ứng lật (SpriteEffects)
                spriteBatch.Draw(
                    _walkAnim.Texture,
                    Position,
                    _walkAnim.CurrentFrameSource, // Cắt đúng khung hình hiện tại
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    _spriteEffect, // Áp dụng lật hình
                    0f
                );
            }
        }
    }
}
