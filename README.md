PROMPT CHO AI CODE GAME MARIO - OOP PROJECT
YÊU CẦU CHUNG
Tôi cần bạn code game Mario bằng C# MonoGame dựa trên thiết kế OOP trong file PDF đính kèm.
Yêu cầu:

Tuân thủ 100% thiết kế trong PDF (Class Diagram, cấu trúc, patterns)
Code đầy đủ, không viết TODO hay placeholder
Mỗi file riêng biệt, không gộp logic
Follow đúng Design Patterns: Singleton, State, Strategy, Factory
Implement đầy đủ 10 màn chơi theo mô tả trong PDF


CẤU TRÚC DỰ ÁN YÊU CẦU
MarioGame/
├── Core/
│   ├── Constants.cs          # Hằng số game, GRAVITY, PLAYER_SPEED, TILE_SIZE, etc
│   └── GameState.cs          # enum: StartMenu, Playing, Paused, GameOver, Victory
│
├── Managers/ (TẤT CẢ SINGLETON)
│   ├── GameManager.cs        # Quản lý game states, level loading, game loop logic
│   ├── SoundManager.cs       # Load/play sounds và music
│   ├── DataManager.cs        # Load/save JSON data (levels.json, savedata.json)
│   ├── InputManager.cs       # Xử lý input cho 2 players (P1: WASD, P2: Arrows)
│   ├── CameraManager.cs      # Quản lý camera chính
│   └── TextureManager.cs     # Load và quản lý sprites (nếu không có sprite thì tạo colored rectangles)
│
├── Entities/
│   ├── Base/
│   │   ├── GameObject.cs     # Abstract base: Position, Velocity, Size, Bounds, Update(), Draw()
│   │   ├── Entity.cs         # Kế thừa GameObject, thêm Health, IsDead
│   │   └── ICollidable.cs    # Interface: Rectangle Bounds, OnCollision()
│   │
│   ├── Player/
│   │   ├── Player.cs         # Class chính, kế thừa GameObject, có Lives, Score, Coins
│   │   ├── PlayerState.cs    # enum: Small, Big, Fire
│   │   └── PlayerController.cs  # Xử lý input riêng cho player
│   │
│   ├── Enemies/
│   │   ├── Enemy.cs          # Abstract base với FSM: EnemyState (Walking, Stunned, Dead)
│   │   ├── Goomba.cs         # Di chuyển thẳng, đổi hướng khi va tường
│   │   ├── Koopa.cs          # State: Walking → Shell → ShellSliding khi bị đá
│   │   └── PiranhaPlant.cs  # Lên xuống từ pipe, không lên nếu player gần
│   │
│   └── Items/
│       ├── Item.cs           # Abstract base
│       ├── Coin.cs           # +100 score, animation quay
│       ├── Mushroom.cs       # Power up lên Big Mario
│       ├── FireFlower.cs     # Power up lên Fire Mario, có thể bắn
│       └── Fireball.cs       # Projectile, bounce trên mặt đất, phá enemy
│
├── Systems/
│   ├── Physics/
│   │   ├── PhysicsEngine.cs      # Quản lý dynamic/static objects
│   │   └── CollisionDetector.cs  # AABB collision, GetCollisionSide(), ResolveCollision()
│   │
│   ├── Rendering/
│   │   ├── Camera.cs             # Position, ViewBounds, 3 modes
│   │   └── SpriteRenderer.cs     # Render sprites với animation support
│   │
│   └── Animation/
│       ├── Animation.cs          # List frames, Update(), CurrentFrame
│       └── AnimationManager.cs   # Quản lý nhiều animations, Play(), Update()
│
├── Level/
│   ├── Level.cs          # LevelId, Name, TimeLimit, TileMap, Players[], Enemies[], Items[]
│   ├── LevelLoader.cs    # Static: LoadLevel(LevelData), CreateTestLevel()
│   ├── Tile.cs           # TileType (Air, Ground, Brick, QuestionBlock, Pipe), IsSolid, OnHit()
│   └── TileMap.cs        # Tile[,], Width, Height, GetTile(), SetTile(), Draw()
│
├── UI/
│   ├── UIManager.cs      # Singleton, quản lý tất cả UI components
│   ├── HUD.cs            # Lives, Score, Time, Coins, World
│   └── Menu.cs           # Base class cho StartMenu, PauseMenu, LevelSelect, Settings, GameOver, Victory
│
├── Data/
│   └── Models/
│       ├── GameData.cs       # Root: Levels[], PlayerData, Settings, CurrentState, LevelHistory[]
│       ├── LevelData.cs      # LevelId, Name, TimeLimit, MapData, Entities[]
│       └── PlayerData.cs     # PlayerName, LastPlayed
│
├── Content/
│   ├── Sprites/          # mario_small.png, goomba.png, koopa.png, tiles.png, etc
│   ├── Sounds/           # jump.wav, stomp.wav, coin.wav, etc
│   ├── Music/            # theme_main.mp3, theme_boss.mp3
│   ├── Fonts/            # Arcade.spritefont
│   └── Data/
│       └── levels.json   # File JSON 10 màn chơi
│
├── Game1.cs              # Main game class: LoadContent(), Update(), Draw()
└── Program.cs            # Entry point

CHI TIẾT THIẾT KẾ THEO PDF
1. GAME LOOP (PDF Slide 9)

Input: Nhận tín hiệu bàn phím từ InputManager
Update: Cập nhật logic dựa trên DeltaTime (60 FPS)
Draw: Vẽ lại toàn bộ khung hình với SpriteBatch

2. VẬT LÝ (PDF Slide 10)
csharp// Constants.cs
public const float GRAVITY = 980f;
public const float MAX_FALL_SPEED = 500f;
public const float FRICTION = 0.85f;
public const float PLAYER_SPEED = 200f;
public const float PLAYER_JUMP_SPEED = -400f;
public const float GOOMBA_SPEED = 50f;
public const float KOOPA_SPEED = 60f;
public const float KOOPA_SHELL_SPEED = 300f;
3. VA CHẠM AABB (PDF Slide 11)
csharp// CollisionDetector.cs
public static bool CheckCollision(Rectangle rect1, Rectangle rect2)
{
    return rect1.Intersects(rect2);
}

public static CollisionSide GetCollisionSide(GameObject obj1, GameObject obj2)
{
    // Tính overlap cho 4 cạnh, trả về cạnh có overlap nhỏ nhất
    // Xét theo velocity để xác định hướng va chạm
}

public static void ResolveCollision(GameObject obj1, GameObject obj2, CollisionSide side)
{
    // Bottom: obj1 đứng trên obj2, set IsGrounded = true
    // Top: obj1 đập đầu, Velocity.Y = 0
    // Left/Right: chặn di chuyển, Velocity.X = 0
}
4. CAMERA (PDF Slide 12)
csharp// Camera.cs - Strategy Pattern
public enum CameraMode { FollowPlayer, AutoScroll, Fixed }

// FollowPlayerStrategy: 
// - targetX = player.Position.X - ViewWidth / 3
// - Smooth lerp
// - Không cho lùi về phía sau

// AutoScrollStrategy:
// - Camera tự động di chuyển với scrollSpeed
// - Nếu player bị bỏ lại phía sau → instant death

// FixedCameraStrategy:
// - Camera không di chuyển (boss room)
5. ENEMY AI - FSM (PDF Slide 13)
csharp// Enemy.cs
public enum EnemyState { Walking, Stunned, Dead }

protected virtual void UpdateState(float deltaTime)
{
    switch (State)
    {
        case Walking: 
            Velocity.X = moveDirection * moveSpeed;
            break;
        case Stunned: 
            stateTimer -= deltaTime;
            if (stateTimer <= 0) State = Dead;
            break;
    }
}

// Goomba.cs
// - Di chuyển thẳng
// - Gặp tường → đổi hướng (moveDirection *= -1)
// - OnStomped → Stunned (flatten) 0.5s → Dead

// Koopa.cs
// - State: Walking → Shell (first stomp) → ShellSliding (kick)
// - Shell timeout 5s → trở lại Walking
// - Shell sliding kills other enemies

// PiranhaPlant.cs
// - HasGravity = false
// - Move up/down từ pipe
// - Check khoảng cách player, nếu < 64px → không lên
6. 10 MÀN CHƠI (PDF Slide 14)
Level 1: Tutorial - Hướng dẫn di chuyển, nhảy
Level 2: Vật thể chặn, platforms, question blocks
Level 3: Xuất hiện Goomba
Level 4: Xuất hiện Koopa (stomp 2 lần: shell → kick)
Level 5: Mushroom power-up → Big Mario
Level 6: Piranha Plant từ pipes
Level 7: Fire Flower → bắn fireball
Level 8: Nhiều enemies hơn, rocket (bonus)
Level 9: AutoScroll camera mode
Level 10: Boss battle (Fixed camera)
7. PLAYER STATES (PDF Slide 31)
csharp// PlayerState.cs
public enum PlayerStateType { Small, Big, Fire }

// Player.cs
public void PowerUp(PlayerStateType newState)
{
    if (newState > CurrentState)
    {
        CurrentState = newState;
        if (newState != Small) Size = new Vector2(32, 64);
    }
}

public void TakeDamage()
{
    if (CurrentState == Small) Die();
    else CurrentState--; // Big→Small, Fire→Big
}

// Fire state: có thể shoot fireball (J key)
8. CONTROLS (PDF Slide 7, 23)
Player 1:
- A/D: Move Left/Right
- W: Jump
- J: Shoot (Fire Mario)
- Shift: Run (speed x1.5)

Player 2:
- Arrow Keys: Move
- Up: Jump  
- RCtrl: Shoot
- RShift: Run

Both:
- ESC: Pause
- Enter: Select (menus)
9. JSON DATA STRUCTURE (PDF Slide 19-20)
json{
  "Levels": [
    {
      "LevelId": 1,
      "Name": "World 1-1: Khoi Dau",
      "TimeLimit": 300,
      "Background": "bg_overworld.png",
      "Music": "theme_main.mp3",
      "CameraMode": "FollowPlayer",
      "MapData": {
        "Width": 200,
        "Height": 15,
        "TileMatrix": [0,0,1,1,2,3,...] // 0:Air, 1:Ground, 2:Brick, 3:QuestionBlock, 4:Pipe
      },
      "Entities": [
        {"Type": "Goomba", "X": 400, "Y": 300},
        {"Type": "Coin", "X": 450, "Y": 200}
      ]
    }
  ],
  "PlayerData": {
    "PlayerName": "MinhKhanh",
    "LastPlayed": "2025-01-02T14:30:00"
  },
  "Settings": {
    "IsMusicOn": true,
    "IsSfxOn": true,
    "MasterVolume": 0.8,
    "KeyBindings": {...}
  },
  "CurrentState": {
    "CurrentLevelId": 5,
    "LivesLeft": 2,
    "TotalScore": 12500,
    "CoinsCollected": 45,
    "PowerUpState": "FireMario"
  },
  "LevelHistory": [...]
}
10. DESIGN PATTERNS (PDF Slide 29-33)
Singleton (Slide 33f):

GameManager, SoundManager, DataManager, InputManager, CameraManager, TextureManager, UIManager
public static Instance { get; private set; }

State Pattern (Slide 31):

Player: Small/Big/Fire states
Enemy: Walking/Stunned/Dead
Koopa: Walking/Shell/ShellSliding

Strategy Pattern (Slide 32):

Camera: FollowPlayerStrategy, AutoScrollStrategy, FixedCameraStrategy
Thay đổi behavior runtime mà không sửa code

Factory Pattern (Slide 33e):
csharp// LevelLoader.SpawnEntities()
switch (entityData.Type) {
    case "Goomba": return new Goomba(position);
    case "Koopa": return new Koopa(position);
    case "Coin": return new Coin(position);
}
OOP Principles (Slide 29):

Encapsulation: private fields, public properties
Inheritance: GameObject → Enemy, Item, Player
Polymorphism: override Update(), OnCollision(), Draw()

SRP (Slide 30):

Mỗi class một trách nhiệm duy nhất
Player: điều khiển nhân vật
Enemy: hành vi quái vật
Camera: quản lý view
InputHandler: xử lý input


YÊU CẦU KỸ THUẬT
MonoGame Setup
csharp// Game1.cs
public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    // Screen: 1280x720
    // Fixed timestep: 60 FPS
    // Tile size: 32x32 pixels
}
Content Pipeline
Content/
├── Sprites/ (PNG, 32x32 hoặc 32x64)
├── Sounds/ (WAV)
├── Music/ (MP3)
├── Fonts/ (SpriteFont XML)
└── Data/levels.json (Copy to output)
Texture Loading
csharp// TextureManager.cs
// - Nếu có sprite file → load từ Content
// - Nếu không có → tạo colored rectangle placeholder:
//   * Mario: Red
//   * Goomba: Brown
//   * Koopa: Green
//   * Coin: Gold
//   * Ground: Brown
//   * Brick: Orange
Rendering
csharpspriteBatch.Begin(
    samplerState: SamplerState.PointClamp, // Pixel-perfect
    sortMode: SpriteSortMode.Deferred
);

CHECKLIST BẮT BUỘC
Core Features

 GameObject base class với Position, Velocity, Bounds, Update(), Draw()
 AABB collision detection với GetCollisionSide()
 Physics: Gravity, friction, jumping
 Player: 3 states (Small/Big/Fire), Lives, Score, Coins
 Enemy FSM: Goomba, Koopa (3 states), PiranhaPlant
 Items: Coin, Mushroom, FireFlower, Fireball projectile
 TileMap: Air, Ground, Brick, QuestionBlock, Pipe
 Camera: 3 modes (Follow/AutoScroll/Fixed)
 10 levels với độ khó tăng dần

Managers (All Singleton)

 GameManager: states, level loading, game loop
 SoundManager: load/play sounds & music
 DataManager: load/save JSON
 InputManager: 2 players input
 CameraManager: camera chính
 TextureManager: load sprites hoặc tạo placeholders

UI System

 HUD: Lives, Score, Time, Coins, World
 StartMenu: 1P/2P game, Level Select, Settings, Exit
 PauseMenu: Resume, Restart, Main Menu
 GameOver & Victory screens
 Settings: Music/SFX toggle, Volume

Data & Persistence

 Load levels từ JSON
 Save/Load game progress
 Settings persistence
 Level history tracking

Polish (Optional nhưng nên có)

 Animation system
 Particle effects (brick breaking, coins)
 Screen transitions
 Sound effects
 Background music


CÁCH TEST

Build & Run

bashdotnet build
dotnet run

Test từng level


Level 1: Tutorial, no enemies
Level 3: Test Goomba AI
Level 4: Test Koopa shell mechanics
Level 9: Test AutoScroll camera
Level 10: Test boss battle


Test 2 Players


Chọn "2 PLAYER GAME"
P1: WASD, P2: Arrows
Cả 2 cùng xuất hiện và chơi


Test Save/Load


Chơi vài màn, thoát game
Mở lại, check "Continue" có load đúng progress


LƯU Ý QUAN TRỌNG

KHÔNG VIẾT TODO: Mọi logic phải hoàn chỉnh
MỖI CLASS MỘT FILE: Không gộp nhiều class vào 1 file
FOLLOW CLASS DIAGRAM: Đúng cấu trúc trong PDF slides 15-18
10 LEVELS ĐẦY ĐỦ: Implement đủ 10 màn như slide 14
DESIGN PATTERNS: Phải có Singleton, State, Strategy, Factory
2 PLAYERS: Hỗ trợ co-op cục bộ
SPRITES OPTIONAL: Game chạy được với colored rectangles
JSON DATA: Load từ levels.json, save vào savedata.json


OUTPUT MONG MUỐN
Bạn hãy tạo cho tôi:

TẤT CẢ FILE .CS theo cấu trúc trên (35+ files)
File levels.json với 10 màn chơi đầy đủ
File README.md hướng dẫn setup và chạy
File Arcade.spritefont cho font

Mỗi file phải:

Đầy đủ code, không có TODO
Namespace đúng theo cấu trúc thư mục
Using statements đầy đủ
Comments giải thích logic quan trọng
Follow C# coding conventions


VÍ DỤ CODE MẪU
GameObject.cs (Base Class)
csharpusing Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Base
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; protected set; }
        public bool IsActive { get; set; } = true;
        public bool IsGrounded { get; set; }
        public bool HasGravity { get; protected set; } = true;
        public bool HasCollision { get; protected set; } = true;
        
        protected Texture2D texture;
        protected Rectangle sourceRect;
        public int Layer { get; protected set; }
        
        public Rectangle Bounds => new Rectangle(
            (int)Position.X, (int)Position.Y,
            (int)Size.X, (int)Size.Y
        );

        public GameObject(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
            Velocity = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Apply gravity
            if (HasGravity && !IsGrounded)
            {
                Velocity = new Vector2(Velocity.X, 
                    MathHelper.Min(Velocity.Y + Core.Constants.GRAVITY * deltaTime, 
                                   Core.Constants.MAX_FALL_SPEED));
            }
            
            // Apply friction
            if (IsGrounded)
            {
                Velocity = new Vector2(Velocity.X * Core.Constants.FRICTION, Velocity.Y);
            }
            
            // Update position
            Position += Velocity * deltaTime;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsActive || texture == null) return;
            
            Vector2 drawPosition = Position - cameraOffset;
            spriteBatch.Draw(texture, drawPosition, sourceRect, Color.White);
        }

        public virtual void OnCollision(GameObject other, CollisionSide side) { }
        public virtual void Destroy() { IsActive = false; }
    }
}

BẮT ĐẦU CODE NGAY BÂY GIỜ!
Tạo đầy đủ tất cả files theo cấu trúc trên. Mỗi file là một artifact riêng biệt.
Bắt đầu từ Core → Managers → Entities → Systems → Level → UI → Data → Game1.cs