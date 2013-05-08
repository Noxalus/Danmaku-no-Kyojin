using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    abstract class BaseBullet : Entity
    {
        protected Vector2 Direction;
        protected float Velocity;

        protected BaseBullet(DnK game)
            : base(game)
        {
        }
    }
}
