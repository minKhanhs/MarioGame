using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using MarioGame._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class LevelCompleteScene : IScene
    {
        private SpriteFont _font;
        private List<Button> _buttons;
        private int _currentLevel;
        private int _totalLevels;
        private int _bonusScore;
        private int _finalScore;
        private int _finalCoins;
        private int _enemiesDefeated;
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        // Cumulative stats tracking
        private int _cumulativeScore;
        private int _cumulativeCoins;
        private int _cumulativeEnemies;
        private float _cumulativeTime;

        public LevelCompleteScene(int currentLevel, int totalLevels, int score, int coins, int bonusScore = 500, int enemiesDefeated = 0)
        {
            _currentLevel = currentLevel;
            _totalLevels = totalLevels;
            _finalScore = score + bonusScore;
            _finalCoins = coins;
            _bonusScore = bonusScore;
            _enemiesDefeated = enemiesDefeated;

            // Initialize cumulative stats (will be set from GameManager or previous state)
            _cumulativeScore = _finalScore;
            _cumulativeCoins = _finalCoins;
            _cumulativeEnemies = _enemiesDefeated;
            _cumulativeTime = 0f;
        }

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

            InitializeButtons();
            _isContentLoaded = true;
        }

        private void InitializeButtons()
        {
            _buttons = new List<Button>();

            int buttonWidth = 160;
            int buttonHeight = 40;
            int spacing = 10;

            int centerX = 640;

            // Next Level button (left)
            if (_currentLevel < _totalLevels)
            {
                _buttons.Add(new Button(
                    new Rectangle(centerX - 180, 650, buttonWidth, buttonHeight),
                    "NEXT LEVEL",
                    _font
                ));
            }
            else
            {
                _buttons.Add(new Button(
                    new Rectangle(centerX - 180, 650, buttonWidth, buttonHeight),
                    "FINISH GAME",
                    _font
                ));
            }

            // Main Menu button (right)
            _buttons.Add(new Button(
                new Rectangle(centerX + 20, 650, buttonWidth, buttonHeight),
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

            // Skip first update
            if (_isFirstUpdate)
            {
                _previousKeyboardState = currentKeyboardState;
                _isFirstUpdate = false;
                return;
            }

            // Handle button clicks
            if (_buttons[0].WasPressed) // Next Level or Game Complete
            {
                if (_currentLevel < _totalLevels)
                {
                    // Go to next level
                    GameManager.Instance.ClearSavedGameState();
                    GameManager.Instance.ChangeScene(new GameplayScene(_currentLevel + 1));
                }
                else
                {
                    // Game finished - go to name input scene
                    GameManager.Instance.ClearSavedGameState();
                    GameManager.Instance.ChangeScene(new PlayerNameInputScene(
                        _cumulativeScore, 
                        _cumulativeCoins, 
                        _cumulativeEnemies, 
                        _currentLevel, 
                        _cumulativeTime, 
                        _currentLevel
                    ));
                }
            }
            else if (_buttons[1].WasPressed) // Main Menu
            {
                GameManager.Instance.ClearSavedGameState();
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
                // Draw LEVEL COMPLETE title
                string title = "LEVEL COMPLETE!";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title,
                    new Vector2(640 - titleSize.X / 2, 15), Color.Gold);

                // Row 1: Level info
                string levelText = $"Level {_currentLevel}/{_totalLevels}";
                Vector2 levelSize = _font.MeasureString(levelText);
                spriteBatch.DrawString(_font, levelText,
                    new Vector2(640 - levelSize.X / 2, 55), Color.LimeGreen);

                // Row 2: Level Statistics Header
                string statsHeader = "LEVEL:";
                spriteBatch.DrawString(_font, statsHeader,
                    new Vector2(100, 95), Color.Cyan);

                // Row 3-5: Level stats (3 columns)
                int col1 = 100;
                int col2 = 350;
                int col3 = 600;
                int row = 125;
                int rowHeight = 28;

                spriteBatch.DrawString(_font, $"Coins: {_finalCoins}", new Vector2(col1, row), Color.Yellow);
                spriteBatch.DrawString(_font, $"Enemies: {_enemiesDefeated}", new Vector2(col2, row), Color.Red);
                spriteBatch.DrawString(_font, $"Score: {_finalScore}", new Vector2(col3, row), Color.Lime);

                // Row 6: Score breakdown
                row += rowHeight + 10;
                string breakdownHeader = "BONUS:";
                spriteBatch.DrawString(_font, breakdownHeader, new Vector2(col1, row), Color.Cyan);
                spriteBatch.DrawString(_font, $"Base: +500", new Vector2(col2, row), Color.White);
                spriteBatch.DrawString(_font, $"Bonus: +{_bonusScore - 500}", new Vector2(col3, row), Color.Cyan);

                // Row 7: Total Progress Header
                row += rowHeight + 10;
                string totalHeader = "TOTAL:";
                spriteBatch.DrawString(_font, totalHeader, new Vector2(col1, row), Color.Magenta);
                spriteBatch.DrawString(_font, $"Coins: {_cumulativeCoins}", new Vector2(col2, row), Color.Gold);
                spriteBatch.DrawString(_font, $"Enemies: {_cumulativeEnemies}", new Vector2(col3, row), Color.Salmon);

                // Row 8: Final total score
                row += rowHeight;
                string totalScore = $"TOTAL SCORE: {_cumulativeScore}";
                Vector2 totalScoreSize = _font.MeasureString(totalScore);
                spriteBatch.DrawString(_font, totalScore,
                    new Vector2(640 - totalScoreSize.X / 2, row), Color.LimeGreen);

                // Row 9: Hint text
                row += rowHeight + 15;
                string hint = "Click buttons or Press ENTER to continue";
                Vector2 hintSize = _font.MeasureString(hint);
                spriteBatch.DrawString(_font, hint,
                    new Vector2(640 - hintSize.X / 2, row), Color.Gray);
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

        public void SetCumulativeStats(int score, int coins, int enemies, float time)
        {
            _cumulativeScore = score;
            _cumulativeCoins = coins;
            _cumulativeEnemies = enemies;
            _cumulativeTime = time;
        }
    }
}
