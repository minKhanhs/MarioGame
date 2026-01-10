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
        private int _mushroomsCollected;  // Add this
        private int _deathCount;  // Add this
        private float _levelTime;  // Add this to store level-specific time
        private KeyboardState _previousKeyboardState;
        private bool _isFirstUpdate = true;
        private bool _isContentLoaded = false;

        // Cumulative stats tracking
        private int _cumulativeScore;
        private int _cumulativeCoins;
        private int _cumulativeEnemies;
        private float _cumulativeTime;

        public LevelCompleteScene(int currentLevel, int totalLevels, int score, int coins, int bonusScore = 500, int enemiesDefeated = 0, float levelTime = 0f, int mushrooms = 0, int deaths = 0)
        {
            _currentLevel = currentLevel;
            _totalLevels = totalLevels;
            _finalScore = score;  // This is the HUD calculated score for this level only
            _finalCoins = coins;
            _bonusScore = bonusScore;  // Just for display purposes
            _enemiesDefeated = enemiesDefeated;
            _mushroomsCollected = mushrooms;  // Store mushrooms
            _deathCount = deaths;  // Store deaths
            _levelTime = levelTime;

            // Get session stats (which already includes this level's stats from GameplayScene)
            GameSession session = GameSession.Instance;

            // Display cumulative as what's in GameSession right now
            // NO NEED to add _finalScore again - it's already been added in GameplayScene!
            _cumulativeScore = session.TotalScore;
            _cumulativeCoins = session.TotalCoins;
            _cumulativeEnemies = session.TotalEnemiesDefeated;
            _cumulativeTime = session.TotalTime;
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

            int buttonWidth = 240;
            int buttonHeight = 50;
            int spacing = 20;

            int centerX = 640;
            int startX = centerX - buttonWidth / 2;
            int startY = 600;

            // Next Level button (left)
            if (_currentLevel < _totalLevels)
            {
                _buttons.Add(new Button(
                    new Rectangle(startX - 140, startY, buttonWidth, buttonHeight),
                    "NEXT LEVEL",
                    _font
                )
                {
                    BackgroundColor = new Color(32, 32, 32),
                    HoverBackgroundColor = new Color(230, 0, 18),
                    BorderColor = Color.White,
                    TextColor = Color.White
                });
            }
            else
            {
                _buttons.Add(new Button(
                    new Rectangle(startX - 140, startY, buttonWidth, buttonHeight),
                    "FINISH",
                    _font
                )
                {
                    BackgroundColor = new Color(32, 32, 32),
                    HoverBackgroundColor = new Color(230, 0, 18),
                    BorderColor = Color.White,
                    TextColor = Color.White
                });
            }

            // Main Menu button (right)
            _buttons.Add(new Button(
                new Rectangle(startX + 140, startY, buttonWidth, buttonHeight),
                "MAIN MENU",
                _font
            )
            {
                BackgroundColor = new Color(32, 32, 32),
                HoverBackgroundColor = Color.White,
                BorderColor = Color.White,
                TextColor = Color.White
            });
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
                    // Go to next level WITHOUT adding stats again
                    // (stats were already added in GameplayScene)
                    GameManager.Instance.ClearSavedGameState();
                    if (GameManager.Instance.GameMode == 2)
                    {
                        GameManager.Instance.ChangeScene(new TwoPlayerGameplayScene(_currentLevel + 1));
                    }
                    else
                    {
                        GameManager.Instance.ChangeScene(new GameplayScene(_currentLevel + 1));
                    }
                }
                else
                {
                    // Game finished - stats were already added in GameplayScene
                    // Get the final GameSession stats
                    GameManager.Instance.ClearSavedGameState();
                    
                    GameSession session = GameSession.Instance;
                    
                    // Check achievements
                    AchievementManager.Instance.CheckAndUnlockAchievements(
                        session.TotalCoins,
                        session.TotalEnemiesDefeated,
                        session.TotalScore,
                        session.TotalTime,
                        session.TotalEnemiesDefeated,
                        session.TotalCoins,
                        session.TotalScore,
                        false
                    );
                    AchievementManager.Instance.SaveAll();
                    
                    GameManager.Instance.ChangeScene(new PlayerNameInputScene(
                        session.TotalScore, 
                        session.TotalCoins, 
                        session.TotalEnemiesDefeated, 
                        _currentLevel, 
                        session.TotalTime, 
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
            device.Clear(new Color(18, 18, 18));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_font != null)
            {
                // Header bar (red) - matching AboutUs style
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 80), new Color(230, 0, 18));
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 76, 1280, 4), Color.Black);
                }

                // Title
                spriteBatch.DrawString(_font, "LEVEL COMPLETE!", new Vector2(60, 20), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, $"Level {_currentLevel} / {_totalLevels}", new Vector2(60, 48), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Content section
                int contentY = 110;
                int lineHeight = 28;

                // Statistics header
                spriteBatch.DrawString(_font, "STATISTICS:", new Vector2(100, contentY), Color.Cyan, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
                contentY += 35;

                spriteBatch.DrawString(_font, $"Coins: {_finalCoins}", new Vector2(120, contentY), Color.Gold, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Enemies: {_enemiesDefeated}", new Vector2(120, contentY), Color.Red, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Mushrooms: {_mushroomsCollected}", new Vector2(120, contentY), Color.Magenta, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Deaths: {_deathCount}", new Vector2(120, contentY), Color.Salmon, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                spriteBatch.DrawString(_font, $"Time: {(int)_levelTime}s", new Vector2(120, contentY), Color.Cyan, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

                // Separator
                contentY += 25;
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(100, contentY, 1080, 2), new Color(100, 100, 100));
                }

                // Score breakdown
                contentY += 20;
                spriteBatch.DrawString(_font, "SCORE BREAKDOWN:", new Vector2(100, contentY), Color.Cyan, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
                contentY += 35;

                spriteBatch.DrawString(_font, $"Base: 500", new Vector2(120, contentY), Color.White, 0f, Vector2.Zero, 0.38f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                int timeDeduction = (int)(_levelTime * 1);
                spriteBatch.DrawString(_font, $"Time: -{timeDeduction}s", new Vector2(120, contentY), Color.Orange, 0f, Vector2.Zero, 0.38f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                int coinBonus = _finalCoins * 200;
                spriteBatch.DrawString(_font, $"Coins: +{coinBonus}", new Vector2(120, contentY), Color.Gold, 0f, Vector2.Zero, 0.38f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                int enemyBonus = _enemiesDefeated * 100;
                spriteBatch.DrawString(_font, $"Enemies: +{enemyBonus}", new Vector2(120, contentY), Color.Salmon, 0f, Vector2.Zero, 0.38f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                int mushroomBonus = _mushroomsCollected * 500;
                spriteBatch.DrawString(_font, $"Mushrooms: +{mushroomBonus}", new Vector2(120, contentY), Color.Magenta, 0f, Vector2.Zero, 0.38f, SpriteEffects.None, 0f);
                contentY += lineHeight;
                int deathPenalty = _deathCount * 200;
                spriteBatch.DrawString(_font, $"Deaths: -{deathPenalty}", new Vector2(120, contentY), Color.Red, 0f, Vector2.Zero, 0.38f, SpriteEffects.None, 0f);

                // Final scores
                contentY += 35;
                string levelScore = $"LEVEL SCORE: {_finalScore}";
                Vector2 levelScoreSize = _font.MeasureString(levelScore);
                spriteBatch.DrawString(_font, levelScore,
                    new Vector2(640 - levelScoreSize.X / 2, contentY), Color.LimeGreen, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                contentY += 40;
                string totalScore = $"TOTAL SCORE: {_cumulativeScore}";
                Vector2 totalScoreSize = _font.MeasureString(totalScore);
                spriteBatch.DrawString(_font, totalScore,
                    new Vector2(640 - totalScoreSize.X / 2, contentY), Color.Cyan, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                // Footer
                if (Game1.WhitePixel != null)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 645, 1280, 2), Color.Black);
                }
                spriteBatch.DrawString(_font, "Click buttons below to continue",
                    new Vector2(500, 660), new Color(100, 100, 100), 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);
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
