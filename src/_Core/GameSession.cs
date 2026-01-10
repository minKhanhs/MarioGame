namespace MarioGame.src._Core
{
    /// <summary>
    /// Tracks statistics for CURRENT game session ONLY
    /// Session stats ???c reset khi t?o world m?i
    /// 
    /// SESSION STATS (Reset when starting new game):
    /// - TotalScore: ?i?m c?ng d?n trong game hi?n t?i
    /// - TotalTime: Th?i gian ch?i trong game hi?n t?i
    /// - TotalCoinsThisGame: Coins ?n ???c trong game hi?n t?i
    /// - TotalEnemiesThisGame: Quái gi?t trong game hi?n t?i
    /// 
    /// CAREER STATS: Xem CareerStats.cs (static class riêng)
    /// </summary>
    public class GameSession
    {
        // ========== SESSION STATS (reset per game) ==========
        public int TotalScore { get; set; }
        public float TotalTime { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevelReached { get; set; }
        public int TotalCoinsThisGame { get; set; }
        public int TotalEnemiesThisGame { get; set; }

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
            TotalCoinsThisGame = 0;
            TotalEnemiesThisGame = 0;

            LoadSession();
        }

        /// <summary>
        /// G?i khi hoàn thành 1 level - c?ng ?i?m vào session và career stats
        /// </summary>
        public void AddLevelStats(int score, int coinsThisLevel, int enemiesThisLevel, float time)
        {
            // 1. C?ng vào SESSION stats
            TotalScore += score;
            TotalTime += time;
            TotalCoinsThisGame += coinsThisLevel;
            TotalEnemiesThisGame += enemiesThisLevel;

            // 2. C?ng vào CAREER stats (static, không reset)
            CareerStats.AddStats(coinsThisLevel, enemiesThisLevel);

            SaveSession();
            System.Diagnostics.Debug.WriteLine($"[SESSION] AddLevelStats - Session Score: {TotalScore}, Coins: {TotalCoinsThisGame}, Enemies: {TotalEnemiesThisGame}");
        }

        /// <summary>
        /// Reset only SESSION stats when starting a new game
        /// Career stats NEVER reset (managed by CareerStats class)
        /// </summary>
        public void ResetSession()
        {
            // Reset SESSION stats
            TotalScore = 0;
            TotalTime = 0f;
            CurrentLevel = 1;
            MaxLevelReached = 1;
            TotalCoinsThisGame = 0;
            TotalEnemiesThisGame = 0;

            // ? KHÔNG reset career stats - chúng managed b?i CareerStats class
            // ? KHÔNG load t? file - ch? reset memory variables

            System.Diagnostics.Debug.WriteLine($"[SESSION] Reset - Session stats cleared");
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
            if (level > MaxLevelReached)
                MaxLevelReached = level;
        }

        /// <summary>
        /// Compatibility properties for old code
        /// </summary>
        public int TotalCoins 
        { 
            get => TotalCoinsThisGame;
            set => TotalCoinsThisGame = value;
        }
        
        public int TotalEnemiesDefeated 
        { 
            get => TotalEnemiesThisGame;
            set => TotalEnemiesThisGame = value;
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
                    // Session stats ONLY
                    TotalScore, 
                    TotalTime, 
                    CurrentLevel, 
                    MaxLevelReached,
                    TotalCoinsThisGame,
                    TotalEnemiesThisGame
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
                    if (data.TryGetProperty("TotalScore", out var scoreElement))
                        TotalScore = scoreElement.GetInt32();
                    if (data.TryGetProperty("TotalTime", out var timeElement))
                        TotalTime = timeElement.GetSingle();
                    if (data.TryGetProperty("CurrentLevel", out var levelElement))
                        CurrentLevel = levelElement.GetInt32();
                    if (data.TryGetProperty("MaxLevelReached", out var maxLevelElement))
                        MaxLevelReached = maxLevelElement.GetInt32();
                    if (data.TryGetProperty("TotalCoinsThisGame", out var coinsElement))
                        TotalCoinsThisGame = coinsElement.GetInt32();
                    if (data.TryGetProperty("TotalEnemiesThisGame", out var enemiesElement))
                        TotalEnemiesThisGame = enemiesElement.GetInt32();

                    System.Diagnostics.Debug.WriteLine($"[SESSION] Loaded - Score: {TotalScore}");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load session: {ex.Message}");
            }
        }
    }
}
