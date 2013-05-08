using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    class Bullet : BaseBullet
    {
        public Bullet(DnK game) : base(game)
        {
            Velocity = 3f;
            Direction = new Vector2(0, 0);
        }
    }
}
