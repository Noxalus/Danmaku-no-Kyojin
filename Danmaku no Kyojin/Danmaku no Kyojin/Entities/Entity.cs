using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    abstract class Entity : ICollidable
    {
        #region Fields
        
        protected Vector2 _position;
        protected BoundingElement _boundingElement;
        
        #endregion

        #region Accessors

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public BoundingElement BoundingElement
        {
            get { return _boundingElement; }
            set { _boundingElement = value; }
        }

        #endregion

        public bool Intersects(Entity entity)
        {
            return false;
        }
    }
}
