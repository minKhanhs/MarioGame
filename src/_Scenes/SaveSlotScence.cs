using MarioGame._Scenes;
using MarioGame.src._Core;
using MarioGame.src._Data;
using MarioGame.src._Data.models;
using MarioGame.src._UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MarioGame.src._Scenes
{
    public class SaveSlotScene : IScene
    {
        // --- COLORS ---
        private readonly Color BG_DARK = new Color(26, 26, 26);
        private readonly Color NES_RED = new Color(255, 0, 0);
        private readonly Color NES_WHITE = Color.White;
        private readonly Color NES_GRAY = Color.Gray;
        private readonly Color NES_BLACK = Color.Black;

        private Texture2D _pixel;
        private SpriteFont _font;
        private Texture2D _thumbnailPlaceholder;

        // --- NEW: BIẾN CHẾ ĐỘ CHƠI ---
        private bool _isTwoPlayerMode;

        // Input & State
        private int _state = 0; // 0: List, 1: Input Name
        private string _inputName = "";
        private float _cursorTimer = 0f;
        private KeyboardState _prevKeyboard;
        private MouseState _prevMouse;

        // Scrolling & UI
        private float _scrollY = 0;
        private float _targetScrollY = 0;
        private int _contentHeight = 0;

        private List<Button> _worldButtons; // Nút ảo (ẩn) để check click
        private Button _createButton;
        private Button _backButton;

        private const int ITEM_HEIGHT = 140;
        private const int ITEM_GAP = 30;
        private const int START_Y = 160;
        private const int CREATE_BTN_HEIGHT = 80;

        // Constructor nhận chế độ chơi
        public SaveSlotScene(bool isTwoPlayer)
        {
            _isTwoPlayerMode = isTwoPlayer;
        }

        public void LoadContent()
        {
            var content = GameManager.Instance.Content;
            var device = GameManager.Instance.GraphicsDevice;

            _pixel = new Texture2D(device, 1, 1);
            _pixel.SetData(new[] { Color.White });

            try { _font = content.Load<SpriteFont>("fonts/GameFont"); } catch { }
            try { _thumbnailPlaceholder = content.Load<Texture2D>("sprites/backgroundMenu"); } catch { }

            SaveSlotManager.LoadSlots();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Khởi tạo các nút tĩnh
            int centerX = 640;
            int width = 900;

            // Nút Back
            _backButton = new Button(new Rectangle(50, 650, 100, 40), "BACK", _font)
            {
                BackgroundColor = Color.DarkRed,
                TextColor = Color.White
            };
        }

        public void Update(GameTime gameTime)
        {
            if (_state == 0) UpdateListMode(gameTime);
            else UpdateInputMode(gameTime);
        }

        private void UpdateListMode(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();

            // 2. Xử lý Click
            if (mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
            {
                int centerX = 640;
                int width = 900;
                int listY = START_Y;

                // Check click các slot hiện có
                for (int i = 0; i < SaveSlotManager.Slots.Count; i++)
                {
                    Rectangle slotRect = new Rectangle(centerX - width / 2, listY + i * (ITEM_HEIGHT + ITEM_GAP), width, ITEM_HEIGHT);
                    
                    // Layout: [PLAY] [DELETE] nằm bên phải
                    int buttonsWidth = 300;
                    int buttonsStartX = slotRect.Right - buttonsWidth - 20;
                    Rectangle playBtnRect = new Rectangle(buttonsStartX, slotRect.Y, 150, slotRect.Height);
                    Rectangle deleteBtnRect = new Rectangle(buttonsStartX + 150, slotRect.Y, 150, slotRect.Height);

                    // Check click
                    if (slotRect.Y > 50 && slotRect.Bottom < 700)
                    {
                        // PLAY Button
                        if (playBtnRect.Contains(mouse.Position))
                        {
                            LoadAndStartGame(SaveSlotManager.Slots[i]);
                            return;
                        }

                        // DELETE Button
                        if (deleteBtnRect.Contains(mouse.Position))
                        {
                            SaveSlotManager.DeleteSlot(i);
                            SaveSlotManager.LoadSlots();
                            _prevMouse = mouse; // ← Update _prevMouse trước khi return
                            return;
                        }
                    }
                }

                // Check click nút "Create New"
                int createBtnY = listY + SaveSlotManager.Slots.Count * (ITEM_HEIGHT + ITEM_GAP);
                Rectangle createRect = new Rectangle(centerX - width / 2, createBtnY, width, CREATE_BTN_HEIGHT);

                if (createRect.Contains(mouse.Position) && createRect.Y > 50 && createRect.Bottom < 700)
                {
                    _state = 1;
                    _inputName = "";
                    _cursorTimer = 0;
                    _prevKeyboard = Keyboard.GetState(); // Reset keyboard state tránh nhận nhầm Enter
                }
            }

            // Update Back Button
            _backButton.Update(gameTime);
            if (_backButton.WasPressed || (kb.IsKeyDown(Keys.Escape) && _prevKeyboard.IsKeyUp(Keys.Escape)))
            {
                GameManager.Instance.ChangeScene(new MenuScene());
            }

            _prevMouse = mouse;
            _prevKeyboard = kb;
        }

        private void UpdateInputMode(GameTime gameTime)
        {
            _cursorTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kb = Keyboard.GetState();
            Keys[] pressedKeys = kb.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (kb.IsKeyDown(key) && _prevKeyboard.IsKeyUp(key))
                {
                    if (key == Keys.Back && _inputName.Length > 0)
                        _inputName = _inputName.Substring(0, _inputName.Length - 1);
                    else if (key == Keys.Space && _inputName.Length < 12)
                        _inputName += " ";
                    else if (key == Keys.Enter)
                    {
                        string name = string.IsNullOrWhiteSpace(_inputName) ? "PLAYER" : _inputName;
                        SaveSlotManager.CreateNewWorld(name);

                        // Tạo world MỚI:
                        // 1. Reset SESSION stats (TotalScore, TotalCoinsThisGame, etc)
                        // 2. Reload CAREER stats từ file (để giữ nguyên từ các lần chơi trước)
                        GameSession.Instance.ResetSession();

                        // Tải game và start
                        LoadAndStartGame(SaveSlotManager.CurrentSlot, isNewWorld: true);
                    }
                    else if (key == Keys.Escape) _state = 0;
                    else if (_inputName.Length < 12 && key >= Keys.A && key <= Keys.Z)
                        _inputName += key.ToString();
                    else if (_inputName.Length < 12 && key >= Keys.D0 && key <= Keys.D9)
                        _inputName += (key - Keys.D0).ToString();
                }
            }
            _prevKeyboard = kb;
        }

        private void LoadAndStartGame(SaveSlot slot, bool isNewWorld = false)
        {
            SaveSlotManager.CurrentSlot = slot;
            
            // Nếu tải world CŨ: reset session stats nhưng restore từ SaveSlot
            // Nếu tạo world MỚI: session đã được reset ở trên, chỉ cần start game
            if (!isNewWorld)
            {
                // Load world CŨ: restore session từ SaveSlot
                GameSession.Instance.ResetSession();
                GameSession.Instance.TotalScore = slot.Score;
                GameSession.Instance.CurrentLevel = slot.CurrentLevel;
                GameSession.Instance.MaxLevelReached = slot.MaxLevelUnlocked;
            }
            // Nếu world mới, không set thêm gì - session đã reset và career stats sẽ từ file
            
            // --- SETUP GAME MODE VÀ CHUYỂN SCENE ---
            if (_isTwoPlayerMode)
            {
                GameManager.Instance.GameMode = 2;
                GameManager.Instance.ChangeScene(new TwoPlayerGameplayScene(slot.CurrentLevel));
            }
            else
            {
                GameManager.Instance.GameMode = 1;
                GameManager.Instance.ChangeScene(new GameplayScene(slot.CurrentLevel));
            }
        }

        public void Draw(SpriteBatch sb)
        {
            var device = GameManager.Instance.GraphicsDevice;
            device.Clear(BG_DARK);

            sb.Begin(samplerState: SamplerState.PointClamp);

            // 1. HEADER
            DrawHeader(sb);

            // 2. LIST (No scrolling)
            int startDrawY = START_Y;
            int centerX = 640;
            int width = 900;

            for (int i = 0; i < SaveSlotManager.Slots.Count; i++)
            {
                Rectangle rect = new Rectangle(centerX - width / 2, startDrawY + i * (ITEM_HEIGHT + ITEM_GAP), width, ITEM_HEIGHT);
                // Culling đơn giản
                if (rect.Bottom > 0 && rect.Top < 720)
                {
                    DrawSlotItem(sb, rect, SaveSlotManager.Slots[i], i);
                }
            }

            // Create Button
            int createY = startDrawY + SaveSlotManager.Slots.Count * (ITEM_HEIGHT + ITEM_GAP);
            Rectangle createRect = new Rectangle(centerX - width / 2, createY, width, CREATE_BTN_HEIGHT);
            if (createRect.Bottom > 0 && createRect.Top < 720)
            {
                DrawCreateButton(sb, createRect);
            }

            // Che phần trôi lên header
            sb.Draw(_pixel, new Rectangle(0, 0, 1280, 140), BG_DARK);
            DrawHeader(sb); // Vẽ lại header đè lên

            // Footer & Back Button
            sb.DrawString(_font, "PRESS ESC TO RETURN", new Vector2(450, 680), NES_GRAY, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            _backButton.Draw(sb);

            // Popup
            if (_state == 1) DrawInputPopup(sb);

            sb.End();
        }

        private void DrawHeader(SpriteBatch sb)
        {
            string title = "SELECT WORLD";
            Vector2 size = _font.MeasureString(title) * 1.5f;
            sb.DrawString(_font, title, new Vector2(640 - size.X / 2 + 4, 44), NES_RED, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            sb.DrawString(_font, title, new Vector2(640 - size.X / 2, 40), NES_WHITE, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

            // --- HIỂN THỊ BADGE 1P HOẶC 2P TÙY CHẾ ĐỘ ---
            string modeText = _isTwoPlayerMode ? "2 PLAYER MODE" : "1 PLAYER MODE";

            Rectangle badge = new Rectangle(580, 90, 120, 30);
            sb.Draw(_pixel, badge, NES_BLACK);
            DrawBorder(sb, badge, 2, NES_GRAY);
            sb.DrawString(_font, modeText, new Vector2(590, 98), NES_RED, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        }

        private void DrawSlotItem(SpriteBatch sb, Rectangle bounds, SaveSlot slot, int index)
        {
            MouseState ms = Mouse.GetState();
            
            // Layout: [PLAY] [DELETE] nằm bên phải, căn giữa
            // Tổng chiều rộng 2 nút = 300px (mỗi nút 150px)
            int buttonsWidth = 300;
            int buttonsStartX = bounds.Right - buttonsWidth - 20; // 20px padding từ cạnh phải
            
            Rectangle playBtnRect = new Rectangle(buttonsStartX, bounds.Y, 150, bounds.Height);
            Rectangle deleteBtnRect = new Rectangle(buttonsStartX + 150, bounds.Y, 150, bounds.Height);
            
            bool isHoverPlayBtn = playBtnRect.Contains(ms.Position);
            bool isHoverDeleteBtn = deleteBtnRect.Contains(ms.Position);

            // Shadow
            sb.Draw(_pixel, new Rectangle(bounds.X + 8, bounds.Y + 8, bounds.Width, bounds.Height), Color.Black * 0.5f);

            // Main Box
            sb.Draw(_pixel, bounds, NES_BLACK);
            DrawBorder(sb, bounds, 4, NES_WHITE);

            // Thumbnail
            Rectangle thumb = new Rectangle(bounds.X + 8, bounds.Y + 8, 220, bounds.Height - 16);
            if (_thumbnailPlaceholder != null) sb.Draw(_thumbnailPlaceholder, thumb, Color.White);
            else sb.Draw(_pixel, thumb, NES_GRAY);
            DrawBorder(sb, thumb, 2, NES_WHITE);

            // Tag World
            sb.DrawString(_font, $"WORLD {slot.CurrentLevel}-1", new Vector2(thumb.X + 5, thumb.Bottom - 20), NES_WHITE, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

            // Info
            int infoX = thumb.Right + 20;

            // File Tag (A, B, C...)
            string fileTag = $"FILE {(char)('A' + (index % 26))}";
            sb.Draw(_pixel, new Rectangle(infoX, bounds.Y + 20, 60, 20), NES_RED);
            sb.DrawString(_font, fileTag, new Vector2(infoX + 5, bounds.Y + 24), NES_WHITE, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

            // Name
            sb.DrawString(_font, slot.PlayerName.ToUpper(), new Vector2(infoX + 70, bounds.Y + 20), NES_WHITE, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

            // Date
            sb.DrawString(_font, slot.LastPlayed.ToString("yyyy-MM-dd HH:mm"), new Vector2(playBtnRect.Left - 180, bounds.Y + 25), NES_GRAY, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

            // Stats
            sb.DrawString(_font, $"SCORE: {slot.Score:D6}", new Vector2(infoX, bounds.Y + 60), NES_GRAY, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            sb.DrawString(_font, $"LIVES: {slot.Lives}", new Vector2(infoX + 250, bounds.Y + 60), NES_RED, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

            // Progress Bar
            int barY = bounds.Y + 100;
            int barW = playBtnRect.Left - infoX - 40;
            sb.DrawString(_font, $"{(int)(slot.ProgressPercent * 100)}%", new Vector2(infoX + barW + 5, barY), NES_GRAY, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0f);

            Rectangle barBg = new Rectangle(infoX, barY, barW, 10);
            sb.Draw(_pixel, barBg, NES_GRAY * 0.3f);
            DrawBorder(sb, barBg, 1, NES_WHITE);
            int fill = (int)(barBg.Width * slot.ProgressPercent);
            if (fill > 0) sb.Draw(_pixel, new Rectangle(barBg.X, barBg.Y, fill, barBg.Height), NES_RED);

            // PLAY Button (bên trái)
            sb.Draw(_pixel, playBtnRect, isHoverPlayBtn ? NES_RED : NES_WHITE);
            sb.Draw(_pixel, new Rectangle(playBtnRect.X, playBtnRect.Y, 4, playBtnRect.Height), NES_WHITE);
            Color playTextColor = isHoverPlayBtn ? NES_WHITE : NES_BLACK;
            sb.DrawString(_font, "PLAY", new Vector2(playBtnRect.Center.X - 30, playBtnRect.Center.Y - 10), playTextColor, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

            // DELETE Button (bên phải)
            sb.Draw(_pixel, deleteBtnRect, isHoverDeleteBtn ? new Color(150, 0, 0) : new Color(100, 100, 100));
            sb.Draw(_pixel, new Rectangle(deleteBtnRect.X, deleteBtnRect.Y, 4, deleteBtnRect.Height), new Color(200, 100, 100));
            Color deleteTextColor = isHoverDeleteBtn ? Color.White : NES_GRAY;
            sb.DrawString(_font, "DELETE", new Vector2(deleteBtnRect.Center.X - 35, deleteBtnRect.Center.Y - 10), deleteTextColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        private void DrawCreateButton(SpriteBatch sb, Rectangle bounds)
        {
            MouseState ms = Mouse.GetState();
            bool isHover = bounds.Contains(ms.Position);

            sb.Draw(_pixel, bounds, isHover ? (NES_RED * 0.2f) : (NES_GRAY * 0.1f));
            DrawBorder(sb, bounds, 4, isHover ? NES_RED : NES_GRAY);

            string txt = "+ CREATE NEW WORLD";
            Vector2 sz = _font.MeasureString(txt) * 0.6f;
            sb.DrawString(_font, txt, new Vector2(bounds.Center.X - sz.X / 2, bounds.Center.Y - sz.Y / 2), isHover ? NES_RED : NES_GRAY, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
        }

        private void DrawInputPopup(SpriteBatch sb)
        {
            sb.Draw(_pixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.8f);
            Rectangle box = new Rectangle(340, 260, 600, 200);
            sb.Draw(_pixel, box, NES_BLACK);
            DrawBorder(sb, box, 4, NES_WHITE);
            sb.DrawString(_font, "NAME YOUR WORLD", new Vector2(box.X + 160, box.Y + 30), NES_RED);

            Rectangle inputRect = new Rectangle(box.X + 50, box.Y + 80, 500, 50);
            sb.Draw(_pixel, inputRect, Color.DarkGray * 0.2f);
            DrawBorder(sb, inputRect, 2, NES_GRAY);

            string display = _inputName + ((int)(_cursorTimer * 2) % 2 == 0 ? "_" : "");
            sb.DrawString(_font, display, new Vector2(inputRect.X + 20, inputRect.Y + 15), NES_WHITE);
            sb.DrawString(_font, "[ENTER] CREATE    [ESC] CANCEL", new Vector2(box.X + 80, box.Y + 150), NES_GRAY, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        private void DrawBorder(SpriteBatch sb, Rectangle r, int t, Color c)
        {
            sb.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, t), c);
            sb.Draw(_pixel, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
            sb.Draw(_pixel, new Rectangle(r.X, r.Y, t, r.Height), c);
            sb.Draw(_pixel, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
        }
    }
}