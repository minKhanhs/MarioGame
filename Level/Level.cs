using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarioGame.Core;
using MarioGame.Entities.Base;
using MarioGame.Entities.Player;
using MarioGame.Entities.Enemies;
using System.Collections.Generic;

namespace MarioGame.Level
{
    public enum TileType
    {
        Air = 0,
        Ground = 1,
        Brick = 2,
        QuestionBlock = 3,
        Pipe = 4,
        Platform = 5
    }

    public class Tile : GameObject
    {
        public TileType Type { get; private set; }
        public bool IsSolid { get; private set; }
        public bool IsBreakable { get; private set; }
        public bool HasItem { get; private set; }
        public string ItemType { get; private set; }

        private bool isHit = false;

        public Tile(Vector2 position, TileType type)
            : base(position, new Vector2(Constants.TILE_SIZE, Constants.TILE_SIZE))
        {
            Type = type;
            Layer = Constants.LAYER_TILES;
            HasGravity = false;

            SetupTileProperties();
        }

        private void SetupTileProperties()
        {
            switch (Type)
            {
                case TileType.Ground:
                case TileType.Brick:
                case TileType.Pipe:
                    IsSolid = true;
                    IsBreakable = (Type == TileType.Brick);
                    break;

                case TileType.QuestionBlock:
                    IsSolid = true;
                    HasItem = true;
                    ItemType = "Coin"; // Default
                    break;

                case TileType.Platform:
                    IsSolid = true; // Only solid from top
                    break;

                default:
                    IsSolid = false;
                    break;
            }
        }

        public override void OnCollision(GameObject other, CollisionSide side)
        {
            if (other is Player player)
            {
                if (side == CollisionSide.Bottom && Type == TileType.QuestionBlock && !isHit)
                {
                    // Player hit question block from below
                    OnHit(player);
                }
                else if (side == CollisionSide.Bottom && Type == TileType.Brick && !isHit)
                {
                    // Player hit brick from below
                    if (player.CurrentState != PlayerStateType.Small)
                    {
                        BreakBrick();
                    }
                    else
                    {
                        OnHit(player);
                    }
                }
            }
        }

        private void OnHit(Player player)
        {
            if (isHit) return;

            isHit = true;

            if (HasItem)
            {
                SpawnItem(player);
            }

            // Change to empty block
            if (Type == TileType.QuestionBlock)
            {
                Type = TileType.Brick; // Visual change
            }

            // Bounce animation
            // TODO: Add animation

            SoundManager.Instance?.PlaySound("bump");
        }

        private void SpawnItem(Player player)
        {
            // TODO: Create item based on ItemType
            Vector2 itemPos = Position - new Vector2(0, Constants.TILE_SIZE);

            switch (ItemType)
            {
                case "Coin":
                    player.AddCoin();
                    player.AddScore(100);
                    break;
                case "Mushroom":
                    // Spawn mushroom powerup
                    break;
                case "FireFlower":
                    // Spawn fire flower
                    break;
            }
        }

        private void BreakBrick()
        {
            IsActive = false;
            IsSolid = false;

            // TODO: Spawn brick particles
            SoundManager.Instance?.PlaySound("break");
        }
    }

    public class TileMap
    {
        private Tile[,] tiles;
        public int Width { get; private set; }
        public int Height { get; private set; }

        private List<Tile> solidTiles = new List<Tile>();

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new Tile[width, height];
        }

        public void SetTile(int x, int y, TileType type)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            Vector2 position = new Vector2(x * Constants.TILE_SIZE, y * Constants.TILE_SIZE);
            tiles[x, y] = new Tile(position, type);

            if (tiles[x, y].IsSolid)
            {
                solidTiles.Add(tiles[x, y]);
            }
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;

            return tiles[x, y];
        }

        public List<Tile> GetSolidTiles()
        {
            return solidTiles;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var tile in solidTiles)
            {
                if (tile != null && tile.IsActive)
                {
                    tile.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset, Rectangle viewBounds)
        {
            // Only draw visible tiles (culling)
            int startX = (int)(viewBounds.X / Constants.TILE_SIZE);
            int endX = (int)((viewBounds.X + viewBounds.Width) / Constants.TILE_SIZE) + 1;
            int startY = (int)(viewBounds.Y / Constants.TILE_SIZE);
            int endY = (int)((viewBounds.Y + viewBounds.Height) / Constants.TILE_SIZE) + 1;

            startX = MathHelper.Clamp(startX, 0, Width);
            endX = MathHelper.Clamp(endX, 0, Width);
            startY = MathHelper.Clamp(startY, 0, Height);
            endY = MathHelper.Clamp(endY, 0, Height);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Tile tile = tiles[x, y];
                    if (tile != null && tile.Type != TileType.Air)
                    {
                        tile.Draw(spriteBatch, cameraOffset);
                    }
                }
            }
        }
    }

    public class Level
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public int TimeLimit { get; set; }

        public TileMap Map { get; private set; }
        public List<Player> Players { get; private set; } = new List<Player>();
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<GameObject> Items { get; private set; } = new List<GameObject>();

        public CameraMode CameraMode { get; set; } = Systems.Rendering.CameraMode.FollowPlayer;

        private float elapsedTime = 0f;
        public int RemainingTime => (int)(TimeLimit - elapsedTime);

        public bool IsCompleted { get; private set; } = false;
        public bool IsFailed { get; private set; } = false;

        public Level(int levelId, string name, int timeLimit, int mapWidth, int mapHeight)
        {
            LevelId = levelId;
            Name = name;
            TimeLimit = timeLimit;
            Map = new TileMap(mapWidth, mapHeight);
        }

        public void Initialize()
        {
            // Setup camera
            CameraManager.Instance.Initialize(
                Map.Width * Constants.TILE_SIZE,
                Map.Height * Constants.TILE_SIZE
            );
            CameraManager.Instance.SetMode(CameraMode);
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
            if (Players.Count == 1)
            {
                CameraManager.Instance.SetTarget(player);
            }
        }

        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void AddItem(GameObject item)
        {
            Items.Add(item);
        }

        public void Update(GameTime gameTime)
        {
            if (IsCompleted || IsFailed) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += deltaTime;

            // Check time limit
            if (RemainingTime <= 0)
            {
                IsFailed = true;
                return;
            }

            // Update map
            Map.Update(gameTime);

            // Update players
            foreach (var player in Players)
            {
                if (player.IsActive)
                    player.Update(gameTime);
            }

            // Update enemies
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                if (Enemies[i].IsActive)
                {
                    Enemies[i].Update(gameTime);
                }
                else
                {
                    Enemies.RemoveAt(i);
                }
            }

            // Update items
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i].IsActive)
                {
                    Items[i].Update(gameTime);
                }
                else
                {
                    Items.RemoveAt(i);
                }
            }

            // Check if all players are dead
            bool allPlayersDead = true;
            foreach (var player in Players)
            {
                if (player.IsActive || player.Lives > 0)
                {
                    allPlayersDead = false;
                    break;
                }
            }

            if (allPlayersDead)
            {
                IsFailed = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 cameraOffset = CameraManager.Instance.GetOffset();
            Rectangle viewBounds = CameraManager.Instance.MainCamera.ViewBounds;

            // Draw background (TODO)

            // Draw map
            Map.Draw(spriteBatch, cameraOffset, viewBounds);

            // Draw items
            foreach (var item in Items)
            {
                if (item.IsActive)
                    item.Draw(spriteBatch, cameraOffset);
            }

            // Draw enemies
            foreach (var enemy in Enemies)
            {
                if (enemy.IsActive)
                    enemy.Draw(spriteBatch, cameraOffset);
            }

            // Draw players
            foreach (var player in Players)
            {
                if (player.IsActive)
                    player.Draw(spriteBatch, cameraOffset);
            }
        }

        public void Complete()
        {
            IsCompleted = true;
            // Calculate bonus score based on remaining time
            foreach (var player in Players)
            {
                player.AddScore(RemainingTime * 10);
            }
        }
    }
}