using System;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Danmaku_no_Kyojin.Utils;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.Collisions
{
    class CollisionConvexPolygon : CollisionElement
    {
        #region Fields

        public List<Vector2> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public bool IsFilled { get; set; }

        public float HealthPoint
        {
            get { return _healthPoint; }
            set { _healthPoint = value; }
        }

        private List<Vector2> _axes;
        private List<Vector2> _circleAxes;
        private List<Vector2> _vertices;
        private float _healthPoint;

        #endregion

        #region Accessors

        public List<Vector2> GetAxes()
        {
            return _axes;
        }

        #endregion

        public CollisionConvexPolygon(Entity parent, Vector2 relativePosition, List<Vector2> vertices, float healthPoint = 100)
            : base(parent, relativePosition)
        {
            Parent = parent;
            Vertices = vertices;
            _axes = new List<Vector2>();
            _circleAxes = new List<Vector2>();
            _healthPoint = healthPoint;

            ComputeAxes();
        }

        public override bool Intersects(CollisionElement collisionElement)
        {
            if (collisionElement is CollisionConvexPolygon)
                return Intersects(collisionElement as CollisionConvexPolygon);

            if (collisionElement is CollisionCircle)
                return Intersects(collisionElement as CollisionCircle);

            return collisionElement.Intersects(this);
        }

        private bool Intersects(CollisionConvexPolygon element)
        {
            // loop over the axes of this polygon
            for (var i = 0; i < _axes.Count; i++)
            {
                var axis = _axes[i];
                // project both shapes onto the axis
                var p1 = Project(axis);
                var p2 = element.Project(axis);
                // do the projections overlap?
                if (!Overlap(p1, p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }
            
            // loop over element polygon's axes
            List<Vector2> axes = element.GetAxes();
            for (int i = 0; i < axes.Count; i++)
            {
                Vector2 axis = axes[i];
                // project both shapes onto the axis
                Vector2 p1 = Project(axis);
                Vector2 p2 = element.Project(axis);
                // do the projections overlap?
                if (!Overlap(p1, p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }

            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }

        private bool Intersects(CollisionCircle element)
        {
            ComputeCircleAxes(element);

            // loop over the axes of this polygon
            for (int i = 0; i < _axes.Count; i++)
            {
                Vector2 axis = _axes[i];
                // project both shapes onto the axis
                Vector2 p1 = this.Project(axis);
                Vector2 p2 = element.Project(axis);

                // do the projections overlap?
                if (!Overlap(p1, p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }

            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }

        public override void Draw(SpriteBatch sp)
        {
            if (Vertices.Count == 0)
                return;

            Vector2 previousPosition = GetPosition(Vertices[0]);

            for (int i = 1; i <= Vertices.Count; i++)
            {
                Vector2 position = GetPosition(i == Vertices.Count ? Vertices[0] : Vertices[i]);

                sp.DrawLine(
                    previousPosition.X,
                    previousPosition.Y,
                    position.X,
                    position.Y, Color.Red);

                Vector2 axis = Vector2.Normalize(position - previousPosition);
                /*
                sp.DrawLine(
                    (previousPosition.X + position.X) / 2f, 
                    (previousPosition.Y + position.Y) / 2f,
                    (previousPosition.X) + axis.Y * 2000, 
                    (previousPosition.Y) - axis.X * 2000,
                    Color.Red);
                */
                /*
                if (_circleAxes.Count > 0)
                {
                    sp.DrawLine(
                        previousPosition.X, previousPosition.Y,
                        (previousPosition.X) + _circleAxes[i - 1].X * 2000, (previousPosition.Y) - _circleAxes[i - 1].Y * -2000,
                        Color.Red);
                }
                */

                previousPosition = position;
            }
        }

        private Vector2 GetPosition(Vector2 vertex)
        {
            var angleCos = (float)Math.Cos(Parent.Rotation);
            var angleSin = (float)Math.Sin(Parent.Rotation);

            // Translate point back to origin  
            vertex.X -= Parent.Origin.X;
            vertex.Y -= Parent.Origin.Y;

            // Rotate point
            var newX = vertex.X * angleCos - vertex.Y * angleSin;
            var newY = vertex.X * angleSin + vertex.Y * angleCos;

            // Translate point back
            vertex.X = newX + Parent.Position.X;
            vertex.Y = newY + Parent.Position.Y;

            return vertex;
        }

        public override Vector2 GetCenter()
        {
            var center = Vector2.Zero;
            var previousCenter = Vector2.Zero;
            for (var i = 0; i < Vertices.Count; i++)
            {
                var currentCenter = (Vertices[i] + Vertices[(i + 1) % Vertices.Count]) / 2f;

                if (previousCenter != Vector2.Zero)
                {
                    center = (previousCenter + currentCenter) / 2f;
                }

                previousCenter = currentCenter;
            }

            center = (center + previousCenter) / 2f;

            return Parent.Position - Parent.Origin + center;
        }

        public float GetMinX()
        {
            var minX = Vertices[0].X;

            for (var i = 1; i < Vertices.Count; i++)
            {
                if (Vertices[i].X < minX)
                    minX = Vertices[i].X;
            }

            return minX;
        }

        public float GetMaxX()
        {
            var maxX = Vertices[0].X;

            for (var i = 1; i < Vertices.Count; i++)
            {
                if (Vertices[i].X > maxX)
                    maxX = Vertices[i].X;
            }

            return maxX;
        }

        private void ComputeAxes()
        {
            if (Vertices.Count == 0)
                return;

            // We start by deleting former axis
            _axes.Clear();

            Vector2 previousPosition = GetPosition(Vertices[0]);

            for (int i = 1; i <= Vertices.Count; i++)
            {
                Vector2 position = GetPosition(i == Vertices.Count ? Vertices[0] : Vertices[i]);

                Vector2 edge = position - previousPosition;
                var normal = new Vector2(edge.Y, -edge.X);

                // We want to avoid to have parallel axes because projection would give us the same result
                if (!_axes.Contains(normal) && !_axes.Contains(-normal))
                    _axes.Add(normal);

                previousPosition = position;
            }
        }

        private void ComputeCircleAxes(CollisionCircle element)
        {
            _circleAxes.Clear();

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector2 position = GetPosition(Vertices[i]);

                Vector2 edge = element.GetCenter() - position;
                var normal = new Vector2(edge.Y, -edge.X);
                _circleAxes.Add(normal);
            }
        }

        public bool Overlap(Vector2 p1, Vector2 p2)
        {
            // P = (X, Y) with X = min and Y = max
            return (p1.Y > p2.X && p1.X < p2.Y) || (p2.Y > p1.X && p2.Y < p1.X);
        }

        public Vector2 Project(Vector2 axis)
        {
            if (Vertices.Count == 0)
                return Vector2.Zero;

            float min = Vector2.Dot(new Vector2(GetPosition(Vertices[0]).X, GetPosition(Vertices[0]).Y), axis);
            float max = min;
            for (int i = 1; i < Vertices.Count; i++)
            {
                // NOTE: the axis must be normalized to get accurate projections
                float p = Vector2.Dot(new Vector2(GetPosition(Vertices[i]).X, GetPosition(Vertices[i]).Y), axis);
                if (p < min)
                {
                    min = p;
                }
                else if (p > max)
                {
                    max = p;
                }
            }

            return new Vector2(min, max);
        }
    }
}
