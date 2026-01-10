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
    public class GameplayScene : IScene
    {
        private int _levelIndex;
        private List<GameObj> _gameObjects;
        private Player _player;
        private Camera _camera;
        private Texture2D _backgroundTex;
        private GameHUD _hud;

        private int _previousPlayerLives = 3;
        private Texture2D _fireballTexture;
        private float _shootCooldown = 0f;

        private bool _isLevelFinished = false;
        private float _finishTimer = 0f;
        private bool _levelStatsAdded = false;

        private bool _isPaused = false;
        private KeyboardState _previousKeyboardState;
        private bool _isResumingFromPause = false;
        private bool _isContentLoaded = false;

        private float _bulletSpawnTimer = 0f;
        private float _bulletSpawnInterval = 5.0f;
        private Random _random = new Random();

        // --- CÁC BIẾN CHO BOSS ---
        private Boss _boss;
        private bool _isBossLevel = false;

        public GameplayScene(int levelIndex)
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
            try { hudFont = content.Load<SpriteFont>("fonts/GameFont"); } catch { }
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

            GameState savedState = GameManager.Instance.GetSavedGameState();
            if (savedState.IsValid && savedState.LevelIndex == _levelIndex)
            {
                RestoreGameState(savedState, playerAnims);
                _isResumingFromPause = true;
                GameManager.Instance.ClearSavedGameState();
            }
            else
            {
                if (_levelIndex == 1) GameSession.Instance.ResetSession();
                LoadLevelFromFile(playerAnims);
                _levelStatsAdded = false; // ← RESET FLAG khi load level mới
            }

            _isContentLoaded = true;
        }

        private void RestoreGameState(GameState savedState, Dictionary<string, SpriteAnimation> playerAnims)
        {
            _gameObjects = new List<GameObj>(savedState.GameObjects);
            _player = new Player(savedState.PlayerPosition, playerAnims, 1);
            _player.Velocity = savedState.PlayerVelocity;
            _player.Lives = savedState.PlayerLives;
            _player.Coins = savedState.PlayerCoins;
            _player.Score = savedState.PlayerScore;
            _player.Scale = savedState.PlayerScale;
            _previousPlayerLives = _player.Lives;

            if (_player.Scale > 1.2f) _player.SetState(new BigState());
            else _player.SetState(new SmallState());

            _hud.LivesRemaining = savedState.PlayerLives;
            _hud.CoinsCollected = savedState.PlayerCoins;
            _hud.ElapsedTime = savedState.ElapsedTime;
            _hud.EnemiesDefeated = savedState.EnemiesDefeated;
            _hud.MushroomsCollected = savedState.MushroomsCollected;
            _hud.DeathCount = savedState.DeathCount;

            // --- RESTORE CAMERA STRATEGY ---
            if (savedState.IsAutoScroll)
            {
                var autoScroll = new AutoScrollStrategy();
                autoScroll.ScrollSpeed = MapLoader.CurrentLevelConfig?.ScrollSpeed ?? 110f;
                _camera.SetStrategy(autoScroll);
            }
            else
            {
                _camera.SetStrategy(new FollowTargetStrategy());
            }
        }

        private void LoadLevelFromFile(Dictionary<string, SpriteAnimation> playerAnims)
        {
            string levelPath = $"Content/levels/level{_levelIndex}.json";
            try
            {
                _gameObjects = MapLoader.LoadLevel(levelPath);

                // --- KIỂM TRA MÀN BOSS ---
                if (_levelIndex == Constants.BOSS_LEVEL)
                {
                    _isBossLevel = true;
                }

                // --- CẤU HÌNH CAMERA ---
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

                // --- TẠO PLAYER ---
                _player = new Player(new Vector2(50, 200), playerAnims, 1);
                _previousPlayerLives = _player.Lives;

                // --- SETUP BOSS NẾU CÓ ---
                if (_isBossLevel)
                {
                    _player.IsFlying = true; // Player bay lơ lửng

                    var content = GameManager.Instance.Content;
                    Texture2D bossTex = content.Load<Texture2D>("sprites/boss");
                    Texture2D blockTex = content.Load<Texture2D>("sprites/blockbreak");
                    Texture2D pipeTex = content.Load<Texture2D>("sprites/pipe");

                    // Tạo boss (Vị trí X sẽ được cập nhật liên tục trong Update để bám theo camera)
                    _boss = new Boss(bossTex, new Vector2(0, 300), blockTex, pipeTex, GameManager.Instance.GraphicsDevice);
                }

                _hud.Reset();
                System.Diagnostics.Debug.WriteLine($"[NEW GAME] Loaded level {_levelIndex}");
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

            // --- 1. LOGIC BULLET BILL (Random độ cao) ---
            if (MapLoader.CurrentLevelConfig != null && MapLoader.CurrentLevelConfig.HasBulletBill)
            {
                // Màn Boss luôn bắn, hoặc màn thường thì đợi đi xa
                if (_player.Position.X > 2 || _isBossLevel)
                {
                    _bulletSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_bulletSpawnTimer > _bulletSpawnInterval)
                    {
                        _bulletSpawnTimer = 0f;
                        SpawnBulletBill();
                        _bulletSpawnInterval = _random.Next(3, 7);
                    }
                }
            }

            // --- 2. LOGIC BOSS HOẠT ĐỘNG ---
            if (_isBossLevel && _boss != null && _boss.IsActive)
            {
                _boss.Update(gameTime);

                // [QUAN TRỌNG] Boss bám theo Camera (bên phải)
                // Boss luôn nằm ở tọa độ: CameraX + Chiều rộng màn hình - 150
                _boss.Position.X = _camera.Position.X + _camera.Viewport.Width - 150;

                // Lấy đạn từ Boss ném ra và thêm vào game để xử lý va chạm/vẽ
                if (_boss.SpawnedObjects.Count > 0)
                {
                    foreach (var obj in _boss.SpawnedObjects)
                    {
                        _gameObjects.Add(obj);
                    }
                    _boss.SpawnedObjects.Clear();
                }

                // Player va chạm thân Boss -> Mất máu và bị đẩy ra
                if (_player.Bounds.Intersects(_boss.Bounds))
                {
                    _player.TakeDamage();
                    _player.Position.X -= 50; // Đẩy lùi
                }
            }

            // --- 3. CHECK CHIẾN THẮNG (BOSS CHẾT) ---
            if (_isBossLevel && _boss != null && !_boss.IsActive)
            {
                _isLevelFinished = true; // Boss chết là qua màn
            }

            KeyboardState currentKbState = Keyboard.GetState();

            // --- 4. PLAYER SHOOTING ---
            _shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            Keys attackKey = InputSettings.Instance.P1_KeyMap[EGameAction.Attack];
            if (currentKbState.IsKeyDown(attackKey) && _shootCooldown <= 0)
            {
                // Màn Boss luôn cho bắn, hoặc check config
                if (_isBossLevel || (MapLoader.CurrentLevelConfig != null && MapLoader.CurrentLevelConfig.CanShoot))
                {
                    _shootCooldown = 0.3f;
                    ShootFireball();
                }
            }

            // Pause Logic
            if (currentKbState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                PauseGame();
                _previousKeyboardState = currentKbState;
                return;
            }
            _previousKeyboardState = currentKbState;

            if (_isPaused) return;

            // Level Complete Logic
            if (_isLevelFinished)
            {
                _finishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                // 1. Cập nhật Session - CHỈ 1 LẦN
                if (!_levelStatsAdded)
                {
                    GameSession.Instance.AddLevelStats(_hud.CurrentScore, _hud.CoinsCollected, _hud.EnemiesDefeated, _hud.ElapsedTime);

                    // 2. LƯU VÀO SLOT (Quan trọng)
                    SaveSlotManager.UpdateCurrentSlot(
                        _levelIndex + 1, // Mở màn tiếp theo
                        GameSession.Instance.TotalScore, // Điểm hiện tại
                        _player.Lives,
                        GameSession.Instance.TotalCoins,
                        GameSession.Instance.TotalEnemiesDefeated,
                        GameSession.Instance.TotalTime
                    );
                    
                    _levelStatsAdded = true; // ← ĐẶT CỜ ĐỂ KHÔNG CỘNG LẠI
                }
                
                if (_finishTimer > 2.0f)
                {
                    _isContentLoaded = false;
                    int bonusScore = _hud.CalculateLevelBonus();
                    GameManager.Instance.ChangeScene(new LevelCompleteScene(_levelIndex, Constants.TOTAL_LEVELS, _hud.CurrentScore, _hud.CoinsCollected, bonusScore, _hud.EnemiesDefeated, _hud.ElapsedTime, _hud.MushroomsCollected, _hud.DeathCount));
                }
                return;
            }

            // --- 5. UPDATE PLAYER & CHẶN TƯỜNG ---
            _player.Update(gameTime);
            _player.IsOnGround = false;

            // Logic chặn tường trái
            if (isAutoScroll)
            {
                float cameraLeft = _camera.Position.X;
                // Nếu bị cuốn ra sau Camera -> Đẩy lên
                if (_player.Position.X < cameraLeft)
                {
                    _player.Position.X = cameraLeft;
                    if (_player.Velocity.X < 0) _player.Velocity.X = 0;
                }
            }
            else
            {
                if (_player.Position.X < 0) _player.Position.X = 0;
            }

            // --- 6. LOGIC CHẾT & HỒI SINH ---
            if (_player.Lives < _previousPlayerLives) { _hud.DeathCount++; _previousPlayerLives = _player.Lives; }
            else if (_player.Lives == _previousPlayerLives) { _previousPlayerLives = _player.Lives; }

            bool isDead = false;
            if (_player.Lives <= 0) isDead = true;
            if (_player.Position.Y > 1000) isDead = true;

            // Nếu auto scroll mà bị kẹt quá xa (ví dụ lỗi game đẩy ra ngoài) -> Chết
            if (isAutoScroll && _player.Position.X < _camera.Position.X - 100) isDead = true;

            if (isDead)
            {
                if (_player.Lives <= 1)
                {
                    GameManager.Instance.ChangeScene(new GameOverScene(_levelIndex, _hud.CurrentScore, _hud.CoinsCollected, _hud.EnemiesDefeated));
                    return;
                }
                else
                {
                    _player.Lives--;
                    _previousPlayerLives = _player.Lives;

                    Vector2 respawnPos;
                    if (isAutoScroll)
                    {
                        // Hồi sinh ở giữa màn hình hiện tại
                        respawnPos = new Vector2(_camera.Position.X + 150, 200);
                    }
                    else
                    {
                        respawnPos = new Vector2(50, 200);
                    }

                    _player.Position = respawnPos;
                    _player.Velocity = Vector2.Zero;
                    _player.SetState(new SmallState());

                    // Nếu là màn Boss thì nhớ bật lại chế độ bay
                    if (_isBossLevel) _player.IsFlying = true;

                    _player.StartInvincible();
                }
            }

            // --- 7. UPDATE GAME OBJECTS & COLLISION ---
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var obj = _gameObjects[i];
                obj.Update(gameTime);

                // Môi trường (Quái/Item va chạm Tường)
                if (obj is MovableObj movableObj && !(obj is Player) && !(obj is PiranhaPlant) && !(obj is BulletBill) && !(obj is Fireball))
                {
                    foreach (var other in _gameObjects)
                        if (other != obj && (other is Block || other is Pipe || other is Castle))
                            Collision.ResolveStaticCollision(movableObj, other);
                }

                if (obj.IsActive)
                {
                    // Mystery Block
                    if (obj is MysteryBlock mysteryBlock)
                    {
                        if (_player.Bounds.Intersects(mysteryBlock.Bounds))
                        {
                            bool isHeadHit = _player.Velocity.Y < 0 && _player.Bounds.Top > mysteryBlock.Bounds.Top;
                            if (isHeadHit)
                            {
                                _player.Velocity.Y = 0;
                                var content = GameManager.Instance.Content;
                                Texture2D coinTex = content.Load<Texture2D>("sprites/coin");
                                Texture2D mushTex = content.Load<Texture2D>("sprites/mushroom");
                                Item spawnedItem = mysteryBlock.SpawnItem(coinTex, mushTex);
                                if (spawnedItem != null) _gameObjects.Add(spawnedItem);
                            }
                            Collision.ResolveStaticCollision(_player, obj);
                        }
                    }
                    // FIREBALL: Xử lý va chạm Boss & Quái
                    else if (obj is Fireball fireball)
                    {
                        // 1. Check trúng Boss
                        if (_isBossLevel && _boss != null && _boss.IsActive && fireball.Bounds.Intersects(_boss.Bounds))
                        {
                            _boss.TakeDamage(); // Trừ máu Boss
                            fireball.IsActive = false; // Đạn biến mất
                        }

                        // 2. Check trúng quái thường / tường
                        foreach (var target in _gameObjects)
                        {
                            if (target != fireball && target.IsActive)
                            {
                                // Trúng Enemy (trừ đạn boss)
                                if (target is Enemy enemy && !(target is BossProjectile) && fireball.Bounds.Intersects(enemy.Bounds))
                                {
                                    enemy.OnStomped();
                                    fireball.IsActive = false;
                                }
                                // Trúng tường
                                else if ((target is Block || target is Pipe || target is Castle) && fireball.Bounds.Intersects(target.Bounds))
                                {
                                    fireball.IsActive = false;
                                }
                            }
                        }
                    }
                    // Block/Pipe
                    else if (obj is Block || obj is Pipe)
                    {
                        Collision.ResolveStaticCollision(_player, obj);
                    }
                    // Item
                    else if (obj is Item item && _player.Bounds.Intersects(item.Bounds))
                    {
                        if (item is Mushroom) _hud.MushroomsCollected++;
                        item.OnCollect(_player);
                    }
                    // Enemy (Gây sát thương cho Player)
                    else if (obj is Enemy enemy && _player.Bounds.Intersects(enemy.Bounds))
                    {
                        if (enemy is PiranhaPlant) _player.TakeDamage();
                        else if (enemy is BossProjectile) _player.TakeDamage(); // Đạn boss
                        else if (Collision.IsTopCollision(_player, enemy))
                        {
                            enemy.OnStomped();
                            _player.Velocity.Y = -5f;
                            _hud.EnemiesDefeated++;
                        }
                        else _player.TakeDamage();
                    }
                    // Castle (Chỉ thắng ở màn thường, màn Boss phải giết Boss)
                    else if (obj is Castle && !_isBossLevel && _player.Bounds.Intersects(obj.Bounds))
                    {
                        _isLevelFinished = true;
                    }
                }

                if (!obj.IsActive) _gameObjects.RemoveAt(i);
            }

            _hud.LivesRemaining = _player.Lives;
            _hud.CoinsCollected = _player.Coins;

            Rectangle mapBounds = new Rectangle(0, 0, Constants.MAP_WIDTH, Constants.MAP_HEIGHT);
            _camera.Update(_player.Position, mapBounds, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (_backgroundTex != null) spriteBatch.Draw(_backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: _camera.ViewMatrix, samplerState: SamplerState.PointClamp);

            // Vẽ Objects (bao gồm đạn boss, quái, tường)
            foreach (var obj in _gameObjects)
                if (_camera.IsVisible(obj.Bounds)) obj.Draw(spriteBatch);

            // Vẽ Boss riêng (để chắc chắn nó được vẽ)
            if (_isBossLevel && _boss != null)
                _boss.Draw(spriteBatch);

            // Vẽ Player
            _player.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void PauseGame()
        {
            _isPaused = true;
            GameState state = new GameState
            {
                LevelIndex = _levelIndex,
                PlayerPosition = _player.Position,
                PlayerVelocity = _player.Velocity,
                PlayerLives = _player.Lives,
                PlayerCoins = _player.Coins,
                PlayerScore = _player.Score,
                PlayerScale = _player.Scale,
                GameObjects = new List<GameObj>(_gameObjects),
                ElapsedTime = _hud.ElapsedTime,
                EnemiesDefeated = _hud.EnemiesDefeated,
                MushroomsCollected = _hud.MushroomsCollected,
                DeathCount = _hud.DeathCount,
                IsAutoScroll = MapLoader.CurrentLevelConfig?.IsAutoScroll ?? false,
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
            if (_isBossLevel) randomY = _random.Next(50, 650); // Màn Boss: Random full
            else randomY = _random.Next(450, 650); // Màn thường: Random thấp

            bill.Spawn(randomY);
            _gameObjects.Add(bill);
        }

        private void ShootFireball()
        {
            if (_fireballTexture == null) return;
            Vector2 spawnPos = new Vector2(_player.Position.X, _player.Position.Y + 16);
            int dir = _player.FacingDirection;
            Fireball ball = new Fireball(_fireballTexture, spawnPos, dir);
            _gameObjects.Add(ball);
        }
    }
}