using System;
using System.Collections.Generic;

namespace MarioGame.Data.Models
{
    public class GameData
    {
        public string PlayerName { get; set; } = "Player";
        public DateTime LastPlayed { get; set; } = DateTime.Now;
        public SettingsData Settings { get; set; } = new SettingsData();
        public CurrentStateData CurrentState { get; set; } = new CurrentStateData();
        public List<LevelHistoryData> LevelHistory { get; set; } = new List<LevelHistoryData>();
    }

    public class SettingsData
    {
        public bool IsMusicOn { get; set; } = true;
        public bool IsSfxOn { get; set; } = true;
        public float MasterVolume { get; set; } = 0.8f;
        public KeyBindingsData KeyBindings { get; set; } = new KeyBindingsData();
    }

    public class KeyBindingsData
    {
        public PlayerKeyBindings P1 { get; set; } = new PlayerKeyBindings();
        public PlayerKeyBindings P2 { get; set; } = new PlayerKeyBindings();
    }

    public class PlayerKeyBindings
    {
        public string MoveLeft { get; set; } = "Keys.A";
        public string MoveRight { get; set; } = "Keys.D";
        public string Jump { get; set; } = "Keys.W";
        public string Shoot { get; set; } = "Keys.J";
    }

    public class CurrentStateData
    {
        public int CurrentLevelId { get; set; } = 1;
        public int LivesLeft { get; set; } = 3;
        public int TotalScore { get; set; } = 0;
        public int CoinsCollected { get; set; } = 0;
        public string PowerUpState { get; set; } = "Small";
        public CheckpointData CheckpointPosition { get; set; } = new CheckpointData();
    }

    public class CheckpointData
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
    }

    public class LevelHistoryData
    {
        public int LevelId { get; set; }
        public string Result { get; set; } // "Win" or "Lose"
        public LevelStatsData Stats { get; set; } = new LevelStatsData();
    }

    public class LevelStatsData
    {
        public float PlayTimeSeconds { get; set; }
        public int ScoreEarned { get; set; }
        public int DeathCount { get; set; }
        public int EnemiesKilled { get; set; }
    }
}