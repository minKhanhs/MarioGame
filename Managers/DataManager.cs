using System;
using System.IO;
using System.Text.Json;
using MarioGame.Data.Models;

namespace MarioGame.Managers
{
    public class DataManager
    {
        private static DataManager _instance;
        public static DataManager Instance => _instance ??= new DataManager();

        private const string SAVE_FILE = "gamedata.json";
        private GameData _currentGameData;

        private DataManager() { }

        public void Initialize()
        {
            LoadGameData();
        }

        public void LoadGameData()
        {
            try
            {
                if (File.Exists(SAVE_FILE))
                {
                    string json = File.ReadAllText(SAVE_FILE);
                    _currentGameData = JsonSerializer.Deserialize<GameData>(json);
                }
                else
                {
                    _currentGameData = new GameData();
                }
            }
            catch
            {
                _currentGameData = new GameData();
            }
        }

        public void SaveGameData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_currentGameData, options);
                File.WriteAllText(SAVE_FILE, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game data: {ex.Message}");
            }
        }

        public void SaveCurrentState(int levelId, int lives, int score, int coins, string powerUp)
        {
            _currentGameData.CurrentState = new CurrentStateData
            {
                CurrentLevelId = levelId,
                LivesLeft = lives,
                TotalScore = score,
                CoinsCollected = coins,
                PowerUpState = powerUp
            };
            SaveGameData();
        }

        public void SaveLevelResult(int levelId, string result, float playTime, int score, int deaths, int enemiesKilled)
        {
            var history = new LevelHistoryData
            {
                LevelId = levelId,
                Result = result,
                Stats = new LevelStatsData
                {
                    PlayTimeSeconds = playTime,
                    ScoreEarned = score,
                    DeathCount = deaths,
                    EnemiesKilled = enemiesKilled
                }
            };

            _currentGameData.LevelHistory.Add(history);
            SaveGameData();
        }

        public GameData GetGameData() => _currentGameData;
    }
}