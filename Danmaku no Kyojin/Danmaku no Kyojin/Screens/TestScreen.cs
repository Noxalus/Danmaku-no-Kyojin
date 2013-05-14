using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Screens
{
    public class TestScreen : BaseGameState
    {
        #region Members

		Texture2D texture;
		private Player _player;
		int timer = 0;
		Mover mover;

		MoverManager _moverManager;

		/// <summary>
		/// A list of all the bulletml samples we have loaded
		/// </summary>
		private List<BulletPattern> _myPatterns = new List<BulletPattern>();

		/// <summary>
		/// The names of all the bulletml patterns that are loaded, stored so we can display what is being fired
		/// </summary>
		private List<string> _patternNames = new List<string>();

		/// <summary>
		/// The current Bullet ML pattern to use to shoot bullets
		/// </summary>
		private int _currentPattern = 0;

        private List<BaseBullet> _bullets; 

		#endregion //Members

		#region Methods

        public TestScreen(Game game, GameStateManager manager)
            : base(game, manager)
		{
            _bullets = new List<BaseBullet>();

            _player = new Player(GameRef, 2, ref _bullets, new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 4));

			_moverManager = new MoverManager(_player.GetPosition);
		}

		public override void Initialize()
		{
			_player.Initialize();

			base.Initialize();
		}

		public float GetRank() { return 0; }

		protected override void LoadContent()
		{
			texture = Game.Content.Load<Texture2D>(@"Graphics/Sprites/ball");

			//Get all the xml files in the Content\\Samples directory
			foreach (var source in Directory.GetFiles(@"Content/XML/Samples", "*.xml"))
			{
				//store the name
				_patternNames.Add(source);

				//load the pattern
				BulletPattern pattern = new BulletPattern();
				pattern.ParseXML(source);
				_myPatterns.Add(pattern);
			}

			GameManager.GameDifficulty = this.GetRank;

			AddBullet();
		}

		protected override void UnloadContent()
		{
		}

		public override void Update(GameTime gameTime)
		{
            //ControlManager.Update(gameTime, PlayerIndex.One);

			//check input to increment/decrement the current bullet pattern
			if (InputHandler.KeyPressed(Keys.A))
			{
				//decrement the pattern
				if (0 >= _currentPattern)
				{
					//if it is at the beginning, move to the end
					_currentPattern = _myPatterns.Count - 1;
				}
				else
				{
					_currentPattern--;
				}

				AddBullet();
			}
			else if (InputHandler.KeyPressed(Keys.X))
			{
				//increment the pattern
				if ((_myPatterns.Count - 1) <= _currentPattern)
				{
					//if it is at the beginning, move to the end
					_currentPattern = 0;
				}
				else
				{
					_currentPattern++;
				}

				AddBullet();
			}
			else if (InputHandler.KeyPressed(Keys.LeftControl))
			{
				AddBullet();
			}

			timer++;
			if (timer > 60)
			{
				timer = 0;
				if (mover.used == false)
				{
					AddBullet();
				}
			}

			//????????Mover???s????????
			_moverManager.Update(gameTime);
			//?g????????????Mover??????
			_moverManager.FreeMovers();
			// ???@???X?V
			_player.Update(gameTime);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//?G???e???`??
			GameRef.SpriteBatch.Begin();

			Vector2 position = Vector2.Zero;

			//say what pattern we are shooting
			//_text.Write(_patternNames[_currentPattern], position, Justify.Left, 1.0f, Color.White, spriteBatch);
			//position.Y += _text.Font.MeasureString("test").Y;

			//how many bullets on the screen
			//_text.Write(_moverManager.movers.Count.ToString(), position, Justify.Left, 1.0f, Color.White, spriteBatch);

			foreach (Mover mover in _moverManager.movers)
                GameRef.SpriteBatch.Draw(texture, mover.pos, Color.White);

			_player.Draw(gameTime);

            GameRef.SpriteBatch.End();

			base.Draw(gameTime);
		}

		private void AddBullet()
		{
			//clear out all the bulelts
			_moverManager.movers.Clear();

			//add a new bullet in the center of the screen
			mover = (Mover)_moverManager.CreateBullet();
            mover.pos = new Vector2(GameRef.Graphics.PreferredBackBufferWidth / 2, GameRef.Graphics.PreferredBackBufferHeight / 2);
			mover.SetBullet(_myPatterns[_currentPattern].RootNode); //BulletML??????????????????
        }

        #endregion
    }
}