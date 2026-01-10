using MarioGame.src._Data.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace MarioGame.src._Data
{
    public static class SaveSlotManager
    {
        private static string _fileName = "worlds_data.json"; // Đổi tên file để tránh conflict cũ
        private static string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);

        public static List<SaveSlot> Slots { get; private set; }
        public static SaveSlot CurrentSlot { get; set; } // Slot đang chơi

        public static void LoadSlots()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    Slots = JsonSerializer.Deserialize<List<SaveSlot>>(json);
                    // Sắp xếp: Mới chơi lên đầu
                    Slots = Slots.OrderByDescending(s => s.LastPlayed).ToList();
                }
                catch { Slots = new List<SaveSlot>(); }
            }
            else { Slots = new List<SaveSlot>(); }
        }

        public static void SaveSlots()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Slots, options);
            File.WriteAllText(_filePath, json);
        }

        public static void CreateNewWorld(string playerName)
        {
            var newSlot = new SaveSlot
            {
                PlayerName = playerName,
                LastPlayed = DateTime.Now
            };
            Slots.Add(newSlot);
            CurrentSlot = newSlot;
            SaveSlots();
        }

        public static void UpdateCurrentSlot(int level, int score, int lives, int coins, int enemies, float time)
        {
            if (CurrentSlot == null) return;

            CurrentSlot.CurrentLevel = level;
            if (level > CurrentSlot.MaxLevelUnlocked) CurrentSlot.MaxLevelUnlocked = level;
            CurrentSlot.Score = score;
            CurrentSlot.Lives = lives;
            CurrentSlot.Coins = coins;
            CurrentSlot.EnemiesDefeated = enemies;
            CurrentSlot.PlayTime = time;
            CurrentSlot.LastPlayed = DateTime.Now;

            SaveSlots();
        }
    }
}