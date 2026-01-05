using MarioGame.src._Core;
using MarioGame.src._Scenes;
using MarioGame.src._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class CompendiumScene : IScene
    {
        private SpriteFont _font;
        private Button _backButton;
        private int _currentTab = 1; // 0 = Items, 1 = Monsters
        private int _selectedIndex = 0;
        private KeyboardState _previousKeyboardState;
        private bool _isContentLoaded = false;
        private float _textScale = 0.6f;

        // Textures for entries
        private Dictionary<string, Texture2D> _spriteTextures = new();

        public class CompendiumEntry
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Behavior { get; set; }
            public string Weakness { get; set; }
            public int Defense { get; set; } // 0-100
            public int Speed { get; set; }    // 0-100
            public string SpriteName { get; set; }
            public int EntryNumber { get; set; }
        }

        private List<CompendiumEntry> _monsters = new()
        {
            new CompendiumEntry
            {
                EntryNumber = 1,
                Name = "GOOMBA",
                SpriteName = "goomba",
                Description = "A sentient mushroom species and one of the most common enemies in the Mario franchise. They are physically weak and are not much of a threat to adventurers, but they usually appear in large groups.",
                Behavior = "Walks straight, falls off ledges",
                Weakness = "Jump, Fireball, Shell",
                Defense = 10,
                Speed = 30
            },
            new CompendiumEntry
            {
                EntryNumber = 2,
                Name = "KOOPA TROOPA",
                SpriteName = "koopa",
                Description = "A turtle-like creature that patrols areas wearing a protective shell. Can retreat into its shell when threatened and becomes a dangerous projectile when struck.",
                Behavior = "Patrols, hides in shell, shell slides across ground",
                Weakness = "Fireball, Jump from above, Stomp",
                Defense = 40,
                Speed = 50
            },
            new CompendiumEntry
            {
                EntryNumber = 3,
                Name = "PIRANHA PLANT",
                SpriteName = "plant",
                Description = "A carnivorous plant that emerges from pipes to attack. It remains stationary in its pipe but attacks anything that comes near with its snapping jaws.",
                Behavior = "Emerges from pipes, snaps at player",
                Weakness = "Fireball, Avoid pipe entrance",
                Defense = 30,
                Speed = 20
            },
            new CompendiumEntry
            {
                EntryNumber = 4,
                Name = "BULLET BILL",
                SpriteName = "bullet",
                Description = "A living projectile launched from cannons. These mechanical missiles travel in straight lines at high speeds and explode on impact.",
                Behavior = "Flies straight from cannon, explodes on contact",
                Weakness = "Jump, Fireball, cannot be stopped",
                Defense = 20,
                Speed = 90
            },
            new CompendiumEntry
            {
                EntryNumber = 5,
                Name = "BOWSER",
                SpriteName = "boss",
                Description = "The King of the Koopas and main antagonist. A massive fire-breathing dragon-turtle who guards the castle. His combination of strength, magic, and projectiles makes him a formidable final boss.",
                Behavior = "Jumps, breathes fire, throws hammers",
                Weakness = "Jump attacks, Fireball to face, Multiple hits required",
                Defense = 80,
                Speed = 40
            }
        };

        private List<CompendiumEntry> _items = new()
        {
            new CompendiumEntry
            {
                EntryNumber = 1,
                Name = "COIN",
                SpriteName = "coin",
                Description = "Small golden coins scattered throughout levels. Collecting coins increases your total score and counts towards completion. Multiple coins can be found in hidden blocks and enemies.",
                Behavior = "Stationary, collected on contact",
                Weakness = "N/A",
                Defense = 0,
                Speed = 0
            },
            new CompendiumEntry
            {
                EntryNumber = 2,
                Name = "MUSHROOM",
                SpriteName = "mushroom",
                Description = "A power-up item that causes Mario to grow to Super Mario form, doubling in size and health. If you take damage as Super Mario, you shrink back to Small Mario but survive.",
                Behavior = "Moves after appearing, bounces slightly",
                Weakness = "N/A",
                Defense = 0,
                Speed = 0
            },
            new CompendiumEntry
            {
                EntryNumber = 3,
                Name = "FIRE FLOWER",
                SpriteName = "coin", // Placeholder until fire flower sprite is available
                Description = "A magical flower that grants Mario the ability to throw fireballs. Fireballs can defeat most enemies and destroy certain blocks. Limited firepower from inventory.",
                Behavior = "Moves after appearing, bounces",
                Weakness = "N/A",
                Defense = 0,
                Speed = 0
            },
            new CompendiumEntry
            {
                EntryNumber = 4,
                Name = "STAR",
                SpriteName = "coin", // Placeholder until star sprite is available
                Description = "A rare power-up that grants temporary invincibility and the ability to dash through enemies. The star effect lasts for a limited time before fading away.",
                Behavior = "Rotates, moves, bounces",
                Weakness = "N/A",
                Defense = 0,
                Speed = 0
            },
            new CompendiumEntry
            {
                EntryNumber = 5,
                Name = "1-UP",
                SpriteName = "present", // Using present as visual placeholder
                Description = "An extra life item that grants Mario an additional life. Rare to find and usually hidden in secret areas or earned through collecting 100 coins.",
                Behavior = "Stationary or bounces, grants life on contact",
                Weakness = "N/A",
                Defense = 0,
                Speed = 0
            }
        };

        public void LoadContent()
        {
            if (_isContentLoaded) return;

            var content = GameManager.Instance.Content;
            try
            {
                _font = content.Load<SpriteFont>("fonts/GameFont");

                // Load all textures
                string[] spriteNames = { "goomba", "koopa", "plant", "bullet", "boss", "coin", "mushroom", "present" };
                foreach (var name in spriteNames)
                {
                    try
                    {
                        _spriteTextures[name] = content.Load<Texture2D>($"sprites/{name}");
                    }
                    catch
                    {
                        // Sprite not found, will be null
                    }
                }
            }
            catch { }

            InitializeBackButton();
            _isContentLoaded = true;
        }

        private void InitializeBackButton()
        {
            _backButton = new Button(
                new Rectangle(1220, 680, 80, 30),
                "BACK",
                _font
            )
            {
                BackgroundColor = new Color(220, 50, 50),
                HoverBackgroundColor = new Color(236, 19, 19),
                BorderColor = Color.White,
                TextColor = Color.White,
                TextScale = 0.5f
            };
        }

        public void Update(GameTime gameTime)
        {
            _backButton.Update(gameTime);

            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _currentTab = (_currentTab - 1 + 2) % 2;
                _selectedIndex = 0;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _currentTab = (_currentTab + 1) % 2;
                _selectedIndex = 0;
            }

            List<CompendiumEntry> currentList = _currentTab == 1 ? _monsters : _items;
            if (currentKeyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedIndex = (_selectedIndex - 1 + currentList.Count) % currentList.Count;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedIndex = (_selectedIndex + 1) % currentList.Count;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Escape) || _backButton.WasPressed)
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            _previousKeyboardState = currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(new Color(34, 16, 16));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_font != null)
            {
                DrawCompendium(spriteBatch);
            }

            spriteBatch.End();
        }

        private void DrawCompendium(SpriteBatch spriteBatch)
        {
            // Left sidebar
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, 0, 280, 720), new Color(42, 22, 22));
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(280, 0, 2, 720), new Color(74, 42, 42));

            // Header
            spriteBatch.DrawString(_font, "COMPENDIUM", new Vector2(15, 10), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

            // Tab section
            int tabY = 50;
            Color itemsColor = _currentTab == 0 ? new Color(236, 19, 19) : Color.Gray;
            Color monstersColor = _currentTab == 1 ? new Color(236, 19, 19) : Color.Gray;

            spriteBatch.DrawString(_font, "ITEMS", new Vector2(30, tabY), itemsColor, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "MONSTERS", new Vector2(110, tabY), monstersColor, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(0, tabY + 15, 280, 1), new Color(74, 42, 42));

            // List
            List<CompendiumEntry> currentList = _currentTab == 1 ? _monsters : _items;
            int listY = 80;
            int itemHeight = 40;

            for (int i = 0; i < currentList.Count && listY + (i * itemHeight) < 680; i++)
            {
                int yPos = listY + (i * itemHeight);
                bool isSelected = (i == _selectedIndex);

                if (isSelected)
                {
                    spriteBatch.Draw(Game1.WhitePixel, new Rectangle(5, yPos, 270, itemHeight - 2), new Color(236, 19, 19) * 0.3f);
                }

                Color nameColor = isSelected ? new Color(236, 19, 19) : Color.White;
                
                // Draw entry number
                spriteBatch.DrawString(_font, $"#{currentList[i].EntryNumber:D2}", new Vector2(12, yPos + 3), Color.Gray, 0f, Vector2.Zero, _textScale * 0.7f, SpriteEffects.None, 0f);
                
                // Draw name
                spriteBatch.DrawString(_font, currentList[i].Name, new Vector2(50, yPos + 3), nameColor, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
                
                // Draw type (category)
                string category = _currentTab == 1 ? "Enemy" : "Item";
                spriteBatch.DrawString(_font, category, new Vector2(50, yPos + 18), Color.Gray, 0f, Vector2.Zero, _textScale * 0.7f, SpriteEffects.None, 0f);
            }

            // Right panel
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(280, 0, 1280, 720), new Color(34, 16, 16));

            if (_selectedIndex < currentList.Count)
            {
                CompendiumEntry entry = currentList[_selectedIndex];
                int x = 300;
                int y = 30;

                // Header with entry number and name
                spriteBatch.DrawString(_font, $"#{entry.EntryNumber:D2} {entry.Name}", new Vector2(x, y), new Color(236, 19, 19), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                y += 40;

                // Description section
                spriteBatch.DrawString(_font, "DESCRIPTION:", new Vector2(x, y), Color.White, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
                y += 20;

                // Wrap description text
                string[] descriptionLines = WrapText(entry.Description, 80);
                foreach (var line in descriptionLines)
                {
                    spriteBatch.DrawString(_font, line, new Vector2(x, y), Color.Gray, 0f, Vector2.Zero, _textScale * 0.8f, SpriteEffects.None, 0f);
                    y += 18;
                }

                y += 15;

                // Stats section in two columns
                int leftX = x;
                int rightX = x + 500;
                int statsY = y;

                // Left column
                spriteBatch.DrawString(_font, "BEHAVIOR:", new Vector2(leftX, statsY), Color.White, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, entry.Behavior, new Vector2(leftX, statsY + 20), Color.Gray, 0f, Vector2.Zero, _textScale * 0.8f, SpriteEffects.None, 0f);

                statsY += 60;
                spriteBatch.DrawString(_font, "WEAKNESS:", new Vector2(leftX, statsY), Color.White, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(_font, entry.Weakness, new Vector2(leftX, statsY + 20), Color.Gray, 0f, Vector2.Zero, _textScale * 0.8f, SpriteEffects.None, 0f);

                // Right column - Stats bars
                statsY = y;
                spriteBatch.DrawString(_font, "DEFENSE:", new Vector2(rightX, statsY), Color.White, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
                DrawStatBar(spriteBatch, rightX, statsY + 20, entry.Defense);

                statsY += 45;
                spriteBatch.DrawString(_font, "SPEED:", new Vector2(rightX, statsY), Color.White, 0f, Vector2.Zero, _textScale, SpriteEffects.None, 0f);
                DrawStatBar(spriteBatch, rightX, statsY + 20, entry.Speed);

                // Sprite display - larger and centered on right side
                int spriteX = rightX + 200;
                int spriteY = statsY + 80;
                int spriteSize = 120;

                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(spriteX - 10, spriteY - 10, spriteSize + 20, spriteSize + 20), new Color(0, 0, 0, 100));
                spriteBatch.Draw(Game1.WhitePixel, new Rectangle(spriteX - 8, spriteY - 8, spriteSize + 16, spriteSize + 16), Color.Gray * 0.3f);

                Texture2D spriteTexture = GetSpriteForEntry(entry.SpriteName);
                if (spriteTexture != null)
                {
                    spriteBatch.Draw(spriteTexture, new Rectangle(spriteX, spriteY, spriteSize, spriteSize), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(_font, "[NO SPRITE]", new Vector2(spriteX + 20, spriteY + 50), Color.Gray, 0f, Vector2.Zero, _textScale * 0.8f, SpriteEffects.None, 0f);
                }
            }

            // Navigation hint at bottom
            spriteBatch.DrawString(_font, "UP/DOWN: Navigate  |  LEFT/RIGHT: Switch Tab  |  ESC: Back", new Vector2(300, 690), Color.Gray, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            // Draw back button
            _backButton.Draw(spriteBatch);
        }

        private void DrawStatBar(SpriteBatch spriteBatch, int x, int y, int value)
        {
            int barWidth = 150;
            int barHeight = 12;

            // Background
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, barWidth, barHeight), new Color(50, 50, 50));

            // Fill (based on value 0-100)
            int fillWidth = (int)(barWidth * (value / 100f));
            Color barColor = value < 30 ? Color.Yellow : (value < 70 ? new Color(236, 19, 19) : new Color(100, 200, 100));
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, fillWidth, barHeight), barColor);

            // Border
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, barWidth, 2), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y + barHeight - 2, barWidth, 2), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x, y, 2, barHeight), Color.White);
            spriteBatch.Draw(Game1.WhitePixel, new Rectangle(x + barWidth - 2, y, 2, barHeight), Color.White);

            // Value text
            spriteBatch.DrawString(_font, $"{value}", new Vector2(x + barWidth + 10, y - 2), Color.White, 0f, Vector2.Zero, _textScale * 0.7f, SpriteEffects.None, 0f);
        }

        private string[] WrapText(string text, int maxCharsPerLine)
        {
            List<string> lines = new();
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (var word in words)
            {
                if ((currentLine + word).Length > maxCharsPerLine)
                {
                    if (currentLine.Length > 0)
                        lines.Add(currentLine.Trim());
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.Trim());

            return lines.ToArray();
        }

        private Texture2D GetSpriteForEntry(string spriteName)
        {
            return _spriteTextures.TryGetValue(spriteName, out var texture) ? texture : null;
        }
    }
}
