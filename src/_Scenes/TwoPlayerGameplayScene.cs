using MarioGame.Entities.Enemies;
using MarioGame.src._Core;
using MarioGame.src._Core.Camera;
using MarioGame.src._Data;
using MarioGame.src._Entities.Base;
using MarioGame.src._Entities.enemies;
using MarioGame.src._Entities.Enviroments;
using MarioGame.src._Entities.items;
using MarioGame.src._Entities.player;
using MarioGame.src._Entities.player.states;
using MarioGame.src._Input;
using MarioGame.src._Scenes;
using MarioGame.src._Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MarioGame._Scenes
{
    public class TwoPlayerGameplayScene : IScene
    {
        private int _levelIndex;
        private List<GameObj> _gameObjects;

        // Hai người chơi riêng biệt
        private Player _player1;
        private Player _player2;

        private Camera _camera;
        private Texture2D _backgroundTex;
        private GameHUD _hud;
        private SpriteFont _playerLabelFont;

        private int _previousPlayer1Lives = 3;
        private int _previousPlayer2Lives = 3;

        // --- CÁC BIẾN TÍNH NĂNG ---
        private Texture2D _fireballTexture;
        // Cooldown bắn riêng cho từng người để công bằng
        private float _shootCooldownP1 = 0f;
        private float _shootCooldownP2 = 0f;

        private float _bulletSpawnTimer = 0f;
        private float _bulletSpawnInterval = 5.0f;
        private Random _random = new Random();

        // --- BOSS ---
        private Boss _boss;
        private bool _isBossLevel = false;

        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;
        private bool _isPaused = false;
        private KeyboardState _previousKeyboardState;
        private bool _isResumingFromPause = false;
        private bool _isContentLoaded = false;

        public TwoPlayerGameplayScene(int levelIndex)
        {
            _levelIndex = levelIndex;
        }

        public void LoadContent()
        {
            if (_isContentLoaded) return;

            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;

            _camera = new Camera(device.Viewport);
            _backgroundTex = content.Load<Texture2D>("sprites/background");
            _fireballTexture = content.Load<Texture2D>("sprites/fireball");

            SpriteFont hudFont = null;
            try
            {
                hudFont = content.Load<SpriteFont>("fonts/GameFont");
                _playerLabelFont = hudFont;
            }
            catch { }
            _hud = new GameHUD(hudFont);

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
            textures.Add("pipe", content.Load<Texture2D>("sprites/pipe"));
            textures.Add("plant", content.Load<Texture2D>("sprites/plant"));
            textures.Add("koopa", content.Load<Texture2D>("sprites/koopa"));
            textures.Add("bullet", content.Load<Texture2D>("sprites/bullet"));
            textures.Add("mystery", content.Load<Texture2D>("sprites/present"));

            MapLoader.Initialize(textures);

            LoadLevelFromFile(playerAnims);

            _isContentLoaded = true;
        }

        private void LoadLevelFromFile(Dictionary<string, SpriteAnimation> playerAnims)
        {
            string levelPath = $"Content/levels/level{_levelIndex}.json";
            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);

                // --- KIỂM TRA MÀN BOSS (Level 10) ---
                if (_levelIndex == 10)
                {
                    _isBossLevel = true;
                }

                // --- CÀI ĐẶT CAMERA ---
                if (MapLoader.CurrentLevelConfig.IsAutoScroll)
                {
                    var autoScroll = new AutoScrollStrategy();
                    autoScroll.ScrollSpeed = MapLoader.CurrentLevelConfig.ScrollSpeed;
                    _camera.SetStrategy(autoScroll);
                }
                else
                {
                    _camera.SetStrategy(new FollowTargetStrategy());
                }

                _player1 = new Player(new Vector2(50, 200), playerAnims, 1);
                _player2 = new Player(new Vector2(100, 200), playerAnims, 2);

                _previousPlayer1Lives = _player1.Lives;
                _previousPlayer2Lives = _player2.Lives;

                // --- SETUP BOSS NẾU CÓ ---
                if (_isBossLevel)
                {
                    // Cả 2 người chơi đều bay
                    _player1.IsFlying = true;
                    _player2.IsFlying = true;

                    var content = GameManager.Instance.Content;
                    Texture2D bossTex = content.Load<Texture2D>("sprites/boss"); // Đổi tên ảnh boss của bạn
                    Texture2D blockTex = content.Load<Texture2D>("sprites/blockbreak");
                    Texture2D pipeTex = content.Load<Texture2D>("sprites/pipe");

                    // Tạo boss
                    _boss = new Boss(bossTex, new Vector2(0, 300), blockTex, pipeTex, GameManager.Instance.GraphicsDevice);
                }

                _hud.Reset();
                System.Diagnostics.Debug.WriteLine($"[TWO PLAYER] Loaded level {_levelIndex}");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load level {_levelIndex}");
                GameManager.Instance.ChangeScene(new MenuScene());
            }
        }

        public void Update(GameTime gameTime)
        {
            _hud.Update(gameTime);
            bool isAutoScroll = MapLoader.CurrentLevelConfig != null && MapLoader.CurrentLevelConfig.IsAutoScroll;

            if (_isResumingFromPause)
            {
                _previousKeyboardState = Keyboard.GetState();
                _isResumingFromPause = false;
                return;
            }

            // --- 1. LOGIC BULLET BILL ---
            if ((_player1.Position.X > 2 || _player2.Position.X > 2 || _isBossLevel)
                && MapLoader.CurrentLevelConfig != null
                && MapLoader.CurrentLevelConfig.HasBulletBill)
            {
                _bulletSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_bulletSpawnTimer > _bulletSpawnInterval)
                {
                    _bulletSpawnTimer = 0f;
                    SpawnBulletBill();
                    _bulletSpawnInterval = _random.Next(3, 7);
                }
            }

            // --- 2. LOGIC BOSS ---
            if (_isBossLevel && _boss != null && _boss.IsActive)
            {
                _boss.Update(gameTime);

                // Boss bám theo Camera bên phải
                if (isAutoScroll)
                    _boss.Position.X = _camera.Position.X + _camera.Viewport.Width - 150;

                // Lấy đạn Boss ném ra
                if (_boss.SpawnedObjects.Count > 0)
                {
                    foreach (var obj in _boss.SpawnedObjects) _gameObjects.Add(obj);
                    _boss.SpawnedObjects.Clear();
                }

                // Va chạm Player vs Boss
                if (_player1.Bounds.Intersects(_boss.Bounds)) { _player1.TakeDamage(); _player1.Position.X -= 50; }
                if (_player2.Bounds.Intersects(_boss.Bounds)) { _player2.TakeDamage(); _player2.Position.X -= 50; }
            }

            // --- 3. CHECK BOSS DEATH ---
            if (_isBossLevel && _boss != null && !_boss.IsActive)
            {
                _isLevelFinished = true;
            }

            KeyboardState currentKbState = Keyboard.GetState();

            // --- 4. PLAYER SHOOTING ---
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _shootCooldownP1 -= dt;
            _shootCooldownP2 -= dt;

            if (MapLoader.CurrentLevelConfig != null && MapLoader.CurrentLevelConfig.CanShoot)
            {
                // P1 Shoot
                Keys p1Attack = InputSettings.Instance.P1_KeyMap[EGameAction.Attack];
                if (currentKbState.IsKeyDown(p1Attack) && _shootCooldownP1 <= 0)
                {
                    ShootFireball(_player1);
                    _shootCooldownP1 = 0.3f;
                }
                // P2 Shoot
                Keys p2Attack = InputSettings.Instance.P2_KeyMap[EGameAction.Attack];
                if (currentKbState.IsKeyDown(p2Attack) && _shootCooldownP2 <= 0)
                {
                    ShootFireball(_player2);
                    _shootCooldownP2 = 0.3f;
                }
            }

            // Pause
            if (currentKbState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                PauseGame();
                _previousKeyboardState = currentKbState;
                return;
            }
            _previousKeyboardState = currentKbState;

            if (_isPaused) return;

            // Level Complete
            if (_isLevelFinished)
            {
                _finishTimer += dt;
                if (_finishTimer > 2.0f)
                {
                    _isContentLoaded = false;
                    int bonusScore = _hud.CalculateLevelBonus();
                    GameManager.Instance.ChangeScene(new LevelCompleteScene(_levelIndex, 3, _hud.CurrentScore, _hud.CoinsCollected, bonusScore, _hud.EnemiesDefeated, _hud.ElapsedTime, _hud.MushroomsCollected, _hud.DeathCount));
                }
                return;
            }

            // --- 5. UPDATE PLAYERS & WALL BLOCK ---
            _player1.Update(gameTime);
            _player1.IsOnGround = false;

            _player2.Update(gameTime);
            _player2.IsOnGround = false;

            if (isAutoScroll)
            {
                float camLeft = _camera.Position.X;
                // Chặn tường trái
                if (_player1.Position.X < camLeft) { _player1.Position.X = camLeft; if (_player1.Velocity.X < 0) _player1.Velocity.X = 0; }
                if (_player2.Position.X < camLeft) { _player2.Position.X = camLeft; if (_player2.Velocity.X < 0) _player2.Velocity.X = 0; }
            }
            else
            {
                if (_player1.Position.X < 0) _player1.Position.X = 0;
                if (_player2.Position.X < 0) _player2.Position.X = 0;
            }

            // --- 6. CHECK LIVES & RESPAWN ---
            CheckPlayerLife(_player1, ref _previousPlayer1Lives, isAutoScroll);
            CheckPlayerLife(_player2, ref _previousPlayer2Lives, isAutoScroll);

            // Game Over Check (Cả 2 cùng chết mới thua, hoặc 1 chết thua luôn tùy bạn. Ở đây để 1 người chết là thua)
            if (_player1.Lives <= 0 || _player1.Position.Y > 1000 || _player2.Lives <= 0 || _player2.Position.Y > 1000)
            {
                // Logic chết khi AutoScroll bị kẹt quá xa
                if (isAutoScroll && (_player1.Position.X < _camera.Position.X - 100 || _player2.Position.X < _camera.Position.X - 100))
                {
                    // Force die
                }

                string deathReason = "Game Over";
                GameManager.Instance.ChangeScene(new TwoPlayerGameOverScene(_levelIndex, _hud.CurrentScore, _hud.CoinsCollected, _hud.EnemiesDefeated, deathReason));
                return;
            }

            // --- 7. UPDATE OBJECTS & COLLISION ---
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var obj = _gameObjects[i];
                obj.Update(gameTime);

                // Môi trường
                if (obj is MovableObj movableObj && !(obj is Player) && !(obj is PiranhaPlant) && !(obj is BulletBill) && !(obj is Fireball))
                {
                    foreach (var other in _gameObjects)
                        if (other != obj && (other is Block || other is Pipe || other is Castle))
                            Collision.ResolveStaticCollision(movableObj, other);
                }

                if (obj.IsActive)
                {
                    // Xử lý va chạm cho từng Player
                    HandlePlayerCollision(_player1, obj);
                    HandlePlayerCollision(_player2, obj);

                    // Xử lý Fireball chung (Trúng Boss/Quái)
                    if (obj is Fireball fireball)
                    {
                        // Check Boss
                        if (_isBossLevel && _boss != null && _boss.IsActive && fireball.Bounds.Intersects(_boss.Bounds))
                        {
                            _boss.TakeDamage();
                            fireball.IsActive = false;
                        }

                        // Check Quái
                        foreach (var target in _gameObjects)
                        {
                            if (target != fireball && target.IsActive)
                            {
                                if (target is Enemy enemy && !(target is BossProjectile) && fireball.Bounds.Intersects(enemy.Bounds))
                                {
                                    enemy.OnStomped();
                                    fireball.IsActive = false;
                                }
                                else if ((target is Block || target is Pipe || target is Castle) && fireball.Bounds.Intersects(target.Bounds))
                                {
                                    fireball.IsActive = false;
                                }
                            }
                        }
                    }

                    // Castle Finish (Màn thường)
                    if (obj is Castle && !_isBossLevel)
                    {
                        if (_player1.HasReachedGoal && _player2.HasReachedGoal) _isLevelFinished = true;
                    }
                }

                if (!obj.IsActive) _gameObjects.RemoveAt(i);
            }

            _hud.LivesRemaining = System.Math.Min(_player1.Lives, _player2.Lives);
            _hud.CoinsCollected = _player1.Coins + _player2.Coins;

            // Camera trung bình cộng
            Vector2 avgPos = (_player1.Position + _player2.Position) / 2;
            Rectangle mapBounds = new Rectangle(0, 0, 3200, 736);
            _camera.Update(avgPos, mapBounds, gameTime);
        }

        // Hàm phụ trợ xử lý va chạm để code đỡ dài
        private void HandlePlayerCollision(Player p, GameObj obj)
        {
            if (obj is MysteryBlock mystery)
            {
                if (p.Bounds.Intersects(mystery.Bounds))
                {
                    bool isHeadHit = p.Velocity.Y < 0 && p.Bounds.Top > mystery.Bounds.Bottom - 10;
                    if (isHeadHit)
                    {
                        p.Velocity.Y = 0;
                        var content = GameManager.Instance.Content;
                        Item spawned = mystery.SpawnItem(content.Load<Texture2D>("sprites/coin"), content.Load<Texture2D>("sprites/mushroom"));
                        if (spawned != null) _gameObjects.Add(spawned);
                    }
                    Collision.ResolveStaticCollision(p, obj);
                }
            }
            else if (obj is Block || obj is Pipe)
            {
                Collision.ResolveStaticCollision(p, obj);
            }
            else if (obj is Item item && p.Bounds.Intersects(item.Bounds))
            {
                if (item is Mushroom) _hud.MushroomsCollected++;
                item.OnCollect(p);
            }
            else if (obj is Enemy enemy && p.Bounds.Intersects(enemy.Bounds))
            {
                if (enemy is PiranhaPlant) p.TakeDamage();
                else if (enemy is BossProjectile) p.TakeDamage();
                else if (Collision.IsTopCollision(p, enemy)) { enemy.OnStomped(); p.Velocity.Y = -5f; _hud.EnemiesDefeated++; }
                else p.TakeDamage();
            }
            else if (obj is Castle)
            {
                if (p.Bounds.Intersects(obj.Bounds)) p.HasReachedGoal = true;
            }
        }

        private void CheckPlayerLife(Player p, ref int prevLives, bool isAutoScroll)
        {
            if (p.Lives < prevLives) { _hud.DeathCount++; prevLives = p.Lives; }
            else if (p.Lives == prevLives) { prevLives = p.Lives; }

            // Logic hồi sinh nếu chưa hết mạng hẳn
            if (p.Lives > 0 && (p.Position.Y > 1000 || (isAutoScroll && p.Position.X < _camera.Position.X - 100)))
            {
                p.Lives--;
                prevLives = p.Lives;

                Vector2 respawnPos;
                if (isAutoScroll) respawnPos = new Vector2(_camera.Position.X + 150, 200);
                else respawnPos = new Vector2(50, 200); // Hoặc về vị trí của người chơi còn sống?

                p.Position = respawnPos;
                p.Velocity = Vector2.Zero;
                p.SetState(new SmallState());
                if (_isBossLevel) p.IsFlying = true; // Nhớ bật lại bay
                p.StartInvincible();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (_backgroundTex != null) spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);

            foreach (var obj in _gameObjects)
                if (_camera.IsVisible(obj.Bounds)) obj.Draw(spriteBatch);

            if (_isBossLevel && _boss != null) _boss.Draw(spriteBatch);

            _player1.Draw(spriteBatch);
            _player2.Draw(spriteBatch);
            spriteBatch.End();

            // Draw Labels
            if (_playerLabelFont != null)
            {
                spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);
                Vector2 p1Pos = new Vector2(_player1.Bounds.X + 8, _player1.Bounds.Y - 25);
                spriteBatch.DrawString(_playerLabelFont, "P1", p1Pos, Color.Cyan);
                Vector2 p2Pos = new Vector2(_player2.Bounds.X + 8, _player2.Bounds.Y - 25);
                spriteBatch.DrawString(_playerLabelFont, "P2", p2Pos, Color.Magenta);
                spriteBatch.End();
            }

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void PauseGame()
        {
            _isPaused = true;
            // Lưu trạng thái P1 đại diện
            GameState state = new GameState
            {
                LevelIndex = _levelIndex,
                PlayerPosition = _player1.Position,
                PlayerVelocity = _player1.Velocity,
                PlayerLives = _player1.Lives,
                PlayerCoins = _player1.Coins,
                PlayerScore = _player1.Score,
                PlayerScale = _player1.Scale,
                GameObjects = new List<GameObj>(_gameObjects),
                IsValid = true
            };
            GameManager.Instance.SaveGameState(state);
            GameManager.Instance.ChangeScene(new PauseScene(_levelIndex));
        }

        private void SpawnBulletBill()
        {
            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;
            Texture2D bulletTex = content.Load<Texture2D>("sprites/bullet");
            BulletBill bill = new BulletBill(bulletTex, _camera, device);

            float randomY;
            if (_isBossLevel) randomY = _random.Next(50, 650);
            else randomY = _random.Next(450, 650);

            bill.Spawn(randomY);
            _gameObjects.Add(bill);
        }

        private void ShootFireball(Player shooter)
        {
            if (_fireballTexture == null) return;
            Vector2 spawnPos = new Vector2(shooter.Position.X, shooter.Position.Y + 16);
            int dir = shooter.FacingDirection;
            Fireball ball = new Fireball(_fireballTexture, spawnPos, dir);
            _gameObjects.Add(ball);
        }
    }
}