using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioGame.Systems.Animation
{
    public class Animation
    {
        public string Name { get; set; }
        public Texture2D SpriteSheet { get; set; }
        public Rectangle[] Frames { get; set; }
        public float FrameDuration { get; set; }
        public bool Loop { get; set; } = true;

        private int _currentFrame = 0;
        private float _frameTimer = 0;
        public bool IsFinished { get; private set; }

        public Animation(string name, Texture2D spriteSheet, Rectangle[] frames, float frameDuration, bool loop = true)
        {
            Name = name;
            SpriteSheet = spriteSheet;
            Frames = frames;
            FrameDuration = frameDuration;
            Loop = loop;
        }

        public void Update(float deltaTime)
        {
            if (IsFinished && !Loop) return;

            _frameTimer += deltaTime;

            if (_frameTimer >= FrameDuration)
            {
                _frameTimer = 0;
                _currentFrame++;

                if (_currentFrame >= Frames.Length)
                {
                    if (Loop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = Frames.Length - 1;
                        IsFinished = true;
                    }
                }
            }
        }

        public Rectangle GetCurrentFrame()
        {
            return Frames[_currentFrame];
        }

        public void Reset()
        {
            _currentFrame = 0;
            _frameTimer = 0;
            IsFinished = false;
        }
    }
}
