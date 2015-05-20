using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    class Laser : Entity
    {
        // Sprite
        private Texture2D _borderTexture;
        private Texture2D _middleTexture;

        private Vector2 _start;
        private Vector2 _end;
        private float _thickness;

        public Laser(DnK game, Vector2 start, Vector2 end, float thickness = 1) : base(game)
        {
            _start = start;
            _end = end;
            _thickness = thickness;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _borderTexture = this.Game.Content.Load<Texture2D>("Graphics/Sprites/laserBorder");
            _middleTexture = this.Game.Content.Load<Texture2D>("Graphics/Sprites/laserMiddle");

            Origin = new Vector2(_borderTexture.Width, _borderTexture.Height / 2f);

            // TODO: Collision box

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 tangent = _end - _start;
            var rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            const float imageThickness = 8;
            float thicknessScale = _thickness / imageThickness;

            Vector2 capOrigin = new Vector2(_borderTexture.Width, _borderTexture.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, _middleTexture.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

            Color color = Color.White;

            Game.SpriteBatch.Draw(_middleTexture, _start, null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0f);
            Game.SpriteBatch.Draw(_borderTexture, _start, null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            Game.SpriteBatch.Draw(_borderTexture, _end, null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }
    }
}
