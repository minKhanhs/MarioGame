using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarioGame.Entities.Base;

namespace MarioGame.Level
{
    public enum TileType
    {
        Air = 0,
        Ground = 1,
        Brick = 2,
        QuestionBlock = 3,
        Pipe = 4
    }

    public class Tile : ICollidable
    {
        public TileType Type { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsSolid { get; set; }
        public bool IsBreakable { get; set; }
        public bool HasItem { get; set; }
        public string ItemType { get; set; }

        public Tile(TileType type, int x, int y, int size)
        {
            Type = type;
            Bounds = new Rectangle(x, y, size, size);

            switch (type)
            {
                case TileType.Air:
                    IsSolid = false;
                    break;
                case TileType.Ground:
                case TileType.Brick:
                case TileType.QuestionBlock:
                case TileType.Pipe:
                    IsSolid = true;
                    break;
            }

            IsBreakable = (type == TileType.Brick);
        }

        public void OnCollision(ICollidable other, CollisionSide side)
        {
            // Handle tile collision (e.g., break brick, spawn item from question block)
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            Rectangle destRect = new Rectangle(
                (int)(Bounds.X - cameraOffset.X),
                (int)(Bounds.Y - cameraOffset.Y),
                Bounds.Width,
                Bounds.Height
            );

            Color color = Type switch
            {
                TileType.Ground => Color.SaddleBrown,
                TileType.Brick => Color.OrangeRed,
                TileType.QuestionBlock => Color.Yellow,
                TileType.Pipe => Color.Green,
                _ => Color.Transparent
            };

            if (Type != TileType.Air)
            {
                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                spriteBatch.Draw(pixel, destRect, color);
            }
        }
    }
}