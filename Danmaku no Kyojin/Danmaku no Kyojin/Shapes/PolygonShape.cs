using System;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Shapes
{
    public class PolygonShape
    {
        private DnK _gameRef;
        private Vector2[] _vertices;
        private bool _triangulated;
        private readonly BasicEffect _effect;
        private Effect _edgeEffect;
        private Vector2 _size;

        private Vector2[] _triangulatedVertices;
        private int[] _indices;

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        int _numVertices, _numPrimitives;

        readonly RasterizerState _wireframe = new RasterizerState
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = FillMode.WireFrame
        };

        public Vector2[] Vertices
        {
            get { return _vertices; }
        }

        /// <summary>
        /// A Polygon object that you will be able to draw.
        /// Animations are being implemented as we speak.
        /// </summary>
        /// <param name="graphicsDevice">The graphicsdevice from a GameRef object</param>
        /// <param name="vertices">The vertices in a clockwise order</param>
        public PolygonShape(DnK game, Vector2[] vertices)
        {
            _gameRef = game;
            _vertices = vertices;
            _triangulated = false;
            _size = Vector2.Zero;

            _edgeEffect = _gameRef.Content.Load<Effect>("Graphics/Shaders/Edge");

            _effect = new BasicEffect(_gameRef.GraphicsDevice)
            {
                Projection = Matrix.CreateOrthographicOffCenter(
                0, _gameRef.GraphicsDevice.Viewport.Width, _gameRef.GraphicsDevice.Viewport.Height, 0, 0, 1),
                VertexColorEnabled = true,
                DiffuseColor = new Vector3(0, 1, 0),
                Alpha = 0.5f
            };
        }

        public Vector2 GetSize()
        {
            if (_size == Vector2.Zero)
                ComputeSize();

            return _size;
        }

        private void ComputeSize()
        {
            var min = new Vector2(_vertices[0].X, _vertices[0].Y);
            var max = new Vector2(_vertices[0].X, _vertices[0].Y);

            for (var i = 1; i < _vertices.Length; i++)
            {
                min.X = Math.Min(min.X, _vertices[i].X);
                min.Y = Math.Min(min.Y, _vertices[i].Y);
                max.X = Math.Max(max.X, _vertices[i].X);
                max.Y = Math.Max(max.Y, _vertices[i].Y);
            }

            _size = new Vector2(Math.Abs(max.X - min.X), Math.Abs(max.Y - min.Y));
        }

        public void UpdateVertices(Vector2[] vertices)
        {
            _vertices = vertices;
            Triangulate();
        }

        /// <summary>
        /// Triangulate the set of VertexPositionColors so it will be drawn properly        
        /// </summary>
        /// <returns>The triangulated vertices array</returns>}
        private void Triangulate()
        {
            // If we need to triangulate, the size should be incorrect
            ComputeSize();

            Triangulator.Triangulator.Triangulate(
                _vertices,
                Triangulator.WindingOrder.CounterClockwise,
                out _triangulatedVertices,
                out _indices
            );

            // save out some data
            _numVertices = _triangulatedVertices.Length;
            _numPrimitives = _indices.Length / 3;

            var verts = new VertexPositionColor[_triangulatedVertices.Length];

            for (var i = 0; i < _triangulatedVertices.Length; i++)
            {
                verts[i] = new VertexPositionColor(new Vector3(_triangulatedVertices[i], 0f), new Color(1f, 1f, 1f));
            }
            
            _vertexBuffer = new VertexBuffer(
                _gameRef.GraphicsDevice,
                typeof(VertexPositionColor),
                verts.Length * VertexPositionColor.VertexDeclaration.VertexStride,
                BufferUsage.WriteOnly
            );

            _vertexBuffer.SetData(verts);

            // branch here to convert our indices to shorts if possible for wider GPU support
            if (verts.Length < 65535)
            {
                var indices = new short[_indices.Length];

                for (var i = 0; i < _indices.Length; i++)
                    indices[i] = (short)_indices[i];

                if (indices.Length > 0)
                {
                    _indexBuffer = new IndexBuffer(
                        _gameRef.GraphicsDevice,
                        IndexElementSize.SixteenBits,
                        indices.Length * sizeof (short),
                        BufferUsage.WriteOnly);

                    _indexBuffer.SetData(indices);
                }
            }
            else
            {
                if (_triangulatedVertices.Length > 0)
                {
                    _indexBuffer = new IndexBuffer(
                        _gameRef.GraphicsDevice,
                        IndexElementSize.ThirtyTwoBits,
                        _triangulatedVertices.Length * sizeof (int),
                        BufferUsage.WriteOnly);

                    _indexBuffer.SetData(_triangulatedVertices);
                }
            }

            _triangulated = true;
        }

        public void Draw(Matrix viewMatrix, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale)
        {
            try
            {
                if (!_triangulated)
                    Triangulate();

                _gameRef.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
                _gameRef.GraphicsDevice.Indices = _indexBuffer;
                _gameRef.GraphicsDevice.BlendState = BlendState.Additive;

                _gameRef.GraphicsDevice.RasterizerState = InputHandler.KeyDown(Keys.W) ? _wireframe : RasterizerState.CullCounterClockwise;

                var worldMatrix = Matrix.CreateTranslation(new Vector3(-origin, 0))
                    * Matrix.CreateScale(new Vector3(scale, 0))
                    * Matrix.CreateRotationZ(rotation)
                    * Matrix.CreateTranslation(new Vector3(position, 0));

                var projection = Matrix.CreateOrthographicOffCenter(
                    0, _gameRef.GraphicsDevice.Viewport.Width, 
                    _gameRef.GraphicsDevice.Viewport.Height, 0,
                    0, 1
                );

                _edgeEffect.Parameters["View"].SetValue(viewMatrix);
                _edgeEffect.Parameters["World"].SetValue(worldMatrix);
                _edgeEffect.Parameters["Projection"].SetValue(projection);

                _edgeEffect.Parameters["DiffuseColor"].SetValue(new Vector3(color.R / 255f, color.G / 255f, color.B / 255f));
                _edgeEffect.Parameters["Alpha"].SetValue(color.A / 255f);

                foreach (var pass in _edgeEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _gameRef.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _numVertices, 0, _numPrimitives);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}