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
        // Handle both custom PlayerIndex (1, 2) and MonoGame PlayerIndex (One, Two)
        public InputFrame GetInput(int playerIndex)
        {
            InputFrame frame = new InputFrame();
            KeyboardState state = Keyboard.GetState();

            // 1. Xác định lấy cấu hình của người chơi nào
            Dictionary<EGameAction, Keys> keyMap;

            if (playerIndex == 1 || playerIndex == (int)PlayerIndex.One)
            {
                keyMap = InputSettings.Instance.P1_KeyMap;
            }
            else if (playerIndex == 2 || playerIndex == (int)PlayerIndex.Two)
            {
                keyMap = InputSettings.Instance.P2_KeyMap;
            }
            else
            {
                return frame; // Chưa hỗ trợ P3, P4
            }

            // 2. Xử lý di chuyển (X_Axis)
            if (keyMap.ContainsKey(EGameAction.MoveLeft) && state.IsKeyDown(keyMap[EGameAction.MoveLeft]))
            {
                frame.X_Axis -= 1.0f;
            }

            if (keyMap.ContainsKey(EGameAction.MoveRight) && state.IsKeyDown(keyMap[EGameAction.MoveRight]))
            {
                frame.X_Axis += 1.0f;
            }

            // 3. Xử lý các hành động khác
            if (keyMap.ContainsKey(EGameAction.Jump))
                frame.IsJumpPressed = state.IsKeyDown(keyMap[EGameAction.Jump]);
            
            if (keyMap.ContainsKey(EGameAction.Run))
                frame.IsRunPressed = state.IsKeyDown(keyMap[EGameAction.Run]);
            
            if (keyMap.ContainsKey(EGameAction.Attack))
                frame.IsAttackPressed = state.IsKeyDown(keyMap[EGameAction.Attack]);
            
            if (keyMap.ContainsKey(EGameAction.Pause))
                frame.IsPausePressed = state.IsKeyDown(keyMap[EGameAction.Pause]);

            return frame;
        }

        // Overload for MonoGame PlayerIndex enum
        public InputFrame GetInput(PlayerIndex playerIndex)
        {
            return GetInput((int)playerIndex);
        }
    }
}
