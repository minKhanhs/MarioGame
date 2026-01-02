using System.Collections.Generic;

namespace MarioGame.Systems.Animation
{
    public class AnimationManager
    {
        private Dictionary<string, Animation> _animations;
        private Animation _currentAnimation;

        public AnimationManager()
        {
            _animations = new Dictionary<string, Animation>();
        }

        public void AddAnimation(Animation animation)
        {
            _animations[animation.Name] = animation;
        }

        public void Play(string animationName)
        {
            if (_animations.ContainsKey(animationName))
            {
                if (_currentAnimation != _animations[animationName])
                {
                    _currentAnimation = _animations[animationName];
                    _currentAnimation.Reset();
                }
            }
        }

        public void Update(float deltaTime)
        {
            _currentAnimation?.Update(deltaTime);
        }

        public Animation GetCurrentAnimation()
        {
            return _currentAnimation;
        }
    }
}