using Microsoft.Xna.Framework;
using MarioGame.Entities.Base;
using System;
using System.Collections.Generic;

namespace MarioGame.Systems.Physics
{
    public class CollisionDetector
    {
        // AABB Collision Detection
        public static bool CheckCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }

        // Determine which side the collision occurred on
        public static CollisionSide GetCollisionSide(GameObject obj1, GameObject obj2, Vector2 previousPosition)
        {
            Rectangle bounds1 = obj1.Bounds;
            Rectangle bounds2 = obj2.Bounds;

            if (!bounds1.Intersects(bounds2))
                return CollisionSide.None;

            // Calculate overlap on each axis
            float overlapLeft = bounds1.Right - bounds2.Left;
            float overlapRight = bounds2.Right - bounds1.Left;
            float overlapTop = bounds1.Bottom - bounds2.Top;
            float overlapBottom = bounds2.Bottom - bounds1.Top;

            // Find minimum overlap
            float minOverlap = Math.Min(
                Math.Min(overlapLeft, overlapRight),
                Math.Min(overlapTop, overlapBottom)
            );

            // Determine collision side based on minimum overlap
            if (minOverlap == overlapTop && obj1.Velocity.Y > 0)
                return CollisionSide.Bottom; // obj1 hit bottom of obj2
            else if (minOverlap == overlapBottom && obj1.Velocity.Y < 0)
                return CollisionSide.Top; // obj1 hit top of obj2
            else if (minOverlap == overlapLeft && obj1.Velocity.X > 0)
                return CollisionSide.Right; // obj1 hit right side of obj2
            else if (minOverlap == overlapRight && obj1.Velocity.X < 0)
                return CollisionSide.Left; // obj1 hit left side of obj2

            return CollisionSide.None;
        }

        // Resolve collision by pushing objects apart
        public static void ResolveCollision(GameObject obj1, GameObject obj2, CollisionSide side)
        {
            Rectangle bounds1 = obj1.Bounds;
            Rectangle bounds2 = obj2.Bounds;

            switch (side)
            {
                case CollisionSide.Bottom:
                    // obj1 is on top of obj2
                    obj1.Position = new Vector2(obj1.Position.X, bounds2.Top - obj1.Size.Y);
                    obj1.Velocity = new Vector2(obj1.Velocity.X, 0);
                    obj1.IsGrounded = true;
                    break;

                case CollisionSide.Top:
                    // obj1 hit obj2 from below
                    obj1.Position = new Vector2(obj1.Position.X, bounds2.Bottom);
                    obj1.Velocity = new Vector2(obj1.Velocity.X, 0);
                    break;

                case CollisionSide.Left:
                    // obj1 hit obj2 from the right
                    obj1.Position = new Vector2(bounds2.Right, obj1.Position.Y);
                    obj1.Velocity = new Vector2(0, obj1.Velocity.Y);
                    break;

                case CollisionSide.Right:
                    // obj1 hit obj2 from the left
                    obj1.Position = new Vector2(bounds2.Left - obj1.Size.X, obj1.Position.Y);
                    obj1.Velocity = new Vector2(0, obj1.Velocity.Y);
                    break;
            }
        }
    }

    public class PhysicsEngine
    {
        private List<GameObject> dynamicObjects = new List<GameObject>();
        private List<GameObject> staticObjects = new List<GameObject>();

        public void AddDynamicObject(GameObject obj)
        {
            if (!dynamicObjects.Contains(obj))
                dynamicObjects.Add(obj);
        }

        public void AddStaticObject(GameObject obj)
        {
            if (!staticObjects.Contains(obj))
                staticObjects.Add(obj);
        }

        public void RemoveObject(GameObject obj)
        {
            dynamicObjects.Remove(obj);
            staticObjects.Remove(obj);
        }

        public void Clear()
        {
            dynamicObjects.Clear();
            staticObjects.Clear();
        }

        public void Update(GameTime gameTime)
        {
            // Reset grounded state
            foreach (var obj in dynamicObjects)
            {
                if (obj.IsActive)
                    obj.IsGrounded = false;
            }

            // Check collisions between dynamic and static objects
            foreach (var dynamicObj in dynamicObjects)
            {
                if (!dynamicObj.IsActive || !dynamicObj.HasCollision)
                    continue;

                // Store previous position for collision side detection
                Vector2 previousPosition = dynamicObj.Position - dynamicObj.Velocity *
                    (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Check against static objects (tiles, platforms)
                foreach (var staticObj in staticObjects)
                {
                    if (!staticObj.IsActive)
                        continue;

                    if (CollisionDetector.CheckCollision(dynamicObj.Bounds, staticObj.Bounds))
                    {
                        CollisionSide side = CollisionDetector.GetCollisionSide(
                            dynamicObj, staticObj, previousPosition);

                        if (side != CollisionSide.None)
                        {
                            CollisionDetector.ResolveCollision(dynamicObj, staticObj, side);
                            dynamicObj.OnCollision(staticObj, side);
                            staticObj.OnCollision(dynamicObj, GetOppositeSide(side));
                        }
                    }
                }

                // Check against other dynamic objects
                foreach (var otherObj in dynamicObjects)
                {
                    if (dynamicObj == otherObj || !otherObj.IsActive || !otherObj.HasCollision)
                        continue;

                    if (CollisionDetector.CheckCollision(dynamicObj.Bounds, otherObj.Bounds))
                    {
                        CollisionSide side = CollisionDetector.GetCollisionSide(
                            dynamicObj, otherObj, previousPosition);

                        if (side != CollisionSide.None)
                        {
                            // Don't resolve position for dynamic-dynamic collisions
                            // Just notify both objects
                            dynamicObj.OnCollision(otherObj, side);
                            otherObj.OnCollision(dynamicObj, GetOppositeSide(side));
                        }
                    }
                }
            }
        }

        private CollisionSide GetOppositeSide(CollisionSide side)
        {
            switch (side)
            {
                case CollisionSide.Top: return CollisionSide.Bottom;
                case CollisionSide.Bottom: return CollisionSide.Top;
                case CollisionSide.Left: return CollisionSide.Right;
                case CollisionSide.Right: return CollisionSide.Left;
                default: return CollisionSide.None;
            }
        }

        // Get all objects in a specific area (for camera culling)
        public List<GameObject> GetObjectsInArea(Rectangle area)
        {
            List<GameObject> result = new List<GameObject>();

            foreach (var obj in dynamicObjects)
            {
                if (obj.IsActive && obj.Bounds.Intersects(area))
                    result.Add(obj);
            }

            foreach (var obj in staticObjects)
            {
                if (obj.IsActive && obj.Bounds.Intersects(area))
                    result.Add(obj);
            }

            return result;
        }
    }
}