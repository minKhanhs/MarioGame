# Achievement System Guide

## Overview

Game bây gi? có h? th?ng Achievement (Thành tích) hoàn ch?nh v?i 16 thành tích khác nhau.

---

## Danh sách Achievements

### Combat Achievements (Chi?n ??u)

| ID | Tên | ?i?u ki?n | Mô t? |
|---|---|---|---|
| `first_kill` | **First Blood** | Di?t enemy ??u tiên | Defeat your first enemy |
| `enemy_10` | **Slayer** | Di?t 10 quái trong 1 level | Defeat 10 enemies in a single game |
| `enemy_50` | **Serial Slayer** | Di?t 50 quái t?ng c?ng | Defeat 50 enemies total |
| `enemy_100` | **Exterminator** | Di?t 100 quái t?ng c?ng | Defeat 100 enemies total |
| `enemy_1000` | **Monster Slayer** | Di?t 1000 quái t?ng c?ng | Defeat 1000 enemies total |

### Collection Achievements (Thu th?p)

| ID | Tên | ?i?u ki?n | Mô t? |
|---|---|---|---|
| `first_mushroom` | **Power Up** | ?n n?m l?n ??u | Collect your first mushroom |
| `coin_10` | **Coin Collector** | ?n 10 coin trong 1 level | Collect 10 coins in a single game |
| `coin_50` | **Gold Rush** | ?n 50 coin t?ng c?ng | Collect 50 coins total |
| `coin_100` | **Wealthy** | ?n 100 coin t?ng c?ng | Collect 100 coins total |

### Level Achievements (Màn ch?i)

| ID | Tên | ?i?u ki?n | Mô t? |
|---|---|---|---|
| `level_complete` | **Hero** | Hoàn thành 1 level | Complete a level |
| `level_2` | **Adventurer** | Hoàn thành 2 level | Complete 2 levels |
| `level_3` | **Legend** | Hoàn thành c? 3 level | Complete all 3 levels |

### Score Achievements (?i?m s?)

| ID | Tên | ?i?u ki?n | Mô t? |
|---|---|---|---|
| `score_1000` | **Score 1000** | ??t 1000 ?i?m trong 1 level | Earn 1000 points in a single game |
| `score_5000` | **Score 5000** | ??t 5000 ?i?m t?ng c?ng | Earn 5000 points total |

### Challenge Achievements (Thách th?c)

| ID | Tên | ?i?u ki?n | Mô t? |
|---|---|---|---|
| `speedrun` | **Speedrunner** | Hoàn level trong < 30 giây | Complete a level in under 30 seconds |
| `perfect_level` | **Perfect Run** | Hoàn level không b? ?ánh | Complete a level without taking damage |

---

## Cách Achievement ???c Unlock

### Khi ch?i game:
```csharp
// M?i l?n ?i?u ki?n ???c ?áp ?ng, achievement t? ??ng unlock
- Gi?t quái ??u tiên ? "First Blood"
- ?n n?m l?n ??u ? "Power Up"
- ?i?m trong level >= 1000 ? "Score 1000"
- Hoàn level d??i 30 giây ? "Speedrunner"
- Hoàn level không h?ng m?ng ? "Perfect Run"
```

### Khi GameOver:
```
GameOverScene.CheckAchievements()
  ? Ki?m tra t?t c? ?i?u ki?n
  ? Unlock thành tích phù h?p
  ? L?u vào file achievements.json
```

### Khi Level Complete:
```
LevelCompleteScene.CheckAchievements()
  ? Ki?m tra ?i?u ki?n level
  ? Unlock các achievement liên quan
  ? L?u d? li?u
```

---

## Achievement UI

### AchievementScene (B?ng Thành tích)

```
???????????????????????????????????????
        ACHIEVEMENTS
    Progress: 5/16
???????????????????????????????????????

[*] First Blood                    (UNLOCKED)
    Defeat your first enemy
    Unlocked: 01/15/2024

[ ] Monster Slayer                 (LOCKED)
    Defeat 1000 enemies total

[ ] Legend                         (LOCKED)
    Complete all 3 levels

    Page 1/3
    
[PREVIOUS] [MAIN MENU] [NEXT]
???????????????????????????????????????
```

**Tính n?ng:**
- ? Hi?n th? 6 achievement/page
- ? Hi?n th? tr?ng thái: [*] = Unlocked, [ ] = Locked
- ? Hi?n th? ngày unlock (n?u có)
- ? Navigation v?i LEFT/RIGHT ho?c buttons
- ? Thanh ti?n ?? (X/16)

### CreditsScene (Tín d?ng)

```
???????????????????????????????????????
            CREDITS
???????????????????????????????????????

DEVELOPMENT TEAM
Lead Developer - Design & Programming
Graphics Artist - Sprite & Asset Design
Audio Designer - Music & Sound Effects

GAME DESIGN
Level Designer - Map Creation & Gameplay
Game Designer - Mechanics & Balance

QUALITY ASSURANCE
QA Lead - Testing & Bug Reports

Special Thanks to Nintendo...
Built with MonoGame & .NET 8

Press ESC or click MAIN MENU to return
???????????????????????????????????????
```

---

## Data Persistence

### File l?u tr?:
```
Content/data/achievements.json
```

### C?u trúc JSON:
```json
[
  {
    "id": "first_kill",
    "name": "First Blood",
    "description": "Defeat your first enemy",
    "isUnlocked": true,
    "unlockedDate": "2024-01-15T14:30:00"
  },
  {
    "id": "enemy_1000",
    "name": "Monster Slayer",
    "description": "Defeat 1000 enemies total",
    "isUnlocked": false,
    "unlockedDate": null
  }
]
```

---

## Integration Points

### 1. GameplayScene
```csharp
// Khi enemy b? ??p
_hud.EnemiesDefeated++; // Track cho achievements
```

### 2. GameOverScene
```csharp
// Khi game over
CheckAchievements();
AchievementManager.Instance.SaveAll();
```

### 3. LevelCompleteScene
```csharp
// Khi hoàn thành level
AchievementManager.Instance.CheckAndUnlockAchievements(
    coins, enemies, score, levelTime, 
    totalEnemies, totalCoins, totalScore, 
    tookDamage
);
```

### 4. MenuScene
```csharp
// Button ACHIEVEMENTS -> AchievementScene
if (_buttons[3].WasPressed)
{
    GameManager.Instance.ChangeScene(new AchievementScene());
}

// Button CREDITS -> CreditsScene
if (_buttons[7].WasPressed)
{
    GameManager.Instance.ChangeScene(new CreditsScene());
}
```

---

## AchievementManager API

```csharp
// L?y achievement theo ID
Achievement ach = AchievementManager.Instance.GetAchievement("first_kill");

// Unlock th? công
ach?.Unlock();

// L?y t?t c? achievements
List<Achievement> all = AchievementManager.Instance.GetAllAchievements();

// L?y nh?ng achievement ?ã unlock
List<Achievement> unlocked = AchievementManager.Instance.GetUnlockedAchievements();

// ??m achievement ?ã unlock
int count = AchievementManager.Instance.GetUnlockedCount(); // 5/16

// Check và unlock d?a trên ?i?u ki?n
AchievementManager.Instance.CheckAndUnlockAchievements(
    coins, enemies, score, levelTime, 
    totalEnemies, totalCoins, totalScore, 
    tookDamage
);

// L?u thay ??i vào file
AchievementManager.Instance.SaveAll();

// Reset t?t c? achievements
AchievementManager.Instance.Reset();
```

---

## Future Enhancements

Nh?ng c?i ti?n ti?m n?ng:

1. **Hidden Achievements** - Nh?ng thành tích bí m?t không hi?n th? tr??c khi unlock
2. **Leaderboard** - So sánh achievements gi?a các player
3. **Notifications** - Thông báo khi unlock achievement
4. **Rarity Tiers** - Common, Rare, Epic, Legendary
5. **Progress Tracking** - Hi?n th? "5/10" cho achievements c?n nhi?u b??c
6. **Badges** - Icon ??c bi?t cho t?ng achievement
7. **Streaks** - Tracking achievements liên ti?p (3 levels hoàn h?o liên ti?p)

---

## Testing Achievements

?? test achievements nhanh:

```csharp
// Force unlock achievement
AchievementManager.Instance.GetAchievement("monster_slayer")?.Unlock();
AchievementManager.Instance.SaveAll();

// Xem progress
int unlockedCount = AchievementManager.Instance.GetUnlockedCount();
Debug.WriteLine($"Unlocked: {unlockedCount}/16");
```

---

## Debug Logging

M?i khi achievement ???c unlock, console s? in:
```
[ACHIEVEMENT] Unlocked: First Blood
[ACHIEVEMENT] Unlocked: Slayer
[ACHIEVEMENTS] Saved to file
```
