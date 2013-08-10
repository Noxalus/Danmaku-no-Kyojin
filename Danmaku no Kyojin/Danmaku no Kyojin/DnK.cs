using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Screens;
using Danmaku_no_Kyojin.Utils;
using Danmaku_no_Kyojin.Camera;
using Microsoft.Xna.Framework.Input;

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
        public KeyboardInputsScreen KeyboardInputsScreen;
        public GamepadInputsScreen GamepadInputsScreen;
        public GameConfigurationScreen GameConfigurationScreen;
        public GameplayScreen GameplayScreen;
        public GameOverScreen GameOverScreen;

        public Rectangle ScreenRectangle;

        public static Texture2D Pixel;

        // Audio
        public SoundEffect Select;
        public SoundEffect Choose;

        public DnK()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Config.Resolution.X,
                PreferredBackBufferHeight = Config.Resolution.Y
            };

            ScreenRectangle = new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y);

            IsMouseVisible = true;

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
            KeyboardInputsScreen = new KeyboardInputsScreen(this, _stateManager);
            GamepadInputsScreen = new GamepadInputsScreen(this, _stateManager);

            _stateManager.ChangeState(TitleScreen);

            // FPS
            Components.Add(new FrameRateCounter(this));
        }


        protected override void Initialize()
        {
            StaticClassSerializer.Load(typeof(PlayerData), "data.bin");

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            TitleScreen.Dispose();
            GameConfigurationScreen.Dispose();
            GameplayScreen.Dispose();
            LeaderboardScreen.Dispose();
            ImprovementScreen.Dispose();
            GameOverScreen.Dispose();
            OptionsScreen.Dispose();
            KeyboardInputsScreen.Dispose();
            GamepadInputsScreen.Dispose();

            _stateManager.Dispose();
            SpriteBatch.Dispose();


            StaticClassSerializer.Save(typeof(PlayerData), "data.bin");

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Pixel = Content.Load<Texture2D>("Graphics/Pictures/pixel");

            Select = Content.Load<SoundEffect>(@"Audio/SE/select");
            Choose = Content.Load<SoundEffect>(@"Audio/SE/choose");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.KeyPressed(Keys.F1) || InputHandler.ButtonPressed(Buttons.Start, PlayerIndex.One))
            {
                Config.FullScreen = !Config.FullScreen;
                Graphics.IsFullScreen = Config.FullScreen; 
                Graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
