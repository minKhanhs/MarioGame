# MarioGame - Code Organization & Architecture

## ?? C?u Trúc Th? M?c D? Án

```
MarioGame/
??? src/                           # Main source code
?   ??? _Core/                     # ?? Core management
?   ?   ??? GameManager.cs         # Scene management (Singleton)
?   ?   ??? SoundManager.cs        # Audio management (Singleton)
?   ?   ??? Constants.cs           # Game constants
?   ?   ??? CareerStats.cs         # Career statistics (Static)
?   ?   ??? GameSession.cs         # Session statistics (Singleton)
?   ?   ??? GameHUD.cs             # HUD display
?   ?   ??? Camera/                # Camera system
?   ?       ??? Camera.cs          # Main camera
?   ?       ??? ICameraStrategy.cs # Strategy interface
?   ?       ??? FollowTargetStategy.cs  # Follow player
?   ?       ??? AutoScrollStategy.cs    # Auto scroll
?   ?
?   ??? _Entities/                 # ?? Game entities
?   ?   ??? base/                  # Base classes
?   ?   ?   ??? GameObj.cs         # Base for all entities
?   ?   ?   ??? MovableObj.cs      # Base for physics
?   ?   ?
?   ?   ??? player/                # ?? Player
?   ?   ?   ??? Player.cs          # Main player class
?   ?   ?   ??? Fireball.cs        # Fireball projectile
?   ?   ?   ??? states/            # State Pattern
?   ?   ?       ??? IMarioState.cs # State interface
?   ?   ?       ??? SmallState.cs  # Small Mario
?   ?   ?       ??? BigState.cs    # Big Mario
?   ?   ?
?   ?   ??? enemies/               # ?? Enemies
?   ?   ?   ??? Enemy.cs           # Base enemy
?   ?   ?   ??? EnemyFactory.cs    # Factory Pattern
?   ?   ?   ??? Goomba.cs          # Walking goomba
?   ?   ?   ??? Koopa.cs           # Turtle
?   ?   ?   ??? PiranhaPlant.cs    # Piranha plant
?   ?   ?   ??? Boss.cs            # Boss enemy
?   ?   ?   ??? BulletBill.cs      # Flying bullet
?   ?   ?   ??? BossProjectile.cs  # Boss attack
?   ?   ?
?   ?   ??? items/                 # ?? Items
?   ?   ?   ??? Item.cs            # Base item
?   ?   ?   ??? Coin.cs            # Coin (money)
?   ?   ?   ??? Mushroom.cs        # Mushroom (life)
?   ?   ?   ??? (Future upgrades)
?   ?   ?
?   ?   ??? Enviroments/           # ??? Environment
?   ?       ??? Block.cs           # Breakable block
?   ?       ??? Pipe.cs            # Warp pipe
?   ?       ??? Castle.ReadOnly    # Goal/Castle
?   ?       ??? MysteryBlock.cs    # Mystery block
?   ?
?   ??? _Input/                    # ?? Input system
?   ?   ??? InputHandler.cs        # Handles keyboard input
?   ?   ??? InputFrame.cs          # Input data frame (Adapter)
?   ?   ??? InputSettings.cs       # Input config (Singleton)
?   ?   ??? EGameAction.cs         # Action enum
?   ?
?   ??? _Scenes/                   # ?? Scene management
?   ?   ??? IScene.cs              # Scene interface (Template)
?   ?   ??? MenuScence.cs          # Main menu (Typo: Scence)
?   ?   ??? SaveSlotScence.cs      # Save slot selector
?   ?   ??? GameplayScence.cs      # Main gameplay
?   ?   ??? TwoPlayerGameplayScene.cs  # 2P gameplay
?   ?   ??? PauseScene.cs          # Pause menu
?   ?   ??? LevelCompleteScene.cs  # Level end screen
?   ?   ??? GameOverScene.cs       # Game over screen
?   ?   ??? TwoPlayerGameOverScene.cs  # 2P game over
?   ?   ??? SettingsScene.cs       # Settings/options
?   ?   ??? AchievementScene.cs    # Achievements
?   ?   ??? PlayHistoryScene.cs    # Play history
?   ?   ??? CompendiumScene.cs     # Item/enemy guide
?   ?   ??? AboutUsScene.cs        # Credits
?   ?   ??? CreditsScene.cs        # End credits
?   ?   ??? InputTestScene.cs      # Input debugging
?   ?   ??? GameFinishScene.cs     # Game end
?   ?   ??? PlayerNameInputScene.cs    # Name entry
?   ?   ??? PlaceholderScene.cs    # Placeholder
?   ?
?   ??? _Data/                     # ?? Data management
?   ?   ??? MapLoader.cs           # Level JSON loader
?   ?   ??? SaveSlotManager.cs     # Save slot handling
?   ?   ??? DataManager.cs         # General data manager
?   ?   ??? models/                # Data models
?   ?       ??? SaveData.cs        # Save data format
?   ?       ??? SaveSlot.cs        # Individual save slot
?   ?       ??? LevelMapData.cs    # Level structure
?   ?       ??? LevelStats.cs      # Per-level stats
?   ?       ??? GameRecord.cs      # Game records
?   ?       ??? PlayerStateData.cs # Player state
?   ?       ??? Achievement.cs     # Achievement data
?   ?
?   ??? _UI/                       # ?? UI components
?   ?   ??? Button.cs              # UI button
?   ?
?   ??? _Utils/                    # ?? Utilities
?       ??? Collision.cs           # Collision detection
?       ??? SpriteAnimation.cs     # Animation system
?
??? Content/                       # ?? Game assets
?   ??? sprites/                   # ??? Sprite images
?   ?   ??? mario.png              # Mario sprite sheet
?   ?   ??? goomba.png             # Goomba
?   ?   ??? koopa.png              # Koopa
?   ?   ??? plant.png              # Piranha plant
?   ?   ??? coin.png               # Coin
?   ?   ??? mushroom.png           # Mushroom
?   ?   ??? blockbreak.png         # Breakable block
?   ?   ??? groundTile.png         # Ground
?   ?   ??? pipe.png               # Pipe
?   ?   ??? present.png            # Mystery block
?   ?   ??? boss.png               # Boss sprite
?   ?   ??? bullet.png             # Bullet Bill
?   ?   ??? castle.png             # Castle
?   ?   ??? background.png         # Background
?   ?   ??? backgroundMenu.png     # Menu background
?   ?   ??? fireball.png           # Fireball projectile
?   ?
?   ??? audio/                     # ?? Sound & Music
?   ?   ??? titleMusic.xnb         # Menu music
?   ?   ??? levelMusic.xnb         # Gameplay music
?   ?   ??? jumpSound.wav          # Jump sound
?   ?   ??? coinSound.wav          # Coin pickup
?   ?   ??? ...
?   ?
?   ??? fonts/                     # ?? Fonts
?   ?   ??? GameFont.xnb           # Main game font
?   ?   ??? UIFont.xnb             # UI font
?   ?
?   ??? levels/                    # ??? Level data
?   ?   ??? level1.json            # Level 1 layout
?   ?   ??? level2.json            # Level 2 layout
?   ?   ??? ...
?   ?
?   ??? data/                      # ?? Game data
?       ??? gamesession.json       # Current session
?       ??? worlds_data.json       # Save slots
?
??? Game1.cs                       # Entry point
??? Program.cs                     # Main program
??? MarioGame.csproj               # Project file

AppData/MarioGame/                # User data (persistent)
??? careerstats.json              # Career statistics
??? careerstats.json.tmp          # Temporary file
```

---

## ??? Layered Architecture

MarioGame s? d?ng **4-layer architecture**:

```
???????????????????????????????????????????
? PRESENTATION LAYER                      ?
? (UI & Rendering)                        ?
???????????????????????????????????????????
? • MenuScene, GameplayScene, etc.        ?
? • Button, GameHUD                       ?
? • Draw() methods                        ?
???????????????????????????????????????????
                   ? depends on
                   v
???????????????????????????????????????????
? GAME LOGIC LAYER                        ?
? (Game mechanics)                        ?
???????????????????????????????????????????
? • Player, Enemy, Item logic             ?
? • Collision detection & resolution      ?
? • Camera strategies                     ?
? • State management                      ?
???????????????????????????????????????????
                   ? depends on
                   v
???????????????????????????????????????????
? CORE LAYER                              ?
? (Infrastructure)                        ?
???????????????????????????????????????????
? • GameManager (scene switching)         ?
? • SoundManager (audio)                  ?
? • InputHandler (input processing)       ?
? • Camera (viewport management)          ?
???????????????????????????????????????????
                   ? depends on
                   v
???????????????????????????????????????????
? DATA LAYER                              ?
? (Persistence)                           ?
???????????????????????????????????????????
? • MapLoader (JSON parsing)              ?
? • SaveSlotManager (save/load)           ?
? • CareerStats (persistent data)         ?
? • GameSession (runtime data)            ?
? • JSON serialization                    ?
???????????????????????????????????????????
```

---

## ?? Dependency Injection Overview

```csharp
// Constructor Injection (Dependencies passed in)
public class Player : MovableObj
{
    private Dictionary<string, SpriteAnimation> _animations;
    
    public Player(
        Vector2 startPos,
        Dictionary<string, SpriteAnimation> animations,  // Injected
        int playerIndex = 1
    )
    {
        Position = startPos;
        _animations = animations;  // Dependency resolved externally
    }
}

// Usage (in GameplayScene)
Dictionary<string, SpriteAnimation> playerAnims = new();
playerAnims.Add("Run", new SpriteAnimation(...));
playerAnims.Add("Idle", new SpriteAnimation(...));
playerAnims.Add("Jump", new SpriteAnimation(...));

Player player = new Player(new Vector2(50, 200), playerAnims, 1);
```

---

## ?? Singleton Pattern Distribution

```csharp
// Controlled Singletons (legitimate use cases)
GameManager.Instance              // Scene management
SoundManager.Instance             // Audio system
InputSettings.Instance            // Input configuration
GameSession.Instance              // Runtime session stats
CareerStats                        // Static stats class

// Accessed globally via
GameManager.Instance.ChangeScene(...);
SoundManager.Instance.PlayMusic(...);
InputSettings.Instance.RemapKey(...);
```

---

## ?? Design Pattern Map

```
???????????????????????????????????????????????????????
? STRUCTURAL PATTERNS                                 ?
???????????????????????????????????????????????????????
? Adapter                                             ?
? ?? InputFrame (converts KeyboardState ? InputFrame) ?
?                                                     ?
? Decorator                                           ?
? ?? IsInvincible (adds temporary immunity)          ?
???????????????????????????????????????????????????????
? BEHAVIORAL PATTERNS                                 ?
???????????????????????????????????????????????????????
? State Pattern                                       ?
? ?? IMarioState (SmallState, BigState)              ?
?                                                     ?
? Strategy Pattern                                    ?
? ?? ICameraStrategy (FollowTarget, AutoScroll)      ?
?                                                     ?
? Template Method                                     ?
? ?? IScene (LoadContent, Update, Draw)              ?
?                                                     ?
? Observer Pattern (Implicit)                         ?
? ?? Game state reactions                            ?
?                                                     ?
? Command Pattern (Potential)                         ?
? ?? Undo/Redo system                                ?
???????????????????????????????????????????????????????
? CREATIONAL PATTERNS                                 ?
???????????????????????????????????????????????????????
? Factory Pattern                                     ?
? ?? EnemyFactory, MapLoader.CreateObjectFromCode()  ?
?                                                     ?
? Singleton Pattern                                   ?
? ?? GameManager, SoundManager, InputSettings        ?
???????????????????????????????????????????????????????
```

---

## ?? Control Flow Diagram

```
Game Initialization:
?? Program.Main()
?  ?? Game1 instance created
?     ?? Initialize()
?        ?? Graphics setup (1280×720)
?
LoadContent():
?? Create white pixel
?? Initialize GameManager
?? Load initial assets
?? Force CareerStats load
?? Change to MenuScene
?
Main Game Loop (repeated):
?? Update(GameTime)
?  ?? GameManager.Update()
?     ?? CurrentScene.Update(gameTime)
?        ?? Process game logic
?           ?? Player input
?           ?? Physics
?           ?? Collisions
?           ?? Animations
?
?? Draw(GameTime)
?  ?? GraphicsDevice.Clear()
?  ?? GameManager.Draw(spriteBatch)
?     ?? CurrentScene.Draw(spriteBatch)
?        ?? Render sprites
?? base.Update/Draw()
```

---

## ?? Namespace Organization

```csharp
// Core infrastructure
using MarioGame.src._Core;           // GameManager, SoundManager, Camera

// Entity system
using MarioGame.src._Entities.Base;  // GameObj, MovableObj
using MarioGame.src._Entities.player;    // Player
using MarioGame.src._Entities.enemies;   // Enemy types
using MarioGame.src._Entities.items;     // Item types

// Input handling
using MarioGame.src._Input;          // InputHandler, InputSettings

// Scenes
using MarioGame.src._Scenes;         // IScene, specific scenes
using MarioGame._Scenes;             // Alternative import

// Data management
using MarioGame.src._Data;           // MapLoader, SaveSlotManager
using MarioGame.src._Data.models;    // Data models

// Utilities
using MarioGame.src._Utils;          // Collision, SpriteAnimation

// UI
using MarioGame.src._UI;             // Button

// MonoGame
using Microsoft.Xna.Framework;       // Vector2, Rectangle, Color, etc.
using Microsoft.Xna.Framework.Graphics;  // SpriteBatch, Texture2D
using Microsoft.Xna.Framework.Input;    // KeyboardState, Keys
```

---

## ?? Class Responsibilities Matrix

| Class | Responsibility | Dependencies | Pattern |
|-------|---------------|----|---------|
| GameManager | Scene switching, state saving | CurrentScene | Singleton |
| SoundManager | Audio playback | Content, MediaPlayer | Singleton |
| InputHandler | Read keyboard input | KeyboardState | Adapter |
| InputSettings | Store key mappings | KeyboardState | Singleton |
| MapLoader | Load level JSON | File I/O, Texture2D | Factory |
| Player | Movement, states, combat | IMarioState, InputHandler | State |
| Enemy | AI, patrol, attack | Collision, physics | Template |
| Goomba | Walk animation, patrol | Enemy | Concrete |
| Camera | Viewport management, following | ICameraStrategy | Strategy |
| Collision | Physics resolution | Rectangle math | Utility |
| SpriteAnimation | Frame animation | Texture2D | Utility |
| SaveSlotManager | Persist game data | JSON serialization | Manager |
| CareerStats | Long-term statistics | File I/O | Singleton |
| GameSession | Session statistics | Scoring logic | Singleton |

---

## ?? Coding Standards

### Naming Conventions
```csharp
// Classes: PascalCase
public class GameManager { }
public class PlayerState { }

// Methods: PascalCase
public void UpdatePosition() { }
public void Draw(SpriteBatch sb) { }

// Properties: PascalCase
public Vector2 Position { get; set; }
public bool IsActive { get; set; }

// Local variables & parameters: camelCase
float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
int playerIndex = 1;

// Constants: UPPER_SNAKE_CASE
public const float GRAVITY = 0.6f;
public const int SCREEN_WIDTH = 1280;

// Private fields: _camelCaseWithUnderscore
private Vector2 _velocity;
private SpriteAnimation _currentAnimation;

// Interfaces: IPascalCase
public interface IScene { }
public interface ICameraStrategy { }
```

### Comment Standards
```csharp
// Single line comments for simple clarification
bool isOnGround = false;

/// <summary>
/// XML documentation for public methods
/// </summary>
public void Jump()
{
    // Implementation
}

// Explain WHY, not WHAT
// Khi to ra, hitbox dài xu?ng d??i làm Mario b? k?t vào ??t.
// Ta c?n kéo Mario lên trên m?t ?o?n b?ng ph?n chi?u cao t?ng thêm.
player.Position.Y -= (baseHeight * 0.5f);

// Mark known issues
// TODO: Optimize collision check - O(n²) too slow
// BUG: Camera shake when paused
// HACK: Workaround for animation timing
```

### Access Modifiers
```csharp
public class Player
{
    // Public API - documented and stable
    public Vector2 Position { get; set; }
    public void Jump() { }
    
    // Protected - for subclasses only (rarely used)
    protected void ApplyGravity() { }
    
    // Internal - within project only
    internal void DebugDrawBounds() { }
    
    // Private - implementation details
    private Vector2 _velocity;
    private void UpdateAnimation() { }
}
```

---

## ?? Assembly Organization

### Current Target
```
.NET Version: .NET 8
C# Version:   C# 12.0
Framework:    MonoGame 3.8.4
```

### Key Dependencies
```xml
<ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.1" />
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
</ItemGroup>
```

---

## ?? Testing Structure

### Unit Test Considerations
```csharp
// Testable design
public class Collision
{
    // Pure function - easy to test
    public static bool IsTopCollision(
        MovableObj topObj,      // Input
        GameObj bottomObj       // Input
    ) : bool                    // Output
    {
        // No side effects
    }
}

public class InputSettings
{
    // Singleton but testable - can reset state
    public void RemapKey(int playerIndex, EGameAction action, Keys newKey)
    {
        // Configuration logic isolated
    }
}
```

### MockING Strategy
```csharp
// Can mock IScene implementations
public class TestScene : IScene
{
    public void LoadContent() { /* empty */ }
    public void Update(GameTime gt) { /* empty */ }
    public void Draw(SpriteBatch sb) { /* empty */ }
}

// Can inject mock animations
var mockAnims = new Dictionary<string, SpriteAnimation>();
var player = new Player(Vector2.Zero, mockAnims, 1);
```

---

## ?? Performance Considerations

### Optimization Areas
```csharp
// Collision: Currently O(n²)
// Potential: Quadtree spatial partitioning
for (int i = 0; i < _gameObjects.Count; i++)
{
    for (int j = i + 1; j < _gameObjects.Count; j++)
    {
        // Check collision - can be optimized
    }
}

// Object pooling for projectiles
private ObjectPool<Fireball> _fireballPool;
Fireball fireball = _fireballPool.Get();
// ...use fireball
_fireballPool.Return(fireball);

// Asset caching in MapLoader
private static Dictionary<string, Texture2D> _textureCache;
```

---

## ?? Scalability Strategy

### Adding New Features

**New Enemy Type**:
```csharp
// 1. Create class inheriting Enemy
public class Piranha : Enemy { }

// 2. Add case to EnemyFactory
case 'P': return new Piranha(texture, position);

// 3. Add ký t? to level JSON
// No changes needed in game logic!
```

**New Camera Strategy**:
```csharp
// 1. Implement ICameraStrategy
public class PathFollowStrategy : ICameraStrategy { }

// 2. Set in GameplayScene
_camera.SetStrategy(new PathFollowStrategy());

// No other changes needed!
```

**New Scene**:
```csharp
// 1. Implement IScene
public class NewScene : IScene
{
    public void LoadContent() { }
    public void Update(GameTime gt) { }
    public void Draw(SpriteBatch sb) { }
}

// 2. Add to GameManager
GameManager.Instance.ChangeScene(new NewScene());
```

---

## ?? Data Safety

### File I/O Strategy
```csharp
// Atomic write with temp file
string temp = filePath + ".tmp";
File.WriteAllText(temp, data);
if (File.Exists(filePath)) File.Delete(filePath);
File.Move(temp, filePath);  // Atomic operation

// Backup before overwrite
if (File.Exists(filePath))
    File.Copy(filePath, filePath + ".backup");

// Error handling
try
{
    // Load/Save logic
}
catch (Exception ex)
{
    Debug.WriteLine($"[ERROR] {ex.Message}");
    // Graceful fallback
}
```

---

## ?? Code Maintenance Guide

### Regular Tasks
1. **Code Review**: Check for consistency
2. **Testing**: Run build verification
3. **Documentation**: Keep comments updated
4. **Refactoring**: Remove code smells

### Common Refactorings
```csharp
// Extract magic numbers to constants
// ? if (velocity.Y > 0.6f) { }
// ? if (velocity.Y > Constants.GRAVITY) { }

// Extract method
// ? Long method with multiple responsibilities
// ? Break into smaller methods

// Replace conditionals with polymorphism
// ? if (state == SMALL) { } else if (state == BIG) { }
// ? CurrentState.HandleInput(this);
```

---

## ?? Summary

**MarioGame Architecture**:

? **Well-Organized**: Clear layer separation and folder structure

? **Design Patterns Applied**: SRP, State, Strategy, Factory, Singleton, Template Method

? **Maintainable**: Consistent naming, clear responsibilities, good comments

? **Extensible**: Easy to add new features without modifying core code

? **Testable**: Dependency injection, pure functions where possible

? **Scalable**: Support for multiple players, levels, and game modes

? **Robust**: Error handling, data persistence, atomic writes

**Code Quality**: Excellent adherence to SOLID principles and industry best practices.

