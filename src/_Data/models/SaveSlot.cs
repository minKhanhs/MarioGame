using System;

namespace MarioGame.src._Data.models
{
    [Serializable]
    public class SaveSlot
    {
        // Định danh duy nhất để phân biệt các file save
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PlayerName { get; set; }
        public int CurrentLevel { get; set; } = 1;
        public int MaxLevelUnlocked { get; set; } = 1;
        public int Score { get; set; }
        public int Lives { get; set; } = 3;
        public int Coins { get; set; }
        public int EnemiesDefeated { get; set; }
        public float PlayTime { get; set; }
        public DateTime LastPlayed { get; set; }

        public float ProgressPercent
        {
            get
            {
                float percent = (float)MaxLevelUnlocked / 10.0f;
                return percent > 1.0f ? 1.0f : percent;
            }
        }
    }
}