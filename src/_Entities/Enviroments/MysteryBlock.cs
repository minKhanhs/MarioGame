

using MarioGame.src._Entities.items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarioGame.src._Entities.Enviroments
{
    public class MysteryBlock : Block
    {
        private bool _isEmpty = false; // Đã bị đập chưa?

        // Biến để tạo hiệu ứng nảy lên khi bị đập
        private float _bounceOffset = 0f;
        private float _bounceSpeed = 0f;
        private bool _isBouncing = false;
        private float _startY; // Vị trí Y ban đầu

        // Random để chọn vật phẩm
        private Random _random = new Random();

        public MysteryBlock(Texture2D texture, Vector2 position) : base(texture, position)
        {
            _startY = position.Y;
        }

        public override void Update(GameTime gameTime)
        {
            // Logic nảy lên rồi rơi xuống
            if (_isBouncing)
            {
                Position.Y += _bounceSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _bounceSpeed += 800f * (float)gameTime.ElapsedGameTime.TotalSeconds; // Trọng lực kéo xuống

                // Khi rơi về vị trí cũ thì dừng
                if (Position.Y >= _startY)
                {
                    Position.Y = _startY;
                    _isBouncing = false;
                    _bounceSpeed = 0;
                }
            }
        }

        // Hàm này được gọi khi Mario đụng đầu vào
        public Item SpawnItem(Texture2D coinTex, Texture2D mushroomTex)
        {
            if (_isEmpty || _isBouncing) return null;

            // 1. Kích hoạt hiệu ứng nảy
            _isBouncing = true;
            _bounceSpeed = -200f; // Vận tốc nảy lên
            _isEmpty = true; // Đánh dấu đã rỗng

            // 2. Thay đổi màu sắc (Tùy chọn: làm tối đi để biết là block rỗng)
            // Color = Color.Gray; (Nếu bạn muốn vẽ màu xám)

            // 3. Random sinh vật phẩm
            // Vị trí sinh ra: Ngay trên đầu block
            Vector2 spawnPos = new Vector2(Position.X, _startY - 32);

            int chance = _random.Next(0, 2); // Random 0 hoặc 1

            if (chance == 0)
            {
                // Sinh ra Coin
                // Tạo hiệu ứng Coin nảy lên rồi biến mất (hoặc cộng điểm)
                // Ở đây ta tạo Coin vật lý bình thường
                return new Coin(coinTex, spawnPos);
            }
            else
            {
                // Sinh ra Nấm
                return new Mushroom(mushroomTex, spawnPos);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Nếu đã rỗng (bị đập rồi), vẽ màu tối hơn (Gray)
            Color color = _isEmpty ? Color.Gray : Color.White;
            spriteBatch.Draw(Texture, Position, color);
        }
    }
}
