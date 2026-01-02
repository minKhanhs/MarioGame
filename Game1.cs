using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MarioGame.Core;
using MarioGame.Managers;

namespace MarioGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game loop timing
        private const double TARGET_FPS = 60.0;
        private const double TARGET_FRAME_TIME = 1.0 / TARGET_FPS;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size
            _graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            // Fixed time step for consistent physics
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(TARGET_FRAME_TIME);
        }

        protected override void Initialize()
        {
            // Initialize game manager
            GameManager.Instance.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: Load textures, sounds, fonts
            // Content.Load<Texture2D>("Sprites/mario_small");
            // Content.Load<Texture2D>("Sprites/goomba");
            // Content.Load<Texture2D>("Tiles/ground");
            // Content.Load<SpriteFont>("Fonts/arcade");
            // Content.Load<SoundEffect>("Sounds/jump");
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Q))
                Exit();

            // Update game manager
            GameManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(92, 148, 252)); // Mario sky blue

            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp, // Pixel-perfect rendering
                sortMode: SpriteSortMode.BackToFront
            );

            // Draw game
            GameManager.Instance.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, System.EventArgs args)
        {
            // Save game data
            // DataManager.Instance.SaveGameData();

            base.OnExiting(sender, args);
        }
    }
}

/*
 * HƯỚNG DẪN TIẾP THEO:
 * 
 * 1. TẠO CONTENT:
 *    - Tạo folder Content/Sprites và thêm sprite sheets
 *    - Tạo folder Content/Sounds và thêm âm thanh
 *    - Tạo folder Content/Fonts và thêm SpriteFont
 *    - Dùng MonoGame Content Builder (MGCB) để build content
 * 
 * 2. HOÀN THIỆN CÁC CLASS CÒN THIẾU:
 *    - Items/Coin.cs
 *    - Items/Mushroom.cs
 *    - Items/FireFlower.cs
 *    - Projectile/Fireball.cs
 *    - UI/HUD.cs
 *    - UI/Menu.cs
 *    - Animation/AnimationManager.cs
 *    - Data/LevelLoader.cs (load từ JSON)
 * 
 * 3. THÊM TÍNH NĂNG:
 *    - Animation system hoàn chỉnh
 *    - Particle effects (brick breaking, coin collection)
 *    - Sound effects và background music
 *    - Save/Load system với JSON
 *    - UI menus đẹp
 *    - Boss battles (level 10)
 * 
 * 4. TESTING & POLISH:
 *    - Test va chạm kỹ
 *    - Balance difficulty cho 10 màn
 *    - Add juice (screen shake, particles, sounds)
 *    - Performance optimization
 * 
 * 5. CẤU TRÚC DỮ LIỆU JSON:
 *    Xem slide 19-20 trong tài liệu để tạo file levels.json
 *    với cấu trúc:
 *    {
 *      "Levels": [...],
 *      "PlayerName": "...",
 *      "Settings": {...},
 *      "CurrentState": {...},
 *      "LevelHistory": [...]
 *    }
 */