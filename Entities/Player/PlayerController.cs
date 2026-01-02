using MarioGame.Managers;
using Microsoft.Xna.Framework;

namespace MarioGame.Entities.Player
{
    public class PlayerController
    {
        private Player _player;
        private int _playerIndex; // 1 or 2

        public PlayerController(Player player, int playerIndex)
        {
            _player = player;
            _playerIndex = playerIndex;
        }

        public void HandleInput(float deltaTime)
        {
            float horizontal = _playerIndex == 1 ?
                InputManager.Instance.GetP1Horizontal() :
                InputManager.Instance.GetP2Horizontal();

            bool jump = _playerIndex == 1 ?
                InputManager.Instance.P1_IsJumping() :
                InputManager.Instance.P2_IsJumping();

            bool run = _playerIndex == 1 ?
                InputManager.Instance.P1_IsRunning() :
                InputManager.Instance.P2_IsRunning();

            bool shoot = _playerIndex == 1 ?
                InputManager.Instance.P1_IsShooting() :
                InputManager.Instance.P2_IsShooting();

            // Movement
            float speed = _player.MoveSpeed;
            if (run) speed *= 1.5f;

            _player.Velocity = new Vector2(horizontal * speed, _player.Velocity.Y);

            // Jump
            if (jump && _player.IsGrounded)
            {
                _player.Jump();
            }

            // Shoot
            if (shoot && _player.PowerUp == Core.PowerUpState.Fire)
            {
                _player.Shoot();
            }
        }
    }
}