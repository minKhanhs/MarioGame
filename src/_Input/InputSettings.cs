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
            // Khi chơi 2 người, P1 có thể remap sang WASD nếu cần
            P1_KeyMap[EGameAction.MoveLeft] = Keys.Left;
            P1_KeyMap[EGameAction.MoveRight] = Keys.Right;
            P1_KeyMap[EGameAction.Jump] = Keys.Up;
            P1_KeyMap[EGameAction.Run] = Keys.RightControl;
            P1_KeyMap[EGameAction.Attack] = Keys.RightShift;
            P1_KeyMap[EGameAction.Pause] = Keys.Escape;

            // Cấu hình mặc định Player 2 (WASD + Space)
            P2_KeyMap[EGameAction.MoveLeft] = Keys.A;
            P2_KeyMap[EGameAction.MoveRight] = Keys.D;
            P2_KeyMap[EGameAction.Jump] = Keys.W;
            P2_KeyMap[EGameAction.Run] = Keys.LeftShift;
            P2_KeyMap[EGameAction.Attack] = Keys.J;
            P2_KeyMap[EGameAction.Pause] = Keys.Back;
        }

        // Hàm đổi nút (Remap)
        public void RemapKey(int playerIndex, EGameAction action, Keys newKey)
        {
            var targetMap = (playerIndex == 1) ? P1_KeyMap : P2_KeyMap;

            if (targetMap.ContainsKey(action))
            {
                targetMap[action] = newKey;
            }
            else
            {
                targetMap.Add(action, newKey);
            }
            // TODO: Gọi SaveSettings() sau khi remap
        }

        // Giả lập lưu/tải cấu hình (Bạn có thể dùng JSON để lưu ra file)
        public void SaveSettings() { /* Logic lưu file JSON */ }
        public void LoadSettings() { /* Logic đọc file JSON */ }
    }
}
