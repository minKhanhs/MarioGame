using MarioGame.src._Entities.player;
using MarioGame.src._Entities.player.states;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.items
{
    public class Mushroom : Item
    {
        private int _width = 37;
        private int _height = 37;

        public Mushroom(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            IsOnGround = false;
        }
        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
            }
        }
        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Nấm chịu vật lý (rơi xuống đất)
            ApplyPhysics();
        }

        public override void OnCollect(Player player)
        {
            // Biến Mario thành người lớn
            player.SetState(new BigState());
            player.Score += 1000;
            IsActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive && Texture != null)
            {
                // VẼ CO DÃN: Ép ảnh texture vào khung hình chữ nhật 32x32
                Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
                spriteBatch.Draw(Texture, destRect, Color.White);
            }
        }
    }
}
