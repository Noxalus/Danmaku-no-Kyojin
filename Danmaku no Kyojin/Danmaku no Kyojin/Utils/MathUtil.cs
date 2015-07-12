using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Danmaku_no_Kyojin.Utils
{
    class MathUtil
    {
        public static Vector2 FromPolar(float angle, float magnitude)
        {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
