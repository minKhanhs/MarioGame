using MarioGame.src._Entities.player;
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
    public class Koopa : Enemy
    {
        private SpriteAnimation _walkAnim;

        // --- 1. CẤU HÌNH TỶ LỆ (SCALE) ---
        // Chỉnh nhỏ lại (ví dụ 0.7 là 70% kích thước gốc)
        private float _scale = 0.6f;

        // Kích thước gốc từ ảnh bạn cung cấp
        private int _baseWalkWidth = 44;
        private int _baseWalkHeight = 65;
        private int _baseShellWidth = 44;
        private int _baseShellHeight = 42;

        // Kích thước thực tế sau khi scale (được tính toán tự động)
        private int _scaledWalkWidth;
        private int _scaledWalkHeight;
        private int _scaledShellWidth;
        private int _scaledShellHeight;

        // --- 2. CẤU HÌNH TUẦN TRA ---
        private float _startX;              // Điểm xuất phát
        private float _patrolRange = 150f;  // Phạm vi đi qua lại (150px)

        // --- TRẠNG THÁI ---
        private enum KoopaState { Walking, Shell_Idle, Shell_Moving }
        private KoopaState _state;

        // --- DI CHUYỂN ---
        private float _walkSpeed = 3f;
        private float _shellSpeed = 8f;
        private int _direction = -1; // -1: Trái, 1: Phải
        private SpriteEffects _effect = SpriteEffects.None;

        public Koopa(Texture2D texture, Vector2 position)
        {
            // Tính toán kích thước sau khi scale
            _scaledWalkWidth = (int)(_baseWalkWidth * _scale);
            _scaledWalkHeight = (int)(_baseWalkHeight * _scale);
            _scaledShellWidth = (int)(_baseShellWidth * _scale);
            _scaledShellHeight = (int)(_baseShellHeight * _scale);

            // Setup Animation
            _walkAnim = new SpriteAnimation(texture, 1, _baseWalkWidth, _baseWalkHeight)
            {
                FrameTime = 0.2f
            };

            Texture = texture;

            // Căn chỉnh vị trí Y để chân chạm đất (Do chiều cao thay đổi vì scale)
            // Map tile là 32. Koopa cao _scaledWalkHeight.
            // Offset = Chiều cao Koopa - 32
            float yOffset = _scaledWalkHeight - 32;
            Position = new Vector2(position.X, position.Y - yOffset);

            // Ghi nhớ điểm xuất phát cho logic tuần tra
            _startX = position.X;

            _state = KoopaState.Walking;
            Velocity.X = -_walkSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ApplyPhysics();

            switch (_state)
            {
                case KoopaState.Walking:
                    _walkAnim.Update(gameTime);

                    // --- LOGIC TUẦN TRA (FIXED DISTANCE) ---
                    // Nếu đi quá xa bên trái -> Quay phải
                    if (Position.X < _startX - _patrolRange && _direction == -1)
                    {
                        ChangeDirection(1);
                    }
                    // Nếu đi quá xa bên phải -> Quay trái
                    else if (Position.X > _startX + _patrolRange && _direction == 1)
                    {
                        ChangeDirection(-1);
                    }

                    // Vẫn giữ logic: Gặp tường thì quay đầu ngay lập tức
                    if (Velocity.X == 0 && IsOnGround)
                    {
                        ChangeDirection(_direction * -1);
                    }

                    // Cập nhật hướng mặt
                    _effect = (_direction > 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    break;

                case KoopaState.Shell_Idle:
                    Velocity.X = 0;
                    break;

                case KoopaState.Shell_Moving:
                    // Mai rùa thì không tuần tra, chỉ nảy khi đụng tường
                    if (Velocity.X == 0 && IsOnGround)
                    {
                        _direction *= -1;
                        Velocity.X = _shellSpeed * _direction;
                    }
                    break;
            }

            // Cập nhật vận tốc
            if (_state != KoopaState.Shell_Idle)
            {
                float currentSpeed = (_state == KoopaState.Walking) ? _walkSpeed : _shellSpeed;
                Velocity.X = currentSpeed * _direction;
            }

            if (Position.Y > 1000) IsActive = false;
        }

        private void ChangeDirection(int newDir)
        {
            _direction = newDir;
        }

        public override void OnStomped()
        {
            switch (_state)
            {
                case KoopaState.Walking:
                    _state = KoopaState.Shell_Idle;
                    Velocity.X = 0;

                    // Khi biến thành mai (thấp hơn), đẩy vị trí xuống để không lơ lửng
                    // Tính chênh lệch chiều cao đã scale
                    Position.Y += (_scaledWalkHeight - _scaledShellHeight);
                    break;

                case KoopaState.Shell_Idle:
                    _state = KoopaState.Shell_Moving;
                    _direction = 1; // Mặc định đá sang phải (hoặc check vị trí Mario)
                    Velocity.X = _shellSpeed;
                    break;

                case KoopaState.Shell_Moving:
                    _state = KoopaState.Shell_Idle;
                    Velocity.X = 0;
                    break;
            }
        }

        public override void OnTouchPlayer(Player player)
        {
            if (_state == KoopaState.Shell_Idle)
            {
                // Đá bay
                _direction = (player.Position.X < Position.X) ? 1 : -1;
                _state = KoopaState.Shell_Moving;
                Velocity.X = _shellSpeed * _direction;
            }
            else
            {
                base.OnTouchPlayer(player);
            }
        }

        // --- HITBOX ĐÃ SCALE ---
        public override Rectangle Bounds
        {
            get
            {
                int w = (_state == KoopaState.Walking) ? _scaledWalkWidth : _scaledShellWidth;
                int h = (_state == KoopaState.Walking) ? _scaledWalkHeight : _scaledShellHeight;

                return new Rectangle((int)Position.X, (int)Position.Y, w, h);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            if (_state == KoopaState.Walking)
            {
                spriteBatch.Draw(
                    _walkAnim.Texture,
                    Position,
                    _walkAnim.CurrentFrameSource,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _scale, // <--- Scale ảnh đi bộ
                    _effect,
                    0f
                );
            }
            else
            {
                // Vẽ Mai rùa
                // Giả sử mai rùa nằm sau 2 frame đi bộ (X = 44 * 2 = 88)
                // Source Rect lấy theo kích thước GỐC (chưa scale)
                Rectangle shellSource = new Rectangle(88, 0, _baseShellWidth, _baseShellHeight);

                spriteBatch.Draw(
                    Texture,
                    Position,
                    shellSource,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _scale, // <--- Scale ảnh mai rùa
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
