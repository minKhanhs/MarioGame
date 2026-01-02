using MarioGame.Entities.Base;
using System.Collections.Generic;

namespace MarioGame.Systems.Physics
{
    public class PhysicsEngine
    {
        private CollisionDetector _collisionDetector;

        public PhysicsEngine()
        {
            _collisionDetector = new CollisionDetector();
        }

        public void Update(List<Entity> entities, List<ICollidable> staticObjects, float deltaTime)
        {
            // Update all entities
            foreach (var entity in entities)
            {
                if (!entity.IsActive) continue;

                entity.IsGrounded = false;
            }

            // Check collisions
            CheckCollisions(entities, staticObjects);
        }

        private void CheckCollisions(List<Entity> entities, List<ICollidable> staticObjects)
        {
            // Entity vs Static objects (tiles, blocks)
            foreach (var entity in entities)
            {
                if (!entity.IsActive) continue;

                foreach (var staticObj in staticObjects)
                {
                    var collision = _collisionDetector.CheckCollision(entity, staticObj);
                    if (collision != CollisionSide.None)
                    {
                        ResolveCollision(entity, staticObj, collision);
                        entity.OnCollision(staticObj, collision);
                    }
                }
            }

            // Entity vs Entity
            for (int i = 0; i < entities.Count; i++)
            {
                if (!entities[i].IsActive) continue;

                for (int j = i + 1; j < entities.Count; j++)
                {
                    if (!entities[j].IsActive) continue;

                    var collision = _collisionDetector.CheckCollision(entities[i], entities[j]);
                    if (collision != CollisionSide.None)
                    {
                        entities[i].OnCollision(entities[j], collision);
                        entities[j].OnCollision(entities[i], GetOppositeCollision(collision));
                    }
                }
            }
        }

        private void ResolveCollision(Entity entity, ICollidable other, CollisionSide side)
        {
            if (!other.IsSolid) return;

            var entityBounds = entity.Bounds;
            var otherBounds = other.Bounds;

            switch (side)
            {
                case CollisionSide.Bottom:
                    entity.Position = new Microsoft.Xna.Framework.Vector2(
                        entity.Position.X,
                        otherBounds.Top - entity.Size.Y
                    );
                    entity.IsGrounded = true;
                    break;

                case CollisionSide.Top:
                    entity.Position = new Microsoft.Xna.Framework.Vector2(
                        entity.Position.X,
                        otherBounds.Bottom
                    );
                    break;

                case CollisionSide.Left:
                    entity.Position = new Microsoft.Xna.Framework.Vector2(
                        otherBounds.Right,
                        entity.Position.Y
                    );
                    break;

                case CollisionSide.Right:
                    entity.Position = new Microsoft.Xna.Framework.Vector2(
                        otherBounds.Left - entity.Size.X,
                        entity.Position.Y
                    );
                    break;
            }
        }

        private CollisionSide GetOppositeCollision(CollisionSide side)
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
    }
}