using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Utils
{
    public class FrameRateCounter : DrawableGameComponent
    {
        readonly ContentManager _content;
        SpriteBatch _spriteBatch;
        SpriteFont _spriteFont;

        int _frameRate = 0;
        int _frameCounter = 0;
        TimeSpan _elapsedTime = TimeSpan.Zero;


        public FrameRateCounter(Game game)
            : base(game)
        {
            // Draw after Screens's Draw method
            /*
            DrawOrder = 6000;
            _content = game.Content;
            */
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = _content.Load<SpriteFont>(@"Graphics\Fonts\ControlFont");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _content.Unload();

            base.UnloadContent();
        }


        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            string fps = string.Format("FPS: {0}", _frameRate);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);

            _spriteBatch.DrawString(_spriteFont, fps, new Vector2(1, 1), Color.Black);
            _spriteBatch.DrawString(_spriteFont, fps, new Vector2(0, 0), Color.White);

            _spriteBatch.End();
        }
    }
}
