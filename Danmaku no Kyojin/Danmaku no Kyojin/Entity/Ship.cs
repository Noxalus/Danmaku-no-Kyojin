using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Entity
{
    class Ship
    {
        private DnK _gameRef;

        private Texture2D _sprite;
        private Vector2 _position;
        private float _velocity;
        private bool _slowMode;
        private float _velocitySlowMode;

        public Ship(DnK gameRef, Vector2 position)
        {
            _gameRef = gameRef;
            _position = position;
            _velocity = 5f;
            _velocitySlowMode = 2.5f;
        }

        public void LoadContent(ContentManager content)
        {
            _sprite = content.Load<Texture2D>("Graphics/Charsets/ship2");
            _position.X = _position.X - _sprite.Width;
        }

        public void Update(GameTime gameTime)
        {
            Point motion = Point.Zero;

            if (InputHandler.KeyDown(Keys.Right))
                motion.X = 1;
            if (InputHandler.KeyDown(Keys.Left))
                motion.X = -1;
            if (InputHandler.KeyDown(Keys.Up))
                motion.Y = -1;
            if (InputHandler.KeyDown(Keys.Down))
                motion.Y = 1;

            _slowMode = InputHandler.KeyDown(Keys.LeftShift) ? true : false;

            UpdatePosition(motion);
        }

        private void UpdatePosition(Point motion)
        {
            if (_slowMode)
            {
                _position.X += motion.X * _velocitySlowMode;
                _position.Y += motion.Y * _velocitySlowMode;
            }
            else
            {
                _position.X += motion.X * _velocity;
                _position.Y += motion.Y * _velocity;
            }
        }

        public void Draw()
        {
            _gameRef.SpriteBatch.Draw(_sprite, _position, Color.White);
        }
    }
}
