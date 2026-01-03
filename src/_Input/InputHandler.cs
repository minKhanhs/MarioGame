using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Input
{
    public class InputHandler
    {
        // PlayerIndex của MonoGame (One, Two, Three, Four)
        public InputFrame GetInput(PlayerIndex playerIndex)
        {
            InputFrame frame = new InputFrame();
            KeyboardState state = Keyboard.GetState();

            // 1. Xác định lấy cấu hình của người chơi nào
            Dictionary<EGameAction, Keys> keyMap;

            if (playerIndex == PlayerIndex.One)
            {
                keyMap = InputSettings.Instance.P1_KeyMap;
            }
            else if (playerIndex == PlayerIndex.Two)
            {
                keyMap = InputSettings.Instance.P2_KeyMap;
            }
            else
            {
                return frame; // Chưa hỗ trợ P3, P4
            }

            // 2. Xử lý di chuyển (X_Axis)
            if (state.IsKeyDown(keyMap[EGameAction.MoveLeft]))
            {
                frame.X_Axis -= 1.0f;
            }

            if (state.IsKeyDown(keyMap[EGameAction.MoveRight]))
            {
                frame.X_Axis += 1.0f;
            }

            // 3. Xử lý các hành động khác
            frame.IsJumpPressed = state.IsKeyDown(keyMap[EGameAction.Jump]);
            frame.IsRunPressed = state.IsKeyDown(keyMap[EGameAction.Run]);
            frame.IsAttackPressed = state.IsKeyDown(keyMap[EGameAction.Attack]);
            frame.IsPausePressed = state.IsKeyDown(keyMap[EGameAction.Pause]);

            return frame;
        }
    }
}
