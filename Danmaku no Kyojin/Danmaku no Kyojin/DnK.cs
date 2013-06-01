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
using Danmaku_no_Kyojin.Camera;
using Danmaku_no_Kyojin.Shaders;

namespace Danmaku_no_Kyojin
{
    public class DnK : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        GameStateManager stateManager;

        // Screens
        public TitleScreen TitleScreen;
        public LeaderboardScreen LeaderboardScreen;
        public OptionsScreen OptionsScreen;
        public GameplayScreen GameplayScreen;

        public Rectangle ScreenRectangle;

        public static Texture2D _pixel;

        // Camera
        public Camera2D Camera;

        public DnK()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Config.Resolution.X,
                PreferredBackBufferHeight = Config.Resolution.Y
            };

            ScreenRectangle = new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y);

            Graphics.IsFullScreen = Config.FullScreen;

            Graphics.SynchronizeWithVerticalRetrace = true;

            // Pass through the FPS capping (60 FPS)
            if (!Config.FpsCapping)
            {
                IsFixedTimeStep = false;
                Graphics.SynchronizeWithVerticalRetrace = false;
            }

            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            Components.Add(new InputHandler(this));

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            // Screens
            TitleScreen = new TitleScreen(this, stateManager);
            GameplayScreen = new GameplayScreen(this, stateManager);
            LeaderboardScreen = new LeaderboardScreen(this, stateManager);
            OptionsScreen = new OptionsScreen(this, stateManager);

            stateManager.ChangeState(TitleScreen);

            // FPS
            Components.Add(new FrameRateCounter(this));
        }


        protected override void Initialize()
        {
            Camera = new Camera2D(GraphicsDevice.Viewport, 1000, 1000, 1);

            bool test1 = Improvements.BulletTimeEnabled;

            StaticClassSerializer.Load(typeof(Improvements), "test.dat");

            bool test2 = Improvements.BulletTimeEnabled;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = Content.Load<Texture2D>("Graphics/Pictures/pixel");
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
