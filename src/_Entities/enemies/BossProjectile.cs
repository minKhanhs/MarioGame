using MarioGame.src._Entities.enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Enemies
{
    public class BossProjectile : Enemy
    {
        private float _speed = 250f; // Tốc độ bay sang trái

        public BossProjectile(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            Velocity = new Vector2(-_speed, 0); // Bay thẳng sang trái
            IsActive = true;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Di chuyển (Không dùng ApplyPhysics để tránh trọng lực rơi xuống đất)
            Position += Velocity * dt;

            // Tự hủy khi bay quá xa
            // (Giả sử camera đang ở X dương, nếu đạn < Camera.X - 100 thì hủy)
            // Tạm thời check số âm lớn
            if (Position.X < -1000) IsActive = false;
        }

        public override void OnStomped()
        {
            // Đạn boss không đạp được -> Không làm gì hoặc hủy
        }

        public override Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive) spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}