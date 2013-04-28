using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    interface BoundingElement
    {
        void Update();

        Vector2 GetPosition();
        void SetPosition(Vector2 position);

        bool Intersects(BoundingRectangle boundingSquare);
        bool Intersects(BoundingCircle boundingCircle);

        void Draw();
    }
}
