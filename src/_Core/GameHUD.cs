using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.src._Core
{
    /// <summary>
    /// Manages HUD display during gameplay
    /// Tracks and displays: Lives, Score, Coins, Time, Enemies Defeated
    /// </summary>
    public class GameHUD
    {
        private SpriteFont _font;
        private int _livesRemaining;
        private int _coinsCollected;
        private int _enemiesDefeated;
        private int _currentScore;
        private float _elapsedTime; // in seconds
        private int _mushroomsCollected; // Track mushroom pickups
        private int _deathCount; // Track deaths

        // Scoring system - NEW FORMULA
        private const int BASE_SCORE = 500;           // Starting score
        private const int TIME_PENALTY_PER_SECOND = 1; // -1 per second
        private const int POINTS_PER_COIN = 200;      // +200 per coin
        private const int POINTS_PER_ENEMY = 100;     // +100 per enemy defeated
        private const int POINTS_PER_MUSHROOM = 500;  // +500 per mushroom collected
        private const int POINTS_PER_DEATH = -200;    // -200 per death

        public int LivesRemaining
        {
            get { return _livesRemaining; }
            set { _livesRemaining = value; }
        }

        public int CoinsCollected
        {
            get { return _coinsCollected; }
            set { _coinsCollected = value; }
        }

        public int EnemiesDefeated
        {
            get { return _enemiesDefeated; }
            set { _enemiesDefeated = value; }
        }

        public int CurrentScore
        {
            get { return _currentScore; }
            set { _currentScore = value; }
        }

        public float ElapsedTime
        {
            get { return _elapsedTime; }
            set { _elapsedTime = value; }
        }

        public int MushroomsCollected
        {
            get { return _mushroomsCollected; }
            set { _mushroomsCollected = value; }
        }

        public int DeathCount
        {
            get { return _deathCount; }
            set { _deathCount = value; }
        }

        public GameHUD(SpriteFont font)
        {
            _font = font;
            _livesRemaining = 3;
            _coinsCollected = 0;
            _enemiesDefeated = 0;
            _currentScore = 0;
            _elapsedTime = 0f;
            _mushroomsCollected = 0;
            _deathCount = 0;
        }

        public void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Calculate score dynamically using new formula:
            // Base + Coin bonus + Enemy bonus + Mushroom bonus - Time penalty - Death penalty
            int timeDeduction = (int)(_elapsedTime * TIME_PENALTY_PER_SECOND);
            int coinBonus = _coinsCollected * POINTS_PER_COIN;
            int enemyBonus = _enemiesDefeated * POINTS_PER_ENEMY;
            int mushroomBonus = _mushroomsCollected * POINTS_PER_MUSHROOM;
            int deathPenalty = _deathCount * POINTS_PER_DEATH;
            
            _currentScore = BASE_SCORE + coinBonus + enemyBonus + mushroomBonus - timeDeduction + deathPenalty;
            
            // Ensure score doesn't go negative
            if (_currentScore < 0) _currentScore = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_font == null) return;

            // Draw HUD background (semi-transparent bar at top)
            if (Game1.WhitePixel != null)
            {
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 1280, 60), Color.Black * 0.5f);
            }

            int padding = 20;
            int yPos = 15;

            // Draw Lives
            string livesText = $"LIVES: {_livesRemaining}";
            spriteBatch.DrawString(_font, livesText, new Vector2(padding, yPos), Color.Red);

            // Draw Score (REAL-TIME)
            string scoreText = $"SCORE: {_currentScore}";
            Vector2 scoreSize = _font.MeasureString(scoreText);
            spriteBatch.DrawString(_font, scoreText, new Vector2(640 - scoreSize.X / 2, yPos), Color.Yellow);

            // Draw Coins
            string coinsText = $"COINS: {_coinsCollected}";
            Vector2 coinsSize = _font.MeasureString(coinsText);
            spriteBatch.DrawString(_font, coinsText, new Vector2(1280 - padding - coinsSize.X, yPos), Color.Gold);

            // Draw Time
            int minutes = (int)(_elapsedTime / 60);
            int seconds = (int)(_elapsedTime % 60);
            string timeText = $"TIME: {minutes:D2}:{seconds:D2}";
            spriteBatch.DrawString(_font, timeText, new Vector2(320, yPos), Color.Cyan);

            // Draw Enemies defeated
            string enemiesText = $"ENEMIES: {_enemiesDefeated}";
            spriteBatch.DrawString(_font, enemiesText, new Vector2(900, yPos), Color.Lime);
        }

        public void Reset()
        {
            _livesRemaining = 3;
            _coinsCollected = 0;
            _enemiesDefeated = 0;
            _currentScore = 0;
            _elapsedTime = 0f;
            _mushroomsCollected = 0;
            _deathCount = 0;
        }

        /// <summary>
        /// Calculate level completion bonus (separate from base score)
        /// </summary>
        public int CalculateLevelBonus()
        {
            // Bonus for completing level
            int completionBonus = 1000;

            // Time speed bonus (faster = more bonus, max 500)
            int speedBonus = 0;
            if (_elapsedTime < 60)
                speedBonus = (int)(500 - (_elapsedTime * 5));
            speedBonus = System.Math.Max(0, speedBonus);

            return completionBonus + speedBonus;
        }

        /// <summary>
        /// Get score breakdown for display
        /// </summary>
        public (int baseScore, int timeDeduction, int coinBonus, int enemyBonus, int mushroomBonus, int deathPenalty, int totalScore) GetScoreBreakdown()
        {
            int timeDeduction = (int)(_elapsedTime * TIME_PENALTY_PER_SECOND);
            int coinBonus = _coinsCollected * POINTS_PER_COIN;
            int enemyBonus = _enemiesDefeated * POINTS_PER_ENEMY;
            int mushroomBonus = _mushroomsCollected * POINTS_PER_MUSHROOM;
            int deathPenalty = _deathCount * POINTS_PER_DEATH;
            int totalScore = _currentScore;

            return (BASE_SCORE, timeDeduction, coinBonus, enemyBonus, mushroomBonus, deathPenalty, totalScore);
        }
    }
}
