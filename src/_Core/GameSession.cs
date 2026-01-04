namespace MarioGame.src._Core
{
    /// <summary>
    /// Tracks cumulative statistics across all levels in a single game session
    /// </summary>
    public class GameSession
    {
        public int TotalScore { get; set; }
        public int TotalCoins { get; set; }
        public int TotalEnemiesDefeated { get; set; }
        public float TotalTime { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevelReached { get; set; }

        private static GameSession _instance;

        public static GameSession Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameSession();
                return _instance;
            }
        }

        public GameSession()
        {
            ResetSession();
        }

        public void AddLevelStats(int score, int coins, int enemies, float time)
        {
            TotalScore += score;
            TotalCoins += coins;
            TotalEnemiesDefeated += enemies;
            TotalTime += time;
            System.Diagnostics.Debug.WriteLine($"[SESSION] Updated - Score: {TotalScore}, Coins: {TotalCoins}, Enemies: {TotalEnemiesDefeated}");
        }

        public void ResetSession()
        {
            TotalScore = 0;
            TotalCoins = 0;
            TotalEnemiesDefeated = 0;
            TotalTime = 0f;
            CurrentLevel = 1;
            MaxLevelReached = 1;
            System.Diagnostics.Debug.WriteLine("[SESSION] Reset");
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
            if (level > MaxLevelReached)
                MaxLevelReached = level;
        }
    }
}
