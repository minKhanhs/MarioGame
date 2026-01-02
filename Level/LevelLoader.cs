using Microsoft.Xna.Framework;
using MarioGame.Data.Models;
using MarioGame.Entities.Enemies;
using MarioGame.Entities.Items;
using MarioGame.Systems.Rendering;
using System;

namespace MarioGame.Level
{
    public static class LevelLoader
    {
        // Load level from LevelData
        public static Level LoadLevel(LevelData data)
        {
            if (data == null)
            {
                Console.WriteLine("Error: LevelData is null!");
                return null;
            }

            try
            {
                // Create level
                var level = new Level(
                    data.LevelId,
                    data.Name,
                    data.TimeLimit,
                    data.MapData.Width,
                    data.MapData.Height
                );

                // Set camera mode
                level.CameraMode = ParseCameraMode(data.CameraMode);

                // Load tile map
                LoadTileMap(level, data.MapData);

                // Spawn entities
                SpawnEntities(level, data.Entities);

                Console.WriteLine($"Level {data.LevelId} loaded successfully: {data.Name}");
                return level;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading level {data.LevelId}: {ex.Message}");
                return null;
            }
        }

        // Load tile map from MapData
        private static void LoadTileMap(Level level, MapData mapData)
        {
            if (mapData?.TileMatrix == null)
            {
                Console.WriteLine("Warning: MapData or TileMatrix is null");
                return;
            }

            int index = 0;
            for (int y = 0; y < mapData.Height; y++)
            {
                for (int x = 0; x < mapData.Width; x++)
                {
                    if (index < mapData.TileMatrix.Length)
                    {
                        TileType tileType = (TileType)mapData.TileMatrix[index];
                        level.Map.SetTile(x, y, tileType);
                    }
                    index++;
                }
            }

            Console.WriteLine($"Loaded {mapData.Width}x{mapData.Height} tile map");
        }

        // Spawn entities from EntityData list
        private static void SpawnEntities(Level level, System.Collections.Generic.List<EntityData> entities)
        {
            if (entities == null)
            {
                Console.WriteLine("Warning: No entities to spawn");
                return;
            }

            int enemyCount = 0;
            int itemCount = 0;

            foreach (var entityData in entities)
            {
                Vector2 position = new Vector2(entityData.X, entityData.Y);

                switch (entityData.Type.ToLower())
                {
                    // Enemies
                    case "goomba":
                        level.AddEnemy(new Goomba(position));
                        enemyCount++;
                        break;

                    case "koopa":
                        level.AddEnemy(new Koopa(position));
                        enemyCount++;
                        break;

                    case "piranhaplant":
                    case "piranhaPlant":
                    case "piranha":
                        level.AddEnemy(new PiranhaPlant(position));
                        enemyCount++;
                        break;

                    // Items
                    case "coin":
                        level.AddItem(new Coin(position));
                        itemCount++;
                        break;

                    case "mushroom":
                        level.AddItem(new Mushroom(position));
                        itemCount++;
                        break;

                    case "fireflower":
                    case "flower":
                        level.AddItem(new FireFlower(position));
                        itemCount++;
                        break;

                    case "1up":
                    case "oneup":
                        level.AddItem(new OneUpMushroom(position));
                        itemCount++;
                        break;

                    case "star":
                        level.AddItem(new Star(position));
                        itemCount++;
                        break;

                    default:
                        Console.WriteLine($"Unknown entity type: {entityData.Type}");
                        break;
                }
            }

            Console.WriteLine($"Spawned {enemyCount} enemies and {itemCount} items");
        }

        // Parse camera mode string
        private static CameraMode ParseCameraMode(string mode)
        {
            if (string.IsNullOrEmpty(mode))
                return CameraMode.FollowPlayer;

            switch (mode.ToLower())
            {
                case "followplayer":
                case "follow":
                    return CameraMode.FollowPlayer;

                case "autoscroll":
                case "auto":
                    return CameraMode.AutoScroll;

                case "fixed":
                    return CameraMode.Fixed;

                default:
                    Console.WriteLine($"Unknown camera mode: {mode}, using FollowPlayer");
                    return CameraMode.FollowPlayer;
            }
        }

        // Create a test level programmatically
        public static Level CreateTestLevel(int levelId)
        {
            var level = new Level(levelId, $"Test Level {levelId}", 300, 200, 15);

            // Create ground
            for (int x = 0; x < level.Map.Width; x++)
            {
                level.Map.SetTile(x, 13, TileType.Ground);
                level.Map.SetTile(x, 14, TileType.Ground);
            }

            // Create starting platform
            for (int x = 0; x < 10; x++)
            {
                level.Map.SetTile(x, 12, TileType.Ground);
            }

            // Add platforms
            for (int x = 15; x < 20; x++)
            {
                level.Map.SetTile(x, 10, TileType.Brick);
            }

            for (int x = 25; x < 30; x++)
            {
                level.Map.SetTile(x, 8, TileType.Brick);
            }

            // Add question blocks
            level.Map.SetTile(10, 9, TileType.QuestionBlock);
            level.Map.SetTile(15, 9, TileType.QuestionBlock);
            level.Map.SetTile(20, 9, TileType.QuestionBlock);
            level.Map.SetTile(25, 6, TileType.QuestionBlock);

            // Add pipes
            AddPipe(level, 30, 11, 3);
            AddPipe(level, 40, 11, 4);
            AddPipe(level, 50, 11, 2);

            // Add gap
            for (int x = 60; x < 65; x++)
            {
                level.Map.SetTile(x, 13, TileType.Air);
                level.Map.SetTile(x, 14, TileType.Air);
            }

            // Stairs
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    level.Map.SetTile(70 + i, 13 - j, TileType.Brick);
                }
            }

            // Goal area
            for (int x = 180; x < 200; x++)
            {
                level.Map.SetTile(x, 12, TileType.Ground);
            }

            // Spawn some enemies
            if (levelId >= 2)
            {
                level.AddEnemy(new Goomba(new Vector2(400, 350)));
                level.AddEnemy(new Goomba(new Vector2(600, 350)));
            }

            if (levelId >= 3)
            {
                level.AddEnemy(new Koopa(new Vector2(800, 350)));
                level.AddEnemy(new Goomba(new Vector2(1000, 350)));
            }

            if (levelId >= 4)
            {
                level.AddEnemy(new Koopa(new Vector2(1200, 350)));
                level.AddEnemy(new Koopa(new Vector2(1400, 350)));
            }

            if (levelId >= 6)
            {
                level.AddEnemy(new PiranhaPlant(new Vector2(30 * 32, 11 * 32)));
                level.AddEnemy(new PiranhaPlant(new Vector2(40 * 32, 11 * 32)));
            }

            // Spawn some coins
            for (int i = 0; i < 10; i++)
            {
                level.AddItem(new Coin(new Vector2(300 + i * 100, 250)));
            }

            // Spawn power-ups
            if (levelId >= 2)
            {
                level.AddItem(new Mushroom(new Vector2(500, 200)));
            }

            if (levelId >= 5)
            {
                level.AddItem(new FireFlower(new Vector2(1000, 200)));
            }

            Console.WriteLine($"Created test level {levelId}");
            return level;
        }

        // Helper to add a pipe
        private static void AddPipe(Level level, int x, int baseY, int height)
        {
            for (int h = 0; h < height; h++)
            {
                level.Map.SetTile(x, baseY - h, TileType.Pipe);
                level.Map.SetTile(x + 1, baseY - h, TileType.Pipe);
            }
        }

        // Create tutorial level (Level 1)
        public static Level CreateTutorialLevel()
        {
            var level = new Level(1, "World 1-1: Tutorial", 400, 150, 15);

            // Ground
            for (int x = 0; x < level.Map.Width; x++)
            {
                level.Map.SetTile(x, 13, TileType.Ground);
                level.Map.SetTile(x, 14, TileType.Ground);
            }

            // Tutorial sections
            // Section 1: Basic movement
            for (int x = 5; x < 10; x++)
            {
                level.Map.SetTile(x, 11, TileType.Brick);
            }

            // Section 2: Jumping
            for (int x = 15; x < 18; x++)
            {
                level.Map.SetTile(x, 10, TileType.Brick);
            }

            for (int x = 20; x < 23; x++)
            {
                level.Map.SetTile(x, 9, TileType.Brick);
            }

            // Section 3: Question blocks
            level.Map.SetTile(30, 9, TileType.QuestionBlock);
            level.Map.SetTile(32, 9, TileType.QuestionBlock);
            level.Map.SetTile(34, 9, TileType.QuestionBlock);

            // Add coins for practice
            for (int i = 0; i < 5; i++)
            {
                level.AddItem(new Coin(new Vector2(100 + i * 64, 300)));
            }

            // Add one mushroom
            level.AddItem(new Mushroom(new Vector2(500, 200)));

            // No enemies in tutorial
            Console.WriteLine("Created tutorial level");
            return level;
        }

        // Create boss level (Level 10)
        public static Level CreateBossLevel()
        {
            var level = new Level(10, "World 1-10: Boss Battle", 500, 80, 15);

            // Boss arena
            for (int x = 0; x < level.Map.Width; x++)
            {
                level.Map.SetTile(x, 13, TileType.Ground);
                level.Map.SetTile(x, 14, TileType.Ground);
            }

            // Platforms in arena
            for (int x = 15; x < 20; x++)
            {
                level.Map.SetTile(x, 10, TileType.Brick);
            }

            for (int x = 30; x < 35; x++)
            {
                level.Map.SetTile(x, 10, TileType.Brick);
            }

            for (int x = 50; x < 55; x++)
            {
                level.Map.SetTile(x, 10, TileType.Brick);
            }

            // TODO: Add Boss enemy (Bowser)
            // For now, add multiple strong enemies
            for (int i = 0; i < 5; i++)
            {
                level.AddEnemy(new Koopa(new Vector2(300 + i * 200, 350)));
            }

            for (int i = 0; i < 3; i++)
            {
                level.AddEnemy(new Goomba(new Vector2(400 + i * 300, 350)));
            }

            // Add fire flowers for boss fight
            level.AddItem(new FireFlower(new Vector2(15 * 32, 9 * 32)));
            level.AddItem(new FireFlower(new Vector2(30 * 32, 9 * 32)));

            level.CameraMode = CameraMode.Fixed;

            Console.WriteLine("Created boss level");
            return level;
        }
    }
}