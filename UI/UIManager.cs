using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarioGame.Core;

namespace MarioGame.UI
{
    public class UIManager
    {
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;
        private HUD _hud;
        private Menu _mainMenu;
        private Texture2D _pixel;

        public UIManager(ContentManager content, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

            // Try to load font, create fallback if not available
            try
            {
                _font = content.Load<SpriteFont>("Fonts/GameFont");
            }
            catch
            {
                _font = null; // Will handle missing font gracefully
            }

            _hud = new HUD(_font, spriteBatch);
            _mainMenu = new Menu(_font, spriteBatch);

            // Create pixel texture for drawing rectangles
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            SetupMainMenu();
        }

        private void SetupMainMenu()
        {
            _mainMenu.Clear();
            _mainMenu.AddItem(new MenuItem("Start Game", () =>
            {
                GameManager.Instance.LoadLevel(1);
            }));
            _mainMenu.AddItem(new MenuItem("Continue", () =>
            {
                // Load saved game
                var gameData = Managers.DataManager.Instance.GetGameData();
                GameManager.Instance.LoadLevel(gameData.CurrentState.CurrentLevelId);
            }));
            _mainMenu.AddItem(new MenuItem("Settings", () =>
            {
                GameManager.Instance.ChangeState(GameState.Settings);
            }));
            _mainMenu.AddItem(new MenuItem("Exit", () =>
            {
                GameManager.Instance.Exit();
            }));
        }

        public void DrawHUD(int score, int lives, int coins, int time)
        {
            _hud.Draw(score, lives, coins, time);
        }

        public void DrawStartScreen()
        {
            // Draw background
            DrawRectangle(new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT),
                new Color(0, 0, 0, 200));

            // Draw title
            Vector2 titlePos = new Vector2(Constants.SCREEN_WIDTH / 2 - 100, 100);
            DrawCenteredText("SUPER MARIO", titlePos, Color.Red, 2.0f);

            // Draw subtitle
            Vector2 subtitlePos = new Vector2(Constants.SCREEN_WIDTH / 2, 180);
            DrawCenteredText("MonoGame Edition", subtitlePos, Color.White, 1.0f);

            // Draw menu
            _mainMenu.Draw(new Vector2(Constants.SCREEN_WIDTH / 2 - 100, 300), 50);

            // Instructions
            Vector2 instructPos = new Vector2(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT - 50);
            DrawCenteredText("Press ENTER to select", instructPos, Color.Gray, 0.8f);
        }

        public void DrawPauseScreen()
        {
            // Semi-transparent overlay
            DrawRectangle(new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT),
                new Color(0, 0, 0, 150));

            Vector2 position = new Vector2(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2);
            DrawCenteredText("PAUSED", position, Color.Yellow, 2.0f);

            position.Y += 60;
            DrawCenteredText("Press ESC to resume", position, Color.White, 1.0f);
        }

        public void DrawLevelComplete()
        {
            DrawRectangle(new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT),
                new Color(0, 0, 0, 180));

            Vector2 position = new Vector2(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2 - 50);
            DrawCenteredText("LEVEL COMPLETE!", position, Color.Green, 2.0f);

            position.Y += 80;
            DrawCenteredText($"Score: {GameManager.Instance.Score}", position, Color.White, 1.2f);

            position.Y += 60;
            DrawCenteredText("Press ENTER to continue", position, Color.Gray, 1.0f);
        }

        public void DrawGameOver()
        {
            DrawRectangle(new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT),
                new Color(0, 0, 0, 200));

            Vector2 position = new Vector2(Constants.SCREEN_WIDTH / 2, Constants.SCREEN_HEIGHT / 2 - 50);
            DrawCenteredText("GAME OVER", position, Color.Red, 2.5f);

            position.Y += 80;
            DrawCenteredText($"Final Score: {GameManager.Instance.Score}", position, Color.White, 1.2f);

            position.Y += 60;
            DrawCenteredText("Press ENTER to restart", position, Color.Gray, 1.0f);
        }

        private void DrawCenteredText(string text, Vector2 position, Color color, float scale)
        {
            if (_font != null)
            {
                Vector2 textSize = _font.MeasureString(text) * scale;
                Vector2 textPosition = new Vector2(position.X - textSize.X / 2, position.Y);
                _spriteBatch.DrawString(_font, text, textPosition, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawRectangle(Rectangle rect, Color color)
        {
            _spriteBatch.Draw(_pixel, rect, color);
        }

        public void HandleMenuInput()
        {
            if (Managers.InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                _mainMenu.MoveUp();
            }
            else if (Managers.InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                _mainMenu.MoveDown();
            }
            else if (Managers.InputManager.Instance.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                _mainMenu.SelectCurrent();
            }
        }
    }
}