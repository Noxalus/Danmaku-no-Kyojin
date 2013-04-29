using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    abstract class BaseBullet : DrawableGameComponent
    {
        private Vector2 _direction;
        private float _velocity;

        protected BaseBullet(DnK game)
            : base(game)
        {
        }
    }
}
