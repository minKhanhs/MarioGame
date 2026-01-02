using MarioGame.Core;

namespace MarioGame.Entities.Player
{
    public interface IPlayerState
    {
        void Enter(Player player);
        void Update(Player player, float deltaTime);
        void Exit(Player player);
        void HandleInput(Player player);
    }

    public class IdleState : IPlayerState
    {
        public void Enter(Player player) { }

        public void Update(Player player, float deltaTime)
        {
            if (player.Velocity.X != 0)
            {
                player.ChangeState(new RunningState());
            }
            else if (!player.IsGrounded)
            {
                player.ChangeState(new FallingState());
            }
        }

        public void Exit(Player player) { }
        public void HandleInput(Player player) { }
    }

    public class RunningState : IPlayerState
    {
        public void Enter(Player player) { }

        public void Update(Player player, float deltaTime)
        {
            if (player.Velocity.X == 0)
            {
                player.ChangeState(new IdleState());
            }
            else if (!player.IsGrounded)
            {
                player.ChangeState(new FallingState());
            }
        }

        public void Exit(Player player) { }
        public void HandleInput(Player player) { }
    }

    public class JumpingState : IPlayerState
    {
        public void Enter(Player player) { }

        public void Update(Player player, float deltaTime)
        {
            if (player.Velocity.Y > 0)
            {
                player.ChangeState(new FallingState());
            }
        }

        public void Exit(Player player) { }
        public void HandleInput(Player player) { }
    }

    public class FallingState : IPlayerState
    {
        public void Enter(Player player) { }

        public void Update(Player player, float deltaTime)
        {
            if (player.IsGrounded)
            {
                if (player.Velocity.X != 0)
                    player.ChangeState(new RunningState());
                else
                    player.ChangeState(new IdleState());
            }
        }

        public void Exit(Player player) { }
        public void HandleInput(Player player) { }
    }

    public class DeadState : IPlayerState
    {
        private float _deathTimer = 2f;

        public void Enter(Player player)
        {
            player.AffectedByGravity = false;
            player.Velocity = new Microsoft.Xna.Framework.Vector2(0, -300);
        }

        public void Update(Player player, float deltaTime)
        {
            _deathTimer -= deltaTime;
            if (_deathTimer <= 0)
            {
                // Notify game systems that player death sequence completed
                GameEvents.PlayerDied?.Invoke();
            }
        }

        public void Exit(Player player) { }
        public void HandleInput(Player player) { }
    }
}
