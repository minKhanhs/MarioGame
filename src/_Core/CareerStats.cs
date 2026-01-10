namespace MarioGame.src._Core
{
    /// <summary>
    /// Career Statistics - Permanently saved, NEVER reset automatically
    /// Stored in user's AppData to avoid being overwritten by Content folder
    /// Supports migration from legacy files in Content/data
    /// </summary>
    public static class CareerStats
    {
        private static int _careerTotalCoins = 0;
        private static int _careerTotalEnemiesDefeated = 0;
        private static readonly string CAREER_FILE;

        public static int TotalCoins 
        { 
            get => _careerTotalCoins;
            set => _careerTotalCoins = value;
        }

        public static int TotalEnemiesDefeated 
        { 
            get => _careerTotalEnemiesDefeated;
            set => _careerTotalEnemiesDefeated = value;
        }

        static CareerStats()
        {
            string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string dir = System.IO.Path.Combine(appData, "MarioGame");
            CAREER_FILE = System.IO.Path.Combine(dir, "careerstats.json");

            // Load from AppData if exists; otherwise try to migrate from legacy Content files
            if (System.IO.File.Exists(CAREER_FILE))
            {
                LoadFromFile();
            }
            else
            {
                bool migrated = TryMigrateFromLegacyFiles();
                if (!migrated)
                {
                    System.Diagnostics.Debug.WriteLine("[CAREER] No existing career file found; will create on first save");
                }
            }
        }

        // Try to import career stats from legacy locations (Content/data)
        private static bool TryMigrateFromLegacyFiles()
        {
            try
            {
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string contentDir = System.IO.Path.Combine(baseDir, "Content", "data");

                // 1) Check Content/data/careerstats.json
                string legacy1 = System.IO.Path.Combine(contentDir, "careerstats.json");
                if (System.IO.File.Exists(legacy1))
                {
                    ImportFromFile(legacy1);
                    SaveToFile();
                    System.Diagnostics.Debug.WriteLine($"[CAREER] Migrated career stats from {legacy1} to {CAREER_FILE}");
                    return true;
                }

                // 2) Check Content/data/gamesession.json for Career properties
                string legacy2 = System.IO.Path.Combine(contentDir, "gamesession.json");
                if (System.IO.File.Exists(legacy2))
                {
                    try
                    {
                        string json = System.IO.File.ReadAllText(legacy2);
                        var doc = System.Text.Json.JsonDocument.Parse(json).RootElement;
                        bool found = false;
                        if (doc.TryGetProperty("CareerTotalCoins", out var coinsEl))
                        {
                            _careerTotalCoins = coinsEl.GetInt32();
                            found = true;
                        }
                        if (doc.TryGetProperty("CareerTotalEnemiesDefeated", out var enemiesEl))
                        {
                            _careerTotalEnemiesDefeated = enemiesEl.GetInt32();
                            found = true;
                        }
                        if (found)
                        {
                            SaveToFile();
                            System.Diagnostics.Debug.WriteLine($"[CAREER] Migrated career stats from {legacy2} to {CAREER_FILE}");
                            return true;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CAREER] Failed to import from {legacy2}: {ex.Message}");
                    }
                }

                return false;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CAREER] Migration failed: {ex.Message}");
                return false;
            }
        }

        private static void ImportFromFile(string path)
        {
            try
            {
                string json = System.IO.File.ReadAllText(path);
                var doc = System.Text.Json.JsonDocument.Parse(json).RootElement;
                if (doc.TryGetProperty("TotalCoins", out var coinsEl))
                    _careerTotalCoins = coinsEl.GetInt32();
                if (doc.TryGetProperty("TotalEnemiesDefeated", out var enemiesEl))
                    _careerTotalEnemiesDefeated = enemiesEl.GetInt32();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CAREER] ImportFromFile failed: {ex.Message}");
            }
        }

        public static void AddStats(int coins, int enemiesDefeated)
        {
            // Reload from disk in case external process changed it
            LoadFromFile();

            _careerTotalCoins += coins;
            _careerTotalEnemiesDefeated += enemiesDefeated;
            SaveToFile();
            System.Diagnostics.Debug.WriteLine($"[CAREER] Added - Coins: {coins}, Enemies: {enemiesDefeated}. Total: Coins={_careerTotalCoins}, Enemies={_careerTotalEnemiesDefeated}");
        }

        public static void ResetStats()
        {
            _careerTotalCoins = 0;
            _careerTotalEnemiesDefeated = 0;
            SaveToFile();
            System.Diagnostics.Debug.WriteLine("[CAREER] Stats reset (explicit)");
        }

        private static void LoadFromFile()
        {
            try
            {
                if (System.IO.File.Exists(CAREER_FILE))
                {
                    string json = System.IO.File.ReadAllText(CAREER_FILE);
                    var data = System.Text.Json.JsonDocument.Parse(json).RootElement;

                    if (data.TryGetProperty("TotalCoins", out var coinsElement))
                        _careerTotalCoins = coinsElement.GetInt32();

                    if (data.TryGetProperty("TotalEnemiesDefeated", out var enemiesElement))
                        _careerTotalEnemiesDefeated = enemiesElement.GetInt32();

                    System.Diagnostics.Debug.WriteLine($"[CAREER] Loaded from file - Coins: {_careerTotalCoins}, Enemies: {_careerTotalEnemiesDefeated}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[CAREER] File not found - will create on first save");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load career stats: {ex.Message}");
            }
        }

        private static void SaveToFile()
        {
            try
            {
                string directory = System.IO.Path.GetDirectoryName(CAREER_FILE);
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                var data = new 
                { 
                    TotalCoins = _careerTotalCoins,
                    TotalEnemiesDefeated = _careerTotalEnemiesDefeated
                };

                // Atomic write: write to temp then move
                string temp = CAREER_FILE + ".tmp";
                var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(temp, json);
                if (System.IO.File.Exists(CAREER_FILE)) System.IO.File.Delete(CAREER_FILE);
                System.IO.File.Move(temp, CAREER_FILE);

                System.Diagnostics.Debug.WriteLine($"[CAREER] Saved to file - Coins: {_careerTotalCoins}, Enemies: {_careerTotalEnemiesDefeated}");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to save career stats: {ex.Message}");
            }
        }
    }
}
