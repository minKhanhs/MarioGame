using MarioGame.src._Core;
using MarioGame.src._Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class AboutUsScene : IScene
    {
        private SpriteFont _font;
        private SpriteFont _smallFont;

        // Team members information (you can modify this later)
        private List<TeamMember> _teamMembers = new()
        {
            new TeamMember()
            {
                Name = "Lead Developer",
                Role = "Game Design & Programming",
                Description = "Responsible for overall game architecture and core mechanics implementation."
            },
            new TeamMember()
            {
                Name = "Graphics Artist",
                Role = "Sprite & Asset Design",
                Description = "Created all visual assets including characters, enemies, and environments."
            },
            new TeamMember()
            {
                Name = "Audio Designer",
                Role = "Music & Sound Effects",
                Description = "Composed background music and implemented sound effects throughout the game."
            },
            new TeamMember()
            {
                Name = "Level Designer",
                Role = "Map Creation & Gameplay Balance",
                Description = "Designed all game levels and balanced difficulty progression."
            },
            new TeamMember()
            {
                Name = "QA Tester",
                Role = "Bug Testing & Feedback",
                Description = "Thoroughly tested the game and provided valuable feedback for improvements."
            }
        };

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            try
            {
                _font = content.Load<SpriteFont>("fonts/GameFont");
                _smallFont = content.Load<SpriteFont>("fonts/GameFont");
            }
            catch
            {
                _font = null;
                _smallFont = null;
            }
        }

        public void Update(GameTime gameTime)
        {
            // Press Escape to go back to menu
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(Color.Black);

            spriteBatch.Begin();

            if (_font != null)
            {
                // Draw title
                string title = "ABOUT US";
                Vector2 titleSize = _font.MeasureString(title);
                spriteBatch.DrawString(_font, title, new Vector2(640 - titleSize.X / 2, 30), Color.Yellow);

                // Draw game info
                string gameInfo = "Super Mario Bros Game";
                Vector2 gameInfoSize = _font.MeasureString(gameInfo);
                spriteBatch.DrawString(_font, gameInfo, new Vector2(640 - gameInfoSize.X / 2, 80), Color.LimeGreen);

                string madeWith = "Built with MonoGame & .NET 8";
                Vector2 madeWithSize = _font.MeasureString(madeWith);
                spriteBatch.DrawString(_font, madeWith, new Vector2(640 - madeWithSize.X / 2, 110), Color.White);

                // Draw team section title
                string teamTitle = "- Development Team -";
                Vector2 teamTitleSize = _font.MeasureString(teamTitle);
                spriteBatch.DrawString(_font, teamTitle, new Vector2(640 - teamTitleSize.X / 2, 160), Color.Cyan);

                // Draw team members
                DrawTeamMembers(spriteBatch);

                // Draw credits section
                DrawCredits(spriteBatch);

                // Draw footer
                string footer = "Press ESC to go back | Special thanks to Nintendo for the original Mario Bros";
                Vector2 footerSize = _font.MeasureString(footer);
                spriteBatch.DrawString(_font, footer, new Vector2(640 - footerSize.X / 2, 680), Color.Gray);
            }

            spriteBatch.End();
        }

        private void DrawTeamMembers(SpriteBatch spriteBatch)
        {
            int yPos = 210;
            int memberSpacing = 85;

            for (int i = 0; i < _teamMembers.Count; i++)
            {
                TeamMember member = _teamMembers[i];

                // Member name
                spriteBatch.DrawString(_font, member.Name, new Vector2(100, yPos), Color.Gold);

                // Role
                spriteBatch.DrawString(_font, member.Role, new Vector2(120, yPos + 22), Color.White);

                // Description
                spriteBatch.DrawString(_font, member.Description, new Vector2(120, yPos + 40), Color.Gray);

                yPos += memberSpacing;

                // Stop if we're running out of space
                if (yPos > 600) break;
            }
        }

        private void DrawCredits(SpriteBatch spriteBatch)
        {
            string credits = "[2024] Your Game Studio. All rights reserved.";
            Vector2 creditsSize = _font.MeasureString(credits);
            spriteBatch.DrawString(_font, credits, new Vector2(640 - creditsSize.X / 2, 630), Color.DarkGray);
        }

        private class TeamMember
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public string Description { get; set; }
        }
    }
}
