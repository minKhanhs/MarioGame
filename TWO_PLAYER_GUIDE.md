# 2-Player Cooperative Mode Guide

## Overview

Game h? tr? ch? ?? ch?i cùng nhau (Cooperative) cho 2 ng??i ch?i trên cùng m?t bàn phím.

---

## ?i?u khi?n

### Player 1 (Mario)
- **? ?**: Di chuy?n trái/ph?i
- **W / SPACEBAR**: Nh?y
- **ESC**: Pause

### Player 2 (Luigi)
- **? ?** (Arrow Keys): Di chuy?n trái/ph?i
- **?** (Arrow Up): Nh?y
- **ESC**: Pause

---

## Lu?t ch?i

### ?i?u ki?n Th?ng
```
? C? HAI ng??i ch?i ph?i ??n ?ích (Castle) 
   ?? hoàn thành level
```

### ?i?u ki?n Thua
```
? CH? C?N M?T NG??I ch?t ? Game Over
   C? 2 s? ph?i ch?i l?i t? ??u
```

### ?i?m s? & Coin
- M?i player có ?i?m s? riêng
- M?i player có m?ng riêng
- **Hi?n th? t?ng** trên HUD:
  - Total Score = Score P1 + Score P2
  - Total Coins = Coins P1 + Coins P2
  - Lives = Minimum(Lives P1, Lives P2)

---

## Game Flow

### 1. Start Game (2-Player Mode)
```
Menu ? [2 PLAYERS] ? TwoPlayerGameplayScene
                          ?
                   Load Level
                   Spawn 2 Players
                   (P1 at 50,200)
                   (P2 at 100,200)
```

### 2. During Gameplay
```
Update both players:
??? Input handling (P1 with WASD, P2 with Arrow Keys)
??? Physics & animation
??? Collision detection (both with world & enemies)
??? Check if either player reached goal

Camera follows AVERAGE position of both players
```

### 3. Level Completion
```
Both players reach Castle simultaneously ?
                    ?
        LevelCompleteScene
       (Shows combined stats)
```

### 4. Game Over
```
Any player dies or falls ?
                    ?
    TwoPlayerGameOverScene
   (Shows who died + reason)
   (Combined score)
       Retry or Menu
```

---

## Implementation Details

### Player Class Updates
```csharp
public class Player
{
    public int PlayerIndex { get; set; } // 1 or 2
    public bool HasReachedGoal { get; set; } // Track goal completion
    
    // Constructor now accepts PlayerIndex
    public Player(Vector2 startPos, Dictionary<string, SpriteAnimation> animations, int playerIndex = 1)
}
```

### InputHandler Support
```csharp
// Get input for specific player
var input = _inputHandler.GetInput((PlayerIndex)player.PlayerIndex);

// P1: Uses WASD + W for jump
// P2: Uses Arrow Keys + Up for jump
```

### TwoPlayerGameplayScene
Key features:
- Creates 2 Player instances
- Updates both players
- Checks collisions for both
- Tracks goal completion for both
- Camera follows average position

```csharp
// Camera follows both players
Vector2 avgPos = new Vector2(
    (_player1.Position.X + _player2.Position.X) / 2,
    (_player1.Position.Y + _player2.Position.Y) / 2
);
```

### Level Completion Logic
```csharp
// Both players must reach goal
if (obj is Castle)
{
    if (_player1.Bounds.Intersects(obj.Bounds))
        _player1.HasReachedGoal = true;
    
    if (_player2.Bounds.Intersects(obj.Bounds))
        _player2.HasReachedGoal = true;

    // Both reached goal = level complete
    if (_player1.HasReachedGoal && _player2.HasReachedGoal)
    {
        _isLevelFinished = true;
    }
}
```

### Game Over Logic
```csharp
// Any player death triggers game over
if (_player1.Lives <= 0 || _player1.Position.Y > 1000 ||
    _player2.Lives <= 0 || _player2.Position.Y > 1000)
{
    // Determine death reason
    string deathReason = GetDeathReason();
    
    // Go to 2-player game over scene
    GameManager.Instance.ChangeScene(
        new TwoPlayerGameOverScene(_levelIndex, score, coins, deathReason)
    );
}
```

---

## Statistics Tracking

### During Gameplay (HUD)
```
Lives: MIN(P1 Lives, P2 Lives)
Score: P1 Score + P2 Score
Coins: P1 Coins + P2 Coins
Time:  Level elapsed time
```

### On Level Complete
```csharp
int combinedScore = _player1.Score + _player2.Score;
int totalCoins = _player1.Coins + _player2.Coins;

// Pass to LevelCompleteScene
GameManager.Instance.ChangeScene(
    new LevelCompleteScene(_levelIndex, 3, combinedScore, totalCoins, ...)
);
```

### On Game Over
```csharp
// Display who died and why
string deathReason = "Player 1 died";  // or fell
string deathReason = "Player 2 died";  // or fell
```

---

## HUD Display in 2-Player Mode

```
?????????????????????????????????????
[LIVES: 2]  [SCORE: 2450]  [COINS: 8]
?????????????????????????????????????

Game World
(Camera centered on average of both players)

```

**Note:** Lives show minimum between P1 and P2
- If P1 has 3 lives and P2 has 1 life ? Shows "1"
- When either reaches 0 ? Game Over

---

## Death Scenarios

### Scenario 1: P1 Dies
```
P1 takes damage 3 times (no mushroom) ? 0 lives
                    ?
        TwoPlayerGameOverScene
        "Player 1 died"
        Both retry together
```

### Scenario 2: P1 Falls
```
P1 walks off cliff ? Y > 1000
                    ?
        TwoPlayerGameOverScene
        "Player 1 fell"
        Both retry together
```

### Scenario 3: P2 Dies
```
P2 takes damage 3 times (no mushroom) ? 0 lives
                    ?
        TwoPlayerGameOverScene
        "Player 2 died"
        Both retry together
```

---

## Cooperative Strategies

### Tips for Players
1. **Stay together** - If one player falls behind, the other can't progress alone
2. **Balance coins** - Collect coins fairly to maximize both scores
3. **Protect each other** - If one player is in danger, the other should position carefully
4. **Time jumps** - Jump together for long sections
5. **Share platforms** - Use each other as platforms if needed (body collision)

### Challenge Ideas
- Complete level without either player taking damage (Perfect Run x2)
- Minimize time by coordinating movements
- Maximize coins collected together
- Defeat maximum enemies as a team

---

## Technical Notes

### Performance
- 2 players = 2x collision checks
- Camera update follows average position (smooth transition)
- HUD combines both stats (no performance impact)

### Compatibility
- Same level file used as single-player
- Enemies don't have allegiance (attack both equally)
- Items collect for the specific player who touches them
- Can pause/resume with 2 players

### Future Enhancements
1. **Revive System** - Allow partner to revive fallen player within time limit
2. **Combo Attacks** - Extra damage when both jump on enemy simultaneously
3. **Shared Power-ups** - Some items boost both players
4. **Split-screen Mode** - Two camera views for larger screens
5. **Co-op Challenges** - Special achievements for 2-player only
6. **Team Difficulty** - Enemies spawn more with 2 players active

---

## Scenes in 2-Player Mode

| Scene | Purpose |
|-------|---------|
| MenuScene | Select 2-PLAYERS option |
| TwoPlayerGameplayScene | Main 2-player gameplay |
| LevelCompleteScene | Show combined stats when both reach goal |
| TwoPlayerGameOverScene | Handle game over with death reason |
| PauseScene | Pause for both players |

---

## Debugging 2-Player

Console logs show:
```
[TWO PLAYER] Loaded level 1
[GAME OVER] Level 1 (2 Players)
[LEVEL COMPLETE] Level 1 (2 Players)
```

Check player states:
```csharp
Debug.WriteLine($"P1: {_player1.Lives} lives, {_player1.Score} score");
Debug.WriteLine($"P2: {_player2.Lives} lives, {_player2.Score} score");
Debug.WriteLine($"Both reached goal: {_player1.HasReachedGoal && _player2.HasReachedGoal}");
```
