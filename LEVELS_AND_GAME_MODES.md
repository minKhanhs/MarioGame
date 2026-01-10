# MarioGame - Levels & Game Modes Documentation

## ?? T?ng Quan H? Th?ng Màn Ch?i

MarioGame có **10 c?p ??** (Levels 1-9 th??ng, Level 10 là Boss).

```
Level 1 ? Level 2 ? ... ? Level 9 ? Level 10 (Boss) ? Victory
   ?         ?                ?          ?
  JSON    JSON             JSON      Boss Arena
```

---

## ?? C?u Trúc D? Li?u Level

### ?? LevelMapData (Model)
```csharp
public class LevelMapData
{
    public int TileSize { get; set; }                    // Kích th??c tile (m?c ??nh 64px)
    public List<string> Layout { get; set; }            // B?n ?? d?ng ký t?
    public bool IsAutoScroll { get; set; }              // T? ??ng cu?n màn hình
    public float ScrollSpeed { get; set; }              // T?c ?? cu?n (px/s)
    public bool CanShoot { get; set; }                  // Cho phép b?n l?a
    public bool HasBulletBill { get; set; }            // Có Bullet Bill
}
```

### ?? Ký T? B?n ??
```
#  ? G?ch c?ng (không th? phá)
G  ? ??t / Ground tile
T  ? ?ng d?n (Pipe)
C  ? Ti?n xu (Coin) ? +1 Coin
M  ? N?m (Mushroom) ? +5 Score, +1 Nhân dân
?  ? G?ch bí m?t (Mystery Block) - Ch?a Coin/N?m
E  ? Goomba (quái v?t b??c ?i)
K  ? Koopa (rùa)
P  ? Piranha Plant (cây ?n th?t)
Z  ? Lâu ?ài (Castle) - ?ích ??n
(space) ? Không gian tr?ng (không v?)
```

---

## ?? C? Ch? ?i?m S?

### ?? ?i?m t? Hành ??ng
```
Goomba b? ??p        ? +50 points
Koopa b? ??p         ? +100 points
Piranha Plant b? ??p ? +150 points
Enemy b? fireball    ? +75 points
Coin ?n ???c        ? +10 points
Mushroom ?n ???c    ? +50 points
Level Complete      ? +500 + Bonus
```

### ?? Bonus ?i?m
```
Time Bonus       ? ?i?m còn l?i × 50
Enemy Killed     ? S? quái × 100
Coins Collected  ? S? ti?n × 10
No Deaths Bonus  ? 1000 (n?u không ch?t l?n nào)
Perfect Boss     ? 5000 (Boss không b? quái ?ánh)
```

---

## ?? Chi Ti?t Các Màn Ch?i

### ?? LEVEL 1: Introduction
```json
{
  "tileSize": 64,
  "layout": [
    "                                    ",
    "                                    ",
    "     C  C  M                        ",
    "  ##   # # #                        ",
    "G#########################################",
    "G#########################################"
  ],
  "isAutoScroll": false,
  "scrollSpeed": 0,
  "canShoot": false,
  "hasBulletBill": false
}
```
**M?c ?ích**: H?c cách di chuy?n và nh?y
**Quái**: Goomba (2-3 con)
**?? khó**: ? (D?)

### ?? LEVEL 2: Basic Platforming
```json
{
  "layout": [
    "   C M                              ",
    "   ##                        E      ",
    "   ##              ###              ",
    "##   ##   ##      ##                ",
    "G###################      E      ###",
    "G######################################"
  ],
  "isAutoScroll": false,
  "canShoot": false,
  "hasBulletBill": false
}
```
**M?c ?ích**: Nh?y qua kho?ng tr?ng
**Quái**: Goomba, Koopa
**?? khó**: ??

### ?? LEVEL 3-5: Intermediate
- T?ng s? l??ng quái
- Thêm Piranha Plant (trong Pipe)
- Gi?i thi?u Mystery Block
- Có ph?n t? cu?n nh?

### ?? LEVEL 6-8: Advanced
```json
{
  "isAutoScroll": true,
  "scrollSpeed": 110,
  "canShoot": true,
  "hasBulletBill": true
}
```
**M?c ?ích**: Ch?y không d?ng, lý thuy?t nhanh
**Quái**: ?? lo?i
**?? khó**: ????

### ?? LEVEL 9: Final Level
- Auto scroll nhanh
- Quái khó và nhi?u
- Boss Bullet Bills bay t? do
- Mystery Blocks ph?c t?p

### ?? LEVEL 10: BOSS ARENA (Lâu ?ài)
```csharp
if (_levelIndex == Constants.BOSS_LEVEL)
{
    _isBossLevel = true;
    _player.IsFlying = true;  // Chuy?n sang ch? ?? bay
}
```

**C? ch? ??c bi?t**:
- Player bay l? l?ng (ch? ?? Flappy Bird)
- Boss ? bên ph?i, di chuy?n v?i camera
- Player b?n fireball ?? ?ánh Boss
- Boss ném ??n ng??c l?i

**Boss Mechanics**:
```csharp
_boss.Update(gameTime);
_boss.Position.X = _camera.Position.X + _camera.Viewport.Width - 150;

// Boss xu?t hi?n ??n
if (_boss.SpawnedObjects.Count > 0)
{
    foreach (var obj in _boss.SpawnedObjects)
    {
        _gameObjects.Add(obj);
    }
    _boss.SpawnedObjects.Clear();
}

// Chi?n th?ng khi Boss ch?t
if (!_boss.IsActive)
{
    _isLevelFinished = true;
}
```

---

## ??? Game Modes

### 1?? Single Player Mode
```csharp
GameManager.Instance.GameMode = 1;
// Player index = 1
// Input: Arrow Keys / WASD
```

### 2?? Two Player Mode
```csharp
GameManager.Instance.GameMode = 2;
// Player 1: Arrow Keys
// Player 2: WASD
// Cùng ch?i 1 level
```

### ?? Save Slot System
```csharp
public class SaveSlot
{
    public string PlayerName { get; set; }
    public int CurrentLevel { get; set; }           // Level ?ang ch?i
    public int MaxLevelUnlocked { get; set; }       // Level cao nh?t ?ã qua
    public int Score { get; set; }                  // T?ng ?i?m
    public int Lives { get; set; }                  // M?ng còn l?i
    public int Coins { get; set; }                  // Ti?n xu còn l?i
    public int EnemiesDefeated { get; set; }        // Quái ?ã gi?t
    public float PlayTime { get; set; }             // Th?i gian ch?i
    public DateTime LastPlayed { get; set; }        // L?n cu?i ch?i
}
```

---

## ?? Game Flow Diagram

```
???????????????
? Game Start  ?
???????????????
       ?
       v
????????????????????
?  MenuScene       ?
?  - 1 Player      ?
?  - 2 Players     ?
?  - Settings      ?
?  - Achievements  ?
?  - Compendium    ?
????????????????????
         ?
         v
????????????????????
? SaveSlotScene    ? ???????????
? - Create World   ?           ?
? - Load World     ?           ?
? - Delete World   ?           ?
????????????????????           ?
         ?                     ?
         v                     ?
????????????????????????????????
? GameplayScene                ?
? - Update game logic          ?
? - Handle collisions          ?
? - Track stats                ?
????????????????????????????????
     ?                      ?
  Level Complete         Game Over
     ?                      ?
     v                      v
????????????????    ????????????????
? LevelComplete?    ? GameOverScene?
? Scene        ?    ? - Show score ?
? - Show bonus ?    ? - Retry      ?
? - Next level ?    ? - Menu       ?
????????????????    ????????????????
     ?
     v
Next Level (Loop)
```

---

## ?? Level Progression Data

```csharp
// Trong Constants.cs
public const int TOTAL_LEVELS = 10;
public const int BOSS_LEVEL = 10;

// Level difficulty scaling
Level  1-2: Easy introductions
Level  3-4: Medium difficulty
Level  5-6: Hard - intro auto-scroll
Level  7-8: Very Hard - continuous auto-scroll
Level  9:   Extreme - final challenge
Level 10:   Boss - Special mechanics
```

---

## ?? C? Ch? H?i Sinh

### ?? Checkpoint System
```csharp
// M?c ??nh h?i sinh ? v? trí ??u
respawnPos = new Vector2(50, 200);

// Ho?c ? gi?a màn hi?n t?i n?u auto-scroll
if (isAutoScroll)
{
    respawnPos = new Vector2(_camera.Position.X + 150, 200);
}

_player.Position = respawnPos;
_player.Velocity = Vector2.Zero;
_player.SetState(new SmallState());
_player.StartInvincible();  // B?t t? 2 giây
```

### ?? Lives System
```
Starting lives: 3
M?t 1 m?ng khi:
- Ch?m quái v?t
- R?i xu?ng v?c
- B? auto-scroll cu?n ra sau

Game Over khi: Lives == 0
```

---

## ?? Scene Transitions

```csharp
// Chuy?n sang gameplay
GameManager.Instance.ChangeScene(new GameplayScene(levelIndex));

// T?m d?ng
GameManager.Instance.ChangeScene(new PauseScene(currentLevel));
GameManager.Instance.SaveGameState(state);  // L?u tr?ng thái

// Resume t? pause
GameState savedState = GameManager.Instance.GetSavedGameState();
if (savedState.IsValid)
{
    RestoreGameState(savedState);
}

// Level hoàn thành
GameManager.Instance.ChangeScene(
    new LevelCompleteScene(level, total, score, coins, bonus, enemies, time, mushrooms, deaths)
);

// Game Over
GameManager.Instance.ChangeScene(
    new GameOverScene(level, score, coins, enemies)
);
```

---

## ?? Th?ng Kê Level

### ?? M?c Tiêu Level
- **Primary**: ??n ???c lâu ?ài
- **Secondary**: Thu th?p coin
- **Tertiary**: Gi?t quái v?t
- **Challenge**: Hoàn thành mà không ch?t

### ?? Tracking Stats
```csharp
public class GameHUD
{
    public int LivesRemaining { get; set; }
    public int CoinsCollected { get; set; }
    public int EnemiesDefeated { get; set; }
    public int CurrentScore { get; set; }
    public float ElapsedTime { get; set; }
    public int MushroomsCollected { get; set; }
    public int DeathCount { get; set; }
}
```

---

## ?? Level Completion Flow

```csharp
// 1. Player ch?m Castle
if (obj is Castle && _player.Bounds.Intersects(obj.Bounds))
{
    _isLevelFinished = true;
}

// 2. Ch? 2 giây
if (_isLevelFinished)
{
    _finishTimer += dt;
    if (_finishTimer > 2.0f)
    {
        // 3. C?p nh?t Session
        GameSession.Instance.AddLevelStats(
            _hud.CurrentScore,
            _hud.CoinsCollected,
            _hud.EnemiesDefeated,
            _hud.ElapsedTime
        );

        // 4. L?u vào SaveSlot
        SaveSlotManager.UpdateCurrentSlot(
            _levelIndex + 1,  // Unlock level ti?p
            GameSession.Instance.TotalScore,
            _player.Lives,
            GameSession.Instance.TotalCoinsThisGame,
            GameSession.Instance.TotalEnemiesThisGame,
            GameSession.Instance.TotalTime
        );

        // 5. Chuy?n sang LevelCompleteScene
        GameManager.Instance.ChangeScene(new LevelCompleteScene(...));
    }
}
```

---

## ?? Recommended Level Design

### Structuring a Level JSON:

```json
{
  "tileSize": 64,
  "layout": [
    "                                                                    ",
    "                                                      M            ",
    "                                      ###   C  C     ###          ",
    "        C       C      E              ##                          ",
    "       ###     ###                    #         E                 ",
    "     ##   #   #  ##       ###    E   ###   ###                    ",
    "G##############################################################################"
  ],
  "isAutoScroll": false,
  "scrollSpeed": 0,
  "canShoot": false,
  "hasBulletBill": false
}
```

### Best Practices:
1. **Spacing**: Tránh quá nhi?u quái g?n nhau
2. **Difficulty Curve**: D? ??n khó t? trái sang ph?i
3. **Collectibles**: ??t coin ? v? trí khó nh?ng không imposible
4. **Checkpoints**: Không ?? gap quá l?n
5. **Enemies**: Spread out, không stack quá nhi?u

---

## ?? Testing Levels

### Debug Level Loading:
```csharp
// Trong Game1.cs
// GameManager.Instance.ChangeScene(new GameplayScene(2));  // Jump to level 2
// GameManager.Instance.ChangeScene(new GameplayScene(10)); // Jump to boss
```

### Level Editor Tips:
- M?t dòng = m?t hàng tiles
- M?t ký t? = m?t tile (64px × 64px)
- S? d?ng text editor ?? thi?t k?
- Validate JSON tr??c khi load

---

## ?? Summary

**MarioGame Level System**:
- ? 10 levels (1-9 regular, 10 boss)
- ? Dynamic level loading t? JSON
- ? Save/Load slots
- ? Auto-scroll support
- ? Multiple game modes (1P, 2P)
- ? Comprehensive stats tracking
- ? Smooth difficulty progression

**Next Steps**:
1. Add more level variations
2. Implement difficulty selector
3. Create custom level editor
4. Add leaderboard system
5. Implement achievements per level

