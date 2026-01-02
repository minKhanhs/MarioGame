using Microsoft.Xna.Framework.Input;

namespace MarioGame.Managers
{
    public sealed class InputManager
    {
        private static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InputManager();
                return _instance;
            }
        }

        private KeyboardState _currentKeyboard;
        private KeyboardState _previousKeyboard;

        private InputManager() { }

        public void Update()
        {
            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();
        }

        // ======================
        // GENERIC
        // ======================

        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key) &&
                   _previousKeyboard.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key);
        }

        // ======================
        // PLAYER 1
        // ======================

        public float GetP1Horizontal()
        {
            if (IsKeyDown(Keys.A)) return -1f;
            if (IsKeyDown(Keys.D)) return 1f;
            return 0f;
        }

        public bool P1_IsJumping()
        {
            return IsKeyPressed(Keys.W);
        }

        public bool P1_IsRunning()
        {
            return IsKeyDown(Keys.LeftShift);
        }

        public bool P1_IsShooting()
        {
            return IsKeyPressed(Keys.Space);
        }

        // ======================
        // PLAYER 2
        // ======================

        public float GetP2Horizontal()
        {
            if (IsKeyDown(Keys.Left)) return -1f;
            if (IsKeyDown(Keys.Right)) return 1f;
            return 0f;
        }

        public bool P2_IsJumping()
        {
            return IsKeyPressed(Keys.Up);
        }

        public bool P2_IsRunning()
        {
            return IsKeyDown(Keys.RightControl);
        }

        public bool P2_IsShooting()
        {
            return IsKeyPressed(Keys.Enter);
        }
    }
}
