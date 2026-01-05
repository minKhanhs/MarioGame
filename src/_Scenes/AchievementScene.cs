using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using MarioGame._Scenes;
using MarioGame.src._Data.models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace MarioGame.src._Scenes
{
    public class AchievementScene : IScene
    {
        private SpriteFont _font;
        private Button _backButton;
        private List<Achievement> _achievements;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;
        private int _scrollOffset = 0;

        public void LoadContent()
        {
            if (_isContentLoaded)
                return;

            var content = GameManager.Instance.Content;

            try
            {
                _font = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch
            {
                _font = null;
            }

            _achievements = AchievementManager.Instance.GetAllAchievements();
            InitializeButtons();
            _isContentLoaded = true;
        }

        private void InitializeButtons()
        {
            _backButton = new Button(
                new Rectangle(640 - 70, 680, 140, 40),
                "BACK",
                _font
            )
            {
                BackgroundColor = new Color(230, 0, 18),
                HoverBackgroundColor = new Color(200, 0, 10),
                BorderColor = Color.Black,
                TextColor = Color.White,
                TextScale = 0.5f
            };
        }

        public void Update(GameTime gameTime)
        {
            _backButton.Update(gameTime);

            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Back button
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || _backButton.WasPressed)
            {
                GameManager.Instance.ChangeScene(new MenuScene());
                return;
            }

            // Scroll with arrow keys
            if (currentKeyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _scrollOffset -= 20;
                if (_scrollOffset < 0) _scrollOffset = 0;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _scrollOffset += 20;
                if (_scrollOffset > 300) _scrollOffset = 300;
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(new Color(18, 18, 18)); // Dark background

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_font != null)
            {
                // Header bar
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 80), new Color(230, 0, 18));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 76, 1280, 4), Color.Black);
                }

                // Title
                spriteBatch.DrawString(_font, "ACHIEVEMENTS", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, "JOURNEY TO RESCUE THE PRINCESS", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Progress section
                int unlockedCount = _achievements.Count(a => a.IsUnlocked);
                DrawProgressCards(spriteBatch, unlockedCount);

                // Achievements grid
                DrawAchievementsGrid(spriteBatch);

                // Footer
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }

                spriteBatch.DrawString(_font, "UP/DOWN: Scroll  |  ESC: Back",
                    new Vector2(400, 660), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            // Draw back button
            spriteBatch.Begin();
            _backButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawProgressCards(SpriteBatch spriteBatch, int unlockedCount)
        {
            int y = 100;
            int cardX = 50;
            int cardSpacing = 350;
            int cardWidth = 300;
            int cardHeight = 80;

            // Total Achievements card
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, cardWidth, cardHeight), new Color(36, 36, 36));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, cardWidth, 2), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y + cardHeight - 2, cardWidth, 2), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, 2, cardHeight), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX + cardWidth - 2, y, 2, cardHeight), new Color(64, 64, 64));
            }

            spriteBatch.DrawString(_font, "TOTAL", new Vector2(cardX + 15, y + 8), new Color(156, 163, 175), 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "ACHIEVEMENTS", new Vector2(cardX + 15, y + 20), new Color(156, 163, 175), 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, $"{unlockedCount}/{_achievements.Count}", new Vector2(cardX + 15, y + 40), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            // Get stats from GameSession
            var gameSession = GameSession.Instance;
            int totalCoins = gameSession?.TotalCoins ?? 0;
            int totalEnemies = gameSession?.TotalEnemiesDefeated ?? 0;

            // Coins card
            cardX += cardSpacing;
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, cardWidth, cardHeight), new Color(36, 36, 36));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, cardWidth, 2), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y + cardHeight - 2, cardWidth, 2), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, 2, cardHeight), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX + cardWidth - 2, y, 2, cardHeight), new Color(64, 64, 64));
            }

            spriteBatch.DrawString(_font, "TOTAL COINS", new Vector2(cardX + 15, y + 15), new Color(156, 163, 175), 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, totalCoins.ToString(), new Vector2(cardX + 15, y + 40), new Color(251, 208, 0), 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            // Enemies card
            cardX += cardSpacing;
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, cardWidth, cardHeight), new Color(36, 36, 36));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, cardWidth, 2), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y + cardHeight - 2, cardWidth, 2), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX, y, 2, cardHeight), new Color(64, 64, 64));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(cardX + cardWidth - 2, y, 2, cardHeight), new Color(64, 64, 64));
            }

            spriteBatch.DrawString(_font, "ENEMIES", new Vector2(cardX + 15, y + 8), new Color(156, 163, 175), 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "DEFEATED", new Vector2(cardX + 15, y + 20), new Color(156, 163, 175), 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, totalEnemies.ToString(), new Vector2(cardX + 15, y + 40), new Color(67, 176, 71), 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        private void DrawAchievementsGrid(SpriteBatch spriteBatch)
        {
            int startY = 210;
            int gridStartX = 50;
            int cardWidth = 370;
            int cardHeight = 100;
            int gridSpacing = 30;
            int cardsPerRow = 2;

            var clippingRect = new Rectangle(40, 210, 1200, 435);
            var previousScissor = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = clippingRect;
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: new RasterizerState { ScissorTestEnable = true });

            int currentY = startY - _scrollOffset;

            for (int i = 0; i < _achievements.Count; i++)
            {
                int row = i / cardsPerRow;
                int col = i % cardsPerRow;
                int x = gridStartX + col * (cardWidth + gridSpacing);
                int y = currentY + row * (cardHeight + gridSpacing);

                DrawAchievementCard(spriteBatch, _achievements[i], x, y, cardWidth, cardHeight);
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = previousScissor;
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }

        private void DrawAchievementCard(SpriteBatch spriteBatch, Achievement ach, int x, int y, int width, int height)
        {
            Color borderColor = ach.IsUnlocked ? new Color(230, 0, 18) : new Color(64, 64, 64);
            Color bgColor = new Color(36, 36, 36);

            if (Game1.WhitePixel != null)
            {
                // Card background
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, width, height), bgColor);

                // Card borders
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, width, 3), borderColor);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y + height - 3, width, 3), borderColor);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, 3, height), borderColor);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 3, y, 3, height), borderColor);
            }

            // Status badge
            if (ach.IsUnlocked && Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 55, y + 5, 50, 16), new Color(230, 0, 18));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 55, y + 5, 50, 2), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 55, y + 19, 50, 2), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 55, y + 5, 2, 16), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 5, y + 5, 2, 16), Color.Black);
                spriteBatch.DrawString(_font, "DONE", new Vector2(x + width - 52, y + 5), Color.White, 0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
            }

            // Icon placeholder
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 20, 40, 40), Color.Black);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 20, 40, 2), borderColor);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 58, 40, 2), borderColor);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 20, 2, 40), borderColor);
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 48, y + 20, 2, 40), borderColor);
            }

            // Title
            string title = ach.Name.Length > 16 ? ach.Name.Substring(0, 16) : ach.Name;
            spriteBatch.DrawString(_font, title, new Vector2(x + 60, y + 22), borderColor, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

            // Description - larger font
            string desc = ach.Description;
            if (desc.Length > 40)
            {
                desc = desc.Substring(0, 37) + "...";
            }
            spriteBatch.DrawString(_font, desc, new Vector2(x + 10, y + 62), new Color(200, 200, 200), 0f, Vector2.Zero, 0.32f, SpriteEffects.None, 0f);

            // Progress bar
            int barWidth = width - 20;
            if (!ach.IsUnlocked && HasProgressTowards(ach))
            {
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 8), Color.Black);
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 2), new Color(64, 64, 64));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 88, barWidth, 2), new Color(64, 64, 64));
                }

                int fillWidth = barWidth / 2;
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, fillWidth, 8), borderColor);
                }
            }
            else if (ach.IsUnlocked)
            {
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 8), Color.Black);
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 2), new Color(64, 64, 64));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 88, barWidth, 2), new Color(64, 64, 64));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 8), borderColor);
                }
            }
            else
            {
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 8), Color.Black);
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 82, barWidth, 2), new Color(64, 64, 64));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + 10, y + 88, barWidth, 2), new Color(64, 64, 64));
                }

                spriteBatch.DrawString(_font, "LOCKED", new Vector2(x + width / 2 - 20, y + 80), new Color(156, 163, 175), 0f, Vector2.Zero, 0.28f, SpriteEffects.None, 0f);
            }
        }

        private bool HasProgressTowards(Achievement ach)
        {
            return ach.Id.Contains("coin") || ach.Id.Contains("enemy") || ach.Id.Contains("score");
        }
    }
}
