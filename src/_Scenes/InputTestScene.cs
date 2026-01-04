using MarioGame.src._Core;
using MarioGame.src._Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MarioGame.src._Scenes
{
    public class InputTestScene : IScene
    {
        private SpriteFont _font;
        private InputHandler _inputHandler1;
        private InputHandler _inputHandler2;

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            try
            {
                _font = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch { }

            _inputHandler1 = new InputHandler();
            _inputHandler2 = new InputHandler();
        }

        public void Update(GameTime gameTime)
        {
            // Just for testing
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin();

            if (_font != null)
            {
                // Get inputs
                var input1 = _inputHandler1.GetInput(PlayerIndex.One);
                var input2 = _inputHandler2.GetInput(PlayerIndex.Two);

                int y = 50;
                spriteBatch.DrawString(_font, "=== INPUT TEST ===", new Vector2(100, y), Color.Gold);
                y += 50;

                // P1 Info
                spriteBatch.DrawString(_font, "PLAYER 1 (WASD):", new Vector2(100, y), Color.Cyan);
                y += 40;
                spriteBatch.DrawString(_font, $"MoveLeft (A): {InputSettings.Instance.P1_KeyMap[EGameAction.MoveLeft]}", new Vector2(120, y), Color.White);
                y += 35;
                spriteBatch.DrawString(_font, $"MoveRight (D): {InputSettings.Instance.P1_KeyMap[EGameAction.MoveRight]}", new Vector2(120, y), Color.White);
                y += 35;
                spriteBatch.DrawString(_font, $"Jump (W): {InputSettings.Instance.P1_KeyMap[EGameAction.Jump]}", new Vector2(120, y), Color.White);
                y += 35;
                spriteBatch.DrawString(_font, $"X_Axis: {input1.X_Axis}", new Vector2(120, y), Color.Yellow);
                y += 35;
                spriteBatch.DrawString(_font, $"IsJumpPressed: {input1.IsJumpPressed}", new Vector2(120, y), Color.Yellow);
                y += 50;

                // P2 Info
                spriteBatch.DrawString(_font, "PLAYER 2 (ARROW KEYS):", new Vector2(100, y), Color.Cyan);
                y += 40;
                spriteBatch.DrawString(_font, $"MoveLeft (Left): {InputSettings.Instance.P2_KeyMap[EGameAction.MoveLeft]}", new Vector2(120, y), Color.White);
                y += 35;
                spriteBatch.DrawString(_font, $"MoveRight (Right): {InputSettings.Instance.P2_KeyMap[EGameAction.MoveRight]}", new Vector2(120, y), Color.White);
                y += 35;
                spriteBatch.DrawString(_font, $"Jump (Up): {InputSettings.Instance.P2_KeyMap[EGameAction.Jump]}", new Vector2(120, y), Color.White);
                y += 35;
                spriteBatch.DrawString(_font, $"X_Axis: {input2.X_Axis}", new Vector2(120, y), Color.Yellow);
                y += 35;
                spriteBatch.DrawString(_font, $"IsJumpPressed: {input2.IsJumpPressed}", new Vector2(120, y), Color.Yellow);

                y += 50;
                spriteBatch.DrawString(_font, "Press any key to test input detection", new Vector2(100, y), Color.Gray);
            }

            spriteBatch.End();
        }
    }
}
