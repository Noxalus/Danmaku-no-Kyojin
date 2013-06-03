using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Screens;
using Danmaku_no_Kyojin.Utils;
using Danmaku_no_Kyojin.Camera;

namespace Danmaku_no_Kyojin
{
    public class DnK : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        readonly GameStateManager _stateManager;

        // Screens
        public TitleScreen TitleScreen;
        public ImprovementScreen ImprovementScreen;
        public LeaderboardScreen LeaderboardScreen;
        public OptionsScreen OptionsScreen;
        public GameConfigurationScreen GameConfigurationScreen;
        public GameplayScreen GameplayScreen;
        public GameOverScreen GameOverScreen;

        public Rectangle ScreenRectangle;

        public static Texture2D Pixel;

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

            _stateManager = new GameStateManager(this);
            Components.Add(_stateManager);

            // Screens
            TitleScreen = new TitleScreen(this, _stateManager);
            GameConfigurationScreen = new GameConfigurationScreen(this, _stateManager);
            GameplayScreen = new GameplayScreen(this, _stateManager);
            LeaderboardScreen = new LeaderboardScreen(this, _stateManager);
            ImprovementScreen = new ImprovementScreen(this, _stateManager);
            GameOverScreen = new GameOverScreen(this, _stateManager);
            OptionsScreen = new OptionsScreen(this, _stateManager);

            _stateManager.ChangeState(TitleScreen);

            // FPS
            Components.Add(new FrameRateCounter(this));
        }


        protected override void Initialize()
        {
            Camera = new Camera2D(GraphicsDevice.Viewport, 1000, 1000, 1);

            StaticClassSerializer.Load(typeof(PlayerData), "data.bin");

            PlayerData.LivesNumberIndex = 4;
            PlayerData.BulletTimeEnabled = true;

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            StaticClassSerializer.Save(typeof(PlayerData), "data.bin");

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Pixel = Content.Load<Texture2D>("Graphics/Pictures/pixel");
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
