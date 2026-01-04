using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.player.states;
using MarioGame.src._Input;
using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.player
{
    public class Player : MovableObj
    {
        private IMarioState _currentState;
        private InputHandler _inputHandler;
        private Dictionary<string, SpriteAnimation> _animations;
        private SpriteAnimation _currentAnim;
        private SpriteEffects _flipEffect = SpriteEffects.None;

        // Player identification
        public int PlayerIndex { get; set; } = 1; // 1 or 2

        // Thông số Gameplay
        public float Scale { get; set; } = 1.0f;
        public int Lives { get; set; } = 3;
        public int Coins { get; set; } = 0;
        public int Score { get; set; } = 0;
        public bool IsInvincible { get; set; } = false;
        public bool HasReachedGoal { get; set; } = false;

        // Hằng số vật lý
        private const float MOVE_SPEED = 200f; // Pixel per second
        private const float JUMP_FORCE = -12f;
        private float _invincibleTimer = 0f;
        private const float INVINCIBLE_DELAY = 2.0f;

        public Player(Vector2 startPos, Dictionary<string, SpriteAnimation> animations, int playerIndex = 1)
        {
            Position = startPos;
            _animations = animations;
            _inputHandler = new InputHandler();
            PlayerIndex = playerIndex;
            SetState(new SmallState());
            // Mặc định là đứng yên
            _currentAnim = _animations["Idle"];
        }

        public void SetState(IMarioState newState)
        {
            _currentState = newState;
            _currentState.Enter(this);
        }
        
        public void StartInvincible()
        {
            IsInvincible = true;
            _invincibleTimer = INVINCIBLE_DELAY; // Bắt đầu đếm ngược 2 giây
        }
        
        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsInvincible)
            {
                _invincibleTimer -= dt; // Trừ dần thời gian

                // Hiệu ứng nhấp nháy (Tùy chọn): Tắt/Bật vẽ liên tục
                // IsVisible = !IsVisible; 

                if (_invincibleTimer <= 0)
                {
                    IsInvincible = false; // Hết giờ -> Tắt bất tử -> Có thể bị đánh tiếp
                                          // IsVisible = true; // Đảm bảo hiện hình lại
                }
            }
            // 1. Xử lý Input
            var input = _inputHandler.GetInput(PlayerIndex);

            // Di chuyển trái phải
            Velocity.X = input.X_Axis * 200f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Velocity.X > 0)
                _flipEffect = SpriteEffects.None; // Mặt phải
            else if (Velocity.X < 0)
                _flipEffect = SpriteEffects.FlipHorizontally; // Lật mặt sang trái

            // 2. Chọn Animation dựa trên vận tốc
            if (input.IsJumpPressed && IsOnGround)
            {
                Velocity.Y = -12f;
                IsOnGround = false;
            }

            // Logic chuyển đổi Animation cơ bản
            if (!IsOnGround)
            {
                // Nếu có animation nhảy thì dùng, không thì dùng chạy
                if (_animations.ContainsKey("Jump")) _currentAnim = _animations["Jump"];
            }
            else if (Velocity.X != 0)
            {
                _currentAnim = _animations["Run"];
            }
            else
            {
                _currentAnim = _animations["Idle"];
            }

            // 3. Cập nhật Animation (để nó chạy frame)
            _currentAnim.Update(gameTime);

            // 2. Áp dụng vật lý (GameObj base)
            ApplyPhysics();

            // 3. Logic khác (Cooldown bất tử, animation...)
        }
        
        public override Rectangle Bounds
        {
            get
            {
                if (_currentAnim == null) return Rectangle.Empty;

                // Nhân chiều rộng và cao với Scale
                int scaledWidth = (int)(_currentAnim.FrameWidth * Scale);
                int scaledHeight = (int)(_currentAnim.FrameHeight * Scale);

                return new Rectangle((int)Position.X, (int)Position.Y, scaledWidth, scaledHeight);
            }
        }

        public void TakeDamage()
        {
            if (!IsInvincible)
            {
                _currentState.TakeDamage(this);
            }
        }

        public void Die()
        {
            System.Diagnostics.Debug.WriteLine($"Player {PlayerIndex} died!");
            Lives--; // Trừ mạng

            if (Lives > 0)
            {
                // Hồi sinh: Reset vị trí về đầu map hoặc Checkpoint
                Position = new Vector2(50, 200); // Vị trí mẫu

                // Reset về dạng nhỏ
                SetState(new SmallState());

                // Reset vận tốc
                Velocity = Vector2.Zero;
            }
            else
            {
                // Hết mạng -> Game Over
                // GameManager.Instance.GameOver();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_currentAnim != null)
            {
                spriteBatch.Draw(
                    _currentAnim.Texture,
                    Position,
                    _currentAnim.CurrentFrameSource,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Scale, // <--- TRUYỀN BIẾN SCALE VÀO ĐÂY
                    _flipEffect,
                    0f
                );
            }
        }
    }
}
