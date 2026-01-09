using MarioGame.Entities.Enemies;
using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.Enviroments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Entities.enemies
{
    public class Boss : Enemy
    {
        public int MaxHP { get; private set; } = 20;
        public int CurrentHP { get; private set; }

        private float _moveSpeedY = 100f;
        private float _minY = 100f; // Biên trên (để không bay ra khỏi màn hình)
        private float _maxY = 600f; // Biên dưới
        private int _dirY = 1;

        private float _spawnTimer = 0f;
        private float _spawnInterval = 2.0f; // Tốc độ ném gạch

        private Texture2D _projectileTex1;
        private Texture2D _projectileTex2;

        // Texture 1x1 để vẽ thanh máu
        private Texture2D _pixelTexture;

        public List<GameObj> SpawnedObjects { get; private set; }

        public Boss(Texture2D texture, Vector2 position, Texture2D proj1, Texture2D proj2, GraphicsDevice graphics)
        {
            Texture = texture;
            Position = position; // Vị trí này sẽ cố định ở cuối map
            _projectileTex1 = proj1;
            _projectileTex2 = proj2;

            CurrentHP = MaxHP;
            SpawnedObjects = new List<GameObj>();

            // Tạo texture 1 pixel màu trắng để vẽ hình chữ nhật
            _pixelTexture = new Texture2D(graphics, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 1. CHỈ DI CHUYỂN LÊN XUỐNG (X giữ nguyên)
            Position.Y += _moveSpeedY * _dirY * dt;

            if (Position.Y > _maxY) _dirY = -1;
            if (Position.Y < _minY) _dirY = 1;

            // 2. TẤN CÔNG
            _spawnTimer += dt;
            if (_spawnTimer > _spawnInterval)
            {
                _spawnTimer = 0;
                SpawnObstacle();
            }
        }

        private void SpawnObstacle()
        {
            Random rnd = new Random();
            Texture2D selectedTex = (rnd.Next(0, 2) == 0) ? _projectileTex1 : _projectileTex2;

            // Đạn bay ra từ giữa người Boss
            Vector2 spawnPos = new Vector2(Position.X, Position.Y + Texture.Height / 2);

            BossProjectile projectile = new BossProjectile(selectedTex, spawnPos);
            SpawnedObjects.Add(projectile);
        }

        public override void OnStomped() { /* Boss không bị đạp */ }

        public void TakeDamage()
        {
            CurrentHP--;
            if (CurrentHP <= 0) IsActive = false;
        }

        public override Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                // 1. Vẽ Boss
                Color color = (CurrentHP < 5) ? Color.Red : Color.White; // Hóa đỏ khi yếu máu
                spriteBatch.Draw(Texture, Position, color);

                // 2. Vẽ Thanh Máu (Health Bar)
                int barWidth = 100;
                int barHeight = 15;
                // Vị trí thanh máu trên đầu Boss
                Vector2 barPos = new Vector2(Position.X + (Texture.Width - barWidth) / 2, Position.Y - 30);

                // Vẽ viền đen (Nền)
                spriteBatch.Draw(_pixelTexture, new Rectangle((int)barPos.X - 2, (int)barPos.Y - 2, barWidth + 4, barHeight + 4), Color.Black);

                // Vẽ phần máu đỏ (Background bar)
                spriteBatch.Draw(_pixelTexture, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.DarkRed);

                // Vẽ phần máu xanh (Current HP)
                float hpPercent = (float)CurrentHP / MaxHP;
                spriteBatch.Draw(_pixelTexture, new Rectangle((int)barPos.X, (int)barPos.Y, (int)(barWidth * hpPercent), barHeight), Color.LimeGreen);
            }
        }
    }
}
