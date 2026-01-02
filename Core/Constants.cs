namespace MarioGame.Core
{
    public static class Constants
    {
        // Screen
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;
        public const int TILE_SIZE = 32;

        // Physics
        public const float GRAVITY = 980f;
        public const float MAX_FALL_SPEED = 500f;
        public const float FRICTION = 0.85f;

        // Player
        public const float PLAYER_SPEED = 200f;
        public const float PLAYER_JUMP_SPEED = -400f;
        public const float PLAYER_RUN_MULTIPLIER = 1.5f;

        // Enemy
        public const float GOOMBA_SPEED = 50f;
        public const float KOOPA_SPEED = 60f;
        public const float KOOPA_SHELL_SPEED = 300f;

        // Camera
        public const float CAMERA_LERP_SPEED = 0.1f;
        public const float CAMERA_DEAD_ZONE = 100f;

        // Layers
        public const int LAYER_BACKGROUND = 0;
        public const int LAYER_TILES = 1;
        public const int LAYER_ITEMS = 2;
        public const int LAYER_ENEMIES = 3;
        public const int LAYER_PLAYER = 4;
        public const int LAYER_UI = 5;
    }

    public enum GameState
    {
        StartMenu,
        LevelSelect,
        Playing,
        Paused,
        GameOver,
        Victory,
        Settings
    }

    public enum PlayerStateType
    {
        Small,
        Big,
        Fire
    }

    public enum Direction
    {
        Left = -1,
        Right = 1
    }
}