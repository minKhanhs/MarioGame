using MarioGame._Scenes;
using MarioGame.src._Core;
using MarioGame.src._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class MenuScene : IScene
    {
        private Texture2D _backgroundTex;
        private SpriteFont _titleFont;
        private SpriteFont _buttonFont;
        private List<Button> _buttons;
        private Button _helpButton;

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            SoundManager.Instance.PlayMusic("TitleTheme");

            // Load fonts - use GameFont from Content
            try
            {
                _titleFont = content.Load<SpriteFont>("fonts/GameFont");
                _buttonFont = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch
            {
                _titleFont = null;
                _buttonFont = null;
            }

            // Load background if available
            try
            {
                _backgroundTex = content.Load<Texture2D>("sprites/background");
            }
            catch
            {
                _backgroundTex = null;
            }

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _buttons = new List<Button>();

            // Button dimensions - slightly smaller to fit 4 rows
            int buttonWidth = 230;
            int buttonHeight = 40;
            int spacing = 12;
            int hSpacing = 250; // Horizontal spacing for 2 columns

            // Center position for 2x4 grid
            int centerX = 640;
            int startX = centerX - hSpacing / 2;
            int startY = 300;

            // Row 1: 1 Player, 2 Players
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
                "1 PLAYER",
                _buttonFont
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY, buttonWidth, buttonHeight),
                "2 PLAYERS",
                _buttonFont
            ));

            // Row 2: Settings, Achievements
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 1, buttonWidth, buttonHeight),
                "SETTINGS",
                _buttonFont
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY + (buttonHeight + spacing) * 1, buttonWidth, buttonHeight),
                "ACHIEVEMENTS",
                _buttonFont
            ));

            // Row 3: About Us, Compendium
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 2, buttonWidth, buttonHeight),
                "ABOUT US",
                _buttonFont
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY + (buttonHeight + spacing) * 2, buttonWidth, buttonHeight),
                "COMPENDIUM",
                _buttonFont
            ));

            // Row 4: Play History, Credits
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 3, buttonWidth, buttonHeight),
                "PLAY HISTORY",
                _buttonFont
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY + (buttonHeight + spacing) * 3, buttonWidth, buttonHeight),
                "CREDITS",
                _buttonFont
            ));

            // Help button (question mark) - top right corner
            _helpButton = new Button(
                new Rectangle(1280 - 60, 20, 40, 40),
                "?",
                _buttonFont
            );
            _helpButton.TextColor = Color.Gold;
            _helpButton.BorderColor = Color.Gold;
            _helpButton.HoverColor = Color.Yellow;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var button in _buttons)
            {
                button.Update(gameTime);
            }
            _helpButton.Update(gameTime);

            // Handle button clicks
            if (_buttons[0].WasPressed) // 1 PLAYER
            {
                GameManager.Instance.ChangeScene(new GameplayScene(1));
            }
            else if (_buttons[1].WasPressed) // 2 PLAYERS
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("2 PLAYER MODE - Coming Soon"));
            }
            else if (_buttons[2].WasPressed) // SETTINGS
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("SETTINGS - Coming Soon"));
            }
            else if (_buttons[3].WasPressed) // ACHIEVEMENTS
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("ACHIEVEMENTS - Coming Soon"));
            }
            else if (_buttons[4].WasPressed) // ABOUT US
            {
                GameManager.Instance.ChangeScene(new AboutUsScene());
            }
            else if (_buttons[5].WasPressed) // COMPENDIUM
            {
                GameManager.Instance.ChangeScene(new CompendiumScene());
            }
            else if (_buttons[6].WasPressed) // PLAY HISTORY
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("PLAY HISTORY - Coming Soon"));
            }
            else if (_buttons[7].WasPressed) // CREDITS
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("CREDITS - Coming Soon"));
            }
            else if (_helpButton.WasPressed) // HELP
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("HELP & CONTROLS - Coming Soon"));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw background if available
            if (_backgroundTex != null)
            {
                spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);
            }

            // Draw title "SUPER MARIO BROS"
            if (_titleFont != null)
            {
                DrawTitle(spriteBatch);
            }

            // Draw all buttons
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }

            // Draw help button
            _helpButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawTitle(SpriteBatch spriteBatch)
        {
            // SUPER text (white, smaller)
            string superText = "SUPER";
            Vector2 superSize = _titleFont.MeasureString(superText);
            spriteBatch.DrawString(_titleFont, superText,
                new Vector2(640 - superSize.X / 2, 80), Color.White);

            // MARIO BROS line (red MARIO + white BROS)
            string marioText = "MARIO";
            string brosText = "BROS";
            Vector2 marioSize = _titleFont.MeasureString(marioText);
            Vector2 brosSize = _titleFont.MeasureString(brosText);

            float totalWidth = marioSize.X + 30 + brosSize.X; // 30px gap
            float startX = 640 - totalWidth / 2;

            // MARIO in red (slightly larger)
            spriteBatch.DrawString(_titleFont, marioText,
                new Vector2(startX, 135), Color.Red, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            // BROS in white (same size as MARIO)
            spriteBatch.DrawString(_titleFont, brosText,
                new Vector2(startX + (marioSize.X + 30) * 1.2f, 135), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        }
    }
}
