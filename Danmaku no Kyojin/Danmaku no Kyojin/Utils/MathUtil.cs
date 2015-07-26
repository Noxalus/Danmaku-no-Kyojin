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

        public static Vector2 ComputePolygonCentroid(Vector2[] vertices)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
            {
                float temp = vertices[i].X * vertices[j].Y - vertices[j].X * vertices[i].Y;
                accumulatedArea += temp;
                centerX += (vertices[i].X + vertices[j].X) * temp;
                centerY += (vertices[i].Y + vertices[j].Y) * temp;
            }

            if (accumulatedArea < 1e-7f)
                return Vector2.Zero; // Avoid division by zero
            else
            {
                accumulatedArea *= 3f;
                return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
            }
        }
    }
}
