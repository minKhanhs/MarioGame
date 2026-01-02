using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MarioGame.Core;

namespace MarioGame.Level
{
    public class TileMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Tile[,] Tiles { get; private set; }

        private int _tileSize = Constants.TILE_SIZE;

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];

            // Initialize all tiles as air
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tiles[x, y] = new Tile(TileType.Air, x * _tileSize, y * _tileSize, _tileSize);
                }
            }
        }

        public void SetTile(int x, int y, TileType type)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Tiles[x, y] = new Tile(type, x * _tileSize, y * _tileSize, _tileSize);
            }
        }

        public Tile GetTile(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return Tiles[x, y];
            }
            return null;
        }

        public List<Tile> GetSolidTiles()
        {
            List<Tile> solidTiles = new List<Tile>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].IsSolid)
                    {
                        solidTiles.Add(Tiles[x, y]);
                    }
                }
            }

            return solidTiles;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            // Only draw visible tiles (culling)
            int startX = (int)(cameraOffset.X / _tileSize) - 1;
            int endX = startX + (Constants.SCREEN_WIDTH / _tileSize) + 2;
            int startY = (int)(cameraOffset.Y / _tileSize) - 1;
            int endY = startY + (Constants.SCREEN_HEIGHT / _tileSize) + 2;

            startX = System.Math.Max(0, startX);
            endX = System.Math.Min(Width, endX);
            startY = System.Math.Max(0, startY);
            endY = System.Math.Min(Height, endY);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Tiles[x, y].Draw(spriteBatch, cameraOffset);
                }
            }
        }
    }
}
