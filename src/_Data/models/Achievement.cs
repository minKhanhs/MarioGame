using System;
using System.Collections.Generic;

namespace MarioGame.src._Data.models
{
    /// <summary>
    /// Represents a single achievement
    /// </summary>
    [Serializable]
    public class Achievement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedDate { get; set; }

        public Achievement(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            IsUnlocked = false;
            UnlockedDate = null;
        }

        public void Unlock()
        {
            if (!IsUnlocked)
            {
                IsUnlocked = true;
                UnlockedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"[ACHIEVEMENT] Unlocked: {Name}");
            }
        }

        public override string ToString()
        {
            return $"{Name} - {Description} {(IsUnlocked ? $"({UnlockedDate:MM/dd/yyyy})" : "(Locked)")}";
        }
    }

    /// <summary>
    /// Manages all achievements
    /// </summary>
    public class AchievementManager
    {
        private List<Achievement> _achievements;
        private const string ACHIEVEMENTS_FILE = "Content/data/achievements.json";
        private static AchievementManager _instance;

        public static AchievementManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AchievementManager();
                return _instance;
            }
        }

        public AchievementManager()
        {
            _achievements = new List<Achievement>();
            InitializeAchievements();
            LoadAchievements();
        }

        private void InitializeAchievements()
        {
            // First kill
            _achievements.Add(new Achievement("first_kill", "First Blood", "Defeat your first enemy"));

            // First mushroom
            _achievements.Add(new Achievement("first_mushroom", "Power Up", "Collect your first mushroom"));

            // Coin collector
            _achievements.Add(new Achievement("coin_10", "Coin Collector", "Collect 10 coins in a single game"));
            _achievements.Add(new Achievement("coin_50", "Gold Rush", "Collect 50 coins total"));
            _achievements.Add(new Achievement("coin_100", "Wealthy", "Collect 100 coins total"));

            // Enemy slayer
            _achievements.Add(new Achievement("enemy_10", "Slayer", "Defeat 10 enemies in a single game"));
            _achievements.Add(new Achievement("enemy_50", "Serial Slayer", "Defeat 50 enemies total"));
            _achievements.Add(new Achievement("enemy_100", "Exterminator", "Defeat 100 enemies total"));
            _achievements.Add(new Achievement("enemy_1000", "Monster Slayer", "Defeat 1000 enemies total"));

            // Level achievements
            _achievements.Add(new Achievement("level_complete", "Hero", "Complete a level"));
            _achievements.Add(new Achievement("level_2", "Adventurer", "Complete 2 levels"));
            _achievements.Add(new Achievement("level_3", "Legend", "Complete all 3 levels"));

            // Score achievements
            _achievements.Add(new Achievement("score_1000", "Score 1000", "Earn 1000 points in a single game"));
            _achievements.Add(new Achievement("score_5000", "Score 5000", "Earn 5000 points total"));

            // Speedrun
            _achievements.Add(new Achievement("speedrun", "Speedrunner", "Complete a level in under 30 seconds"));

            // Perfect
            _achievements.Add(new Achievement("perfect_level", "Perfect Run", "Complete a level without taking damage"));
        }

        public void CheckAndUnlockAchievements(int coins, int enemies, int score, float levelTime, int totalEnemies, int totalCoins, int totalScore, bool tookDamage)
        {
            // First kill
            if (enemies > 0)
                GetAchievement("first_kill")?.Unlock();

            // First mushroom (simplified - just check if score increased significantly)
            if (score > 100)
                GetAchievement("first_mushroom")?.Unlock();

            // Coin achievements (per game)
            if (coins >= 10)
                GetAchievement("coin_10")?.Unlock();

            // Enemy achievements (per game)
            if (enemies >= 10)
                GetAchievement("enemy_10")?.Unlock();

            // Level complete
            GetAchievement("level_complete")?.Unlock();

            // Score achievements (per game)
            if (score >= 1000)
                GetAchievement("score_1000")?.Unlock();

            // Speedrun
            if (levelTime < 30)
                GetAchievement("speedrun")?.Unlock();

            // Perfect level
            if (!tookDamage)
                GetAchievement("perfect_level")?.Unlock();

            // Total achievements
            if (totalCoins >= 50)
                GetAchievement("coin_50")?.Unlock();
            if (totalCoins >= 100)
                GetAchievement("coin_100")?.Unlock();

            if (totalEnemies >= 50)
                GetAchievement("enemy_50")?.Unlock();
            if (totalEnemies >= 100)
                GetAchievement("enemy_100")?.Unlock();
            if (totalEnemies >= 1000)
                GetAchievement("enemy_1000")?.Unlock();

            if (totalScore >= 5000)
                GetAchievement("score_5000")?.Unlock();
        }

        public Achievement GetAchievement(string id)
        {
            return _achievements.Find(a => a.Id == id);
        }

        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(_achievements);
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            return _achievements.FindAll(a => a.IsUnlocked);
        }

        public int GetUnlockedCount()
        {
            return _achievements.FindAll(a => a.IsUnlocked).Count;
        }

        private void SaveAchievements()
        {
            try
            {
                string directory = System.IO.Path.GetDirectoryName(ACHIEVEMENTS_FILE);
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                var json = System.Text.Json.JsonSerializer.Serialize(_achievements, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(ACHIEVEMENTS_FILE, json);
                System.Diagnostics.Debug.WriteLine("[ACHIEVEMENTS] Saved to file");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to save achievements: {ex.Message}");
            }
        }

        private void LoadAchievements()
        {
            try
            {
                if (System.IO.File.Exists(ACHIEVEMENTS_FILE))
                {
                    string json = System.IO.File.ReadAllText(ACHIEVEMENTS_FILE);
                    var loaded = System.Text.Json.JsonSerializer.Deserialize<List<Achievement>>(json);
                    if (loaded != null)
                    {
                        // Update achievements with loaded data
                        foreach (var loadedAch in loaded)
                        {
                            var existing = GetAchievement(loadedAch.Id);
                            if (existing != null)
                            {
                                existing.IsUnlocked = loadedAch.IsUnlocked;
                                existing.UnlockedDate = loadedAch.UnlockedDate;
                            }
                        }
                        System.Diagnostics.Debug.WriteLine($"[ACHIEVEMENTS] Loaded achievements");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load achievements: {ex.Message}");
            }
        }

        public void SaveAll()
        {
            SaveAchievements();
        }

        public void Reset()
        {
            foreach (var ach in _achievements)
            {
                ach.IsUnlocked = false;
                ach.UnlockedDate = null;
            }
            SaveAchievements();
        }
    }
}
