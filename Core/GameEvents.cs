using System;
using MarioGame.Entities.Player;

namespace MarioGame.Core
{
    public static class GameEvents
    {
        // Event invoked when a coin is collected. Subscriber should update score/coins/etc.
        public static Action<Player>? CoinCollected;

        // Event invoked when a player death sequence completes and the game should handle life loss / respawn
        public static Action? PlayerDied;

        // Additional events can be added here as needed (enemy killed, level complete, etc.)
    }
}
