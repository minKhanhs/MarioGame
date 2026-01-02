using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Base
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; protected set; }
        public bool IsActive { get; set; } = true;
        public bool IsVisible { get; set; } = true;

        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y
        );

        protected Texture2D _texture;
        protected Rectangle _sourceRect;

        public GameObject(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public abstract void Update(float deltaTime);
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset);

        public virtual void Destroy()
        {
            IsActive = false;
        }
    }
}
