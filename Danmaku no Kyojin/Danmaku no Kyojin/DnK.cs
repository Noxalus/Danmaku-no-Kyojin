using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Danmaku_no_Kyojin.Entities;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Screens;
using Danmaku_no_Kyojin.Utils;

namespace Danmaku_no_Kyojin
{
    public class DnK : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        GameStateManager stateManager;

        // Screens
        public GameplayScreen GameplayScreen;

        public Rectangle ScreenRectangle;

        public DnK()
        {
            Point resolution = new Point(800, 600);
            bool isFullScreen = false;

            this.IsMouseVisible = true;

            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = resolution.X,
                PreferredBackBufferHeight = resolution.Y
            };

            ScreenRectangle = new Rectangle(0, 0, resolution.X, resolution.Y);

            Graphics.IsFullScreen = isFullScreen;

            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            Components.Add(new InputHandler(this));

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            // Screens
            GameplayScreen = new GameplayScreen(this, stateManager);

            stateManager.ChangeState(GameplayScreen);

            // FPS
            Components.Add(new FrameRateCounter(this));
        }


        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
