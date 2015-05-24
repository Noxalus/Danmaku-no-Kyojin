
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Shapes;
using Microsoft.Xna.Framework;

namespace Danmaku_no_Kyojin.Entities
{
    class BossStructure
    {
        private enum Direction { Up, Down, Right, Left, UpLeft, DownLeft, UpRight, DownRight };
        private enum Symmetry { Vertical, Horizontal };

        private DnK _gameRef;
        public List<Vector2> Vertices { get; private set; }

        private PolygonShape _polygonShape;
        private int _iteration;
        private float _step;
        private Vector2 _size;

        public Vector2 Origin { get; private set; }



        public Vector2 GetSize()
        {
            return _size;
        }

        public BossStructure(DnK game, int iteration = 50, float step = 25)
        {
            _gameRef = game;
            Vertices = new List<Vector2>();

            GenerateStructure();
        }

        public void GenerateStructure()
        {
            Vertices.Clear();
            _size = new Vector2();
            var minY = 0f;
            var maxY = 0f;

            var possibleDirections = new List<Direction>()
            {
                Direction.Up, Direction.Down, Direction.Right, Direction.UpRight, Direction.DownRight
            };

            var vertexPosition = Vector2.Zero;

            // Generate the bottom left part
            var bottomLeftVertices = new List<Vector2>() { vertexPosition };
            for (var i = 1; i < _iteration; i++)
            {
                if (vertexPosition.Y <= _step)
                {
                    possibleDirections.Remove(Direction.Up);
                    possibleDirections.Remove(Direction.UpRight);
                    possibleDirections.Remove(Direction.Right);
                }

                if (i == _iteration - 1)
                {
                    possibleDirections.Remove(Direction.Down);
                    possibleDirections.Remove(Direction.Up);
                }

                vertexPosition = GenerateRandomPosition(vertexPosition, ref possibleDirections);

                if (vertexPosition.Y > maxY)
                    maxY = vertexPosition.Y;

                bottomLeftVertices.Add(vertexPosition);
            }

            _size.X = vertexPosition.X * 2;

            // Perform Y-axis symetry
            var bottomRightVertices = GenerateSymmetry(bottomLeftVertices, Symmetry.Vertical);
            
            // Generate the top part
            vertexPosition = Vector2.Zero;
            var topLeftVertices = new List<Vector2> { vertexPosition };

            while (vertexPosition.X < _size.X / 2)
            {
                if (vertexPosition.Y >= -_step)
                {
                    possibleDirections.Remove(Direction.Down);
                    possibleDirections.Remove(Direction.DownRight);
                    possibleDirections.Remove(Direction.Right);
                }

                vertexPosition = GenerateRandomPosition(vertexPosition, ref possibleDirections);

                if (vertexPosition.Y < minY)
                    minY = vertexPosition.Y;

                topLeftVertices.Add(vertexPosition);
            }

            _size.Y = Math.Abs(minY - maxY);
            Origin = new Vector2(_size.X / 2f, _size.Y / 2f);

            // Perform Y-axis symetry
            var topRightVertices = GenerateSymmetry(topLeftVertices, Symmetry.Vertical);
            topRightVertices.Add(new Vector2(0f, _size.X));

            // We need counterclockwise order
            topLeftVertices.Reverse();
            topRightVertices.Reverse();

            Vertices.AddRange(topRightVertices);
            Vertices.AddRange(topLeftVertices);
            Vertices.AddRange(bottomLeftVertices);
            Vertices.AddRange(bottomRightVertices);

            _polygonShape = new PolygonShape(_gameRef.GraphicsDevice, Vertices.ToArray());
        }

        private Vector2 GenerateRandomPosition(Vector2 position, ref List<Direction> possibleDirections)
        {
            var vertex = position;

            var randomDirectionIndex = _gameRef.Rand.Next(possibleDirections.Count);
            var randomDirection = possibleDirections[randomDirectionIndex];

            possibleDirections = new List<Direction>()
            {
                Direction.Up, Direction.Down, Direction.Right, Direction.UpRight, Direction.DownRight
            };

            switch (randomDirection)
            {
                case Direction.Up:
                    vertex.Y -= _step;

                    // We don't want to go down for the next step
                    possibleDirections.Remove(Direction.Down);
                    break;
                case Direction.UpRight:
                    vertex.X += _step;
                    vertex.Y -= _step;
                    break;
                case Direction.Right:
                    vertex.X += _step;
                    break;
                case Direction.DownRight:
                    vertex.X += _step;
                    vertex.Y += _step;
                    break;
                case Direction.Down:
                    vertex.Y += _step;

                    // We don't want to go up for the next step
                    possibleDirections.Remove(Direction.Up);
                    break;
            }

            return vertex;
        }

        private List<Vector2> GenerateSymmetry(List<Vector2> vertices, Symmetry symmetry)
        {
            var symmetricVertices = new List<Vector2>();

            switch (symmetry)
            {
                case Symmetry.Vertical:
                    var width = vertices[vertices.Count - 1].X - vertices[0].X;
                    var initialX = vertices[vertices.Count - 1];

                    for (var i = vertices.Count - 1; i >= 0; i--)
                    {
                        var position = vertices[i];
                        
                        // We want to avoid duplicates
                        if (Math.Abs(position.X - initialX.X) < 0f)
                            continue;

                        position.X = (width * 2) - position.X;
                        symmetricVertices.Add(position);
                    }
                    break;
                case Symmetry.Horizontal:
                    // TODO: Horizontal symmetry
                    break;
            }

            return symmetricVertices;
        }

        public  void Iterate(int iterationNumber = 1)
        {
        }

        public void Draw(Matrix viewMatrix, Vector2 position, Vector2 origin, float rotation, Vector2 scale)
        {
            _polygonShape.Draw(viewMatrix, position, origin, rotation, scale);
        }
    }
}
