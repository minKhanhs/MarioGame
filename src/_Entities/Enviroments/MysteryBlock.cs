using MarioGame.src._Entities.items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarioGame.src._Entities.Enviroments
{
    public class MysteryBlock : Block
    {
        private bool _isEmpty = false;

        // Biến di chuyển hình ảnh (không ảnh hưởng hitbox)
        private float _visualOffsetY = 0f;
        private float _bounceVelocity = 0f;
        private bool _isBouncing = false;

        private Random _random = new Random();

        public MysteryBlock(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_isBouncing)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Chỉ thay đổi Offset hiển thị
                _visualOffsetY += _bounceVelocity * dt;
                _bounceVelocity += 1000f * dt; // Trọng lực kéo xuống nhanh

                // Khi rơi về vị trí cũ (hoặc thấp hơn)
                if (_visualOffsetY >= 0)
                {
                    _visualOffsetY = 0;
                    _bounceVelocity = 0;
                    _isBouncing = false;
                }
            }
        }

        public Item SpawnItem(Texture2D coinTex, Texture2D mushroomTex)
        {
            if (_isEmpty || _isBouncing) return null;

            // 1. Kích hoạt nảy
            _isBouncing = true;
            _bounceVelocity = -250f; // Nảy lên mạnh
            _isEmpty = true;

            // 2. Sinh vật phẩm
            // Vị trí sinh ra: Chính giữa block rồi trồi lên
            Vector2 spawnPos = new Vector2(Position.X, Position.Y - 32);

            int chance = _random.Next(0, 2);
            if (chance == 0) return new Coin(coinTex, spawnPos);
            else return new Mushroom(mushroomTex, spawnPos);
        }

        // Hitbox luôn giữ nguyên vị trí gốc -> Mario không bị kẹt
        public override Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = _isEmpty ? Color.Gray : Color.White;

            // Vẽ tại vị trí gốc + độ nảy
            Vector2 drawPos = new Vector2(Position.X, Position.Y + _visualOffsetY);

            spriteBatch.Draw(Texture, drawPos, color);
        }
    }
}