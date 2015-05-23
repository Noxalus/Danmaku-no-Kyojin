using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private List<BulletPattern> _myPatterns = new List<BulletPattern>();
        private List<string> _patternNames = new List<string>();
        private int _currentPattern = 0;

        // Random
        public static readonly Random Rand = new Random();

        // Background
        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;

        public PatternTestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _bulletInitialPosition = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 3f);
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
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _backgroundImage = GameRef.Content.Load<Texture2D>("Graphics/Pictures/background");

            //Get all the xml files
            foreach (var source in Directory.GetFiles(@"Content\XML\Tests", "*.xml", SearchOption.AllDirectories))
            {
                //store the name
                _patternNames.Add(source);

                //load the pattern
                var pattern = new BulletPattern();
                pattern.ParseXML(source);
                _myPatterns.Add(pattern);
            }

            _currentPattern = 0;
            AddBullet(true);
        }

        protected override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();

            if (InputHandler.PressedCancel())
            {
                UnloadContent();
                StateManager.ChangeState(GameRef.TitleScreen);
            }

            base.Update(gameTime);

            _player.Update(gameTime);

            // Fire the pattern when there is no bullet on the screen
            if (_moverManager.movers.Count < 1)
                AddBullet();

            _moverManager.Update(gameTime);
            _moverManager.FreeMovers();
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            var backgroundColor = Color.CornflowerBlue;
            GraphicsDevice.Clear(backgroundColor);

            // Draw game arena background
            GameRef.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, _player.Camera.GetTransformation());
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            GameRef.SpriteBatch.End();
            
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _player.Camera.GetTransformation());

            _player.Draw(gameTime);

            foreach (var mover in _moverManager.movers)
            {
                mover.Draw(gameTime);
            }

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
        }

        private void AddBullet(bool clear = false)
        {
            if (clear)
            {
                //clear out all the bulelts
                _moverManager.movers.Clear();
            }

            //add a new bullet in the center of the screen
            var mover = (Mover)_moverManager.CreateBullet();
            mover.X = _bulletInitialPosition.X;
            mover.Y = _bulletInitialPosition.Y - 5;
            mover.SetBullet(_myPatterns[_currentPattern].RootNode);
        }
    }
}
