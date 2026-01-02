using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MarioGame.Core;
using MarioGame.Entities.Player;
using MarioGame.Entities.Enemies;
using MarioGame.Entities.Items;
using System.IO;
using System.Text.Json;
using MarioGame.Data.Models;

namespace MarioGame.Level
{
    public class LevelLoader
    {
        private ContentManager _content;

        public LevelLoader(ContentManager content)
        {
            _content = content;
        }

        public Level LoadLevel(int levelId)
        {
            // For now, create hardcoded levels
            // TODO: Load from JSON file
            return CreateHardcodedLevel(levelId);
        }

        private Level CreateHardcodedLevel(int levelId)
        {
            Level level = null;

            switch (levelId)
            {
                case 1:
                    level = CreateLevel1();
                    break;
                case 2:
                    level = CreateLevel2();
                    break;
                default:
                    level = CreateLevel1();
                    break;
            }

            return level;
        }

        private Level CreateLevel1()
        {
            var level = new Level(1, "World 1-1: Tutorial", 300, CameraMode.FollowPlayer);

            // Create tilemap (20x15)
            int[,] tileData = new int[20, 15];

            // Fill with air
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    tileData[x, y] = (int)TileType.Air;
                }
            }

            // Ground
            for (int x = 0; x < 20; x++)
            {
                tileData[x, 14] = (int)TileType.Ground;
                tileData[x, 13] = (int)TileType.Ground;
            }

            // Some platforms
            tileData[5, 10] = (int)TileType.Brick;
            tileData[6, 10] = (int)TileType.QuestionBlock;
            tileData[7, 10] = (int)TileType.Brick;

            // Pipe
            tileData[10, 12] = (int)TileType.Pipe;
            tileData[10, 13] = (int)TileType.Pipe;

            level.CreateTileMap(20, 15, tileData);

            // Add player
            var player = new Player(new Vector2(32, 200), 1);
            level.AddPlayer(player);

            // Add enemies
            level.AddEnemy(new Goomba(new Vector2(150, 200)));
            level.AddEnemy(new Goomba(new Vector2(250, 200)));
            level.AddEnemy(new Koopa(new Vector2(350, 200)));

            // Add items
            level.AddItem(new Coin(new Vector2(100, 150)));
            level.AddItem(new Coin(new Vector2(120, 150)));
            level.AddItem(new Mushroom(new Vector2(200, 150)));

            return level;
        }

        private Level CreateLevel2()
        {
            var level = new Level(2, "World 1-2: Underground", 300, CameraMode.FollowPlayer);

            // Create tilemap
            int[,] tileData = new int[30, 15];

            // Fill with air
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    tileData[x, y] = (int)TileType.Air;
                }
            }

            // Ground
            for (int x = 0; x < 30; x++)
            {
                tileData[x, 14] = (int)TileType.Ground;
                tileData[x, 13] = (int)TileType.Ground;
            }

            // Platforms and obstacles
            for (int x = 8; x < 12; x++)
            {
                tileData[x, 10] = (int)TileType.Brick;
            }

            level.CreateTileMap(30, 15, tileData);

            // Add player
            var player = new Player(new Vector2(32, 200), 1);
            level.AddPlayer(player);

            // More enemies
            level.AddEnemy(new Goomba(new Vector2(200, 200)));
            level.AddEnemy(new Koopa(new Vector2(300, 200)));
            level.AddEnemy(new Koopa(new Vector2(400, 200)));
            level.AddEnemy(new PiranhaPlant(new Vector2(500, 200)));

            return level;
        }

        public Level LoadFromJson(string jsonPath)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonPath);
                var levelData = JsonSerializer.Deserialize<LevelData>(jsonContent);

                var level = new Level(
                    levelData.LevelId,
                    levelData.Name,
                    levelData.TimeLimit,
                    levelData.CameraMode
                );

                // Create tilemap from data
                if (levelData.MapData != null)
                {
                    int[,] tileData = new int[levelData.MapData.Width, levelData.MapData.Height];
                    int index = 0;

                    for (int y = 0; y < levelData.MapData.Height; y++)
                    {
                        for (int x = 0; x < levelData.MapData.Width; x++)
                        {
                            if (index < levelData.MapData.TileMatrix.Length)
                            {
                                tileData[x, y] = levelData.MapData.TileMatrix[index];
                                index++;
                            }
                        }
                    }

                    level.CreateTileMap(levelData.MapData.Width, levelData.MapData.Height, tileData);
                }

                // Add entities from data
                if (levelData.Entities != null)
                {
                    foreach (var entityData in levelData.Entities)
                    {
                        Vector2 position = new Vector2(entityData.X, entityData.Y);

                        switch (entityData.Type.ToLower())
                        {
                            case "goomba":
                                level.AddEnemy(new Goomba(position));
                                break;
                            case "koopa":
                                level.AddEnemy(new Koopa(position));
                                break;
                            case "piranhaplant":
                                level.AddEnemy(new PiranhaPlant(position));
                                break;
                            case "coin":
                                level.AddItem(new Coin(position));
                                break;
                            case "mushroom":
                                level.AddItem(new Mushroom(position));
                                break;
                            case "fireflower":
                                level.AddItem(new FireFlower(position));
                                break;
                        }
                    }
                }

                return level;
            }
            catch
            {
                return CreateLevel1(); // Fallback
            }
        }
    }
}