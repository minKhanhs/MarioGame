using Microsoft.Xna.Framework;

namespace MarioGame.Data.Models
{
    public class PlayerData
    {
        public int PlayerIndex { get; set; }
        public Vector2 Position { get; set; }
        public int Lives { get; set; }
        public int Score { get; set; }
        public int Coins { get; set; }
        public string PowerUpState { get; set; }
        public bool IsActive { get; set; }
    }
}
