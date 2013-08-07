using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Camera
{
    public class CameraTest
    {
        public Matrix transform;
        Vector2 centre;

        public CameraTest()
        {
        }

        public void Update(Vector2 position)
        {
            centre = new Vector2(position.X - Config.Resolution.X / 4, position.Y - Config.Resolution.Y / 2);
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(-centre.X, -centre.Y, 0));
        }
    }
}
