using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    public abstract class ColisionElement
    {
        public abstract Entity Parent { get; set; }

        public abstract bool Intersects(ColisionElement collisionElement);

        public abstract void Draw();
        public abstract void DrawDebug(Vector2 position, float rotation, Vector2 entitySize);
    }
}
