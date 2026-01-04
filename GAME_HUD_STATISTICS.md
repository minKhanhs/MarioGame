# Game HUD and Statistics System

## Overview

The game now tracks and displays comprehensive player statistics during gameplay and at level completion.

---

## In-Game HUD Display

### What's Displayed at Top of Screen

```
[LIVES: 3]              [SCORE: 1250]           [COINS: 5]         [TIME: 02:45]
```

Located in a semi-transparent bar at the top of the game screen:

- **LIVES**: Remaining player lives (decreases when taking damage)
- **SCORE**: Current accumulated points
- **COINS**: Total coins collected in level
- **TIME**: Elapsed time in MM:SS format

### HUD Updates in Real-Time

The HUD updates every frame with current player stats:

```csharp
// In GameplayScene.Update()
_hud.LivesRemaining = _player.Lives;
_hud.CoinsCollected = _player.Coins;
_hud.CurrentScore = _player.Score;
_hud.Update(gameTime); // Updates elapsed time
```

---

## Score Calculation System

### Base Score
- **Coins collected**: +10 points per coin
- **Enemies defeated**: +100 points per enemy (when jumped on)

### Level Completion Bonuses

When level is completed, additional bonus is calculated:

```csharp
int CalculateLevelBonus()
{
    int baseBonus = 500;           // Base level completion bonus
    int enemyBonus = enemies * 100; // 100 points per enemy defeated
    int timeBonus = 0;
    
    if (completionTime < 60 seconds)
        timeBonus = 300 - (time * 5); // Faster = more bonus, max 300
    
    return baseBonus + enemyBonus + timeBonus;
}
```

### Example Calculation

Level completed with:
- Base bonus: 500
- Enemies defeated: 3 × 100 = 300
- Time bonus (45 seconds): 300 - (45 × 5) = 75

**Total bonus: 500 + 300 + 75 = 875 points**

---

## Level Complete Screen

Displays after 2 seconds of reaching castle:

### Information Shown
```
??????????????????????????????
?    LEVEL COMPLETE!         ?
?    Level 1 / 3             ?
?  --- STATISTICS ---        ?
?  Coins Collected: 7        ?
?  Enemies Defeated: 5       ?
?  Base Score: +500          ?
?  Bonus Score: +675         ?
?  Total Score: 2175         ?
?                            ?
?  [NEXT LEVEL] [MAIN MENU]  ?
??????????????????????????????
```

### Score Breakdown
1. **Coins Collected**: Shows total coins found in level
2. **Enemies Defeated**: Shows number of enemies jumped on
3. **Base Score**: Fixed 500 points for level completion
4. **Bonus Score**: Calculated from enemies and time
5. **Total Score**: Base + previous score + bonus

---

## Game Over Screen

Displayed when Mario loses all lives or falls off map:

### Information Shown
```
??????????????????????????????
?      GAME OVER             ?
?  Score: 1350               ?
?  Coins Collected: 4        ?
?  Level: 1                  ?
?                            ?
?  [RETRY LEVEL] [MAIN MENU] ?
??????????????????????????????
```

---

## Statistics Tracked

### During Gameplay
| Stat | Type | Updates |
|------|------|---------|
| Lives | Integer | Every damage hit |
| Score | Integer | Coin pickup, enemy defeat |
| Coins | Integer | Item collection |
| Time | Float (seconds) | Every frame |
| Enemies Defeated | Integer | Enemy stomp |

### On Level Complete
- All above stats saved
- Bonus calculated
- Final score computed
- Stats displayed to player

### On Game Over
- Current level index
- Final score achieved
- Coins collected
- Option to retry

---

## GameHUD Class

Located in: `src/_Core/GameHUD.cs`

### Key Methods

```csharp
public class GameHUD
{
    // Update elapsed time each frame
    public void Update(GameTime gameTime)
    
    // Render HUD bar at top of screen
    public void Draw(SpriteBatch spriteBatch)
    
    // Calculate level completion bonus
    public int CalculateLevelBonus()
    
    // Reset stats for new level
    public void Reset()
}
```

### Properties

```csharp
public int LivesRemaining { get; set; }
public int CoinsCollected { get; set; }
public int EnemiesDefeated { get; set; }
public int CurrentScore { get; set; }
public float ElapsedTime { get; set; }
```

---

## Integration Points

### GameplayScene
- Creates `GameHUD` instance in `LoadContent()`
- Updates HUD every frame in `Update()`
- Draws HUD after game world in `Draw()`
- Increments `EnemiesDefeated` when enemy stomped
- Passes HUD stats to `LevelCompleteScene`

### LevelCompleteScene
- Receives final stats from `GameplayScene`
- Displays breakdown of score bonuses
- Shows total score earned

### GameOverScene
- Receives score and coins from `GameplayScene`
- Displays final achievements

---

## Time Bonus Logic

The time bonus rewards speed completion:

```
Time Taken | Time Bonus | Total with Base+Enemies
-----------+------------+------------------------
   < 30s   |    300     |  500 + 300 (enemies) + 300 = 1100+
   30-60s  |  100-300   |  500 + 300 (enemies) + 100-300
   > 60s   |      0     |  500 + 300 (enemies) + 0
```

---

## Future Enhancements

Potential additions to statistics system:

1. **Damage Taken Counter**
   - Track number of times hit
   - Display in level complete screen

2. **Combo System**
   - Consecutive enemies defeated = combo multiplier
   - Display combo counter during gameplay

3. **Collectible Tracking**
   - Track special items collected
   - Display in statistics

4. **Leaderboard**
   - Save high scores per level
   - Display best times

5. **Power-up Statistics**
   - Track power-ups collected
   - Time spent invincible

6. **Death Counter**
   - Total deaths per level
   - Optional hardcore mode (no retries)

---

## Debug Output

When events occur, console displays:

```
[NEW GAME] Loaded level 1
[LEVEL COMPLETE] Level 1, Score: 1250, Coins: 5, Time: 45.2s, Enemies: 3
[GAME OVER] Level 1, Score: 1250, Coins: 5
```

This helps track gameplay metrics and performance.

---

## Example Gameplay Flow

```
1. Player starts Level 1
   ?
2. HUD displays: LIVES: 3, SCORE: 0, COINS: 0, TIME: 00:00
   ?
3. Player collects coin ? COINS: 1, SCORE: 10
   ?
4. Player defeats enemy ? ENEMIES: 1, SCORE: 110
   ?
5. Player continues for 45 seconds
   ?
6. Player reaches castle ? Level Complete timer starts
   ?
7. After 2 seconds, Level Complete screen shows:
   - Coins: 7
   - Enemies: 5
   - Base: 500 + Enemies: 500 + Time: 75 = 1075 bonus
   - Total: previous score + 1075
   ?
8. Player clicks "NEXT LEVEL"
   ?
9. Level 2 starts with HUD reset (but score persists)
```

---

## Configuration

To adjust scoring values, edit `GameHUD.cs`:

```csharp
public int CalculateLevelBonus()
{
    int baseBonus = 500;           // Change base bonus here
    int enemyBonus = _enemiesDefeated * 100; // Change enemy points here
    int timeBonus = _elapsedTime < 60 ? (int)(300 - (_elapsedTime * 5)) : 0;
    // Adjust time bonus calculation here
    return baseBonus + enemyBonus + timeBonus;
}
```
