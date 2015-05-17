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
    public class TestScreen : BaseGameState
    {
        // Camera
        Viewport _defaultView;

        private Texture2D _pixel;

        private List<Player> Players { get; set; }

        // Random
        public static readonly Random Rand = new Random();

        // Background
        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;
        private Rectangle _backgroundTopRectangle;

        private PolygonShape _polygonShape;
        private Vector2[] _vertices;

        private bool _wireframeMode;

        public TestScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            Players = new List<Player>();
        }

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.GameArea.X, Config.GameArea.Y);
            _backgroundTopRectangle = new Rectangle(0, -Config.GameArea.Y, Config.GameArea.X, Config.GameArea.Y);

            base.Initialize();

            _defaultView = GraphicsDevice.Viewport;

            Players.Clear();

            // First player
            var player1 = new Player(GameRef, _defaultView, 1, Config.PlayersController[0], new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y - 150));
            player1.Initialize();
            Players.Add(player1);

            GenerateNewPolygonShape();

            _wireframeMode = false;
        }

        protected override void LoadContent()
        {
            _backgroundImage = Game.Content.Load<Texture2D>("Graphics/Pictures/background");
            _pixel = Game.Content.Load<Texture2D>("Graphics/Pictures/pixel");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Move the background
            if (_backgroundMainRectangle.Y >= Config.Resolution.Y)
                _backgroundMainRectangle.Y = _backgroundTopRectangle.Y - Config.Resolution.Y;
            if (_backgroundTopRectangle.Y >= Config.Resolution.Y)
                _backgroundTopRectangle.Y = _backgroundMainRectangle.Y - Config.Resolution.Y;

            HandleInput();

            if (InputHandler.PressedCancel())
            {
                UnloadContent();
                StateManager.ChangeState(GameRef.TitleScreen);
            }

            base.Update(gameTime);

            foreach (Player p in Players)
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
                    }

                    p.Update(gameTime);
                }
            }

            _polygonShape.Update(gameTime);

            if (Config.Debug && InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;
        }

        public override void Draw(GameTime gameTime)
        {

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin(0, BlendState.Opaque);

            GameRef.SpriteBatch.End();

            GameRef.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            var backgroundColor = new Color(5, 5, 5);
            GraphicsDevice.Clear(backgroundColor);

            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Players[0].Camera.GetTransformation());

            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);

            foreach (var bullet in Players[0].GetBullets())
                bullet.Draw(gameTime);

            Players[0].Draw(gameTime);
            
            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            // Text
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont,
            "Position: " + Players.First().GetPosition().ToString(),
            new Vector2(0, 20), Color.White);

            PrintPolygonShapeVerticesPosition();

            _polygonShape.Draw(Players[0].Camera.GetTransformation(), _wireframeMode);

            GameRef.SpriteBatch.End();
        }

        /// <summary>
        /// Handles input for quitting or changing the bloom settings.
        /// </summary>
        private void HandleInput()
        {
            if (InputHandler.KeyDown(Keys.Space))
            {
                // Update polygon shape
                GenerateNewPolygonShape();
            }
            else if (InputHandler.KeyPressed(Keys.W))
            {
                _wireframeMode = !_wireframeMode;
            }
        }

        private void GenerateNewPolygonShape()
        {
            var iteration = 50;
            var step = 20;
            var bossPosition = new Vector2(0, 0);

            _vertices = new Vector2[((iteration) + 2) * 2];
            List<int> possibleDirections = new List<int>()
            {
                // 0 => Up, 1 => Up diagonal, 2 => Right, 3 => Down diagonal, 4 => Down
                0, 1, 2, 3, 4
            };

            var vertexPosition = new Vector2(0f, 0f);
            _vertices[0] = vertexPosition;

            for (int i = 1; i < (_vertices.Length - 1) / 2; i++)
            {
                if (vertexPosition.Y <= step)
                {
                    possibleDirections.Remove(0);
                    possibleDirections.Remove(1);
                    possibleDirections.Remove(2);
                }

                var randomDirectionIndex = Rand.Next(possibleDirections.Count);
                var randomDirection = possibleDirections[randomDirectionIndex];

                // Reset the list of possible directions
                possibleDirections = new List<int> { 0, 1, 2, 3, 4 };

                // Up
                if (randomDirection == 0)
                {
                    vertexPosition.Y -= step;

                    // We don't want to go down for the next step
                    possibleDirections.Remove(4);
                }
                // Up diagonal
                else if (randomDirection == 1)
                {
                    vertexPosition.X += step;
                    vertexPosition.Y -= step;
                }
                // Right
                else if (randomDirection == 2)
                {
                    vertexPosition.X += step;
                }
                // Down diagonal
                else if (randomDirection == 3)
                {
                    vertexPosition.X += step;
                    vertexPosition.Y += step;
                }
                // Down
                else if (randomDirection == 4)
                {
                    vertexPosition.Y += step;

                    // We don't want to go up for the next step
                    possibleDirections.Remove(0);
                }

                _vertices[i] = vertexPosition;
            }

            var origin = new Vector2(vertexPosition.X, vertexPosition.Y/2);


            _vertices[(_vertices.Length / 2) - 1] = new Vector2(vertexPosition.X, 0f);

            var bossWidth = vertexPosition.X;
            // Perform Y-axis symetry
            for (int i = (_vertices.Length - 1) / 2; i >= 0; i--)
            {
                var position = _vertices[i];
                position.X = (bossWidth * 2) - position.X;

                _vertices[((_vertices.Length / 2) + (_vertices.Length - 1) / 2) - i] = position;
            }

            _polygonShape = new PolygonShape(GameRef.GraphicsDevice, _vertices);
            _polygonShape.Origin(origin);
            _polygonShape.Position(new Vector2(
                (Config.GameArea.X / 2f),
                Config.GameArea.Y / 4f
            ));
        }

        private void PrintPolygonShapeVerticesPosition()
        {
            var counter = 0;
            Vector2 previousVertex = _vertices[0];
            foreach (var vertex in _vertices)
            {
                var s = vertex.X + ", " + vertex.Y;

                if (counter > 0)
                {
                    if (previousVertex.X != vertex.X)
                    {
                        // Left
                        if (previousVertex.Y == vertex.Y)
                            s += " (Right)";
                        else if (previousVertex.Y > vertex.Y)
                            s += " (Diagonal up)";
                        else if (previousVertex.Y < vertex.Y)
                            s += " (Diagonal down)";
                    }
                    else
                    {
                        if (previousVertex.Y > vertex.Y)
                            s += " (Up)";
                        else if (previousVertex.Y < vertex.Y)
                            s += " (Down)";
                    }
                }

                GameRef.SpriteBatch.DrawString(
                    ControlManager.SpriteFont,
                    s,
                    new Vector2(0, 60 + (counter * 20)), 
                    Color.White
                );

                previousVertex = vertex;
                counter++;
            }
        }
    }
}
