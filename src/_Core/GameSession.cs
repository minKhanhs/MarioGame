namespace MarioGame.src._Core
{
    /// <summary>
    /// Tracks cumulative statistics across all levels in a single game session
    /// 
    /// PERMANENT STATS (NEVER reset, saved to file):
    /// - TotalCoins
    /// - TotalEnemiesDefeated
    /// 
    /// SESSION STATS (Reset when starting new game):
    /// - TotalScore
    /// - TotalTime
    /// - CurrentLevel
    /// - MaxLevelReached
    /// </summary>
    public class GameSession
    {
        // SESSION STATS (reset per game)
        public int TotalScore { get; set; }
        public float TotalTime { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevelReached { get; set; }

        // PERMANENT STATS (NEVER reset - these are career stats)
        public int TotalCoins { get; set; }
        public int TotalEnemiesDefeated { get; set; }

        private static GameSession _instance;
        private const string SESSION_FILE = "Content/data/gamesession.json";

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
            // Initialize session stats
            TotalScore = 0;
            TotalTime = 0f;
            CurrentLevel = 1;
            MaxLevelReached = 1;

            // Permanent stats will be loaded from file
            TotalCoins = 0;
            TotalEnemiesDefeated = 0;

            LoadSession();
        }

        public void AddLevelStats(int score, int coins, int enemies, float time)
        {
            // Add to permanent stats
            TotalCoins += coins;
            TotalEnemiesDefeated += enemies;

            // Add to session stats
            TotalScore += score;
            TotalTime += time;

            SaveSession();
            System.Diagnostics.Debug.WriteLine($"[SESSION] Updated - Score: {TotalScore}, Coins: {TotalCoins}, Enemies: {TotalEnemiesDefeated}");
        }

        /// <summary>
        /// Reset only SESSION stats when starting a new game
        /// PERMANENT stats (TotalCoins, TotalEnemiesDefeated) are NEVER reset
        /// </summary>
        public void ResetSession()
        {
            // Reset SESSION stats
            TotalScore = 0;
            TotalTime = 0f;
            CurrentLevel = 1;
            MaxLevelReached = 1;

            // DO NOT reset TotalCoins and TotalEnemiesDefeated - they are permanent!

            SaveSession();
            System.Diagnostics.Debug.WriteLine("[SESSION] Reset - Session stats cleared, Permanent stats preserved");
        }

        /// <summary>
        /// Reset PERMANENT stats (only use for testing or special cases)
        /// </summary>
        public void ResetPermanentStats()
        {
            TotalCoins = 0;
            TotalEnemiesDefeated = 0;
            SaveSession();
            System.Diagnostics.Debug.WriteLine("[SESSION] Permanent stats cleared");
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
            if (level > MaxLevelReached)
                MaxLevelReached = level;
        }

        private void SaveSession()
        {
            try
            {
                string directory = System.IO.Path.GetDirectoryName(SESSION_FILE);
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                var data = new 
                { 
                    // Session stats
                    TotalScore, 
                    TotalTime, 
                    CurrentLevel, 
                    MaxLevelReached,
                    // Permanent stats
                    TotalCoins, 
                    TotalEnemiesDefeated
                };
                var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(SESSION_FILE, json);
                System.Diagnostics.Debug.WriteLine("[SESSION] Saved to file");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to save session: {ex.Message}");
            }
        }

        private void LoadSession()
        {
            try
            {
                if (System.IO.File.Exists(SESSION_FILE))
                {
                    string json = System.IO.File.ReadAllText(SESSION_FILE);
                    var data = System.Text.Json.JsonDocument.Parse(json).RootElement;

                    // Load session stats
                    TotalScore = data.GetProperty("TotalScore").GetInt32();
                    TotalTime = data.GetProperty("TotalTime").GetSingle();
                    CurrentLevel = data.GetProperty("CurrentLevel").GetInt32();
                    MaxLevelReached = data.GetProperty("MaxLevelReached").GetInt32();

                    // Load permanent stats
                    TotalCoins = data.GetProperty("TotalCoins").GetInt32();
                    TotalEnemiesDefeated = data.GetProperty("TotalEnemiesDefeated").GetInt32();

                    System.Diagnostics.Debug.WriteLine($"[SESSION] Loaded - Score: {TotalScore}, Coins: {TotalCoins}, Enemies: {TotalEnemiesDefeated}");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load session: {ex.Message}");
            }
        }
    }
}
