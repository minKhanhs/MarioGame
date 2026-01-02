using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using MarioGame.Level;

namespace MarioGame.Data.Models
{
    // Data Models
    public class LevelData
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public int TimeLimit { get; set; }
        public string Background { get; set; }
        public string Music { get; set; }
        public string CameraMode { get; set; }
        public MapData MapData { get; set; }
        public List<EntityData> Entities { get; set; }
    }

    public class MapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] TileMatrix { get; set; }
    }

    public class EntityData
    {
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class GameData
    {
        public List<LevelData> Levels { get; set; }
        public PlayerData PlayerData { get; set; }
        public GameSettings Settings { get; set; }
        public CurrentStateData CurrentState { get; set; }
        public List<LevelHistoryEntry> LevelHistory { get; set; }
    }

    public class PlayerData
    {
        public string PlayerName { get; set; }
        public string LastPlayed { get; set; }
    }

    public class GameSettings
    {
        public bool IsMusicOn { get; set; }
        public bool IsSfxOn { get; set; }
        public float MasterVolume { get; set; }
        public Dictionary<string, KeyBindings> KeyBindings { get; set; }
    }

    public class KeyBindings
    {
        public string MoveLeft { get; set; }
        public string MoveRight { get; set; }
        public string Jump { get; set; }
        public string Shoot { get; set; }
    }

    public class CurrentStateData
    {
        public string Description { get; set; }
        public int CurrentLevelId { get; set; }
        public int LivesLeft { get; set; }
        public int TotalScore { get; set; }
        public int CoinsCollected { get; set; }
        public string PowerUpState { get; set; }
        public PositionData CheckpointPosition { get; set; }
    }

    public class PositionData
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class LevelHistoryEntry
    {
        public int LevelId { get; set; }
        public string Result { get; set; }
        public LevelStats Stats { get; set; }
    }

    public class LevelStats
    {
        public float PlayTimeSeconds { get; set; }
        public int ScoreEarned { get; set; }
        public int DeathCount { get; set; }
        public int EnemiesKilled { get; set; }
        public int CoinsCollected { get; set; }
    }
}

namespace MarioGame.Managers
{
    // DataManager - Singleton for loading and saving data
    public class DataManager
    {
        private static DataManager instance;
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataManager();
                return instance;
            }
        }

        private const string SAVE_FILE_PATH = "savedata.json";
        private const string LEVELS_FILE_PATH = "Content/Data/levels.json";

        public Data.Models.GameData GameData { get; private set; }

        private DataManager() { }

        // Load game data from JSON
        public void LoadGameData()
        {
            try
            {
                if (File.Exists(LEVELS_FILE_PATH))
                {
                    string json = File.ReadAllText(LEVELS_FILE_PATH);
                    GameData = JsonSerializer.Deserialize<Data.Models.GameData>(json);
                    Console.WriteLine($"Loaded {GameData?.Levels?.Count ?? 0} levels from {LEVELS_FILE_PATH}");
                }
                else
                {
                    Console.WriteLine($"Levels file not found: {LEVELS_FILE_PATH}");
                    CreateDefaultGameData();
                }

                // Load save data if exists
                LoadSaveData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game data: {ex.Message}");
                CreateDefaultGameData();
            }
        }

        // Save game progress
        public void SaveGameData()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(GameData, options);
                File.WriteAllText(SAVE_FILE_PATH, json);
                Console.WriteLine($"Game saved to {SAVE_FILE_PATH}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game data: {ex.Message}");
            }
        }

        // Load save data
        private void LoadSaveData()
        {
            try
            {
                if (File.Exists(SAVE_FILE_PATH))
                {
                    string json = File.ReadAllText(SAVE_FILE_PATH);
                    var saveData = JsonSerializer.Deserialize<Data.Models.GameData>(json);

                    // Merge save data with game data
                    if (saveData != null)
                    {
                        GameData.PlayerData = saveData.PlayerData;
                        GameData.Settings = saveData.Settings;
                        GameData.CurrentState = saveData.CurrentState;
                        GameData.LevelHistory = saveData.LevelHistory;
                    }

                    Console.WriteLine("Save data loaded successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading save data: {ex.Message}");
            }
        }

        // Create default game data if file doesn't exist
        private void CreateDefaultGameData()
        {
            GameData = new Data.Models.GameData
            {
                Levels = CreateDefaultLevels(),
                PlayerData = new Data.Models.PlayerData
                {
                    PlayerName = "Player",
                    LastPlayed = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                },
                Settings = new Data.Models.GameSettings
                {
                    IsMusicOn = true,
                    IsSfxOn = true,
                    MasterVolume = 0.8f,
                    KeyBindings = new Dictionary<string, Data.Models.KeyBindings>
                    {
                        ["P1"] = new Data.Models.KeyBindings
                        {
                            MoveLeft = "A",
                            MoveRight = "D",
                            Jump = "W",
                            Shoot = "J"
                        },
                        ["P2"] = new Data.Models.KeyBindings
                        {
                            MoveLeft = "Left",
                            MoveRight = "Right",
                            Jump = "Up",
                            Shoot = "RightControl"
                        }
                    }
                },
                CurrentState = new Data.Models.CurrentStateData
                {
                    Description = "New game",
                    CurrentLevelId = 1,
                    LivesLeft = 3,
                    TotalScore = 0,
                    CoinsCollected = 0,
                    PowerUpState = "Small",
                    CheckpointPosition = new Data.Models.PositionData { X = 0, Y = 0 }
                },
                LevelHistory = new List<Data.Models.LevelHistoryEntry>()
            };

            Console.WriteLine("Created default game data");
        }

        // Create default levels programmatically
        private List<Data.Models.LevelData> CreateDefaultLevels()
        {
            var levels = new List<Data.Models.LevelData>();

            for (int i = 1; i <= 10; i++)
            {
                levels.Add(new Data.Models.LevelData
                {
                    LevelId = i,
                    Name = $"World 1-{i}",
                    TimeLimit = 300,
                    Background = "bg_overworld.png",
                    Music = "theme_main.mp3",
                    CameraMode = i == 9 ? "AutoScroll" : "FollowPlayer",
                    MapData = CreateDefaultMapData(i),
                    Entities = CreateDefaultEntities(i)
                });
            }

            return levels;
        }

        private Data.Models.MapData CreateDefaultMapData(int levelId)
        {
            int width = 200;
            int height = 15;
            int[] tiles = new int[width * height];

            // Fill with air
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = 0;

            // Create ground
            for (int x = 0; x < width; x++)
            {
                tiles[(height - 1) * width + x] = 1; // Ground
                tiles[(height - 2) * width + x] = 1; // Ground
            }

            // Add some platforms
            for (int x = 10; x < 15; x++)
            {
                tiles[10 * width + x] = 2; // Brick
            }

            // Add question blocks
            tiles[9 * width + 15] = 3;
            tiles[9 * width + 20] = 3;
            tiles[9 * width + 25] = 3;

            // Add pipes
            for (int y = 11; y < 14; y++)
            {
                tiles[y * width + 30] = 4;
                tiles[y * width + 31] = 4;
            }

            return new Data.Models.MapData
            {
                Width = width,
                Height = height,
                TileMatrix = tiles
            };
        }

        private List<Data.Models.EntityData> CreateDefaultEntities(int levelId)
        {
            var entities = new List<Data.Models.EntityData>();

            // Add coins
            for (int i = 0; i < 5; i++)
            {
                entities.Add(new Data.Models.EntityData
                {
                    Type = "Coin",
                    X = 200 + (i * 100),
                    Y = 300
                });
            }

            // Add enemies based on level
            if (levelId >= 3)
            {
                entities.Add(new Data.Models.EntityData { Type = "Goomba", X = 400, Y = 300 });
                entities.Add(new Data.Models.EntityData { Type = "Goomba", X = 600, Y = 300 });
            }

            if (levelId >= 4)
            {
                entities.Add(new Data.Models.EntityData { Type = "Koopa", X = 800, Y = 300 });
            }

            if (levelId >= 6)
            {
                entities.Add(new Data.Models.EntityData { Type = "PiranhaPlant", X = 30 * 32, Y = 11 * 32 });
            }

            return entities;
        }

        // Get level data by ID
        public Data.Models.LevelData GetLevelData(int levelId)
        {
            if (GameData?.Levels == null) return null;

            return GameData.Levels.Find(l => l.LevelId == levelId);
        }

        // Update current state
        public void UpdateCurrentState(int levelId, int lives, int score, int coins, string powerUp)
        {
            if (GameData?.CurrentState == null) return;

            GameData.CurrentState.CurrentLevelId = levelId;
            GameData.CurrentState.LivesLeft = lives;
            GameData.CurrentState.TotalScore = score;
            GameData.CurrentState.CoinsCollected = coins;
            GameData.CurrentState.PowerUpState = powerUp;
            GameData.PlayerData.LastPlayed = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        // Add level history entry
        public void AddLevelHistory(int levelId, string result, Data.Models.LevelStats stats)
        {
            if (GameData?.LevelHistory == null)
                GameData.LevelHistory = new List<Data.Models.LevelHistoryEntry>();

            GameData.LevelHistory.Add(new Data.Models.LevelHistoryEntry
            {
                LevelId = levelId,
                Result = result,
                Stats = stats
            });
        }

        // Apply settings to game
        public void ApplySettings()
        {
            if (GameData?.Settings == null) return;

            GameManager.Instance.IsMusicOn = GameData.Settings.IsMusicOn;
            GameManager.Instance.IsSfxOn = GameData.Settings.IsSfxOn;
            GameManager.Instance.MasterVolume = GameData.Settings.MasterVolume;
        }
    }
}