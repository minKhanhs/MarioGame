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
    public class PlayHistoryScene : IScene
    {
        private SpriteFont _font;
        private Button _backButton;
        private List<GameRecord> _records;
        private int _currentPage = 0;
        private const int RECORDS_PER_PAGE = 8;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

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

            _records = GameRecordManager.Instance.GetTopScores(100);
            _records.Sort((a, b) => b.PlayDate.CompareTo(a.PlayDate)); // Sort by date descending

            InitializeButtons();
            _isContentLoaded = true;
        }

        private void InitializeButtons()
        {
            _backButton = new Button(
                new Rectangle(640 - 75, 650, 150, 40),
                "BACK",
                _font
            )
            {
                BackgroundColor = Color.Black,
                HoverBackgroundColor = new Color(230, 0, 18),
                BorderColor = Color.White,
                TextColor = Color.White,
                TextScale = 0.4f
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

            // Page navigation
            if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                if (_currentPage > 0) _currentPage--;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                int maxPages = (_records.Count + RECORDS_PER_PAGE - 1) / RECORDS_PER_PAGE;
                if (_currentPage < maxPages - 1) _currentPage++;
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(new Color(32, 32, 32)); // Dark background

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_font != null)
            {
                // Title
                spriteBatch.DrawString(_font, "PLAY HISTORY", new Vector2(60, 130), new Color(230, 150, 30), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, "Track your journey to rescue the princess...", new Vector2(60, 155), new Color(200, 200, 200), 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

                // Separator
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, 175, 1160, 2), new Color(200, 200, 200));
                }

                // Draw stat cards
                DrawStatCards(spriteBatch);

                // Draw records table
                DrawRecordsTable(spriteBatch);

                // Draw pagination
                DrawPagination(spriteBatch);

                // Draw hint
                spriteBatch.DrawString(_font, "LEFT/RIGHT: Navigate Pages  |  ESC: Back",
                    new Vector2(400, 685), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            // Draw back button
            spriteBatch.Begin();
            _backButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawStatCards(SpriteBatch spriteBatch)
        {
            int cardY = 195;
            int cardWidth = 360;
            int cardHeight = 90;
            int spacing = 30;

            // Get stats from GameSession
            var gameSession = GameSession.Instance;
            int totalPlays = _records.Count;
            int highScore = _records.Count > 0 ? _records.Max(r => r.TotalScore) : 0;

            // Card 1: Total Plays
            DrawStatCard(spriteBatch, 60, cardY, cardWidth, cardHeight, "TOTAL PLAYS", totalPlays.ToString("D3"));

            // Card 2: High Score
            DrawStatCard(spriteBatch, 60 + cardWidth + spacing, cardY, cardWidth, cardHeight, "HIGH SCORE", highScore.ToString());

            // Card 3: Total Coins (from achievements display)
            int totalCoins = gameSession?.TotalCoins ?? 0;
            DrawStatCard(spriteBatch, 60 + (cardWidth + spacing) * 2, cardY, cardWidth, cardHeight, "TOTAL COINS", totalCoins.ToString());
        }

        private void DrawStatCard(SpriteBatch spriteBatch, int x, int y, int width, int height, string label, string value)
        {
            if (Game1.WhitePixel == null) return;

            // Card background
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, width, height), new Color(0, 0, 0));

            // Card border
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, width, 3), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y + height - 3, width, 3), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, 3, height), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + width - 3, y, 3, height), Color.White);

            // Label
            spriteBatch.DrawString(_font, label, new Vector2(x + 15, y + 15), new Color(150, 150, 150), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

            // Value
            spriteBatch.DrawString(_font, value, new Vector2(x + 15, y + 45), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
        }

        private void DrawRecordsTable(SpriteBatch spriteBatch)
        {
            int tableY = 310;
            int tableWidth = 1160;
            int tableHeight = 350;

            if (Game1.WhitePixel == null) return;

            // Table background
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, tableY, tableWidth, tableHeight), new Color(0, 0, 0));

            // Table border
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, tableY, tableWidth, 3), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, tableY + tableHeight - 3, tableWidth, 3), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, tableY, 3, tableHeight), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60 + tableWidth - 3, tableY, 3, tableHeight), Color.White);

            // Header row
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, tableY, tableWidth, 35), Color.White);
            spriteBatch.DrawString(_font, "DATE/TIME", new Vector2(80, tableY + 8), Color.Black, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "PLAYER", new Vector2(320, tableY + 8), Color.Black, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "SCORE", new Vector2(550, tableY + 8), Color.Black, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "DURATION", new Vector2(750, tableY + 8), Color.Black, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "MODE", new Vector2(1000, tableY + 8), Color.Black, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

            // Separator
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, tableY + 35, tableWidth, 2), new Color(64, 64, 64));

            // Draw records
            int startIndex = _currentPage * RECORDS_PER_PAGE;
            int endIndex = System.Math.Min(startIndex + RECORDS_PER_PAGE, _records.Count);

            int rowY = tableY + 40;
            int rowHeight = 35;

            if (_records.Count == 0)
            {
                spriteBatch.DrawString(_font, "No play history yet. Start playing!", new Vector2(450, rowY + 50), new Color(150, 150, 150), 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            else
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    GameRecord record = _records[i];
                    bool isHighScore = record.TotalScore == _records.Max(r => r.TotalScore);

                    // Highlight high score row
                    if (isHighScore)
                    {
                        spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, rowY, tableWidth, rowHeight), new Color(230, 0, 18) * 0.3f);
                    }

                    // Separator line
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(60, rowY + rowHeight, tableWidth, 1), new Color(64, 64, 64));

                    // Row data
                    string dateStr = record.PlayDate.ToString("MMM dd, yyyy HH:mm");
                    string playerStr = string.IsNullOrEmpty(record.PlayerName) ? "Anonymous" : record.PlayerName;
                    string scoreStr = record.TotalScore.ToString();
                    string durationStr = record.GetFormattedDuration();
                    string modeStr = record.GameMode == 2 ? "2P" : "1P";

                    Color textColor = isHighScore ? new Color(230, 0, 18) : Color.White;
                    Color scoreColor = isHighScore ? new Color(230, 150, 30) : new Color(200, 200, 0);

                    spriteBatch.DrawString(_font, dateStr, new Vector2(80, rowY + 8), textColor, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, playerStr, new Vector2(320, rowY + 8), textColor, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, scoreStr, new Vector2(550, rowY + 8), scoreColor, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, durationStr, new Vector2(750, rowY + 8), textColor, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, modeStr, new Vector2(1000, rowY + 8), textColor, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

                    rowY += rowHeight;
                }
            }
        }

        private void DrawPagination(SpriteBatch spriteBatch)
        {
            int maxPages = (_records.Count + RECORDS_PER_PAGE - 1) / RECORDS_PER_PAGE;
            if (maxPages == 0) maxPages = 1;

            int paginationY = 670;

            // Page info
            spriteBatch.DrawString(_font, $"PAGE {_currentPage + 1} / {maxPages}", new Vector2(60, paginationY), new Color(150, 150, 150), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

            // Navigation text
            if (_currentPage > 0)
            {
                spriteBatch.DrawString(_font, "< PREV", new Vector2(400, paginationY), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            }

            if (_currentPage < maxPages - 1)
            {
                spriteBatch.DrawString(_font, "NEXT >", new Vector2(1100, paginationY), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            }
        }
    }
}
