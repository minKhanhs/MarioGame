using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.src._Entities.Base
{
    public abstract class GameObj
    {
        public Vector2 Position;
        public Texture2D Texture;
        public bool IsActive = true; // Để kiểm tra vật thể còn tồn tại không (đã bị ăn/chết chưa)

        // Hitbox cơ bản
        public virtual Rectangle Bounds
        {
            get
            {
                if (Texture == null) return Rectangle.Empty;
                return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            }
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
