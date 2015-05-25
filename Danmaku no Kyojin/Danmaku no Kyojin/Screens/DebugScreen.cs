using System;
using System.Collections.Generic;
using System.Linq;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Danmaku_no_Kyojin.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class DebugScreen : BaseGameState
    {
        // Camera
        Viewport _defaultView;

        private Texture2D _pixel;

        private List<Player> Players { get; set; }

        private Boss _boss;

        // Random
        public static readonly Random Rand = new Random();

        // Background
        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;

        public DebugScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            Players = new List<Player>();
        }

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.GameArea.X, Config.GameArea.Y);

            base.Initialize();

            _defaultView = GraphicsDevice.Viewport;

            Players.Clear();

            // First player
            var player1 = new Player(GameRef, _defaultView, 1, Config.PlayersController[0], new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y - 150));
            player1.Initialize();
            Players.Add(player1);

            _boss = new Boss(GameRef, Players, 4);
            _boss.Initialize();
        }

        protected override void LoadContent()
        {
            _backgroundImage = Game.Content.Load<Texture2D>("Graphics/Pictures/background");

            base.LoadContent();
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

            foreach (var p in Players)
            {
                if (p.IsAlive)
                {
                    if (p.BulletTime)
                    {
                        var newGameTime = new GameTime(
                            gameTime.TotalGameTime,
                            new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks / Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex].Key))
                        );

                        gameTime = newGameTime;
                    }

                    for (int i = 0; i < p.GetBullets().Count; i++)
                    {
                        p.GetBullets()[i].Update(gameTime);

                        if (_boss.Intersects(p.GetBullets()[i]))
                        {
                            _boss.TakeDamage(p.GetBullets()[i].Power);

                            p.GetBullets().Remove(p.GetBullets()[i]);
                            continue;
                        }

                        if (p.GetBullets()[i].X < 0 || p.GetBullets()[i].X > Config.GameArea.X ||
                            p.GetBullets()[i].Y < 0 || p.GetBullets()[i].Y > Config.GameArea.Y)
                        {
                            p.GetBullets().Remove(p.GetBullets()[i]);
                            continue;
                        }

                        // Collision with turrets
                        for (int j = 0; j < _boss.Turrets.Count; j++)
                        {
                            if (_boss.Turrets[j].Intersects(p.GetBullets()[i]))
                            {
                                _boss.Turrets[j].Color = Color.Blue;
                                _boss.DestroyTurret(_boss.Turrets[j], p.GetBullets()[i]);
                                p.GetBullets().Remove(p.GetBullets()[i]);
                                break;
                            }
                        }
                    }

                    p.Update(gameTime);
                }
            }

            _boss.Update(gameTime);

            if (Config.Debug && InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            var backgroundColor = new Color(50, 50, 50);
            GraphicsDevice.Clear(backgroundColor);

            // Draw game arena background
            GameRef.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, Players[0].Camera.GetTransformation());
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            GameRef.SpriteBatch.End();

            GameRef.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Players[0].Camera.GetTransformation());

            Players[0].Draw(gameTime);

            _boss.Draw(gameTime, Players[0].Camera.GetTransformation());

            foreach (var bullet in Players[0].GetBullets())
                bullet.Draw(gameTime);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            // Text
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
            "Position: " + Players.First().GetPosition(),
            new Vector2(0, 20), Color.White);

            //_boss.PrintPolygonShapeVerticesPosition();

            GameRef.SpriteBatch.End();
        }

        /// <summary>
        /// Handles input for quitting or changing the bloom settings.
        /// </summary>
        private void HandleInput()
        {
            if (InputHandler.KeyDown(Keys.Space))
            {
            }
        }
    }
}
