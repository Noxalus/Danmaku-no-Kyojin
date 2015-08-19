using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class PatternTestScreen : BaseGameState
    {
        // Camera
        Viewport _defaultView;

        private Player _player;
        private Vector2 _bulletInitialPosition;

        MoverManager _moverManager;
        private readonly List<BulletPattern> _myPatterns = new List<BulletPattern>();
        private readonly List<string> _patternNames = new List<string>();
        private readonly String _patternDirectory;
        private readonly String _patternFileName;
        private readonly FileInfo _patternFile;
        private int _currentPattern = 0;
        private FileSystemWatcher _watcher;

        // Random
        public static readonly Random Rand = new Random();

        // Background
        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;

        public PatternTestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _bulletInitialPosition = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 3f);
            _patternDirectory = @"Content\Data\Patterns\";
            _patternFileName = "test.xml";
            _patternFile = new FileInfo(_patternDirectory + _patternFileName);
        }

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.GameArea.X, Config.GameArea.Y);

            // Bullet manager
            _moverManager = new MoverManager((DnK)Game);
            GameManager.GameDifficulty = Config.GameDifficultyDelegate;

            base.Initialize();

            _defaultView = GraphicsDevice.Viewport;

            // First player
            _player = new Player(GameRef, _defaultView, 1, Config.PlayersController[0], new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y - 150));
            _player.Initialize();

            _moverManager.Initialize(_player.GetPosition);
            _moverManager.movers.Clear();

            AddBullet(true);

            // Watch the bullet pattern file
            _watcher = new FileSystemWatcher
            {
                Path = _patternDirectory,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = _patternFileName
            };

            // Add event handler
            _watcher.Changed += OnChanged;

            // Begin watching
            _watcher.EnableRaisingEvents = true;

        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                _watcher.EnableRaisingEvents = false;
             
                // Wait until the file is not in used
                while (IsFileLocked(_patternFile)) { Thread.Sleep(10); }

                LoadPatternFile();
                AddBullet();
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _backgroundImage = GameRef.Content.Load<Texture2D>("Graphics/Pictures/background");

            LoadPatternFile();
        }

        private void LoadPatternFile()
        {
            _myPatterns.Clear();
            _patternNames.Clear();
            
            _patternNames.Add("Test");
            var pattern = new BulletPattern();
            pattern.ParseXML(_patternDirectory + _patternFileName);
            _myPatterns.Add(pattern);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.PressedCancel())
            {
                UnloadContent();
                StateManager.ChangeState(GameRef.TitleScreen);
            }

            HandleInput();

            _player.Update(gameTime);

            _moverManager.Update(gameTime);
            _moverManager.FreeMovers();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw game arena background
            GameRef.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, _player.Camera.GetTransformation());
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            GameRef.SpriteBatch.End();
            
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _player.Camera.GetTransformation());

            _player.Draw(gameTime);

            _moverManager.Draw(gameTime);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            // Text
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
            "Bullets: " +
            _moverManager.movers.Count.ToString(CultureInfo.InvariantCulture),
            new Vector2(1, 21), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
            "Bullets: " +
            _moverManager.movers.Count.ToString(CultureInfo.InvariantCulture),
            new Vector2(0, 20), Color.White);

            GameRef.SpriteBatch.End();
        }

        private void HandleInput()
        {
            if (Config.Debug && InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;
            else if (InputHandler.KeyPressed(Keys.LeftControl))
                AddBullet();
            else if (InputHandler.KeyDown(Keys.Space))
                AddBullet();
            else if (InputHandler.KeyPressed(Keys.R))
                _moverManager.movers.Clear();
            else if (InputHandler.KeyPressed(Keys.E))
                System.Diagnostics.Process.Start(_patternDirectory + _patternFileName);
        }

        private void AddBullet(bool clear = false)
        {
            if (clear)
                _moverManager.movers.Clear();

            //add a new bullet in the center of the screen
            var mover = (Mover)_moverManager.CreateBullet();
            mover.X = _bulletInitialPosition.X;
            mover.Y = _bulletInitialPosition.Y;
            mover.SetBullet(_myPatterns[_currentPattern].RootNode);
        }
    }
}
