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

        private List<Vector2> _bottomRightVertices;
        private List<Vector2> _bottomLeftVertices;
        private List<Vector2> _topRightVertices;
        private List<Vector2> _topLeftVertices;

        private Direction _bottomRightLastDirection;
        private Direction _topRightLastDirection;

        // Collisions
        private List<CollisionConvexPolygon> _leftCollisionBoxes;
        private List<CollisionConvexPolygon> _rightCollisionBoxes;

        public Vector2 Origin { get; private set; }

        public CollisionElements CollisionBoxes { get; private set; } 

        public Vector2 GetSize()
        {
            return _size;
        }

        public Vector2[] GetVertices()
        {
            return _polygonShape.Vertices;
        }

        public BossStructure(DnK game, Entity parent, int iteration = 50, float step = 25, PolygonShape polygonShape = null)
        {
            _gameRef = game;
            _parent = parent;
            CollisionBoxes = new CollisionElements();

            _iteration = iteration;
            _step = step;
            _iteration = iteration;

            _bottomRightVertices = new List<Vector2>();
            _bottomLeftVertices = new List<Vector2>();
            _topRightVertices = new List<Vector2>();
            _topLeftVertices = new List<Vector2>();

            _bottomRightLastDirection = Direction.Left;
            _topRightLastDirection = Direction.Left;

            _leftCollisionBoxes = new List<CollisionConvexPolygon>();
            _rightCollisionBoxes = new List<CollisionConvexPolygon>();

            if (polygonShape != null)
            {
                _polygonShape = polygonShape;
                _size = polygonShape.GetSize();
            }
            else
                _polygonShape = new PolygonShape(_gameRef, null);

            GenerateBaseStructure();
        }

        public void GenerateBaseStructure()
        {
            _topRightVertices.Clear();
            _topLeftVertices.Clear();
            _bottomLeftVertices.Clear();
            _bottomRightVertices.Clear();
            CollisionBoxes.Clear();
            _leftCollisionBoxes.Clear();
            _rightCollisionBoxes.Clear();

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

            var previousDownPosition = bottomVertex;
            var previousUpPosition = topVertex;
            var collisionBoxesDownPositions = new List<Vector2>();
            var collisionBoxesUpPositions = new List<Vector2>();

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

                if (previousDownPosition.X < bottomVertex.X)
                {
                    collisionBoxesDownPositions.Add(previousDownPosition);
                    collisionBoxesDownPositions.Add(bottomVertex);
                }

                previousDownPosition = bottomVertex;

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

                    if (previousUpPosition.X < topVertex.X)
                    {
                        collisionBoxesUpPositions.Add(previousUpPosition);
                        collisionBoxesUpPositions.Add(topVertex);
                    }

                    previousUpPosition = topVertex;

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

            foreach (var cb in _rightCollisionBoxes)
            {
                for (var i = 0; i < cb.Vertices.Count; i++)
                    cb.Vertices[i] = new Vector2(cb.Vertices[i].X + iterationNumber * 2 * _step, cb.Vertices[i].Y);
            }

            GenerateCollisionBoxes(collisionBoxesDownPositions, collisionBoxesUpPositions);

            // We want CCW order
            _topRightVertices.Reverse();
            _topLeftVertices.Reverse();

            var vertices = new List<Vector2>();
            vertices.AddRange(_bottomLeftVertices);
            vertices.AddRange(_bottomRightVertices);
            vertices.AddRange(_topRightVertices);
            vertices.AddRange(_topLeftVertices);

            _polygonShape.UpdateVertices(vertices.ToArray());
        }

        public PolygonShape Split(CollisionConvexPolygon collisionBox)
        {
            var minX = collisionBox.GetMinX();
            var maxX = collisionBox.GetMaxX();
            var collisionBoxCenter = collisionBox.GetCenter();
            var destroyAll = false;

            // TODO: This structure is dead only if its area is less than a specific number, not because its center is destroyed
            // Structure is dead
            if (collisionBoxCenter.X > (_size.X / 2f - 2 * _step) + _parent.Position.X - _parent.Origin.X &&
                collisionBoxCenter.X < (_size.X / 2f + 2 * _step) + _parent.Position.X - _parent.Origin.X)
            {
                destroyAll = true;
            }

            // When we retrieve vertices, we need to transform them from world to local
            var newPolygonShapeVertices = new List<Vector2>();

            var toDelete = new List<CollisionElement>();
            
            if (!destroyAll)
            {
                // The part to remove is at left
                if (minX < _size.X/2f)
                {
                    var newBottomLeftVertices = _bottomLeftVertices.FindAll(vertex => vertex.X <= minX);
                    var newTopLeftVertices = _topLeftVertices.FindAll(vertex => vertex.X <= minX);

                    newPolygonShapeVertices.AddRange(newBottomLeftVertices);
                    newPolygonShapeVertices.AddRange(newTopLeftVertices);

                    foreach (var vertex in newBottomLeftVertices)
                        _bottomLeftVertices.Remove(vertex);
                    foreach (var vertex in newTopLeftVertices)
                        _topLeftVertices.Remove(vertex);

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
                }
                // The part to remove is at right
                else if (maxX > _size.X/2f)
                {
                    var newBottomRightVertices = _bottomRightVertices.FindAll(vertex => vertex.X >= maxX);
                    var newTopRightVertices = _topRightVertices.FindAll(vertex => vertex.X >= maxX);

                    newPolygonShapeVertices.AddRange(newBottomRightVertices);
                    newPolygonShapeVertices.AddRange(newTopRightVertices);

                    foreach (var vertex in newBottomRightVertices)
                        _bottomRightVertices.Remove(vertex);
                    foreach (var vertex in newTopRightVertices)
                        _topRightVertices.Remove(vertex);

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
                }
            }
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

            var vertices = new List<Vector2>();
            vertices.AddRange(_bottomLeftVertices);
            vertices.AddRange(_bottomRightVertices);
            vertices.AddRange(_topRightVertices);
            vertices.AddRange(_topLeftVertices);

            _polygonShape.UpdateVertices(vertices.ToArray());

            if (_leftCollisionBoxes.Contains(collisionBox))
                _leftCollisionBoxes.Remove(collisionBox);
            else if (_rightCollisionBoxes.Contains(collisionBox))
                _rightCollisionBoxes.Remove(collisionBox);

            CollisionBoxes.Remove(collisionBox);

            ComputeCollisionBoxesHp();

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

        public void Draw(Matrix viewMatrix, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale)
        {
            _polygonShape.Draw(viewMatrix, position, color, rotation, origin, scale);
        }

        private void ParticleExplosion(Vector2 position)
        {
            float hue1 = _gameRef.Rand.NextFloat(0, 6);
            float hue2 = (hue1 + _gameRef.Rand.NextFloat(0, 2)) % 6f;
            Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
            Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

            for (int j = 0; j < 120; j++)
            {
                float speed = 18f * (1f - 1 / _gameRef.Rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = _gameRef.Rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(color1, color2, _gameRef.Rand.NextFloat(0, 1));

                _gameRef.ParticleManager.CreateParticle(_gameRef.LineParticle, position,
                    color, 190, 1.5f, state);
            }
        }

        // TODO: Move this function to BossPart
        private void GenerateCollisionBoxes(List<Vector2> collisionBoxesDownPositions, List<Vector2> collisionBoxesUpPositions)
        {
            // Generate collision boxes
            for (var i = 0; i < collisionBoxesDownPositions.Count - 1; i += 2)
            {
                var bottomRightVertex = collisionBoxesDownPositions[i + 1];
                var bottomLeftVertex = collisionBoxesDownPositions[i];
                var topLeftVertex = collisionBoxesUpPositions[i];
                var topRightVertex = collisionBoxesUpPositions[i + 1];

                var symmetricBottomRightVertex = new Vector2(_size.X - collisionBoxesDownPositions[i + 1].X, collisionBoxesDownPositions[i + 1].Y);
                var symmetricBottomLeftVertex = new Vector2(_size.X - collisionBoxesDownPositions[i].X, collisionBoxesDownPositions[i].Y);
                var symmetricTopLeftVertex = new Vector2(_size.X - collisionBoxesUpPositions[i].X, collisionBoxesUpPositions[i].Y);
                var symmetricTopRightVertex = new Vector2(_size.X - collisionBoxesUpPositions[i + 1].X, collisionBoxesUpPositions[i + 1].Y);

                var collisionVertices = new List<Vector2> { bottomRightVertex };
                var symetricCollisionVertices = new List<Vector2> { symmetricBottomRightVertex };

                if (!collisionVertices.Contains(bottomLeftVertex))
                    collisionVertices.Add(bottomLeftVertex);

                if (!collisionVertices.Contains(topLeftVertex))
                    collisionVertices.Add(topLeftVertex);

                if (!collisionVertices.Contains(topRightVertex))
                    collisionVertices.Add(topRightVertex);

                // Symmetric
                if (!symetricCollisionVertices.Contains(symmetricBottomLeftVertex))
                    symetricCollisionVertices.Add(symmetricBottomLeftVertex);

                if (!symetricCollisionVertices.Contains(symmetricTopLeftVertex))
                    symetricCollisionVertices.Add(symmetricTopLeftVertex);

                if (!symetricCollisionVertices.Contains(symmetricTopRightVertex))
                    symetricCollisionVertices.Add(symmetricTopRightVertex);

                _leftCollisionBoxes.Add(new CollisionConvexPolygon(_parent, Vector2.Zero, collisionVertices));
                _rightCollisionBoxes.Add(new CollisionConvexPolygon(_parent, Vector2.Zero, symetricCollisionVertices));
            }

            CollisionBoxes.Clear();
            CollisionBoxes.AddRange(_leftCollisionBoxes);
            CollisionBoxes.AddRange(_rightCollisionBoxes);

            ComputeCollisionBoxesHp();
        }

        private void ComputeCollisionBoxesHp()
        {
            _leftCollisionBoxes.Sort((box1, box2) => box1.GetMinX().CompareTo(box2.GetMinX()));
            _rightCollisionBoxes.Sort((box1, box2) => box1.GetMaxX().CompareTo(box2.GetMaxX()));

            const int initialHp = 20;
            const int hpStep = 5;
            var hp = initialHp;
            foreach (var collisionBox in _leftCollisionBoxes)
            {
                collisionBox.HealthPoint = hp;
                hp += hpStep;
            }

            hp = initialHp;
            for (int i = 0; i < _rightCollisionBoxes.Count; i++)
            {
                var collisionBox = _rightCollisionBoxes[_rightCollisionBoxes.Count - 1 - i];
                collisionBox.HealthPoint = hp;
                hp += hpStep;
            }
        }
    }
}
