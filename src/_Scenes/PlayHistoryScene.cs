using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using MarioGame._Scenes;
using MarioGame.src._Data.models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class PlayHistoryScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private List<GameRecord> _records;
        private int _currentPage = 0;
        private const int RECORDS_PER_PAGE = 6;
        private int _currentFilter = 0; // 0 = All, 1 = 1-Player, 2 = 2-Player
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

            InitializeButtons();
            _isContentLoaded = true;
        }

        private void InitializeButtons()
        {
            _buttons = new List<Button>();

            int buttonWidth = 120;
            int buttonHeight = 40;
            int spacing = 10;

            int centerX = 640;
            int startX = centerX - 200;
            int startY = 670;

            // Filter buttons
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
                "ALL",
                _font
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + 135, startY, buttonWidth, buttonHeight),
                "1-PLAYER",
                _font
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + 270, startY, buttonWidth, buttonHeight),
                "2-PLAYER",
                _font
            ));

            // Navigation buttons
            _buttons.Add(new Button(
                new Rectangle(startX - 100, startY, buttonWidth - 20, buttonHeight),
                "PREV",
                _font
            ));

            _buttons.Add(new Button(
                new Rectangle(startX + 410, startY, buttonWidth - 20, buttonHeight),
                "NEXT",
                _font
            ));

            // Main Menu button
            _buttons.Add(new Button(
                new Rectangle(centerX - 75, startY + 50, 150, buttonHeight),
                "MAIN MENU",
                _font
            ));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var button in _buttons)
            {
                button.Update(gameTime);
            }

            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Filter buttons
            if (_buttons[0].WasPressed) // ALL
            {
                _currentFilter = 0;
                _currentPage = 0;
                _records = GameRecordManager.Instance.GetTopScores(100);
            }
            else if (_buttons[1].WasPressed) // 1-PLAYER
            {
                _currentFilter = 1;
                _currentPage = 0;
                var sorted = GameRecordManager.Instance.Get1PlayerRecords();
                sorted.Sort((a, b) => b.TotalScore.CompareTo(a.TotalScore));
                _records = sorted;
            }
            else if (_buttons[2].WasPressed) // 2-PLAYER
            {
                _currentFilter = 2;
                _currentPage = 0;
                var sorted = GameRecordManager.Instance.Get2PlayerRecords();
                sorted.Sort((a, b) => b.TotalScore.CompareTo(a.TotalScore));
                _records = sorted;
            }
            else if (_buttons[3].WasPressed) // Previous
            {
                if (_currentPage > 0)
                    _currentPage--;
            }
            else if (_buttons[4].WasPressed) // Next
            {
                int maxPages = (_records.Count + RECORDS_PER_PAGE - 1) / RECORDS_PER_PAGE;
                if (_currentPage < maxPages - 1)
                    _currentPage++;
            }
            else if (_buttons[5].WasPressed) // Main Menu
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            // Keyboard navigation
            if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                if (_currentPage > 0) _currentPage--;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                int maxPages = (_records.Count + RECORDS_PER_PAGE - 1) / RECORDS_PER_PAGE;
                if (_currentPage < maxPages - 1) _currentPage++;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin();

            if (_font != null)
            {
                // Draw title
                string title = "PLAY HISTORY - HIGH SCORES";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 15), Color.Gold);

                // Draw filter info
                string filterText = _currentFilter == 0 ? "Showing: All Records" : 
                                   _currentFilter == 1 ? "Showing: 1-Player Only" : 
                                   "Showing: 2-Player Only";
                Vector2 filterSize = _font.MeasureString(filterText);
                spriteBatch.DrawString(_font, filterText,
                    new Vector2(640 - filterSize.X / 2, 40), Color.Cyan);

                // Draw column headers with fixed widths
                string rankHeader = "RANK";
                string modeHeader = "MODE";
                string nameHeader = "NAME";
                string scoreHeader = "SCORE";
                string levelHeader = "LVL";
                string dateHeader = "DATE";

                int col1 = 50;      // RANK
                int col2 = 120;     // MODE
                int col3 = 180;     // NAME
                int col4 = 380;     // SCORE
                int col5 = 550;     // LEVEL
                int col6 = 650;     // DATE

                int headerY = 70;
                spriteBatch.DrawString(_font, rankHeader, new Vector2(col1, headerY), Color.Cyan);
                spriteBatch.DrawString(_font, modeHeader, new Vector2(col2, headerY), Color.Cyan);
                spriteBatch.DrawString(_font, nameHeader, new Vector2(col3, headerY), Color.Cyan);
                spriteBatch.DrawString(_font, scoreHeader, new Vector2(col4, headerY), Color.Cyan);
                spriteBatch.DrawString(_font, levelHeader, new Vector2(col5, headerY), Color.Cyan);
                spriteBatch.DrawString(_font, dateHeader, new Vector2(col6, headerY), Color.Cyan);

                // Draw separator
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(50, 100, 900, 2), Color.Cyan);
                }

                // Draw records for current page
                int startIndex = _currentPage * RECORDS_PER_PAGE;
                int endIndex = System.Math.Min(startIndex + RECORDS_PER_PAGE, _records.Count);

                int displayY = 125;
                for (int i = startIndex; i < endIndex; i++)
                {
                    GameRecord record = _records[i];
                    int rank = i + 1;

                    // Alternate colors for readability
                    Color color = (i % 2 == 0) ? Color.White : Color.LightGray;

                    // Draw each column
                    string rankStr = rank.ToString("D2");
                    string modeStr = record.GameMode == 2 ? "2P" : "1P";
                    string nameStr = record.PlayerName.Length > 15 ? record.PlayerName.Substring(0, 15) : record.PlayerName;
                    string scoreStr = record.TotalScore.ToString();
                    string levelStr = record.MaxLevel.ToString();
                    string dateStr = record.PlayDate.ToString("MM/dd");

                    spriteBatch.DrawString(_font, rankStr, new Vector2(col1, displayY), color);
                    spriteBatch.DrawString(_font, modeStr, new Vector2(col2, displayY), record.GameMode == 2 ? Color.Magenta : Color.LimeGreen);
                    spriteBatch.DrawString(_font, nameStr, new Vector2(col3, displayY), color);
                    spriteBatch.DrawString(_font, scoreStr, new Vector2(col4, displayY), Color.Yellow);
                    spriteBatch.DrawString(_font, levelStr, new Vector2(col5, displayY), Color.Cyan);
                    spriteBatch.DrawString(_font, dateStr, new Vector2(col6, displayY), Color.Gray);

                    displayY += 40;
                }

                // Draw page info
                int maxPages = (_records.Count + RECORDS_PER_PAGE - 1) / RECORDS_PER_PAGE;
                int totalRecords = _records.Count;
                string pageInfo = $"Page {_currentPage + 1}/{System.Math.Max(1, maxPages)} | Total Records: {totalRecords}";
                Vector2 pageInfoSize = _font.MeasureString(pageInfo);
                spriteBatch.DrawString(_font, pageInfo,
                    new Vector2(640 - pageInfoSize.X / 2, 600), Color.Gray);

                // Draw hint
                string hint = "Use filter buttons or LEFT/RIGHT arrows | ESC to go back";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, 620), Color.DarkGray);

                // No records message
                if (_records.Count == 0)
                {
                    string noRecords = "No records yet. Play a game to add a record!";
                    Vector2 noRecordsSize = _font.MeasureString(noRecords);
                    spriteBatch.DrawString(_font, noRecords,
                        new Vector2(640 - noRecordsSize.X / 2, 300), Color.Red);
                }
            }

            spriteBatch.End();

            // Draw buttons
            spriteBatch.Begin();
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
