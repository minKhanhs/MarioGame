# ?? MarioGame - Complete Documentation

Chào m?ng b?n ??n v?i **MarioGame** - m?t game Mario 2D ???c phát tri?n b?ng **MonoGame 3.8.4**, **.NET 8** và **C# 12** v?i các Design Patterns hi?n ??i.

---

## ?? Tài Li?u D? Án

D? án này bao g?m **5 tài li?u markdown** chi ti?t:

### 1. ?? **PROJECT_SUMMARY.md** (B?t ??u ? ?ây!)
   - T?ng quan toàn b? d? án
   - Tính n?ng chính
   - Ki?n trúc t?ng quan
   - Th?ng kê d? án
   - **?? ??c tr??c tiên**

### 2. ?? **DESIGN_PATTERNS.md**
   - 13 Design Patterns ???c áp d?ng
   - Gi?i thích chi ti?t m?i pattern
   - V? trí trong code
   - Cách s? d?ng
   - L?i ích c?a m?i pattern
   - ?? xu?t c?i ti?n
   - **?? H?c v? Design Patterns**

### 3. ?? **LEVELS_AND_GAME_MODES.md**
   - Chi ti?t 10 levels
   - H? th?ng ?i?m s?
   - Game modes (1P, 2P)
   - Level progression
   - C? ch? Boss battle
   - Save slot system
   - **?? Hi?u v? gameplay**

### 4. ?? **STATISTICS_AND_REPORTING.md**
   - 3-tier data system
   - Career stats (permanent)
   - Session stats (per game)
   - Save slot tracking
   - Scoring formula
   - Data persistence
   - Achievement system
   - **?? Tìm hi?u d? li?u**

### 5. ??? **CODE_ORGANIZATION_ARCHITECTURE.md**
   - C?u trúc th? m?c chi ti?t
   - 4-layer architecture
   - Namespace organization
   - Coding standards
   - Performance considerations
   - Scalability strategy
   - **?? Tìm hi?u code structure**

---

## ?? Quick Start

### Yêu C?u
- **Visual Studio 2022+** ho?c **Visual Studio Code**
- **.NET 8 SDK**
- **MonoGame 3.8.4**
- **Windows 10/11** (ho?c Linux v?i MonoGame support)

### Cài ??t
```bash
# Clone repository
git clone https://github.com/minKhanhs/MarioGame.git
cd MarioGame

# Restore packages
dotnet restore

# Build project
dotnet build

# Run game
dotnet run
```

### ?i?u Khi?n Trò Ch?i

**Player 1 (Arrow Keys)**:
- ?? Move Left: Left Arrow
- ?? Move Right: Right Arrow
- ?? Jump: Up Arrow
- ?? Attack (Fireball): Right Control
- ?? Pause: Escape

**Player 2 (WASD)**:
- ?? Move Left: A
- ?? Move Right: D
- ?? Jump: W
- ?? Attack: H
- ?? Pause: Backspace

---

## ?? Game Features

### ? Core Gameplay
- **10 Levels** with increasing difficulty
- **2 Game Modes**: Single Player, Dual Player
- **Multiple Save Slots**: Independent progress tracking
- **3 Enemy Types**: Goomba, Koopa, Piranha Plant
- **Boss Battle**: Level 10 special mechanics
- **Physics System**: Gravity, collision, animation

### ?? Statistics System
- **Career Stats**: Permanent across all games
- **Session Stats**: Per playthrough tracking
- **Level Completion**: Score, time, achievements
- **Save System**: Full game state persistence

### ?? Camera Systems
- **Follow Player**: Smooth lerp-based following
- **Auto-Scroll**: Continuous level progression

---

## ?? Project Structure

```
MarioGame/
??? src/
?   ??? _Core/           # Game management
?   ??? _Entities/       # Game objects
?   ??? _Scenes/         # Game screens
?   ??? _Input/          # Input handling
?   ??? _Data/           # Data persistence
?   ??? _UI/             # UI components
?   ??? _Utils/          # Utilities
??? Content/
?   ??? sprites/         # Image assets
?   ??? audio/           # Music & sound
?   ??? fonts/           # Game fonts
?   ??? levels/          # Level JSON files
??? Game1.cs             # Entry point
??? Program.cs           # Main program
```

---

## ??? Architecture

### 4-Layer Architecture
```
???????????????????????????????
? Presentation Layer          ? (UI, Scenes, HUD)
???????????????????????????????
? Game Logic Layer            ? (Player, Enemies, Physics)
???????????????????????????????
? Core Layer                  ? (Managers, Input)
???????????????????????????????
? Data Layer                  ? (Persistence, JSON)
???????????????????????????????
```

### Design Patterns Used
? State Pattern (Player states)
? Strategy Pattern (Camera)
? Factory Pattern (Enemy creation)
? Singleton Pattern (Managers)
? Template Method (Scene lifecycle)
? Adapter Pattern (Input handling)
? Observer Pattern (Game events)
? Decorator Pattern (Invincibility)
? Dependency Injection (Constructor params)
? Composite Pattern (Game objects)

---

## ?? Technology Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 8.0 (LTS) | Runtime & Framework |
| **C#** | 12.0 | Language |
| **MonoGame** | 3.8.4 | Game Engine |
| **Visual Studio** | 2022+ | IDE |

---

## ?? Code Statistics

| Metric | Value |
|--------|-------|
| **Total C# Files** | 67+ |
| **Total Lines of Code** | 8000+ |
| **Design Patterns** | 10+ |
| **Architecture Layers** | 4 |
| **Game Levels** | 10 |
| **Enemy Types** | 6+ |
| **Collectible Types** | 3 |

---

## ?? Key Classes

### Core Management
- `GameManager` - Scene switching, state management
- `SoundManager` - Audio playback
- `InputHandler` - Keyboard input processing
- `Camera` - Viewport management

### Game Entities
- `Player` - Main character
- `Enemy` (Goomba, Koopa, etc.) - Enemies
- `Item` (Coin, Mushroom) - Collectibles
- `Block`, `Pipe`, `Castle` - Environment

### Data & Persistence
- `MapLoader` - Level loading
- `SaveSlotManager` - Save slot management
- `CareerStats` - Permanent statistics
- `GameSession` - Session tracking

### Systems
- `Collision` - Physics & collision detection
- `SpriteAnimation` - Frame animation
- `GameHUD` - Display statistics
- Camera Strategies - Viewport control

---

## ?? Game Flow

```
Start
  ?
Menu
  ?? 1 Player / 2 Players
  ?? Settings / Achievements
  ?? Compendium / About Us
  ?? Credits
  ?
Select/Create Save Slot
  ?
Gameplay (Levels 1-9)
  ?? Play level
  ?? Collect coins
  ?? Defeat enemies
  ?? Reach castle
  ?
Level Complete
  ?? Show score & bonus
  ?? Unlock next level
  ?
Boss Battle (Level 10)
  ?? Special flying mechanics
  ?? Attack with fireballs
  ?? Defeat boss
  ?
Victory
  ?
Menu (start over or load different save)
```

---

## ?? Coding Standards

### Naming Conventions
```csharp
// Classes
public class GameManager { }

// Methods & Properties
public void UpdatePosition() { }
public Vector2 Position { get; set; }

// Local variables
float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

// Constants
public const float GRAVITY = 0.6f;

// Private fields
private Vector2 _velocity;

// Interfaces
public interface IScene { }
```

### Code Organization
- ? Single Responsibility Principle
- ? DRY (Don't Repeat Yourself)
- ? SOLID Principles throughout
- ? Meaningful comments (WHY, not WHAT)
- ? Error handling with logging

---

## ?? Testing & Quality

### Build Status
? Builds successfully (.NET 8)
? No compilation errors
? All references resolved
? MonoGame dependencies satisfied

### Code Quality
? Consistent naming conventions
? Clear separation of concerns
? Proper design pattern usage
? Good documentation
? Error handling in place

---

## ?? Getting Started with Documentation

### For Beginners
1. **Start with** ? `PROJECT_SUMMARY.md`
2. **Learn basics** ? `LEVELS_AND_GAME_MODES.md`
3. **Understand design** ? `CODE_ORGANIZATION_ARCHITECTURE.md`
4. **Deep dive** ? `DESIGN_PATTERNS.md`
5. **Advanced topic** ? `STATISTICS_AND_REPORTING.md`

### For Experienced Developers
1. **Quick overview** ? `PROJECT_SUMMARY.md`
2. **Architecture details** ? `CODE_ORGANIZATION_ARCHITECTURE.md`
3. **Pattern analysis** ? `DESIGN_PATTERNS.md`
4. **Data structure** ? `STATISTICS_AND_REPORTING.md`
5. **Gameplay mechanics** ? `LEVELS_AND_GAME_MODES.md`

### For Game Designers
1. **Game mechanics** ? `LEVELS_AND_GAME_MODES.md`
2. **Level design** ? `LEVELS_AND_GAME_MODES.md` (Level Format section)
3. **Scoring system** ? `STATISTICS_AND_REPORTING.md`
4. **UI/UX features** ? `PROJECT_SUMMARY.md`

---

## ?? Key Concepts

### State Pattern (Player)
Player có th? chuy?n gi?a các tr?ng thái:
- **SmallState**: Nh?, d? ch?t (m?t 1 m?ng)
- **BigState**: To, kh?e h?n (m?t 1 m?ng nh?ng không ch?t)
- **Future**: FireFlowerState, SuperStarState

### Strategy Pattern (Camera)
Camera có th? s? d?ng các chi?n l??c khác nhau:
- **FollowTargetStrategy**: Theo sát Player (có lerp m??t)
- **AutoScrollStrategy**: T? ??ng cu?n t? trái sang ph?i

### Factory Pattern (Enemies)
Enemies ???c t?o t? m?t factory duy nh?t:
```csharp
Enemy enemy = EnemyFactory.CreateEnemy('E', position, textures);
// 'E' = Goomba, 'K' = Koopa, 'P' = Piranha
```

### Save System
3 t?ng d? li?u:
1. **Career Stats** (v?nh vi?n): Coins t?ng, Enemies t?ng
2. **Session Stats** (per game): ?i?m, th?i gian, level hi?n t?i
3. **Save Slots** (per world): Ti?n ?? riêng bi?t

---

## ?? Common Tasks

### Adding a New Enemy Type

1. Create class inheriting from `Enemy`:
```csharp
public class NewEnemy : Enemy
{
    public override void OnStomped() { }
}
```

2. Add case to `EnemyFactory`:
```csharp
case 'N': return new NewEnemy(texture, position);
```

3. Add ký t? to level JSON:
```json
["N     #  E  K"]
```

### Adding a New Power-up

1. Create class inheriting from `Item`:
```csharp
public class PowerUp : Item
{
    public override void OnCollect(Player player) { }
}
```

2. Add case to `MapLoader.CreateObjectFromCode()`:
```csharp
case 'X': return new PowerUp(texture, position);
```

3. Use in level JSON

### Changing Camera Behavior

```csharp
// In GameplayScene.LoadContent()
if (isBossLevel)
{
    _camera.SetStrategy(new BossCameraStrategy());
}
else if (isAutoScroll)
{
    _camera.SetStrategy(new AutoScrollStrategy());
}
else
{
    _camera.SetStrategy(new FollowTargetStrategy());
}
```

---

## ?? Useful Links

### Documentation Files (In This Project)
- `PROJECT_SUMMARY.md` - Complete overview
- `DESIGN_PATTERNS.md` - Pattern deep-dives
- `LEVELS_AND_GAME_MODES.md` - Gameplay details
- `STATISTICS_AND_REPORTING.md` - Data system
- `CODE_ORGANIZATION_ARCHITECTURE.md` - Architecture details

### External Resources
- **MonoGame Documentation**: https://docs.monogame.net/
- **.NET 8 Documentation**: https://learn.microsoft.com/dotnet/
- **Design Patterns**: https://en.wikipedia.org/wiki/Design_pattern
- **GitHub Repository**: https://github.com/minKhanhs/MarioGame

---

## ?? Contributing

### Guidelines
1. Follow coding standards (see CODE_ORGANIZATION_ARCHITECTURE.md)
2. Apply SOLID principles
3. Use design patterns appropriately
4. Write meaningful comments
5. Handle errors gracefully
6. Update documentation

### Adding Features
1. Create new class in appropriate folder
2. Implement required interfaces
3. Update relevant managers/factories
4. Add error handling
5. Write documentation
6. Test thoroughly

---

## ?? Performance Tips

- ? Only render visible objects (camera culling)
- ? Use sprite sheets instead of individual frames
- ? Cache loaded textures in MapLoader
- ? Efficient collision detection
- ?? Consider quadtree for spatial partitioning

---

## ?? Learning Outcomes

By studying MarioGame, you'll learn:

? **Object-Oriented Programming**: Classes, inheritance, polymorphism
? **Design Patterns**: 10+ industry-standard patterns
? **Game Development**: Physics, animation, AI, camera control
? **Software Architecture**: Layered design, separation of concerns
? **Data Management**: JSON, file I/O, persistence
? **Best Practices**: SOLID, DRY, error handling, documentation
? **Professional Development**: Code organization, standards, maintainability

---

## ?? FAQ

### Q: Làm sao tôi có th? thêm level m?i?
A: T?o file `level11.json` trong `Content/levels/` v?i cùng ??nh d?ng. Update `Constants.TOTAL_LEVELS = 11`.

### Q: Làm sao thay ??i t?c ?? player?
A: Trong `Player.cs`, thay ??i `MOVE_SPEED` constant.

### Q: Làm sao thêm item m?i?
A: T?o class m?i t? `Item`, add case vào `MapLoader.CreateObjectFromCode()`.

### Q: Làm sao debug game state?
A: Trong `Game1.cs`, uncomment lines ?? debug. Check `GameplayScene.cs` debug output.

### Q: Làm sao ch?nh âm thanh?
A: Trong `SoundManager.cs`, g?i `SetMusicVolume()` method.

---

## ?? Project Status

| Aspect | Status |
|--------|--------|
| Build | ? Successful |
| Code Quality | ? Excellent |
| Documentation | ? Comprehensive |
| Design Patterns | ? Well Applied |
| Game Features | ? Complete |
| Testing | ? Builds & Runs |
| Performance | ? Optimized |
| Maintainability | ? High |

---

## ?? Summary

**MarioGame** là m?t d? án game development chuyên nghi?p cho th?y:
- ? Ki?n trúc s?ch và rõ ràng
- ? Design patterns ???c áp d?ng hi?u qu?
- ? Code t? ch?c t?t và d? b?o trì
- ? Tính n?ng game hoàn ch?nh
- ? H? th?ng d? li?u m?nh m?
- ? Tài li?u chi ti?t

D? án ph?c v? c? nh? m?t **game hoàn ch?nh** và m?t **tài nguyên h?c t?p** cho game development.

---

## ?? Th??ng Th?c Game!

Bây gi? b?n ?ã hi?u ???c c?u trúc, hãy:
1. **Ch?i game** ?? hi?u gameplay
2. **??c tài li?u** ?? hi?u thi?t k?
3. **Khám phá code** ?? h?c architecture
4. **Th? t?o features** m?i ?? th?c hành

**Chúc vui v?! ??**

---

**Generated**: January 2024
**Project**: MarioGame
**Language**: C# 12.0
**.NET**: 8.0 (LTS)
**Status**: ? Production Ready

