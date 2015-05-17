using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Danmaku_no_Kyojin.Shapes
{
    public class PolygonShape
    {
        private GraphicsDevice _graphicsDevice;
        private Vector2[] _vertices;
        private bool _triangulated;
        private BasicEffect _effect;

        private Vector2[] _triangulatedVertices;
        private int[] _indices;

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        int _numVertices, _numPrimitives;

        private Vector2 _position;
        private Vector2 _origin;
        private Vector2 _scale;
        private float _rotation;

        RasterizerState _wireframe = new RasterizerState
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = FillMode.WireFrame
        };

        /// <summary>
        /// A Polygon object that you will be able to draw.
        /// Animations are being implemented as we speak.
        /// </summary>
        /// <param name="graphicsDevice">The graphicsdevice from a Game object</param>
        /// <param name="vertices">The vertices in a clockwise order</param>
        public PolygonShape(GraphicsDevice graphicsDevice, Vector2[] vertices)
        {
            _graphicsDevice = graphicsDevice;
            _vertices = vertices;
            _triangulated = false;
            _position = new Vector2(0f, 0f);
            _origin = new Vector2(0, 0);
            _scale = new Vector2(1, 1);
            _rotation = 0f;

            _effect = new BasicEffect(_graphicsDevice)
            {
                Projection = Matrix.CreateOrthographicOffCenter(
                0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0, 1),
                VertexColorEnabled = true,
                //DiffuseColor = new Vector3(1, 0, 0)
            };
        }

        public void Origin(Vector2 value)
        {
            _origin = value;
        }
        public void Position(Vector2 value)
        {
            _position = value;
        }

        /// <summary>
        /// Triangulate the set of VertexPositionColors so it will be drawn properly        
        /// </summary>
        /// <returns>The triangulated vertices array</returns>}
        private void Triangulate()
        {
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
                verts[i] = new VertexPositionColor(new Vector3(_triangulatedVertices[i], 0f), Color.White);

            verts[0].Color = Color.Red;
            verts[1].Color = Color.Blue;
            verts[2].Color = Color.Yellow;
            verts[3].Color = Color.Violet;
            verts[_triangulatedVertices.Length - 1].Color = Color.Green;

            _vertexBuffer = new VertexBuffer(
                _graphicsDevice,
                typeof(VertexPositionColor),
                verts.Length * VertexPositionColor.VertexDeclaration.VertexStride,
                BufferUsage.WriteOnly
            );

            _vertexBuffer.SetData(verts);

            // branch here to convert our indices to shorts if possible for wider GPU support
            if (verts.Length < 65535)
            {
                var indices = new short[_indices.Length];

                for (int i = 0; i < _indices.Length; i++)
                    indices[i] = (short)_indices[i];

                _indexBuffer = new IndexBuffer(
                    _graphicsDevice,
                    IndexElementSize.SixteenBits,
                    indices.Length * sizeof(short),
                    BufferUsage.WriteOnly);

                _indexBuffer.SetData(indices);
            }
            else
            {
                _indexBuffer = new IndexBuffer(
                    _graphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    _triangulatedVertices.Length * sizeof(int),
                    BufferUsage.WriteOnly);

                _indexBuffer.SetData(_triangulatedVertices);
            }

            _triangulated = true;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _position.Y += dt * 50;
            //_rotation += (dt * 2) % (float)Math.PI;
        }

        /// <summary>
        /// Draw the polygon. If you haven't called Triangulate yet, I wil do it for you.
        /// </summary>
        /// <param name="view"></param>
        public void Draw(Matrix viewMatrix, bool wireframe)
        {
            try
            {
                if (!_triangulated)
                    Triangulate();

                _graphicsDevice.SetVertexBuffer(_vertexBuffer);
                _graphicsDevice.Indices = _indexBuffer;

                if (wireframe)
                    _graphicsDevice.RasterizerState = _wireframe;
                else
                {
                    _graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                }

                _effect.View = viewMatrix;
                var worldMatrix = Matrix.CreateTranslation(new Vector3(-_origin, 0))
                    * Matrix.CreateScale(new Vector3(_scale, 0))
                    * Matrix.CreateRotationZ(_rotation)
                    * Matrix.CreateTranslation(new Vector3(_position, 0));

                _effect.World = worldMatrix;

                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _graphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        _numVertices,
                        0,
                        _numPrimitives
                    );
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}