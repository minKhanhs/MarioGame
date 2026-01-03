using MarioGame.src._Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.Enviroments
{
    public class Castle : GameObj
    {
        public Castle(Texture2D texture, Vector2 position)
        {
            Texture = texture;

            // XỬ LÝ VỊ TRÍ: 
            // MapLoader đưa vào vị trí của ô đất (góc trên trái của ô 32x32).
            // Nhưng Lâu đài rất cao, nên ta phải đẩy nó lên trên để chân lâu đài chạm đất.
            // Công thức: Y mới = Y cũ - (Chiều cao ảnh - Kích thước ô đất)

            float yOffset = texture.Height - 32;
            Position = new Vector2(position.X, position.Y - yOffset);
        }

        public override void Update(GameTime gameTime)
        {
            // Lâu đài đứng yên
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
