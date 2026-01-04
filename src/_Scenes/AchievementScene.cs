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
    public class AchievementScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private List<Achievement> _achievements;
        private int _currentPage = 0;
        private const int ACHIEVEMENTS_PER_PAGE = 6;
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

            _achievements = AchievementManager.Instance.GetAllAchievements();

            InitializeButtons();
            _isContentLoaded = true;
        }

        private void InitializeButtons()
        {
            _buttons = new List<Button>();

            int buttonWidth = 140;
            int buttonHeight = 45;
            int spacing = 15;

            int centerX = 640;
            int startX = centerX - (buttonWidth + spacing) / 2;
            int startY = 670;

            // Previous Page button
            _buttons.Add(new Button(
                new Rectangle(startX - 180, startY, buttonWidth, buttonHeight),
                "PREVIOUS",
                _font
            ));

            // Main Menu button
            _buttons.Add(new Button(
                new Rectangle(startX, startY, buttonWidth, buttonHeight),
                "MAIN MENU",
                _font
            ));

            // Next Page button
            _buttons.Add(new Button(
                new Rectangle(startX + 180, startY, buttonWidth, buttonHeight),
                "NEXT",
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

            // Handle button clicks
            if (_buttons[0].WasPressed) // Previous
            {
                if (_currentPage > 0)
                    _currentPage--;
            }
            else if (_buttons[1].WasPressed) // Main Menu
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }
            else if (_buttons[2].WasPressed) // Next
            {
                int maxPages = (_achievements.Count + ACHIEVEMENTS_PER_PAGE - 1) / ACHIEVEMENTS_PER_PAGE;
                if (_currentPage < maxPages - 1)
                    _currentPage++;
            }

            // Keyboard navigation
            if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                if (_currentPage > 0) _currentPage--;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                int maxPages = (_achievements.Count + ACHIEVEMENTS_PER_PAGE - 1) / ACHIEVEMENTS_PER_PAGE;
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
                string title = "ACHIEVEMENTS";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 15), Color.Gold);

                // Draw progress
                int unlockedCount = AchievementManager.Instance.GetUnlockedCount();
                int totalCount = _achievements.Count;
                string progress = $"Progress: {unlockedCount}/{totalCount}";
                Vector2 progressSize = _font.MeasureString(progress);
                spriteBatch.DrawString(_font, progress,
                    new Vector2(640 - progressSize.X / 2, 55), Color.Cyan);

                // Draw separator
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(50, 85, 1180, 2), Color.Cyan);
                }

                // Draw achievements for current page
                int startIndex = _currentPage * ACHIEVEMENTS_PER_PAGE;
                int endIndex = System.Math.Min(startIndex + ACHIEVEMENTS_PER_PAGE, _achievements.Count);

                int displayY = 115;
                for (int i = startIndex; i < endIndex; i++)
                {
                    Achievement ach = _achievements[i];

                    // Locked or Unlocked
                    string lockIcon = ach.IsUnlocked ? "[*]" : "[ ]";
                    Color color = ach.IsUnlocked ? Color.LimeGreen : Color.DarkGray;

                    // Achievement line
                    string achLine = $"{lockIcon} {ach.Name}";
                    spriteBatch.DrawString(_font, achLine, new Vector2(80, displayY), color);

                    // Description
                    string descLine = $"    {ach.Description}";
                    spriteBatch.DrawString(_font, descLine, new Vector2(100, displayY + 25), ach.IsUnlocked ? Color.Yellow : Color.Gray);

                    // Unlock date
                    if (ach.IsUnlocked && ach.UnlockedDate.HasValue)
                    {
                        string dateStr = $"    Unlocked: {ach.UnlockedDate:MM/dd/yyyy}";
                        spriteBatch.DrawString(_font, dateStr, new Vector2(100, displayY + 45), Color.LightGray);
                    }

                    displayY += 90;
                }

                // Draw page info
                int maxPages = (_achievements.Count + ACHIEVEMENTS_PER_PAGE - 1) / ACHIEVEMENTS_PER_PAGE;
                string pageInfo = $"Page {_currentPage + 1}/{System.Math.Max(1, maxPages)}";
                Vector2 pageInfoSize = _font.MeasureString(pageInfo);
                spriteBatch.DrawString(_font, pageInfo,
                    new Vector2(640 - pageInfoSize.X / 2, 620), Color.Gray);

                // Draw hint
                string hint = "LEFT/RIGHT arrows or buttons to navigate | ESC to go back";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, 645), Color.DarkGray);
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
