using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core
{
    internal class Constants
    {
        // Game Configuration
        public const int TOTAL_LEVELS = 10;
        public const int BOSS_LEVEL = 10;

        // Screen Dimensions
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;

        // Player Configuration
        public const int PLAYER_STARTING_LIVES = 3;
        public const float PLAYER_GRAVITY = 0.6f;
        public const float PLAYER_JUMP_POWER = 12f;

        // Camera Configuration
        public const int MAP_WIDTH = 3200;
        public const int MAP_HEIGHT = 736;
    }
}
