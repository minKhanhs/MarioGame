using Microsoft.Xna.Framework;
using MarioGame.Entities.Base;

namespace MarioGame.Systems.Physics
{
    public class CollisionDetector
    {
        public CollisionSide CheckCollision(ICollidable a, ICollidable b)
        {
            if (!a.Bounds.Intersects(b.Bounds))
                return CollisionSide.None;

            // Calculate overlap on each side
            int overlapLeft = a.Bounds.Right - b.Bounds.Left;
            int overlapRight = b.Bounds.Right - a.Bounds.Left;
            int overlapTop = a.Bounds.Bottom - b.Bounds.Top;
            int overlapBottom = b.Bounds.Bottom - a.Bounds.Top;

            // Find minimum overlap
            int minOverlap = int.MaxValue;
            CollisionSide side = CollisionSide.None;

            if (overlapLeft < minOverlap)
            {
                minOverlap = overlapLeft;
                side = CollisionSide.Right;
            }

            if (overlapRight < minOverlap)
            {
                minOverlap = overlapRight;
                side = CollisionSide.Left;
            }

            if (overlapTop < minOverlap)
            {
                minOverlap = overlapTop;
                side = CollisionSide.Bottom;
            }

            if (overlapBottom < minOverlap)
            {
                minOverlap = overlapBottom;
                side = CollisionSide.Top;
            }

            return side;
        }

        public bool IsAABBCollision(Rectangle a, Rectangle b)
        {
            return a.Intersects(b);
        }
    }
}