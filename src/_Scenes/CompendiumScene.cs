using MarioGame.src._Core;
using MarioGame.src._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class CompendiumScene : IScene
    {
        private SpriteFont _font;
        private int _currentTab = 0; // 0 = Items, 1 = Monsters
        private KeyboardState _previousKeyboardState;

        // Items data
        private List<(string name, string description)> _items = new()
        {
            ("Coin", "Worth 1 point. Collect them to increase your score."),
            ("Mushroom", "Transforms Small Mario into Big Mario. Grants +1 health."),
            ("Fire Flower", "Gives Mario the ability to throw fireballs at enemies."),
            ("Super Star", "Makes Mario invincible for a short period of time."),
            ("1-Up Mushroom", "Grants an extra life to Mario."),
        };

        // Monsters data
        private List<(string name, string description)> _monsters = new()
        {
            ("Goomba", "A small brown creature that walks back and forth. Can be defeated by jumping on it."),
            ("Koopa Troopa", "A turtle-like enemy that walks around and can retreat into its shell."),
            ("Piranha Plant", "A carnivorous plant that emerges from pipes. Jumps if approached."),
            ("Bowser", "The main antagonist and final boss of the game. Breathes fire and throws hammers."),
            ("Lakitu", "A creature that rides in a cloud and throws projectiles at Mario."),
        };

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            try
            {
                _font = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch
            {
                _font = null;
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Switch tabs with Left/Right arrow keys
            if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _currentTab = (_currentTab - 1 + 2) % 2; // Toggle between 0 and 1
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _currentTab = (_currentTab + 1) % 2; // Toggle between 0 and 1
            }

            // Go back to menu
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentKeyboardState.IsKeyDown(Keys.Back))
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
                string title = "COMPENDIUM";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title, new Vector2(640 - titleSize.X / 2, 30), Color.Yellow);

                // Draw tabs
                DrawTabs(spriteBatch);

                // Draw content based on active tab
                if (_currentTab == 0)
                {
                    DrawItemsTab(spriteBatch);
                }
                else
                {
                    DrawMonstersTab(spriteBatch);
                }

                // Draw help text
                string helpText = "LEFT/RIGHT arrows to switch tabs | ESC to go back";
                Vector2 helpSize = _font.MeasureString(helpText);
                spriteBatch.DrawString(_font, helpText, new Vector2(640 - helpSize.X / 2, 680), Color.Gray);
            }

            spriteBatch.End();
        }

        private void DrawTabs(SpriteBatch spriteBatch)
        {
            string itemsTab = "[ ITEMS ]";
            string monstersTab = "[ MONSTERS ]";

            Color itemsColor = _currentTab == 0 ? Color.Yellow : Color.White;
            Color monstersColor = _currentTab == 1 ? Color.Yellow : Color.White;

            Vector2 itemsSize = _font.MeasureString(itemsTab);
            Vector2 monstersSize = _font.MeasureString(monstersTab);

            spriteBatch.DrawString(_font, itemsTab, new Vector2(300, 80), itemsColor);
            spriteBatch.DrawString(_font, monstersTab, new Vector2(700, 80), monstersColor);
        }

        private void DrawItemsTab(SpriteBatch spriteBatch)
        {
            int yPos = 140;
            int lineSpacing = 80;

            for (int i = 0; i < _items.Count; i++)
            {
                string itemName = _items[i].name;
                string itemDesc = _items[i].description;

                // Draw item name in gold
                spriteBatch.DrawString(_font, itemName, new Vector2(80, yPos), Color.Gold);

                // Draw description in gray
                spriteBatch.DrawString(_font, itemDesc, new Vector2(80, yPos + 25), Color.Gray);

                yPos += lineSpacing;
            }
        }

        private void DrawMonstersTab(SpriteBatch spriteBatch)
        {
            int yPos = 140;
            int lineSpacing = 80;

            for (int i = 0; i < _monsters.Count; i++)
            {
                string monsterName = _monsters[i].name;
                string monsterDesc = _monsters[i].description;

                // Draw monster name in red
                spriteBatch.DrawString(_font, monsterName, new Vector2(80, yPos), Color.Red);

                // Draw description in gray
                spriteBatch.DrawString(_font, monsterDesc, new Vector2(80, yPos + 25), Color.Gray);

                yPos += lineSpacing;
            }
        }
    }
}
