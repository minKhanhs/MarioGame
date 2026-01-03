using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.player;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.enemies
{
    public abstract class Enemy : MovableObj
    {
        public int ScoreValue { get; set; } = 100;

        // Hàm xử lý khi bị Mario đạp lên đầu
        public abstract void OnStomped();

        // Hàm xử lý khi chạm vào Mario (gây sát thương)
        public virtual void OnTouchPlayer(Player player)
        {
            player.TakeDamage();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive && Texture != null)
                spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
