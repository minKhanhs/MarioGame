# MarioGame - Design Patterns Implementation Report

## ?? T?ng Quan

D? án MarioGame ???c thi?t k? theo các nguyên t?c SOLID và s? d?ng nhi?u Design Patterns hi?n ??i ?? ??t ???c:
- **Tính m? r?ng**: D? thêm tính n?ng m?i
- **Tính b?o trì**: Code d? hi?u và s?a ??i
- **Tính tái s? d?ng**: Compnent có th? ???c s? d?ng ? nhi?u n?i
- **Tính ki?m th?**: Các ph?n tách bi?t, d? test

---

## 1. SINGLE RESPONSIBILITY PRINCIPLE (SRP) ?

M?i class có **?úng m?t lý do ?? thay ??i**.

### ?? Core Management Classes
```
GameManager          ? Qu?n lý chuy?n ??i Scene ONLY
SoundManager         ? X? lý nh?c và âm thanh ONLY
InputHandler         ? ??c input t? bàn phím ONLY
InputSettings        ? L?u tr? c?u hình phím ONLY
MapLoader            ? Load và parse JSON level ONLY
```

### ?? Scene Classes
M?i Scene ch?u trách nhi?m duy nh?t:
```
IScene (Interface)
??? MenuScene              ? Hi?n th? menu chính
??? GameplayScene          ? Qu?n lý logic game
??? GameplayScence (Typo)  ? Alias cho GameplayScene
??? PauseScene             ? Hi?n th? menu pause
??? LevelCompleteScene     ? Hi?n th? k?t qu? level
??? GameOverScene          ? Hi?n th? game over
??? TwoPlayerGameOverScene ? Game over 2 ng??i ch?i
??? SaveSlotScene          ? Ch?n slot l?u trò ch?i
??? SettingsScene          ? C?u hình game
??? AchievementScene       ? Hi?n th? thành tích
??? PlayHistoryScene       ? L?ch s? ch?i
??? CompendiumScene        ? T? ?i?n v?t ph?m/quái
??? AboutUsScene           ? Thông tin team
??? CreditsScene           ? Credits game
??? PlaceholderScene       ? Placeholder
??? InputTestScene         ? Test input
```

### ?? Entity Classes
```
GameObj (Base)
??? MovableObj
?   ??? Player          ? Di chuy?n, qu?n lý tr?ng thái
?   ??? Enemy           ? Hành vi quái v?t
?   ?   ??? Goomba      ? AI ?i tu?n tra
?   ?   ??? Koopa       ? Rùa
?   ?   ??? PiranhaPlant ? Cây ?n th?t
?   ?   ??? Boss        ? Quái boss
?   ?   ??? BulletBill  ? ??n bay
?   ?   ??? BossProjectile ? ??n t? Boss
?   ??? Item            ? V?t ph?m
?   ?   ??? Coin        ? Ti?n xu
?   ?   ??? Mushroom    ? N?m
?   ?   ??? Fireball    ? Qu? l?a
?   ??? Pipe            ? ?ng d?n
?
??? Enviroments
    ??? Block           ? G?ch (static)
    ??? Castle          ? Lâu ?ài (?ích)
    ??? MysteryBlock    ? G?ch bí m?t
    ??? Pipe            ? ?ng d?n

Utilities:
??? Collision           ? X? lý va ch?m ONLY
??? SpriteAnimation     ? Logic animation ONLY
??? GameHUD             ? Hi?n th? HUD
```

### ?? Data Classes
```
CareerStats            ? Th?ng kê career (permanent)
GameSession            ? Th?ng kê phiên ch?i (temporary)
SaveSlotManager        ? Qu?n lý l?u trò ch?i
MapLoader              ? Load level t? JSON
DataManager            ? Qu?n lý d? li?u

Models:
??? SaveData            ? D? li?u l?u
??? SaveSlot            ? Slot l?u
??? GameRecord          ? B?n ghi trò ch?i
??? LevelMapData        ? D? li?u map
??? LevelStats          ? Th?ng kê level
??? PlayerStateData     ? Tr?ng thái player
??? Achievement         ? Thành tích
```

---

## 2. STATE PATTERN ??

**M?c ?ích**: Cho phép Player thay ??i hành vi tùy theo tr?ng thái (nh?, to, b?n l?a, sao may m?n).

### ?? V? trí: `src/_Entities/player/states/`

### ?? Interface
```csharp
public interface IMarioState
{
    void Enter(Player player);           // G?i khi vào tr?ng thái
    void HandleInput(Player player);     // X? lý input riêng
    void TakeDamage(Player player);      // X? lý khi b? ?ánh
}
```

### ?? Các Tr?ng Thái
```
IMarioState
??? SmallState
?   ??? Scale: 1.0f
?   ??? Hành ??ng: Không th? b?n
?   ??? B? ?ánh: Ch?t ngay
?
??? BigState
    ??? Scale: 1.5f
    ??? Hành ??ng: Có th? ??p g?ch
    ??? B? ?ánh: Chuy?n v? SmallState + B?t t? 2 giây
```

### ?? Cách S? D?ng
```csharp
// Chuy?n tr?ng thái
_player.SetState(new BigState());
_player.SetState(new SmallState());

// X? lý hành ??ng tùy tr?ng thái
_player.CurrentState.HandleInput(_player);
_player.CurrentState.TakeDamage(_player);
```

### ? L?i Ích
- ? D? thêm tr?ng thái m?i (FireFlowerState, SuperStarState)
- ? Logic tr?ng thái ???c tách bi?t rõ ràng
- ? Không c?n if/else dài trong Player class
- ? Chuy?n ??i tr?ng thái m??t mà, an toàn

---

## 3. STRATEGY PATTERN ??

**M?c ?ích**: Cho phép Camera s? d?ng các thu?t toán khác nhau ?? theo dõi Player.

### ?? V? trí: `src/_Core/Camera/`

### ?? Interface
```csharp
public interface ICameraStrategy
{
    Vector2 CalculatePosition(
        Vector2 currentPos, 
        Vector2 targetPos, 
        Rectangle viewport, 
        Rectangle mapBounds, 
        float dt
    );
}
```

### ?? Các Chi?n L??c
```
ICameraStrategy
??? FollowTargetStrategy
?   ??? Theo sát Player
?   ??? Có lerp ?? m??t
?   ??? B? gi?i h?n b?i biên map
?
??? AutoScrollStrategy
    ??? T? ??ng cu?n t? trái sang ph?i
    ??? T?c ?? c? ??nh (ScrollSpeed)
    ??? Dùng cho màn auto-scroll
```

### ?? Cách S? D?ng
```csharp
// ??t chi?n l??c camera
var strategy = new FollowTargetStrategy();
_camera.SetStrategy(strategy);

// C?p nh?t camera
_camera.Update(playerPos, mapBounds, gameTime);

// L?u/Khôi ph?c tr?ng thái camera
if (savedState.IsAutoScroll)
{
    var autoScroll = new AutoScrollStrategy();
    autoScroll.ScrollSpeed = MapLoader.CurrentLevelConfig?.ScrollSpeed ?? 110f;
    _camera.SetStrategy(autoScroll);
}
```

### ? L?i Ích
- ? D? thêm ki?u camera m?i (ZoomStrategy, PathFollowStrategy)
- ? Có th? ??i strategy runtime
- ? Code camera tách bi?t kh?i logic game

---

## 4. FACTORY PATTERN ??

**M?c ?ích**: T?p trung logic t?o ??i t??ng ? m?t n?i, d? m? r?ng.

### ?? V? trí: `src/_Entities/enemies/` và `src/_Data/`

### ?? Enemy Factory
```csharp
public static class EnemyFactory
{
    public static Enemy CreateEnemy(
        char typeCode, 
        Vector2 position, 
        Dictionary<string, Texture2D> textures
    )
    {
        switch (typeCode)
        {
            case 'E': return new Goomba(...);
            case 'K': return new Koopa(...);
            case 'P': return new PiranhaPlant(...);
            default: return null;
        }
    }
}
```

### ??? Map Object Factory (trong MapLoader)
```csharp
private static GameObj CreateObjectFromCode(char code, Vector2 pos)
{
    switch (code)
    {
        case '#': return new Block(...);           // G?ch
        case 'T': return new Pipe(...);            // ?ng
        case 'G': return new Block(...);           // ??t
        case 'C': return new Coin(...);            // Ti?n xu
        case 'M': return new Mushroom(...);        // N?m
        case '?': return new MysteryBlock(...);    // G?ch bí m?t
        case 'E': case 'K': case 'P':             // Quái v?t
            return EnemyFactory.CreateEnemy(code, pos, _textureMap);
        case 'Z': return new Castle(...);          // Lâu ?ài
        default: return null;
    }
}
```

### ?? Cách S? D?ng
```csharp
// Load level t? JSON
List<GameObj> objects = MapLoader.LoadLevel("level1.json");

// M?i ký t? trong b?n ?? ???c chuy?n thành ??i t??ng game
// Không c?n bi?t chi ti?t cách t?o t?ng lo?i
```

### ? L?i Ích
- ? Thêm lo?i quái v?t m?i ch? c?n 1 case trong factory
- ? MapLoader không c?n bi?t chi ti?t t?ng lo?i quái
- ? Code d? b?o trì và m? r?ng
- ? D? test factory logic riêng

---

## 5. SINGLETON PATTERN ??

**M?c ?ích**: ??m b?o ch? có 1 instance global c?a m?t class.

### ?? Các Singleton

```csharp
// GameManager - Qu?n lý Scene toàn c?c
public static GameManager Instance => _instance ??= new GameManager();

// SoundManager - Qu?n lý âm thanh toàn c?c
public static SoundManager Instance => _instance ??= new SoundManager();

// InputSettings - L?u c?u hình phím toàn c?c
public static InputSettings Instance => _instance ??= new InputSettings();

// GameSession - Th?ng kê phiên ch?i
public static GameSession Instance => _instance ??= new GameSession();

// CareerStats - Th?ng kê career (static class)
public static int TotalCoins { get; set; }
public static int TotalEnemiesDefeated { get; set; }
```

### ?? Cách S? D?ng
```csharp
// Truy c?p toàn c?c
GameManager.Instance.ChangeScene(new MenuScene());
SoundManager.Instance.PlayMusic("TitleTheme");
InputSettings.Instance.RemapKey(1, EGameAction.MoveLeft, Keys.A);
GameSession.Instance.AddLevelStats(100, 5, 3, 45f);
CareerStats.AddStats(50, 2);
```

### ? L?i Ích
- ? D? truy c?p t? b?t k? ?âu
- ? Thread-safe (dùng ??= operator)
- ? Không c?n truy?n tham s? qua hàm

### ?? L?u Ý
- ?? global state trong gi?i h?n (ch? cho manager)
- Test mocking d? h?n n?u dùng Dependency Injection

---

## 6. ADAPTER PATTERN ??

**M?c ?ích**: Chuy?n ??i interface c?a m?t class sang interface khác mà client mong ??i.

### ?? InputFrame Adapter
```csharp
public class InputFrame
{
    public float X_Axis { get; set; }          // -1, 0, 1
    public bool IsJumpPressed { get; set; }
    public bool IsRunPressed { get; set; }
    public bool IsAttackPressed { get; set; }
    public bool IsPausePressed { get; set; }
}

public class InputHandler
{
    public InputFrame GetInput(int playerIndex)
    {
        // Chuy?n ??i t? KeyboardState thành InputFrame
        // Code không ph?i bi?t v? Keyboard API
    }
}
```

### ? L?i Ích
- ? Tách bi?t logic game kh?i input API
- ? D? thay ??i input source (gamepad, touch, mouse)

---

## 7. TEMPLATE METHOD PATTERN ??

**M?c ?ích**: ??nh ngh?a skeleton c?a m?t thu?t toán trong base class.

### ?? IScene Interface
```csharp
public interface IScene
{
    void LoadContent();      // B??c 1: Load tài nguyên
    void Update(GameTime);   // B??c 2: Update logic
    void Draw(SpriteBatch);  // B??c 3: V?
}
```

### ?? Các Scene Implement Template
```csharp
public class MenuScene : IScene
{
    public void LoadContent() { /* Load fonts, buttons */ }
    public void Update(GameTime gt) { /* Update buttons */ }
    public void Draw(SpriteBatch sb) { /* Draw menu */ }
}

public class GameplayScene : IScene
{
    public void LoadContent() { /* Load level, assets */ }
    public void Update(GameTime gt) { /* Update game logic */ }
    public void Draw(SpriteBatch sb) { /* Draw game */ }
}
```

### ? L?i Ích
- ? T?t c? Scene tuân theo cùng chu k?
- ? D? hi?u flow chung

---

## 8. OBSERVER PATTERN ???

**M?c ?ích**: M?t ??i t??ng (Subject) thông báo cho nhi?u ??i t??ng khác (Observers) v? tr?ng thái.

### ?? Implicit Observable
```csharp
// GameplayScene quan sát tr?ng thái ??i t??ng
if (!obj.IsActive)
    _gameObjects.RemoveAt(i);  // Ph?n ?ng khi IsActive thay ??i

// Player quan sát Coin
if (obj is Item item && _player.Bounds.Intersects(item.Bounds))
{
    item.OnCollect(_player);   // Ph?n ?ng khi va ch?m
}

// HUD quan sát thay ??i score/coins
_hud.LivesRemaining = _player.Lives;
_hud.CoinsCollected = _player.Coins;
```

### ?? C?i Ti?n (C# Events)
```csharp
// Có th? dùng C# events cho cách ti?p c?n t??ng minh
public class GameEvents
{
    public static event Action<int> OnCoinsCollected;
    public static event Action OnEnemyDefeated;
    public static event Action OnLevelComplete;
}

// Subscribe
GameEvents.OnCoinsCollected += (coins) => 
{
    _hud.UpdateCoins(coins);
};

// Publish
GameEvents.OnCoinsCollected?.Invoke(5);
```

---

## 9. DECORATOR PATTERN ??

**M?c ?ích**: Thêm ch?c n?ng m?i vào ??i t??ng m?t cách ??ng.

### ?? IsInvincible Decorator
```csharp
public class Player : MovableObj
{
    public bool IsInvincible { get; set; }
    private float _invincibleTimer = 0f;
    
    public void StartInvincible()
    {
        IsInvincible = true;
        _invincibleTimer = 2.0f;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (IsInvincible)
        {
            _invincibleTimer -= dt;
            if (_invincibleTimer <= 0)
                IsInvincible = false;
        }
        
        // ... other logic
    }
}
```

### ? L?i Ích
- ? Thêm b?t t? t?m th?i mà không s?a core logic
- ? Có th? combine nhi?u effect

---

## 10. DEPENDENCY INJECTION PATTERN ??

**M?c ?ích**: Gi?m coupling b?ng inject dependency t? bên ngoài.

### ?? Constructor Injection
```csharp
public class Player : MovableObj
{
    private InputHandler _inputHandler;
    private Dictionary<string, SpriteAnimation> _animations;
    
    public Player(
        Vector2 startPos, 
        Dictionary<string, SpriteAnimation> animations,
        int playerIndex = 1
    )
    {
        Position = startPos;
        _animations = animations;  // Injected
        _inputHandler = new InputHandler();
    }
}

public class Enemy : MovableObj
{
    // Texture ???c inject qua constructor
    public Enemy(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;
    }
}
```

### ? L?i Ích
- ? D? test (inject mock objects)
- ? Gi?m coupling
- ? Tái s? d?ng code t?t h?n

---

## 11. COMMAND PATTERN ??

**M?c ?ích**: ?óng gói request nh? m?t object ?? queue, log ho?c undo.

### ?? Ti?m N?ng
```csharp
public interface IGameCommand
{
    void Execute();
    void Undo();
}

public class MoveCommand : IGameCommand
{
    private Vector2 _oldPos;
    private Vector2 _newPos;
    private Player _player;
    
    public void Execute() { _player.Position = _newPos; }
    public void Undo() { _player.Position = _oldPos; }
}

// Có th? dùng cho Undo/Redo
List<IGameCommand> _commandHistory = new();
```

---

## 12. COMPOSITE PATTERN ??

**M?c ?ích**: T?o c?u trúc cây t? ??i t??ng ??n gi?n.

### ?? Game Object Hierarchy
```
GameObj (Base)
??? MovableObj (Decorator cho dynamic objects)
?   ??? Player
?   ??? Enemy (Abstract)
?   ?   ??? Goomba
?   ?   ??? Koopa
?   ?   ??? ... (specific enemies)
?   ??? Item
?       ??? Coin
?       ??? Mushroom
?       ??? Fireball
?
??? Static Objects
    ??? Block
    ??? Pipe
    ??? Castle
```

### ? L?i Ích
- ? C?u trúc logic rõ ràng
- ? D? thêm lo?i ??i t??ng m?i

---

## 13. SUMMARY MATRIX ??

| Pattern | V? Trí | M?c ?ích | Hi?n T?i |
|---------|--------|----------|----------|
| **SRP** | T?t c? | M?i class 1 trách nhi?m | ? T?t |
| **State** | `player/states/` | Player states | ? T?t |
| **Strategy** | `Camera/` | Camera behavior | ? T?t |
| **Factory** | `enemies/`, `MapLoader` | T?o ??i t??ng | ? T?t |
| **Singleton** | `Manager/` | Qu?n lý toàn c?c | ? T?t |
| **Adapter** | `InputFrame` | Input abstraction | ? T?t |
| **Template** | `IScene` | Scene cycle | ? T?t |
| **Observer** | `Implicit` | Event handling | ?? C?n c?i ti?n |
| **Decorator** | `IsInvincible` | Feature wrapper | ? T?t |
| **Dependency Injection** | `Constructors` | Reduce coupling | ? T?t |
| **Command** | Ti?m n?ng | Undo/Redo | ? Ch?a implement |
| **Composite** | `GameObj hierarchy` | Object tree | ? T?t |

---

## ?? Khuy?n Ngh? C?i Ti?n

### 1. **Implement Event System Rõ Ràng** 
```csharp
public static class GameEvents
{
    public static event Action OnLevelComplete;
    public static event Action<int> OnCoinsCollected;
    public static event Action OnEnemyDefeated;
    public static event Action<int> OnPlayerHealthChanged;
}
```

### 2. **Extract Magic Numbers to Constants**
```csharp
public static class GameConstants
{
    public const float PLAYER_GRAVITY = 0.6f;
    public const float PLAYER_JUMP_POWER = 12f;
    public const int SCREEN_WIDTH = 1280;
    public const int SCREEN_HEIGHT = 720;
}
```

### 3. **Implement Command Pattern cho Undo/Redo**
```csharp
public interface ICommand
{
    void Execute();
    void Undo();
}

public class CommandManager
{
    private Stack<ICommand> _undoStack;
    private Stack<ICommand> _redoStack;
}
```

### 4. **Add Explicit Observer Pattern**
```csharp
public interface IObserver
{
    void Update(GameEvent @event);
}

public class EventManager
{
    private List<IObserver> _observers = new();
    
    public void Subscribe(IObserver observer) => _observers.Add(observer);
    public void Unsubscribe(IObserver observer) => _observers.Remove(observer);
    public void Notify(GameEvent @event) => _observers.ForEach(o => o.Update(@event));
}
```

### 5. **Implement Object Pool Pattern**
```csharp
public class ObjectPool<T> where T : class, new()
{
    private Stack<T> _available = new();
    private HashSet<T> _inUse = new();
    
    public T Get() => _available.Count > 0 ? _available.Pop() : new T();
    public void Return(T obj) => _available.Push(obj);
}
```

---

## ? Tóm T?t

MarioGame s? d?ng m?t b? Design Patterns ??y ?? và hi?u qu?:

? **?ã implement t?t**:
- Single Responsibility (SRP)
- State Pattern (Player states)
- Strategy Pattern (Camera)
- Factory Pattern (Enemies, Objects)
- Singleton Pattern (Managers)
- Template Method (Scenes)
- Adapter (Input handling)

?? **Có th? c?i ti?n**:
- Observer Pattern (dùng explicit events)
- More robust error handling
- Configuration management

? **Ch?a implement nh?ng có th? thêm**:
- Command Pattern (Undo/Redo)
- Object Pool (Performance)
- More Strategy patterns
- Asset loading cache

**K?t lu?n**: Code base tuân theo các nguyên t?c SOLID và Design Patterns m?t cách r?t t?t, making it scalable và maintainable cho future development.

