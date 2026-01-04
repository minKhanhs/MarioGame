using MarioGame._Scenes;
using MarioGame.src._Core;
using MarioGame.src._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MarioGame.src._Scenes
{
    public class PlaceholderScene : IScene
    {
        private SpriteFont _font;
        private string _title;

        public PlaceholderScene(string title = "Coming Soon")
        {
            _title = title;
        }

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            try
            {
                _font = content.Load<SpriteFont>("fonts/Arial");
            }
            catch
            {
                // If font doesn't exist, it will be null and we'll handle it in Draw
            }
        }

        public void Update(GameTime gameTime)
        {
            // Press Escape or any key to go back to menu
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin();

            if (_font != null)
            {
                Vector2 titleSize = _font.MeasureString(_title);
                spriteBatch.DrawString(_font, _title,
                    new Vector2(640 - titleSize.X / 2, 200), Color.White);

                string backText = "Press ESC to go back";
                Vector2 backSize = _font.MeasureString(backText);
                spriteBatch.DrawString(_font, backText,
                    new Vector2(640 - backSize.X / 2, 600), Color.Gray);
            }

            spriteBatch.End();
        }
    }
}
