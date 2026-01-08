using MarioGame.src._Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarioGame.src._Entities.enemies
{
    public static class EnemyFactory
    {
        // Hàm này nhận vào mã (code) từ bản đồ và trả về đúng loại quái vật
        public static Enemy CreateEnemy(char typeCode, Vector2 position, Dictionary<string, Texture2D> textures)
        {
            switch (typeCode)
            {
                case 'E': // Goomba
                    if (textures.ContainsKey("goomba"))
                    {
                        return new Goomba(textures["goomba"], position);
                    }
                    break;

                case 'K': // Koopa (Ví dụ sau này bạn thêm rùa)
                    if (textures.ContainsKey("koopa")) // Key texture phải là "koopa"
                        return new Koopa(textures["koopa"], position);
                    break;

                case 'P': // Piranha Plant (Cây ăn thịt)
                    if (textures.ContainsKey("plant")) // Key texture phải là "plant"
                        return new PiranhaPlant(textures["plant"], position);
                    break;

                default:
                    return null;
            }
            return null;
        }
    }
}
