using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.UI
{
    public class HUD
    {
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;

        public HUD(SpriteFont font, SpriteBatch spriteBatch)
        {
            _font = font;
            _spriteBatch = spriteBatch;
        }

        public void Draw(int score, int lives, int coins, int time)
        {
            // Draw HUD elements at the top of screen
            Vector2 position = new Vector2(10, 10);

            // Score
            DrawText($"SCORE: {score:D6}", position, Color.White);

            // Lives
            position.X = 250;
            DrawText($"LIVES: {lives}", position, Color.White);

            // Coins
            position.X = 450;
            DrawText($"COINS: {coins:D2}", position, Color.Gold);

            // Time
            position.X = 650;
            Color timeColor = time < 30 ? Color.Red : Color.White;
            DrawText($"TIME: {time}", position, timeColor);
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            if (_font != null)
            {
                _spriteBatch.DrawString(_font, text, position, color);
            }
        }
    }
}