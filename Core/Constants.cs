namespace MarioGame.Core
{
    public static class Constants
    {
        // Screen
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;
        public const int GAME_WIDTH = 256;
        public const int GAME_HEIGHT = 240;

        // Physics
        public const float GRAVITY = 980f;
        public const float MAX_FALL_SPEED = 600f;
        public const float FRICTION = 0.85f;

        // Player
        public const float PLAYER_SPEED = 150f;
        public const float PLAYER_JUMP_FORCE = -400f;
        public const float PLAYER_RUN_MULTIPLIER = 1.5f;

        // Tile
        public const int TILE_SIZE = 16;

        // Camera
        public const float CAMERA_LERP = 5f;
        public const float CAMERA_OFFSET_X = 100f;

        // Layers
        public const int LAYER_BACKGROUND = 0;
        public const int LAYER_TILES = 1;
        public const int LAYER_ENTITIES = 2;
        public const int LAYER_PLAYER = 3;
        public const int LAYER_UI = 4;
    }
}