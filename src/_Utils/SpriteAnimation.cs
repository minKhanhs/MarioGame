using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Utils
{
    public class SpriteAnimation
    {
        public Texture2D Texture { get; private set; }

        // Kích thước 1 khung hình đơn lẻ (Ví dụ Mario là 32x32 hoặc 16x32)
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        // Số lượng khung hình trong animation này
        public int FrameCount { get; set; }

        // Tốc độ chuyển frame (giây). 0.1f = 10 hình/giây
        public float FrameTime { get; set; } = 0.1f;

        // Chỉ số dòng trong sprite sheet (nếu sheet có nhiều dòng hành động)
        public int RowIndex { get; set; } = 0;

        // Biến nội bộ để tính toán
        private int _currentFrame;
        private float _timer;
        public bool IsLooping { get; set; } = true;

        public SpriteAnimation(Texture2D texture, int frameCount, int frameWidth, int frameHeight, int rowIndex = 0)
        {
            Texture = texture;
            FrameCount = frameCount;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            RowIndex = rowIndex;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > FrameTime)
            {
                _timer = 0f;
                _currentFrame++;

                if (_currentFrame >= FrameCount)
                {
                    if (IsLooping)
                        _currentFrame = 0;
                    else
                        _currentFrame = FrameCount - 1; // Giữ ở frame cuối nếu không loop
                }
            }
        }

        // Tính toán hình chữ nhật cắt từ Sprite Sheet
        public Rectangle CurrentFrameSource
        {
            get
            {
                return new Rectangle(
                    _currentFrame * FrameWidth,  // X: Dịch chuyển theo số frame
                    RowIndex * FrameHeight,      // Y: Dịch chuyển theo dòng
                    FrameWidth,
                    FrameHeight
                );
            }
        }
    }
}
