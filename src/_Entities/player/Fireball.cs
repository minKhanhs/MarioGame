using MarioGame.src._Entities.Base;
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
    public class Fireball : MovableObj
    {
        private float _speed = 600f;
        private float _lifeTime = 5.0f;
        private SpriteEffects _effect = SpriteEffects.None;
        private float _scale = 0.08f;

        public Fireball(Texture2D texture, Vector2 position, int direction)
        {
            Texture = texture;
            Position = position;

            // Bay theo hướng
            Velocity.X = _speed * direction;
            Velocity.Y = 0; // Đảm bảo Y bằng 0
            IsOnGround = false;

            // Nếu bắn sang trái (direction = -1) thì lật ngược ảnh lại
            if (direction < 0)
                _effect = SpriteEffects.FlipHorizontally;
            else
                _effect = SpriteEffects.None;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position.X += Velocity.X * dt;
            Velocity.Y = 0; // Bay thẳng tắp, không trọng lực

            // 2. Tự hủy theo thời gian
            _lifeTime -= dt;
            if (_lifeTime <= 0) IsActive = false;
        }

        // Hitbox lấy theo kích thước thật của ảnh
        public override Rectangle Bounds
        {
            get
            {
                // Tính kích thước đã thu nhỏ
                int scaledWidth = (int)(Texture.Width * _scale);
                int scaledHeight = (int)(Texture.Height * _scale);

                return new Rectangle((int)Position.X, (int)Position.Y, scaledWidth, scaledHeight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(
                    Texture,
                    Position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _scale, // <-- Truyền tỷ lệ thu nhỏ vào đây
                    _effect,
                    0f
                );
            }
        }
    }
}
