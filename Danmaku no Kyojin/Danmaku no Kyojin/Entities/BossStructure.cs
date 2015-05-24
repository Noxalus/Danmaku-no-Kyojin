
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
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

        private List<Vector2> _bottomRightVertices;
        private List<Vector2> _bottomLeftVertices;
        private List<Vector2> _topRightVertices;
        private List<Vector2> _topLeftVertices;

        private Direction _bottomRightLastDirection;
        private Direction _topRightLastDirection;

        public Vector2 Origin { get; private set; }



        public Vector2 GetSize()
        {
            return _size;
        }

        public BossStructure(DnK game, int iteration = 50, float step = 25)
        {
            _gameRef = game;
            Vertices = new List<Vector2>();

            _iteration = iteration;
            _step = step;
            _iteration = iteration;

            _bottomRightVertices = new List<Vector2>();
            _bottomLeftVertices = new List<Vector2>();
            _topRightVertices = new List<Vector2>();
            _topLeftVertices = new List<Vector2>();

            _bottomRightLastDirection = Direction.Left;
            _topRightLastDirection = Direction.Left;

            _polygonShape = new PolygonShape(_gameRef.GraphicsDevice, null);

            GenerateBaseStructure();
        }

        public void GenerateBaseStructure()
        {
            _topRightVertices.Clear();
            _topLeftVertices.Clear();
            _bottomLeftVertices.Clear();
            _bottomRightVertices.Clear();
            Vertices.Clear();

            // Add the origin point
            _bottomLeftVertices.Add(Vector2.Zero);

            Iterate(_iteration);
        }

        private Vector2 GenerateRandomPosition(Vector2 position, List<Direction> possibleDirections, ref Direction lastDirection)
        {
            var vertex = position;

            switch (lastDirection)
            {
                case Direction.Up:
                    // We don't want to go down for the next step
                    possibleDirections.Remove(Direction.Down);
                    break;
                case Direction.Down:
                    // We don't want to go up for the next step
                    possibleDirections.Remove(Direction.Up);
                    break;
            }

            if (position.X == 0f)
            {
                possibleDirections.Remove(Direction.Up);
                possibleDirections.Remove(Direction.Down);
            }

            var randomDirectionIndex = _gameRef.Rand.Next(possibleDirections.Count);
            var randomDirection = possibleDirections[randomDirectionIndex];

            lastDirection = randomDirection;

            switch (randomDirection)
            {
                case Direction.Up:
                    vertex.Y -= _step;
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
                    break;
                case Direction.Left:
                    vertex.X -= _step;
                    break;
                case Direction.UpLeft:
                    vertex.Y -= _step;
                    vertex.X -= _step;
                    break;
                case Direction.DownLeft:
                    vertex.Y += _step;
                    vertex.X -= _step;
                    break;
            }

            return vertex;
        }

        private List<Vector2> GenerateSymmetry(List<Vector2> vertices, Symmetry symmetry, float pivot)
        {
            var symmetricVertices = new List<Vector2>();

            switch (symmetry)
            {
                case Symmetry.Vertical:
                    var initialX = vertices[vertices.Count - 1];

                    for (var i = vertices.Count - 2; i >= 0; i--)
                    {
                        var position = vertices[i];
                        
                        // We want to avoid duplicates
                        if (Math.Abs(position.X - initialX.X) < 0f)
                            continue;

                        position.X = (pivot * 2) - position.X;
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
            var minY = 0f;
            var maxY = 0f;

            var bottomVertex = _bottomLeftVertices.Last();

            // We revert it earlier
            _topLeftVertices.Reverse();

            var topVertex = Vector2.Zero;

            if (_topLeftVertices.Count > 0)
                topVertex = _topLeftVertices.Last();

            for (var i = 0; i < iterationNumber; i++)
            {
                var possibleDirections = new List<Direction>()
                {
                    Direction.Up, Direction.Down, Direction.Right, Direction.UpRight, Direction.DownRight
                };

                if (bottomVertex.Y <= _step)
                {
                    possibleDirections.Remove(Direction.Up);
                    possibleDirections.Remove(Direction.UpRight);
                    possibleDirections.Remove(Direction.Right);
                }

                if (i == iterationNumber - 1 || _bottomLeftVertices.Count == 1)
                {
                    possibleDirections.Remove(Direction.Down);
                    possibleDirections.Remove(Direction.Up);
                }

                bottomVertex = GenerateRandomPosition(bottomVertex, possibleDirections, ref _bottomRightLastDirection);

                if (bottomVertex.Y > maxY)
                    maxY = bottomVertex.Y;

                _bottomLeftVertices.Add(bottomVertex);

                _size.X = bottomVertex.X * 2;

                // Update top part
                while (topVertex.X < bottomVertex.X)
                {
                    possibleDirections = new List<Direction>()
                    {
                        Direction.Up, Direction.Down, Direction.Right, Direction.UpRight, Direction.DownRight
                    };

                    if (topVertex.Y >= -_step)
                    {
                        possibleDirections.Remove(Direction.Down);
                        possibleDirections.Remove(Direction.DownRight);
                        possibleDirections.Remove(Direction.Right);
                    }

                    topVertex = GenerateRandomPosition(topVertex, possibleDirections, ref _topRightLastDirection);

                    if (topVertex.Y < minY)
                        minY = topVertex.Y;

                    _topLeftVertices.Add(topVertex);
                }
            }

            // Generate vertical symmetry
            _bottomRightVertices = GenerateSymmetry(_bottomLeftVertices, Symmetry.Vertical, _size.X / 2f);
            _topRightVertices = GenerateSymmetry(_topLeftVertices, Symmetry.Vertical, _size.X / 2f);

            _size.Y = Math.Abs(minY - maxY);
            Origin = new Vector2(_size.X / 2f, _size.Y / 2f);

            // We want CCW order
            _topRightVertices.Reverse();
            _topLeftVertices.Reverse();

            Vertices.Clear();
            Vertices.AddRange(_bottomLeftVertices);
            Vertices.AddRange(_bottomRightVertices);
            Vertices.AddRange(_topRightVertices);
            Vertices.AddRange(_topLeftVertices);

            _polygonShape.UpdateVertices(Vertices.ToArray());
        }

        public void Draw(Matrix viewMatrix, Vector2 position, Vector2 origin, float rotation, Vector2 scale)
        {
            _polygonShape.Draw(viewMatrix, position, origin, rotation, scale);
        }
    }
}
