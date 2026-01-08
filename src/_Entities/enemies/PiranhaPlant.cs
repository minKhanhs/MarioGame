using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.enemies
{
    public class PiranhaPlant : Enemy
    {
        private SpriteAnimation _anim;

        private float _scale = 0.6f;
        private int _scaledWidth;
        private int _scaledHeight;

        // --- CÁC BIẾN LOGIC DI CHUYỂN ---
        private float _minY; // Điểm cao nhất (chui ra khỏi cống)
        private float _maxY; // Điểm thấp nhất (chui vào trong cống)
        private float _speed = 40f;

        private float _waitTimer = 0f;
        private float _waitTime = 1.0f; // Thời gian đợi ở đỉnh/đáy

        // Trạng thái của cây
        private enum PlantState { Rising, Sinking, WaitingTop, WaitingBottom }
        private PlantState _state;

        public PiranhaPlant(Texture2D texture, Vector2 position)
        {
            // 1. Khởi tạo animation trước để lấy kích thước gốc
            // GIẢ SỬ ảnh gốc của bạn là 32x32 mỗi khung. Hãy thay số đúng nếu ảnh bạn khác.
            _anim = new SpriteAnimation(texture, 2, 43, 63) { FrameTime = 0.15f };
            Texture = texture;

            // 2. Tính toán kích thước sau khi thu nhỏ
            _scaledWidth = (int)(_anim.FrameWidth * _scale);
            _scaledHeight = (int)(_anim.FrameHeight * _scale);

            // 3. Thiết lập vị trí và giới hạn di chuyển
            _minY = position.Y;

            // Cập nhật _maxY dựa trên chiều cao đã thu nhỏ để nó chui vừa đủ hết xuống cống
            _maxY = position.Y + _scaledHeight;

            // Căn giữa lại vị trí X nếu cần (do cây nhỏ hơn miệng cống 32px)
            // Offset = (Rộng cống - Rộng cây) / 2
            float xOffset = (32 - _scaledWidth) / 2f;

            // Đặt vị trí bắt đầu ở đáy cống
            Position = new Vector2(position.X + xOffset, _maxY);
            _state = PlantState.WaitingBottom;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _anim.Update(gameTime);

            switch (_state)
            {
                case PlantState.Rising:
                    Position.Y -= _speed * dt;
                    if (Position.Y <= _minY)
                    {
                        Position.Y = _minY;
                        _state = PlantState.WaitingTop;
                        _waitTimer = 0;
                    }
                    break;

                case PlantState.WaitingTop:
                    _waitTimer += dt;
                    if (_waitTimer >= _waitTime) _state = PlantState.Sinking;
                    break;

                case PlantState.Sinking:
                    Position.Y += _speed * dt;
                    if (Position.Y >= _maxY)
                    {
                        Position.Y = _maxY;
                        _state = PlantState.WaitingBottom;
                        _waitTimer = 0;
                    }
                    break;

                case PlantState.WaitingBottom:
                    _waitTimer += dt;
                    if (_waitTimer >= _waitTime) _state = PlantState.Rising;
                    break;
            }
        }

        public override void OnStomped()
        {
            // Piranha Plant KHÔNG THỂ BỊ ĐẠP CHẾT!
            // Nếu Mario đạp vào, Mario bị thương.
            // Logic xử lý va chạm trong Game1/GameplayScene sẽ gọi hàm TakeDamage của Mario 
            // nếu hàm này không làm gì (hoặc ta có thể return false nếu sửa lại kiến trúc).

            // Hiện tại kiến trúc của bạn: Game1 gọi OnStomped xong nảy Mario lên.
            // Để Mario bị thương, ta không làm gì ở đây cả, 
            // nhưng cần sửa logic check va chạm ở Game1 một chút (xem lưu ý bên dưới).
        }

        // Ghi đè Bounds để hitbox chuẩn theo Animation
        public override Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, _scaledWidth, _scaledHeight);

        public override void Draw(SpriteBatch spriteBatch)
        {
            // CẬP NHẬT DRAW: Sử dụng overload có tham số scale
            spriteBatch.Draw(
                _anim.Texture,
                Position,
                _anim.CurrentFrameSource,
                Color.White,
                0f,             // Rotation
                Vector2.Zero,   // Origin
                _scale,         // <-- Truyền tỷ lệ thu nhỏ vào đây
                SpriteEffects.None,
                0f
            );
        }
    }
}
