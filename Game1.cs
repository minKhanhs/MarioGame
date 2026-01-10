using MarioGame._Scenes;
using MarioGame.src._Core;
using MarioGame.src._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MarioGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Texture2D WhitePixel { get; private set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create white pixel for drawing borders and rectangles
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            // Cấu hình GameManager
            GameManager.Instance.Content = Content;
            GameManager.Instance.GraphicsDevice = GraphicsDevice;
            SoundManager.Instance.LoadSong("TitleTheme", "audio/titleMusic");

            // Force initialize CareerStats to load career file early (avoid timing issues)
            var _ = CareerStats.TotalCoins;

            // Khởi động vào Menu hoặc vào thẳng Level 1
            // GameManager.Instance.ChangeScene(new MenuScene()); 

            // Test vào thẳng Level 2 luôn
            GameManager.Instance.ChangeScene(new MenuScene());
        }

        protected override void Update(GameTime gameTime)
        {
            // GameManager lo update scene hiện tại
            GameManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // GameManager lo draw scene hiện tại
            GameManager.Instance.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}