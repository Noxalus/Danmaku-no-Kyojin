using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Camera
{
    public class Camera2D
    {
        public Matrix transform;
        private Vector2 _center;
        private float _xLag;

        public Camera2D(float xLag)
        {
            _xLag = xLag;
        }

        public void Update(Vector2 position)
        {
            _center = new Vector2(position.X - _xLag, position.Y - Config.Resolution.Y / 2);
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(-_center.X, -_center.Y, 0));
        }
    }
}
