using MarioGame.src._Data.models;
using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.enemies;
using MarioGame.src._Entities.items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MarioGame.src._Entities.Enviroments;

namespace MarioGame.src._Data
{
    public static class MapLoader
    {
        // Dictionary lưu trữ Texture để không phải load lại nhiều lần
        private static Dictionary<string, Texture2D> _textureMap;

        public static void Initialize(Dictionary<string, Texture2D> textures)
        {
            _textureMap = textures;
        }

        public static List<GameObj> LoadLevel(string filePath)
        {
            List<GameObj> gameObjects = new List<GameObj>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Map file not found: {filePath}");

            string jsonContent = File.ReadAllText(filePath);
            var mapData = JsonSerializer.Deserialize<LevelMapData>(jsonContent);

            int tileSize = mapData.TileSize;
            int y = 0;

            foreach (var row in mapData.Layout)
            {
                for (int x = 0; x < row.Length; x++)
                {
                    char code = row[x];
                    Vector2 position = new Vector2(x * tileSize, y * tileSize);

                    // Factory tạo đối tượng dựa trên ký tự
                    GameObj obj = CreateObjectFromCode(code, position);

                    if (obj != null)
                    {
                        gameObjects.Add(obj);
                    }
                }
                y++;
            }

            return gameObjects;
        }

        private static GameObj CreateObjectFromCode(char code, Vector2 pos)
        {
            switch (code)
            {
                // Môi trường
                case '#': return new Block(_textureMap["brick"], pos);
                case 'G': return new Block(_textureMap["ground"], pos);

                // Vật phẩm
                case 'C': return new Coin(_textureMap["coin"], pos);
                case 'M': return new Mushroom(_textureMap["mushroom"], pos);

                // Kẻ thù
                case 'E':
                case 'K': // Nếu map có Koopa
                case 'P': // Nếu map có Plant
                    return EnemyFactory.CreateEnemy(code, pos, _textureMap);
                case 'Z': // Castle
                    return new Castle(_textureMap["castle"], pos);

                // Player (thường Player được Game1 quản lý riêng, 
                // nhưng map có thể đánh dấu vị trí xuất phát)
                // case 'P': return new SpawnPoint(pos); 

                default: return null; // Khoảng trắng hoặc ký tự lạ
            }
        }
    }
}
