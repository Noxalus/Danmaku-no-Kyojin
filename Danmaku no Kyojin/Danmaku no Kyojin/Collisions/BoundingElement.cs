using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        bool Intersects(BoundingElement boundingElement);

        void Draw();
        void DrawDebug(Vector2 position, float rotation, Vector2 entitySize);
    }
}
