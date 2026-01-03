using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.enemies;
using MarioGame.src._Entities.Enviroments;
using MarioGame.src._Entities.items;
using MarioGame.src._Core;
using MarioGame.src._Core.Camera;
using MarioGame.src._Data;
using MarioGame.src._Entities.player;
using MarioGame.src._Scenes;
using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class GameplayScene : IScene
    {
        private int _levelIndex;
        private List<GameObj> _gameObjects;
        private Player _player;
        private Camera _camera;
        private Texture2D _backgroundTex;

        // Biến để quản lý trạng thái thắng thua tạm thời
        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;

        public GameplayScene(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;

            _camera = new Camera(device.Viewport);
            _backgroundTex = content.Load<Texture2D>("sprites/background"); // Tên file background

            // --- LOAD TEXTURES & MAP (Code cũ từ Game1 chuyển sang) ---
            var textures = new Dictionary<string, Texture2D>();
            Texture2D marioSheet = content.Load<Texture2D>("sprites/mario");

            var playerAnims = new Dictionary<string, SpriteAnimation>();
            playerAnims.Add("Run", new SpriteAnimation(marioSheet, 4, 36, 52, rowIndex: 0));
            playerAnims.Add("Idle", new SpriteAnimation(marioSheet, 1, 36, 52, rowIndex: 0));
            playerAnims.Add("Jump", new SpriteAnimation(marioSheet, 1, 36, 52, rowIndex: 0));

            textures.Add("mario", marioSheet);
            textures.Add("castle", content.Load<Texture2D>("sprites/castle"));
            textures.Add("ground", content.Load<Texture2D>("sprites/groundTile"));
            textures.Add("brick", content.Load<Texture2D>("sprites/blockbreak"));
            textures.Add("coin", content.Load<Texture2D>("sprites/coin"));
            textures.Add("goomba", content.Load<Texture2D>("sprites/goomba"));
            textures.Add("mushroom", content.Load<Texture2D>("sprites/mushroom"));

            MapLoader.Initialize(textures);

            // LOGIC LOAD LEVEL: Dựa vào _levelIndex để load file json tương ứng
            // Ví dụ: level1.json, level2.json...
            string levelPath = $"Content/levels/level{_levelIndex}.json";

            // Kiểm tra file có tồn tại không, nếu không có (ví dụ hết level 10) thì về Menu
            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);
            }
            catch
            {
                // Hết game -> Về lại Menu hoặc EndGameScene
                GameManager.Instance.ChangeScene(new MenuScene());
                return;
            }

            _player = new Player(new Vector2(50, 200), playerAnims);
        }

        public void Update(GameTime gameTime)
        {
            if (_isLevelFinished)
            {
                // Chờ 2 giây rồi chuyển màn
                _finishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_finishTimer > 2.0f)
                {
                    // LOGIC QUA MÀN: Tăng index lên 1
                    GameManager.Instance.ChangeScene(new GameplayScene(_levelIndex + 1));
                }
                return; // Không update game nữa khi đã thắng
            }

            // --- LOGIC GAME CŨ ---
            _player.Update(gameTime);
            _player.IsOnGround = false;

            // Xử lý game over khi Mario chết (rơi vực hoặc hết máu)
            if (_player.Lives <= 0 || _player.Position.Y > 1000)
            {
                // LOGIC THUA: Load lại màn hiện tại
                GameManager.Instance.ChangeScene(new GameplayScene(_levelIndex));
                return;
            }

            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var obj = _gameObjects[i];
                obj.Update(gameTime);

                // ... (Phần code Collision giữ nguyên như Game1 cũ) ...
                if (obj is MovableObj movableObj && !(obj is Player))
                {
                    foreach (var other in _gameObjects) if (other is Block && other != obj) Collision.ResolveStaticCollision(movableObj, other);
                }

                if (obj.IsActive)
                {
                    if (obj is Block) Collision.ResolveStaticCollision(_player, obj);
                    else if (obj is Item item && _player.Bounds.Intersects(item.Bounds)) item.OnCollect(_player);
                    else if (obj is Enemy enemy && _player.Bounds.Intersects(enemy.Bounds))
                    {
                        if (Collision.IsTopCollision(_player, enemy)) { enemy.OnStomped(); _player.Velocity.Y = -5f; }
                        else _player.TakeDamage();
                    }

                    // --- LOGIC CHIẾN THẮNG ---
                    else if (obj is Castle && _player.Bounds.Intersects(obj.Bounds))
                    {
                        System.Diagnostics.Debug.WriteLine("LEVEL COMPLETE!");
                        _isLevelFinished = true;
                        // Play sound victory...
                    }
                }
                if (!obj.IsActive) _gameObjects.RemoveAt(i);
            }

            // Update Camera
            Rectangle mapBounds = new Rectangle(0, 0, 3200, 736);
            _camera.Update(_player.Position, mapBounds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Vẽ Background
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (_backgroundTex != null) spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();

            // Vẽ Game World
            spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);
            foreach (var obj in _gameObjects) if (_camera.IsVisible(obj.Bounds)) obj.Draw(spriteBatch);
            _player.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}