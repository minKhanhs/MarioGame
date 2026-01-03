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
    public class Block : GameObj
    {
        public Block(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            // Block tĩnh nên không cần update vị trí hay vật lý
            // Nếu là Block hỏi chấm (?) thì sau này thêm logic animation ở đây
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive && Texture != null)
                spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
