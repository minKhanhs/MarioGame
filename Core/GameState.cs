namespace MarioGame.Core
{
    public enum GameState
    {
        StartScreen,
        Playing,
        Paused,
        LevelComplete,
        GameOver,
        Settings,
        LevelSelect
    }

    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        Dead
    }

    public enum PowerUpState
    {
        Small,
        Big,
        Fire
    }

    public enum EnemyState
    {
        Walking,
        Shell,
        Sliding,
        Dead
    }

    public enum CameraMode
    {
        FollowPlayer,
        AutoScroll,
        Fixed
    }
}