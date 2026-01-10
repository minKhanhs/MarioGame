using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Input
{
    // Cần Singleton như trong biểu đồ để truy cập toàn cục
    public class InputSettings
    {
        private static InputSettings _instance;
        public static InputSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new InputSettings();
                return _instance;
            }
        }

        // Dictionary lưu cấu hình phím cho P1 và P2
        public Dictionary<EGameAction, Keys> P1_KeyMap { get; private set; }
        public Dictionary<EGameAction, Keys> P2_KeyMap { get; private set; }

        private InputSettings()
        {
            P1_KeyMap = new Dictionary<EGameAction, Keys>();
            P2_KeyMap = new Dictionary<EGameAction, Keys>();
            SetDefaultKeys();
        }

        private void SetDefaultKeys()
        {
            // Cấu hình mặc định Player 1 (Arrow Keys - để support single player)
            // Chỉ cho phép rebind MoveLeft, MoveRight, Jump
            P1_KeyMap[EGameAction.MoveLeft] = Keys.Left;
            P1_KeyMap[EGameAction.MoveRight] = Keys.Right;
            P1_KeyMap[EGameAction.Jump] = Keys.Up;
            // Run, Attack, Pause không cho rebind - fixed keys
            P1_KeyMap[EGameAction.Run] = Keys.RightShift;
            P1_KeyMap[EGameAction.Attack] = Keys.RightControl;
            P1_KeyMap[EGameAction.Pause] = Keys.Escape;

            // Cấu hình mặc định Player 2 (WASD)
            // Chỉ cho phép rebind MoveLeft, MoveRight, Jump
            P2_KeyMap[EGameAction.MoveLeft] = Keys.A;
            P2_KeyMap[EGameAction.MoveRight] = Keys.D;
            P2_KeyMap[EGameAction.Jump] = Keys.W;
            // Run, Attack, Pause không cho phép rebind - fixed keys
            P2_KeyMap[EGameAction.Run] = Keys.LeftShift;
            P2_KeyMap[EGameAction.Attack] = Keys.H;
            P2_KeyMap[EGameAction.Pause] = Keys.Back;
        }

        // Hàm đổi nút (Remap) - chỉ cho phép rebind MoveLeft, MoveRight, Jump, Attack
        public void RemapKey(int playerIndex, EGameAction action, Keys newKey)
        {
            // Không cho phép rebind Run, Pause
            if (action == EGameAction.Run || action == EGameAction.Pause)
            {
                System.Diagnostics.Debug.WriteLine($"[INPUT] Cannot rebind {action} - fixed key only");
                return;
            }

            var targetMap = (playerIndex == 1) ? P1_KeyMap : P2_KeyMap;

            if (targetMap.ContainsKey(action))
            {
                targetMap[action] = newKey;
            }
            else
            {
                targetMap.Add(action, newKey);
            }
        }

        // Giả lập lưu/tải cấu hình (Bạn có thể dùng JSON để lưu ra file)
        public void SaveSettings() { /* Logic lưu file JSON */ }
        public void LoadSettings() { /* Logic đọc file JSON */ }
    }
}
