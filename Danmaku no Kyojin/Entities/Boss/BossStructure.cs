using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Particles;
using Danmaku_no_Kyojin.Shapes;
using Danmaku_no_Kyojin.Utils;
using Microsoft.Xna.Framework;

namespace Danmaku_no_Kyojin.Entities.Boss
{
    public struct BossStructureVertices
    {
        private List<Vector2> bottomRightVertices;
        private List<Vector2> bottomLeftVertices;
        private List<Vector2> topRightVertices;
        private List<Vector2> topLeftVertices;
    };

    class BossStructure
    {
        private enum Direction { Up, Down, Right, Left, UpLeft, DownLeft, UpRight, DownRight };
        private enum Symmetry { Vertical, Horizontal };

        private DnK _gameRef;

        private Entity _parent;
        private PolygonShape _polygonShape;
        private int _iteration;
        private float _step;
        private Vector2 _size;
        private float _area;

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

        public float GetArea()
        {
            return _area;
        }

        public Vector2[] GetVertices()
        {
            return _polygonShape.Vertices;
        }

        public BossStructure(DnK game, Entity parent, int iteration = 50, float step = 25, PolygonShape polygonShape = null)
        {
            _gameRef = game;
            _parent = parent;

            _iteration = iteration;
            _step = step;
            _iteration = iteration;
            _area = 0f;

            _bottomRightVertices = new List<Vector2>();
            _bottomLeftVertices = new List<Vector2>();
            _topRightVertices = new List<Vector2>();
            _topLeftVertices = new List<Vector2>();

            _bottomRightLastDirection = Direction.Left;
            _topRightLastDirection = Direction.Left;

            if (polygonShape != null)
            {
                _polygonShape = polygonShape;
                _size = polygonShape.GetSize();
                ComputeArea();
            }
            else
            {
                _polygonShape = new PolygonShape(_gameRef, null);
                GenerateBaseStructure();
            }
        }

        private void GenerateBaseStructure()
        {
            _topRightVertices.Clear();
            _topLeftVertices.Clear();
            _bottomLeftVertices.Clear();
            _bottomRightVertices.Clear();

            // Add the origin point
            _bottomLeftVertices.Add(Vector2.Zero);

            Iterate(_iteration);
        }

        // TODO: Use _polygonShape.Vertices instead of _bottomLeftVertices, etc...
        public  void Iterate(int iterationNumber = 1)
        {
            if (iterationNumber == 0)
                return;

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

            var centerVerticesDistance = _bottomRightVertices.First().Y + _topRightVertices.First().Y;

            _size.Y = Math.Abs(minY - maxY);
            Origin = new Vector2(_size.X / 2f, centerVerticesDistance / 2f);

            // We want CCW order
            _topRightVertices.Reverse();
            _topLeftVertices.Reverse();

            var vertices = new List<Vector2>();
            vertices.AddRange(_bottomLeftVertices);
            vertices.AddRange(_bottomRightVertices);
            vertices.AddRange(_topRightVertices);
            vertices.AddRange(_topLeftVertices);

            _polygonShape.UpdateVertices(vertices.ToArray());
            ComputeArea();
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

        private List<Vector2> GenerateSymmetry(List<Vector2> vertices, Symmetry symmetry, float pivot, bool avoidDuplicates = true)
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
                        if (avoidDuplicates && Math.Abs(position.X - initialX.X) < 0f)
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

        public PolygonShape Split(CollisionConvexPolygon collisionBox)
        {
            var collisionBoxCenter = collisionBox.GetCenter();
            var destroyAll = false;

            /*
            // TODO: This structure is dead only if its area is less than a specific number, not because its center is destroyed
            // Structure is dead
            if (collisionBoxCenter.X > (_size.X / 2f - 2 * _step) + _parent.Position.X - _parent.Origin.X &&
                collisionBoxCenter.X < (_size.X / 2f + 2 * _step) + _parent.Position.X - _parent.Origin.X)
            {
                destroyAll = true;
            }
            */

            // When we retrieve vertices, we need to transform them from world to local
            var newPolygonShapeVertices = new List<Vector2>();
            var newVertices = _polygonShape.Vertices.ToList();

            if (!destroyAll)
            {
                // The part to remove is at left
                if (collisionBoxCenter.X < _size.X / 2f)
                {
                    var leftVertices = newVertices.FindAll(vertex => vertex.X <= collisionBoxCenter.X);

                    newPolygonShapeVertices.AddRange(leftVertices);

                    foreach (var vertex in leftVertices)
                    {
                        newVertices.Remove(vertex);
                    }

                    /*
                    foreach (var box in CollisionBoxes)
                    {
                        var center = box.GetCenter();

                        if (center.X <= minX + _parent.Position.X - _parent.Origin.X)
                        {
                            ParticleExplosion(center);
                            toDelete.Add(box);
                        }
                    }

                    foreach (var collisionElement in toDelete)
                    {
                        var box = (CollisionConvexPolygon) collisionElement;
                        CollisionBoxes.Remove(box);
                        _leftCollisionBoxes.Remove(box);
                    }
                    */
                }
                // The part to remove is at right
                else if (collisionBoxCenter.X > _size.X / 2f)
                {
                    var rightVertices = newVertices.FindAll(vertex => vertex.X >= collisionBoxCenter.X);

                    newPolygonShapeVertices.AddRange(rightVertices);

                    foreach (var vertex in rightVertices)
                        newVertices.Remove(vertex);

                    /*
                    foreach (CollisionElement box in CollisionBoxes)
                    {
                        var center = box.GetCenter();

                        if (center.X >= maxX + _parent.Position.X - _parent.Origin.X)
                        {
                            ParticleExplosion(center);
                            toDelete.Add(box);
                        }
                    }

                    foreach (var collisionElement in toDelete)
                    {
                        var box = (CollisionConvexPolygon) collisionElement;
                        CollisionBoxes.Remove(box);
                        _rightCollisionBoxes.Remove(box);
                    }
                    */
                }
            }
            /*
            else
            {

                foreach (var center in CollisionBoxes.Select(box => box.GetCenter()))
                {
                    ParticleExplosion(center);
                }

                _leftCollisionBoxes.Clear();
                _rightCollisionBoxes.Clear();
                CollisionBoxes.Clear();
            }
            */

            /*
            var vertices = new List<Vector2>();
            vertices.AddRange(_bottomLeftVertices);
            vertices.AddRange(_bottomRightVertices);
            vertices.AddRange(_topRightVertices);
            vertices.AddRange(_topLeftVertices);
            */
            _polygonShape.UpdateVertices(newVertices.ToArray());

            /*
            if (_leftCollisionBoxes.Contains(collisionBox))
                _leftCollisionBoxes.Remove(collisionBox);
            else if (_rightCollisionBoxes.Contains(collisionBox))
                _rightCollisionBoxes.Remove(collisionBox);

            CollisionBoxes.Remove(collisionBox);
            */

            var newPolygonShapeVerticesArray = newPolygonShapeVertices.ToArray();

            if (newPolygonShapeVertices.Count > 0)
            {
                var newVerticesOriginPosition = newPolygonShapeVertices[0];
                for (int i = 0; i < newPolygonShapeVerticesArray.Length; i++)
                {
                    newPolygonShapeVerticesArray[i].X -= newVerticesOriginPosition.X;
                }
            }
            else
                newPolygonShapeVerticesArray = null;

            return new PolygonShape(_gameRef, newPolygonShapeVerticesArray);
        }

        private void ComputeArea()
        {
            var sum = 0f;
            var vertices = _polygonShape.Vertices;
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                sum += ((vertices[i].X * vertices[i + 1].Y) - (vertices[i].Y * vertices[i + 1].X));
            }

            _area = Math.Abs(sum / 2f);
        }

        public void Draw(Matrix viewMatrix, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale)
        {
            _polygonShape.Draw(viewMatrix, position, color, rotation, origin, scale);
        }
    }
}
