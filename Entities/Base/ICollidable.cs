using Microsoft.Xna.Framework;

namespace MarioGame.Entities.Base
{
    public interface ICollidable
    {
        Rectangle Bounds { get; }
        bool IsSolid { get; }
        void OnCollision(ICollidable other, CollisionSide side);
    }

    public enum CollisionSide
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }
}