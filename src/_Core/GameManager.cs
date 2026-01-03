using MarioGame.src._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        // Scene hiện tại đang chạy
        public IScene CurrentScene { get; private set; }

        // Để load content khi chuyển scene
        public Microsoft.Xna.Framework.Content.ContentManager Content { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        public void ChangeScene(IScene newScene)
        {
            CurrentScene = newScene;
            CurrentScene.LoadContent(); // Tự động load tài nguyên cho màn mới
        }

        public void Update(GameTime gameTime)
        {
            CurrentScene?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentScene?.Draw(spriteBatch);
        }
    }
}
