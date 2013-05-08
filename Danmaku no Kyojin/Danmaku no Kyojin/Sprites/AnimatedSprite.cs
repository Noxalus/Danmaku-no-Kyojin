using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Sprites
{
    public class AnimatedSprite
    {
        #region Field Region

        readonly Dictionary<AnimationKey, Animation> _animations;
        AnimationKey _currentAnimation;
        bool _isAnimating;

        readonly Texture2D _texture;
        Vector2 _position;
        Vector2 _velocity;
        float _speed = 2.5f;

        readonly Animation _animation;

        readonly bool _controlable;

        #endregion

        #region Property Region

        public AnimationKey CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; }
        }

        public Animation[] Animations
        {
            get
            {
                var anim = new Animation[_animations.Count];
                int i = 0;
                foreach (Animation a in _animations.Values)
                {
                    anim[i] = a;
                    i++;
                }
                return anim;
            }
        }

        public Animation Animation
        {
            get { return _animation; }
        }

        public bool IsAnimating
        {
            get { return _isAnimating; }
            set { _isAnimating = value; }
        }

        public int Width
        {
            get
            {
                if (_controlable)
                    return _animations[_currentAnimation].FrameWidth;
                else
                    return _animation.FrameWidth;
            }
        }

        public int Height
        {
            get
            {
                if (_controlable)
                    return _animations[_currentAnimation].FrameHeight;
                else
                    return _animation.FrameHeight;
            }
        }

        public Point Dimension
        {
            get { return new Point(Width, Height); }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = MathHelper.Clamp(value, 0, 10); }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            { _position = value; }
        }

        public float PositionX
        {
            set { _position.X = value; }
            get { return _position.X; }
        }

        public float PositionY
        {
            set { _position.Y = value; }
            get { return _position.Y; }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set
            {
                _velocity = value;
                if (_velocity != Vector2.Zero)
                    _velocity.Normalize();
            }
        }

        #endregion

        #region Constructor Region

        public AnimatedSprite(Texture2D sprite, Dictionary<AnimationKey, Animation> animation, Vector2 position)
        {
            _texture = sprite;
            _animations = new Dictionary<AnimationKey, Animation>();

            foreach (AnimationKey key in animation.Keys)
                _animations.Add(key, (Animation)animation[key].Clone());

            _controlable = true;
            this._position = position;
        }

        public AnimatedSprite(Texture2D sprite, Animation animation, Vector2 position)
        {
            _texture = sprite;
            this._animation = animation;
            _controlable = false;
            this._position = position;
        }

        #endregion

        #region XNA Method Region

        public void Update(GameTime gameTime)
        {
            if (_controlable)
            {
                if (_isAnimating)
                    _animations[_currentAnimation].Update(gameTime);
                else
                    _animations[_currentAnimation].Reset();
            }
            else
            {
                if (_isAnimating)
                    _animation.Update(gameTime);
                else
                    _animation.Reset();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle? sourceRectangle = _controlable ? _animations[_currentAnimation].CurrentFrameRect : _animation.CurrentFrameRect;

            spriteBatch.Draw(
            _texture,
            new Vector2(_position.X, _position.Y),
            sourceRectangle,
            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        #endregion

        #region Method Regions

        public void LockToMap()
        {
            _position.X = MathHelper.Clamp(_position.X, 0, -Width);
            _position.Y = MathHelper.Clamp(_position.Y, 0, -Height);
        }

        public void ChangeFramesPerSecond(int newValue)
        {
            if (_controlable)
            {
                foreach (Animation a in _animations.Values)
                    a.FramesPerSecond = newValue;
            }
            else
                _animation.FramesPerSecond = newValue;
        }

        public void ChangePosition(Vector2 p)
        {
            _position = p;
        }

        public void ChangePosition(float x, float y)
        {
            _position.X = x;
            _position.Y = y;
        }
        #endregion
    }
}
