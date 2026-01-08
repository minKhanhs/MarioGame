using MarioGame.src._Core.Camera;
using MarioGame.src._Entities.enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.items
{
    public class BulletBill : Enemy
    {
        private float _speed = 15f;
        private Camera _camera;

        // --- CẤU HÌNH TỶ LỆ (SCALE) ---
        // 0.6f = 60% kích thước gốc. Bạn chỉnh số này để to/nhỏ tùy ý.
        private float _scale = 0.5f;

        private enum BillState { Warning, Firing }
        private BillState _state;

        private float _warningTimer = 1.5f;
        private float _blinkTimer = 0f;

        private static Texture2D _warningTexture;

        public BulletBill(Texture2D texture, Camera camera, GraphicsDevice graphicsDevice)
        {
            Texture = texture;
            _camera = camera;

            if (_warningTexture == null)
            {
                _warningTexture = new Texture2D(graphicsDevice, 1, 1);
                _warningTexture.SetData(new[] { Color.White });
            }

            _state = BillState.Warning;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
        }

        public void Spawn(float yPosition)
        {
            Position = new Vector2(0, yPosition);
            _state = BillState.Warning;
            _warningTimer = 1.5f;
            IsActive = true;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_state == BillState.Warning)
            {
                _warningTimer -= dt;
                _blinkTimer += dt;

                float camRight = _camera.Position.X + _camera.Viewport.Width;
                Position = new Vector2(camRight - 50, Position.Y);

                if (_warningTimer <= 0)
                {
                    _state = BillState.Firing;
                    Position = new Vector2(camRight, Position.Y);
                    Velocity.X = -_speed;
                }
            }
            else
            {
                ApplyPhysics();
                Velocity.Y = 0; // Đạn bay thẳng, không rơi

                if (Position.X < _camera.Position.X - 100)
                {
                    IsActive = false;
                }
            }
        }

        public override void OnStomped()
        {
            IsActive = false;
        }

        // CẬP NHẬT HITBOX THEO SCALE
        public override Rectangle Bounds
        {
            get
            {
                if (_state == BillState.Warning)
                {
                    // Trả về kích thước của ô cảnh báo (32x32) để Camera biết mà vẽ
                    return new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
                }

                // Kích thước đạn thật (đã scale)
                int scaledWidth = (int)(Texture.Width * _scale);
                int scaledHeight = (int)(Texture.Height * _scale);

                return new Rectangle((int)Position.X, (int)Position.Y, scaledWidth, scaledHeight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            if (_state == BillState.Warning)
            {
                if (_blinkTimer % 0.2f < 0.1f)
                {
                    Rectangle warningRect = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
                    spriteBatch.Draw(_warningTexture, warningRect, Color.Red);
                }
            }
            else
            {
                // VẼ VIÊN ĐẠN VỚI SCALE
                spriteBatch.Draw(
                    Texture,
                    Position,
                    null, // Source rectangle (null lấy cả ảnh)
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _scale, // <-- Truyền tỷ lệ thu nhỏ vào đây
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
