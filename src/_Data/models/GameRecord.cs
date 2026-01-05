using System;
using System.Collections.Generic;

namespace MarioGame.src._Data.models
{
    /// <summary>
    /// Represents a single game record/playthrough
    /// </summary>
    [Serializable]
    public class GameRecord
    {
        public string PlayerName { get; set; }
        public DateTime PlayDate { get; set; }
        public int TotalScore { get; set; }
        public int TotalCoins { get; set; }
        public int TotalEnemiesDefeated { get; set; }
        public int LevelsCompleted { get; set; }
        public float TotalTime { get; set; } // in seconds
        public int MaxLevel { get; set; }
        public int GameMode { get; set; } // 1 = single player, 2 = two players

        public GameRecord()
        {
            PlayDate = DateTime.Now;
            TotalScore = 0;
            TotalCoins = 0;
            TotalEnemiesDefeated = 0;
            LevelsCompleted = 0;
            TotalTime = 0f;
            MaxLevel = 1;
            GameMode = 1;
        }

        public GameRecord(string playerName, int score, int coins, int enemies, int levelsCompleted, float time, int maxLevel, int gameMode = 1)
        {
            PlayerName = playerName;
            PlayDate = DateTime.Now;
            TotalScore = score;
            TotalCoins = coins;
            TotalEnemiesDefeated = enemies;
            LevelsCompleted = levelsCompleted;
            TotalTime = time;
            MaxLevel = maxLevel;
            GameMode = gameMode;
        }

        public override string ToString()
        {
            string modeStr = GameMode == 2 ? "2P" : "1P";
            return $"[{modeStr}] {PlayerName} | Score: {TotalScore} | Coins: {TotalCoins} | Level: {MaxLevel} | {PlayDate:yyyy-MM-dd HH:mm}";
        }

        public string GetFormattedDuration()
        {
            int hours = (int)(TotalTime / 3600);
            int minutes = (int)((TotalTime % 3600) / 60);
            int seconds = (int)(TotalTime % 60);

            if (hours > 0)
                return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            else
                return $"{minutes:D2}:{seconds:D2}";
        }
    }

    /// <summary>
    /// Manages game record storage and retrieval
    /// </summary>
    public class GameRecordManager
    {
        private List<GameRecord> _records;
        private const string RECORDS_FILE = "Content/data/game_records.json";
        private static GameRecordManager _instance;

        public static GameRecordManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameRecordManager();
                return _instance;
            }
        }

        public GameRecordManager()
        {
            _records = new List<GameRecord>();
            LoadRecords();
        }

        public void AddRecord(GameRecord record)
        {
            _records.Add(record);
            SaveRecords();
            System.Diagnostics.Debug.WriteLine($"[RECORD] Saved: {record}");
        }

        public List<GameRecord> GetRecords()
        {
            return new List<GameRecord>(_records);
        }

        public List<GameRecord> GetTopScores(int limit = 10)
        {
            var sorted = new List<GameRecord>(_records);
            sorted.Sort((a, b) => b.TotalScore.CompareTo(a.TotalScore));
            return sorted.Count > limit ? sorted.GetRange(0, limit) : sorted;
        }

        public List<GameRecord> GetRecentRecords(int limit = 10)
        {
            var sorted = new List<GameRecord>(_records);
            sorted.Sort((a, b) => b.PlayDate.CompareTo(a.PlayDate));
            return sorted.Count > limit ? sorted.GetRange(0, limit) : sorted;
        }

        public List<GameRecord> Get1PlayerRecords()
        {
            return _records.FindAll(r => r.GameMode == 1);
        }

        public List<GameRecord> Get2PlayerRecords()
        {
            return _records.FindAll(r => r.GameMode == 2);
        }

        private void SaveRecords()
        {
            try
            {
                string directory = System.IO.Path.GetDirectoryName(RECORDS_FILE);
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                var json = System.Text.Json.JsonSerializer.Serialize(_records, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(RECORDS_FILE, json);
                System.Diagnostics.Debug.WriteLine("[RECORDS] Saved to file");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to save records: {ex.Message}");
            }
        }

        private void LoadRecords()
        {
            try
            {
                if (System.IO.File.Exists(RECORDS_FILE))
                {
                    string json = System.IO.File.ReadAllText(RECORDS_FILE);
                    _records = System.Text.Json.JsonSerializer.Deserialize<List<GameRecord>>(json) ?? new List<GameRecord>();
                    System.Diagnostics.Debug.WriteLine($"[RECORDS] Loaded {_records.Count} records");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load records: {ex.Message}");
                _records = new List<GameRecord>();
            }
        }

        public void Clear()
        {
            _records.Clear();
            SaveRecords();
        }
    }
}
