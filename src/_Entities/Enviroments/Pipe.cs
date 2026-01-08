using MarioGame.src._Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.src._Entities.Enviroments
{
    public class Pipe : GameObj
    {
        public Pipe(Texture2D texture, Vector2 position)
        {
            Texture = texture;

            // Xử lý vị trí:
            // Tương tự Castle, vì ảnh Pipe thường cao (ví dụ 64px) 
            // mà map định nghĩa ô 32px, nên ta cần đẩy nó lên để chân chạm đất.
            // Nếu ảnh Pipe của bạn chuẩn 32x64 (rộng 1 ô, cao 2 ô):
            float yOffset = texture.Height - 32;
            Position = new Vector2(position.X, position.Y - yOffset);
        }

        public override void Update(GameTime gameTime)
        {
            // Cống đứng yên
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive && Texture != null)
                spriteBatch.Draw(Texture, Position, Color.White);
        }

        // Ghi đè Hitbox nếu ảnh có khoảng trắng, 
        // nhưng thường ảnh Pipe hình chữ nhật đặc nên dùng mặc định là ổn.
    }
}
