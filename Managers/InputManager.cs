using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.Managers
{
    public enum InputAction
    {
        MoveLeft,
        MoveRight,
        Jump,
        Shoot,
        Run,
        Pause,
        Select,
        Back
    }

    public class InputManager
    {
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new InputManager();
                return instance;
            }
        }

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;

        // Key bindings for Player 1 and Player 2
        private Dictionary<int, Dictionary<InputAction, Keys>> keyBindings;

        private InputManager()
        {
            InitializeDefaultBindings();
        }

        private void InitializeDefaultBindings()
        {
            keyBindings = new Dictionary<int, Dictionary<InputAction, Keys>>();

            // Player 1 bindings (WASD)
            keyBindings[1] = new Dictionary<InputAction, Keys>
            {
                { InputAction.MoveLeft, Keys.A },
                { InputAction.MoveRight, Keys.D },
                { InputAction.Jump, Keys.W },
                { InputAction.Shoot, Keys.J },
                { InputAction.Run, Keys.LeftShift },
                { InputAction.Pause, Keys.Escape },
                { InputAction.Select, Keys.Enter },
                { InputAction.Back, Keys.Back }
            };

            // Player 2 bindings (Arrow Keys)
            keyBindings[2] = new Dictionary<InputAction, Keys>
            {
                { InputAction.MoveLeft, Keys.Left },
                { InputAction.MoveRight, Keys.Right },
                { InputAction.Jump, Keys.Up },
                { InputAction.Shoot, Keys.RightControl },
                { InputAction.Run, Keys.RightShift },
                { InputAction.Pause, Keys.Escape },
                { InputAction.Select, Keys.Enter },
                { InputAction.Back, Keys.Back }
            };
        }

        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(0);
        }

        // Check if key is currently pressed
        public bool IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        // Check if key was just pressed this frame
        public bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) &&
                   !previousKeyboardState.IsKeyDown(key);
        }

        // Check if key was just released this frame
        public bool IsKeyReleased(Keys key)
        {
            return !currentKeyboardState.IsKeyDown(key) &&
                   previousKeyboardState.IsKeyDown(key);
        }

        // Check if action is currently pressed for a player
        public bool IsActionDown(int playerId, InputAction action)
        {
            if (!keyBindings.ContainsKey(playerId))
                return false;

            Keys key = keyBindings[playerId][action];
            return IsKeyDown(key);
        }

        // Check if action was just pressed for a player
        public bool IsActionPressed(int playerId, InputAction action)
        {
            if (!keyBindings.ContainsKey(playerId))
                return false;

            Keys key = keyBindings[playerId][action];
            return IsKeyPressed(key);
        }

        // Check if action was just released for a player
        public bool IsActionReleased(int playerId, InputAction action)
        {
            if (!keyBindings.ContainsKey(playerId))
                return false;

            Keys key = keyBindings[playerId][action];
            return IsKeyReleased(key);
        }

        // Get horizontal axis for a player (-1 left, 0 none, 1 right)
        public float GetHorizontalAxis(int playerId)
        {
            float axis = 0f;

            if (IsActionDown(playerId, InputAction.MoveLeft))
                axis -= 1f;
            if (IsActionDown(playerId, InputAction.MoveRight))
                axis += 1f;

            return axis;
        }

        // Rebind a key for a player
        public void RebindKey(int playerId, InputAction action, Keys newKey)
        {
            if (keyBindings.ContainsKey(playerId))
            {
                keyBindings[playerId][action] = newKey;
            }
        }

        // Get current key binding for an action
        public Keys GetKeyBinding(int playerId, InputAction action)
        {
            if (keyBindings.ContainsKey(playerId) &&
                keyBindings[playerId].ContainsKey(action))
            {
                return keyBindings[playerId][action];
            }
            return Keys.None;
        }

        // GamePad support
        public bool IsGamePadConnected(int playerIndex)
        {
            return GamePad.GetState(playerIndex).IsConnected;
        }

        public float GetGamePadLeftStickX(int playerIndex)
        {
            return GamePad.GetState(playerIndex).ThumbSticks.Left.X;
        }

        public bool IsGamePadButtonPressed(int playerIndex, Buttons button)
        {
            GamePadState current = GamePad.GetState(playerIndex);
            GamePadState previous = previousGamePadState;

            return current.IsButtonDown(button) && previous.IsButtonUp(button);
        }

        // Get all currently pressed keys (useful for key rebinding UI)
        public Keys[] GetPressedKeys()
        {
            return currentKeyboardState.GetPressedKeys();
        }

        // Check if any key was just pressed
        public bool IsAnyKeyPressed()
        {
            Keys[] currentKeys = currentKeyboardState.GetPressedKeys();
            Keys[] previousKeys = previousKeyboardState.GetPressedKeys();

            return currentKeys.Length > previousKeys.Length;
        }

        // Get the first key that was just pressed
        public Keys? GetFirstPressedKey()
        {
            Keys[] currentKeys = currentKeyboardState.GetPressedKeys();
            Keys[] previousKeys = previousKeyboardState.GetPressedKeys();

            foreach (Keys key in currentKeys)
            {
                bool wasPressed = false;
                foreach (Keys prevKey in previousKeys)
                {
                    if (key == prevKey)
                    {
                        wasPressed = true;
                        break;
                    }
                }

                if (!wasPressed)
                    return key;
            }

            return null;
        }

        // Save key bindings to data
        public void SaveBindings()
        {
            // TODO: Save to DataManager
        }

        // Load key bindings from data
        public void LoadBindings()
        {
            // TODO: Load from DataManager
        }
    }
}