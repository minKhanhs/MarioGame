using MarioGame.Core;
using MarioGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Entities.Items
{
    public class Coin : Item
    {
        public Coin(Vector2 position) : base(position, new Vector2(16, 16))
        {
            AffectedByGravity = false;
        }

        public override void OnCollect(Player.Player player)
        {
            GameManager.Instance.AddCoin();
            SoundManager.Instance.PlaySound("coin");
        }

        public override void Update(float deltaTime)
        {
            // Coins don't move
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
        {
            if (!IsVisible) return;

            Rectangle destRect = new Rectangle(
                (int)(Position.X - cameraOffset.X),
                (int)(Position.Y - cameraOffset.Y),
                (int)Size.X,
                (int)Size.Y
            );

            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, destRect, Color.Gold);
        }
    }
}