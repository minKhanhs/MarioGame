using MarioGame.src._Entities.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.items
{
    public class Coin : Item
    {
        public Coin(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            // Coin thường bay lơ lửng, không chịu trọng lực
            // Nếu muốn coin nảy ra từ gạch thì set Gravity > 0 sau
            Gravity = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            // Có thể thêm logic xoay vòng (Animation) tại đây
        }

        public override void OnCollect(Player player)
        {
            player.Coins++;
            // Play Sound: Coin
            IsActive = false; // Biến mất sau khi ăn
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive) spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
