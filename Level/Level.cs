using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarioGame.Core;
using MarioGame.Entities.Base;
using MarioGame.Entities.Player;
using MarioGame.Entities.Enemies;
using MarioGame.Entities.Items;
using MarioGame.Managers;
using MarioGame.Systems.Physics;
using System.Collections.Generic;
using System.Linq;

namespace MarioGame.Level
{
    public class Level
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public float TimeLimit { get; set; }
        public bool IsCompleted { get; set; }

        public TileMap TileMap { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Item> Items { get; private set; }

        private PhysicsEngine _physics;
        private CameraManager _camera;
        private CameraMode _cameraMode;

        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;

        public Level(int levelId, string name, float timeLimit, CameraMode cameraMode)
        {
            LevelId = levelId;
            Name = name;
            TimeLimit = timeLimit;
            _cameraMode = cameraMode;

            Players = new List<Player>();
            Enemies = new List<Enemy>();
            Items = new List<Item>();

            _physics = new PhysicsEngine();
            _camera = new CameraManager();
            _camera.Mode = cameraMode;
        }

        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
        }

        public void CreateTileMap(int width, int height, int[,] tileData)
        {
            TileMap = new TileMap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TileMap.SetTile(x, y, (TileType)tileData[x, y]);
                }
            }

            _camera.SetBounds(0, width * Constants.TILE_SIZE - Constants.SCREEN_WIDTH);
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void Update(float deltaTime)
        {
            // Update camera
            if (Players.Count > 0)
            {
                _camera.Update(deltaTime, Players[0].Position);
            }

            // Update players
            foreach (var player in Players)
            {
                if (player.IsActive)
                {
                    player.Update(deltaTime);
                }
            }

            // Update enemies
            foreach (var enemy in Enemies.ToList())
            {
                if (enemy.IsActive)
                {
                    enemy.Update(deltaTime);
                }
                else
                {
                    Enemies.Remove(enemy);
                }
            }

            // Update items
            foreach (var item in Items.ToList())
            {
                if (item.IsActive)
                {
                    item.Update(deltaTime);
                }
                else
                {
                    Items.Remove(item);
                }
            }

            // Physics and collision
            var allEntities = new List<Entity>();
            allEntities.AddRange(Players);
            allEntities.AddRange(Enemies);
            allEntities.AddRange(Items);

            var solidTiles = TileMap.GetSolidTiles().Cast<ICollidable>().ToList();
            _physics.Update(allEntities, solidTiles, deltaTime);

            // Check player collision with enemies
            CheckPlayerEnemyCollisions();
        }

        private void CheckPlayerEnemyCollisions()
        {
            foreach (var player in Players)
            {
                if (!player.IsActive || player.IsDead) continue;

                foreach (var enemy in Enemies)
                {
                    if (!enemy.IsActive) continue;

                    if (player.Bounds.Intersects(enemy.Bounds))
                    {
                        // Check if player is stomping enemy (from above)
                        if (player.Velocity.Y > 0 && player.Bounds.Bottom - enemy.Bounds.Top < 10)
                        {
                            enemy.Stomp();
                            player.Velocity = new Vector2(player.Velocity.X, -200); // Small bounce
                        }
                        else
                        {
                            // Player takes damage
                            player.TakeDamage();
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 cameraOffset = _camera.Position;

            // Draw background
            _graphicsDevice.Clear(Color.CornflowerBlue);

            // Draw tilemap
            TileMap?.Draw(spriteBatch, cameraOffset);

            // Draw items
            foreach (var item in Items)
            {
                if (item.IsActive && item.IsVisible)
                {
                    item.Draw(spriteBatch, cameraOffset);
                }
            }

            // Draw enemies
            foreach (var enemy in Enemies)
            {
                if (enemy.IsActive && enemy.IsVisible)
                {
                    enemy.Draw(spriteBatch, cameraOffset);
                }
            }

            // Draw players
            foreach (var player in Players)
            {
                if (player.IsActive && player.IsVisible)
                {
                    player.Draw(spriteBatch, cameraOffset);
                }
            }
        }
    }
}