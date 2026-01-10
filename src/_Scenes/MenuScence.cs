using MarioGame._Scenes;
using MarioGame.src._Core;
using MarioGame.src._Data;
using MarioGame.src._Scenes;
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

            // Load background if available - use backgroundMenu
            try
            {
                _backgroundTex = content.Load<Texture2D>("sprites/backgroundMenu");
            }
            catch
            {
                try
                {
                    _backgroundTex = content.Load<Texture2D>("sprites/background");
                }
                catch
                {
                    _backgroundTex = null;
                }
            }

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _buttons = new List<Button>();

            // Button dimensions - retro style
            int buttonWidth = 200;
            int buttonHeight = 50;
            int spacing = 20;
            int hSpacing = 240; // Horizontal spacing for 2 columns

            // Center position for 2x4 grid
            int centerX = 640;
            int startX = centerX - hSpacing / 2 - buttonWidth / 2;
            int startY = 200;

            // Row 1: 1 Player, 2 Players
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
                "1 PLAYER",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(230, 0, 18),
                BorderColor = new Color(240, 240, 240),
                TextScale = 0.65f
            });

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY, buttonWidth, buttonHeight),
                "2 PLAYERS",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(67, 176, 71),
                BorderColor = new Color(240, 240, 240),
                TextScale = 0.65f
            });

            // Row 2: Settings, Achievements
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 1, buttonWidth, buttonHeight),
                "SETTINGS",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = Color.White,
                BorderColor = new Color(240, 240, 240),
                TextColor = Color.White,
                TextScale = 0.65f
            });

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY + (buttonHeight + spacing) * 1, buttonWidth, buttonHeight),
                "ACHIEVEMENTS",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(251, 208, 0),
                BorderColor = new Color(240, 240, 240),
                TextColor = Color.White,
                TextScale = 0.6f
            });

            // Row 3: Play History, Compendium
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 2, buttonWidth, buttonHeight),
                "PLAY HISTORY",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(156, 39, 176),
                BorderColor = new Color(240, 240, 240),
                TextColor = Color.White,
                TextScale = 0.6f
            });

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY + (buttonHeight + spacing) * 2, buttonWidth, buttonHeight),
                "COMPENDIUM",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(180, 100, 180),
                BorderColor = new Color(240, 240, 240),
                TextColor = Color.White,
                TextScale = 0.65f
            });

            // Row 4: About Us, Credits
            _buttons.Add(new Button(
                new Rectangle(startX, startY + (buttonHeight + spacing) * 3, buttonWidth, buttonHeight),
                "ABOUT US",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(6, 147, 227),
                BorderColor = new Color(240, 240, 240),
                TextColor = Color.White,
                TextScale = 0.65f
            });

            _buttons.Add(new Button(
                new Rectangle(startX + hSpacing, startY + (buttonHeight + spacing) * 3, buttonWidth, buttonHeight),
                "CREDITS",
                _buttonFont
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = new Color(100, 150, 200),
                BorderColor = new Color(240, 240, 240),
                TextColor = Color.White,
                TextScale = 0.65f
            });

            // Help button (question mark) - top right corner
            _helpButton = new Button(
                new Rectangle(1280 - 80, 20, 60, 60),
                "?",
                _buttonFont
            )
            {
                BackgroundColor = new Color(251, 208, 0),
                HoverBackgroundColor = new Color(251, 208, 0),
                BorderColor = Color.Black,
                TextColor = Color.Black
            };
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
                SaveSlotManager.LoadSlots();
                GameManager.Instance.ChangeScene(new SaveSlotScene(false));
            }
            else if (_buttons[1].WasPressed) // 2 PLAYERS
            {
                GameManager.Instance.GameMode = 2;
                // Truyền true vì là 2 người
                GameManager.Instance.ChangeScene(new SaveSlotScene(true));
            }
            else if (_buttons[2].WasPressed) // SETTINGS
            {
                GameManager.Instance.ChangeScene(new SettingsScene());
            }
            else if (_buttons[3].WasPressed) // ACHIEVEMENTS
            {
                GameManager.Instance.ChangeScene(new AchievementScene());
            }
            else if (_buttons[4].WasPressed) // PLAY HISTORY
            {
                GameManager.Instance.ChangeScene(new PlayHistoryScene());
            }
            else if (_buttons[5].WasPressed) // COMPENDIUM
            {
                GameManager.Instance.ChangeScene(new CompendiumScene());
            }
            else if (_buttons[6].WasPressed) // ABOUT US
            {
                GameManager.Instance.ChangeScene(new AboutUsScene());
            }
            else if (_buttons[7].WasPressed) // CREDITS
            {
                GameManager.Instance.ChangeScene(new CreditsScene());
            }
            else if (_helpButton.WasPressed) // HELP
            {
                GameManager.Instance.ChangeScene(new PlaceholderScene("HELP & CONTROLS - Coming Soon"));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(new Color(16, 16, 16)); // #101010 dark background

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw background if available (with reduced opacity for overlay effect)
            if (_backgroundTex != null)
            {
                spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White * 0.3f);
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

            // Draw help button with custom styling
            _helpButton.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawTitle(SpriteBatch spriteBatch)
        {
            // SUPER text (white)
            string superText = "SUPER";
            Vector2 superSize = _titleFont.MeasureString(superText);
            spriteBatch.DrawString(_titleFont, superText,
                new Vector2(640 - superSize.X / 2, 50), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

            // MARIO BROS line (red MARIO + white BROS)
            string marioText = "MARIO";
            string brosText = "BROS";
            Vector2 marioSize = _titleFont.MeasureString(marioText);
            Vector2 brosSize = _titleFont.MeasureString(brosText);

            float totalWidth = (marioSize.X + 30 + brosSize.X) * 1.3f;
            float startX = 640 - totalWidth / 2;

            // MARIO in red
            spriteBatch.DrawString(_titleFont, marioText,
                new Vector2(startX, 120), new Color(230, 0, 18), 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            // BROS in white
            spriteBatch.DrawString(_titleFont, brosText,
                new Vector2(startX + (marioSize.X + 30) * 1.3f, 120), Color.White, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }
    }
}
