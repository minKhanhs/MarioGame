# MarioGame - Statistics & Reporting System

## ?? T?ng Quan H? Th?ng Th?ng Kê

H? th?ng th?ng kê MarioGame ???c chia thành **3 t?ng**:

```
??????????????????????????????????????????????
? CAREER STATS (Permanent)                    ?
? - TotalCoins (all games)                   ?
? - TotalEnemiesDefeated (all games)         ?
??????????????????????????????????????????????
                     ?
                     ?? Saved to: AppData/MarioGame/careerstats.json
                     ?? Never auto-reset
                     
??????????????????????????????????????????????
? GAME SESSION (Per Playthrough)             ?
? - TotalScore                               ?
? - TotalTime                                ?
? - CurrentLevel                             ?
? - TotalCoinsThisGame                       ?
? - TotalEnemiesThisGame                     ?
??????????????????????????????????????????????
                     ?
                     ?? Saved to: Content/data/gamesession.json
                     ?? Reset khi start new game
                     
??????????????????????????????????????????????
? SAVE SLOT (Per World)                      ?
? - PlayerName                               ?
? - CurrentLevel / MaxLevelUnlocked          ?
? - Score / Lives / Coins                    ?
? - EnemiesDefeated / PlayTime                ?
? - LastPlayed                               ?
??????????????????????????????????????????????
                     ?
                     ?? Saved to: worlds_data.json
                     ?? Multiple slots
```

---

## ?? Data Models

### 1. CareerStats (Static Class)
```csharp
public static class CareerStats
{
    // Properties
    public static int TotalCoins { get; set; }
    public static int TotalEnemiesDefeated { get; set; }
    
    // Methods
    public static void AddStats(int coins, int enemiesDefeated)
    public static void ResetStats()
    public static void LoadFromFile()
    public static void SaveToFile()
}
```

**??c ?i?m**:
- ? L?u tr? v?nh vi?n (AppData folder)
- ? T? ??ng load khi game kh?i ??ng
- ? H? tr? migration t? file c?
- ? Thread-safe atomic writes

**Cách S? D?ng**:
```csharp
// ??c stats
int totalCoins = CareerStats.TotalCoins;
int totalEnemies = CareerStats.TotalEnemiesDefeated;

// C?ng stats
CareerStats.AddStats(50, 5);  // +50 coins, +5 enemies

// Reset (c?c k? hi?m)
CareerStats.ResetStats();
```

---

### 2. GameSession (Singleton)
```csharp
public class GameSession
{
    // Session Stats (reset per playthrough)
    public int TotalScore { get; set; }
    public float TotalTime { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevelReached { get; set; }
    public int TotalCoinsThisGame { get; set; }
    public int TotalEnemiesThisGame { get; set; }
    
    // Singleton
    public static GameSession Instance { get; }
    
    // Methods
    public void AddLevelStats(int score, int coins, int enemies, float time)
    public void ResetSession()
    public void SetCurrentLevel(int level)
}
```

**L?u Tr?**:
```json
{
  "TotalScore": 5250,
  "TotalTime": 245.5,
  "CurrentLevel": 5,
  "MaxLevelReached": 5,
  "TotalCoinsThisGame": 87,
  "TotalEnemiesThisGame": 34
}
```

**Cách S? D?ng**:
```csharp
// Khi hoàn thành level
GameSession.Instance.AddLevelStats(
    score: 500,
    coinsThisLevel: 12,
    enemiesThisLevel: 8,
    time: 45.5f
);

// Chuy?n level
GameSession.Instance.SetCurrentLevel(3);

// Reset khi start new game
GameSession.Instance.ResetSession();

// ??c stats
int currentScore = GameSession.Instance.TotalScore;
float totalTime = GameSession.Instance.TotalTime;
```

---

### 3. SaveSlot (Per World)
```csharp
public class SaveSlot
{
    public string PlayerName { get; set; }
    public DateTime LastPlayed { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevelUnlocked { get; set; }
    public int Score { get; set; }
    public int Lives { get; set; }
    public int Coins { get; set; }
    public int EnemiesDefeated { get; set; }
    public float PlayTime { get; set; }
}
```

**L?u Tr?**:
```json
[
  {
    "PlayerName": "Mario",
    "LastPlayed": "2024-01-15T14:30:45",
    "CurrentLevel": 5,
    "MaxLevelUnlocked": 5,
    "Score": 5250,
    "Lives": 3,
    "Coins": 87,
    "EnemiesDefeated": 34,
    "PlayTime": 245.5
  },
  {
    "PlayerName": "Luigi",
    "LastPlayed": "2024-01-14T10:15:20",
    "CurrentLevel": 3,
    "MaxLevelUnlocked": 4,
    "Score": 2100,
    "Lives": 2,
    "Coins": 45,
    "EnemiesDefeated": 18,
    "PlayTime": 120.3
  }
]
```

---

### 4. GameRecord (Achievements)
```csharp
public class GameRecord
{
    public string RecordName { get; set; }
    public int Value { get; set; }
    public DateTime AchievedDate { get; set; }
    public string Description { get; set; }
}

// Records:
// - First Boss Defeated
// - Perfect Run (no deaths)
// - All Coins Collected (level)
// - Speed Record (fastest completion)
// - Enemies Defeated Record (level)
```

---

### 5. PlayerStateData
```csharp
public class PlayerStateData
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public int Lives { get; set; }
    public int Coins { get; set; }
    public int Score { get; set; }
    public float Scale { get; set; }
    public bool IsInvincible { get; set; }
}
```

---

### 6. LevelStats
```csharp
public class LevelStats
{
    public int LevelIndex { get; set; }
    public int Score { get; set; }
    public int CoinsCollected { get; set; }
    public int EnemiesDefeated { get; set; }
    public float CompletionTime { get; set; }
    public int Deaths { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CompletedDate { get; set; }
}
```

---

## ?? Data Flow Diagram

```
????????????????????????????????
? Start New Game               ?
? - Create SaveSlot            ?
? - Reset GameSession          ?
????????????????????????????????
                   ?
                   v
        ????????????????????
        ? Play Level 1     ?
        ????????????????????
                 ?
                 v
    ??????????????????????????????
    ? During Gameplay            ?
    ? - Track coins              ?
    ? - Count enemies defeated   ?
    ? - Track time               ?
    ? - Count deaths             ?
    ??????????????????????????????
                 ?
                 v
    ??????????????????????????????
    ? Level Complete             ?
    ? - Calculate bonus          ?
    ? - AddLevelStats()          ?
    ??????????????????????????????
                 ?
                 v
????????????????????????????????????
? GameSession.AddLevelStats()      ?
? ?? Sum to TotalScore             ?
? ?? Sum to TotalCoinsThisGame     ?
? ?? Sum to TotalEnemiesThisGame   ?
? ?? Call CareerStats.AddStats()   ?
????????????????????????????????????
             ?
             v
????????????????????????????????????
? CareerStats.AddStats()           ?
? ?? Reload from disk              ?
? ?? += coins, enemies             ?
? ?? Save to AppData               ?
? ?? Log to debug                  ?
????????????????????????????????????
             ?
             v
????????????????????????????????????
? SaveSlotManager.UpdateCurrentSlot?
? ?? Update SaveSlot               ?
? ?? Unlock next level             ?
? ?? Update MaxLevelUnlocked       ?
? ?? Save to worlds_data.json      ?
????????????????????????????????????
```

---

## ?? File Locations

### ??? AppData (Persistent)
```
C:\Users\<Username>\AppData\Roaming\MarioGame\
??? careerstats.json              ? Career stats (permanent)
??? careerstats.json.tmp          (Temporary during write)
```

### ?? Content Directory (Game Installation)
```
Content/
??? data/
?   ??? gamesession.json          ? Current session
?   ??? worlds_data.json          ? Save slots
?   ??? achievements.json         (Future)
??? levels/
    ??? level1.json
    ??? level2.json
    ??? ...
```

---

## ?? Statistics Tracking in GameplayScene

```csharp
public class GameplayScene : IScene
{
    private GameHUD _hud;  // Tracks current level stats
    
    public void Update(GameTime gameTime)
    {
        // Update HUD (real-time display)
        _hud.Update(gameTime);
        _hud.LivesRemaining = _player.Lives;
        _hud.CoinsCollected = _player.Coins;
        
        // When level completes
        if (_isLevelFinished)
        {
            // 1. Update session
            GameSession.Instance.AddLevelStats(
                _hud.CurrentScore,
                _hud.CoinsCollected,
                _hud.EnemiesDefeated,
                _hud.ElapsedTime
            );
            
            // 2. Update save slot
            SaveSlotManager.UpdateCurrentSlot(
                _levelIndex + 1,
                GameSession.Instance.TotalScore,
                _player.Lives,
                GameSession.Instance.TotalCoinsThisGame,
                GameSession.Instance.TotalEnemiesThisGame,
                GameSession.Instance.TotalTime
            );
        }
    }
}
```

---

## ?? HUD Display System

```csharp
public class GameHUD
{
    // Real-time display
    public int LivesRemaining { get; set; }
    public int CoinsCollected { get; set; }
    public int EnemiesDefeated { get; set; }
    public int CurrentScore { get; set; }
    public float ElapsedTime { get; set; }
    public int MushroomsCollected { get; set; }
    public int DeathCount { get; set; }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        // Display current stats
        // Lives: ?? ?? ??
        // Score: 12500
        // Coins: ?? 87
        // Enemies: 34
        // Time: 04:32
    }
    
    public int CalculateLevelBonus()
    {
        // Bonus = remaining time × 50
        return (int)(_remainingTime * 50);
    }
}
```

---

## ?? Scoring System

### Points Per Action
```csharp
public static class ScoreValues
{
    public const int COIN_VALUE = 10;
    public const int MUSHROOM_VALUE = 50;
    public const int GOOMBA_VALUE = 50;
    public const int KOOPA_VALUE = 100;
    public const int PIRANHA_PLANT_VALUE = 150;
    public const int ENEMY_FIREBALL_VALUE = 75;
    public const int LEVEL_COMPLETE_BONUS = 500;
    public const int TIME_BONUS_MULTIPLIER = 50;
    public const int NO_DEATHS_BONUS = 1000;
    public const int PERFECT_BOSS_BONUS = 5000;
}
```

### Scoring Formula
```
Level Score = 
    (CoinsCollected × 10) +
    (EnemiesDefeated × 75) +
    (MushroomsCollected × 50) +
    500 (level complete) +
    (TimeBonus × 50) +
    (if no deaths: 1000)

Total Score = Session Total +
    All Level Scores
```

---

## ?? Achievement System

### Achievements (Future)
```csharp
public enum AchievementType
{
    FirstBlood,              // Defeat first enemy
    CoinCollector,           // Collect 100 coins
    BossSlayer,              // Defeat boss
    PerfectRun,              // Complete level without dying
    Speedrunner,             // Complete level in < 60 seconds
    Mushroom Master,         // Collect 50 mushrooms
    CareerCoinCollector,     // 1000 total coins lifetime
    AllLevelsMastered,       // Complete all 10 levels
}

public class Achievement
{
    public AchievementType Type { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime UnlockedDate { get; set; }
    public int ProgressValue { get; set; }
    public int ProgressTarget { get; set; }
}
```

---

## ?? Save/Load Cycle

### Saving Flow
```csharp
// 1. Level Complete
_isLevelFinished = true;
_finishTimer += dt;

if (_finishTimer > 2.0f)
{
    // 2. Update GameSession
    GameSession.Instance.AddLevelStats(
        score: _hud.CurrentScore,
        coins: _hud.CoinsCollected,
        enemies: _hud.EnemiesDefeated,
        time: _hud.ElapsedTime
    );
    // ? This calls CareerStats.AddStats internally
    
    // 3. Update SaveSlot
    SaveSlotManager.UpdateCurrentSlot(
        _levelIndex + 1,
        GameSession.Instance.TotalScore,
        _player.Lives,
        GameSession.Instance.TotalCoinsThisGame,
        GameSession.Instance.TotalEnemiesThisGame,
        GameSession.Instance.TotalTime
    );
    // ? Saves to worlds_data.json
    
    // 4. Show results
    GameManager.Instance.ChangeScene(
        new LevelCompleteScene(...)
    );
}
```

### Loading Flow
```csharp
// 1. Load SaveSlot
SaveSlotManager.LoadSlots();
SaveSlot slot = SaveSlotManager.Slots[0];

// 2. Load GameSession
GameSession.Instance.SetCurrentLevel(slot.CurrentLevel);

// 3. Start Level
GameManager.Instance.ChangeScene(new GameplayScene(slot.CurrentLevel));

// 4. GameSession state is maintained across levels
// 5. On new game: GameSession.ResetSession()
```

---

## ?? Debugging Statistics

### Debug Output
```
[CAREER] Added - Coins: 50, Enemies: 2. 
    Total: Coins=450, Enemies=78
[SESSION] AddLevelStats - Session Score: 5250, 
    Coins: 87, Enemies: 34
[SAVE] Updated slot - Level: 5, MaxLevel: 5, 
    Score: 5250, PlayTime: 245.5
```

### Console Commands (Future)
```csharp
// Gi? l?p commands
Stats.Print()          // Print current stats
Stats.Reset()          // Reset all stats
Stats.AddCoins(100)    // Add coins
Stats.SetLevel(10)     // Jump to level
```

---

## ?? Statistics Dashboard (UI)

### MainMenu Stats Display
```
???????????????????????????????????
? CAREER STATISTICS               ?
???????????????????????????????????
? Total Coins Collected:  450 ??   ?
? Total Enemies Defeated: 78 ??   ?
? Total Time Played:      12:45   ?
? Games Started:          8        ?
? Games Completed:        5        ?
???????????????????????????????????
```

### Level Complete Screen
```
????????????????????????????????????
? LEVEL 5 COMPLETE!                ?
????????????????????????????????????
? Time Bonus:      1850 points     ?
? Coins:           120 × 10 = 1200 ?
? Enemies:         15 × 75 = 1125  ?
? Perfect Bonus:   1000 points     ?
????????????????????????????????????
? LEVEL TOTAL:     5175 points     ?
? SESSION TOTAL:   12500 points    ?
????????????????????????????????????
```

### Play History Scene
```
??????????????????????????????????
? PLAY HISTORY                   ?
??????????????????????????????????
? Mario      Level 5   5250 pts   ?
? Luigi      Level 3   2100 pts   ?
? Player 3   Level 1   500 pts    ?
??????????????????????????????????
```

---

## ?? Best Practices

### ? Do's
- ? Save after each level
- ? Load career stats on startup
- ? Use atomic writes for data safety
- ? Track time accurately
- ? Update HUD real-time

### ? Don'ts
- ? Don't reset career stats automatically
- ? Don't save during gameplay (performance)
- ? Don't mix session and career tracking
- ? Don't lose player progress on crash

---

## ?? Future Enhancements

### 1. Leaderboard System
```csharp
public class Leaderboard
{
    public List<LeaderboardEntry> Entries { get; set; }
    
    public void AddEntry(string playerName, int score, int level)
    public List<LeaderboardEntry> GetTopScores(int count = 10)
}

public class LeaderboardEntry
{
    public string PlayerName { get; set; }
    public int Score { get; set; }
    public int LevelReached { get; set; }
    public DateTime Date { get; set; }
}
```

### 2. Advanced Analytics
```csharp
public class GameAnalytics
{
    public float AverageTimePerLevel { get; set; }
    public float AverageDeathsPerLevel { get; set; }
    public int MostDifficultLevel { get; set; }
    public float CompletionRate { get; set; }
}
```

### 3. Cloud Save Support
```csharp
public interface ICloudSaveProvider
{
    Task UploadStats();
    Task DownloadStats();
    Task SyncWithCloud();
}
```

---

## ?? Summary

**MarioGame Statistics System**:

? **Multi-tier tracking**:
- Career Stats (permanent, global)
- Session Stats (per playthrough)
- Save Slots (per world)
- Level Stats (per level)

? **Robust data handling**:
- AppData persistence
- Save file migration
- Atomic writes
- Thread-safe operations

? **Real-time display**:
- Live HUD updates
- Score calculation
- Bonus tracking
- Death counting

? **Future-proof**:
- Achievement framework
- Leaderboard support
- Analytics ready

