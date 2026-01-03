using MarioGame._Scenes;
using MarioGame.src._Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Scenes
{
    public class MenuScene : IScene
    {
        private Texture2D _logo; // Nếu có ảnh logo
        // private SpriteFont _font; // Nếu bạn chưa tạo font thì tạm thời chưa dùng

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            SoundManager.Instance.PlayMusic("TitleTheme");
            // Load ảnh Logo nếu bạn có (ví dụ: gameLogo.png trong thư mục Content)
            // _logo = content.Load<Texture2D>("gameLogo"); 
        }

        public void Update(GameTime gameTime)
        {
            // Kiểm tra phím Enter
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                // Chuyển sang Level 1
                GameManager.Instance.ChangeScene(new GameplayScene(1));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;

            // 1. Vẽ màn hình đen (hoặc màu nền menu)
            spriteBatch.Begin();
            device.Clear(Color.Black);

            // 2. Vẽ Logo hoặc Hướng dẫn (Nếu chưa có Font thì vẽ tạm hình Mario vào đây cho đỡ trống)
            // spriteBatch.Draw(_logo, new Vector2(400, 100), Color.White);

            // Tạm thời nếu chưa có Font chữ để hiện "Press Enter", 
            // bạn sẽ thấy màn hình đen xì. Hãy bấm Enter để vào game.

            spriteBatch.End();
        }
    }
}
