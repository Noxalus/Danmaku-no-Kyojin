using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class Entity : DrawableGameComponent, ICollidable
    {
        #region Fields

        protected Vector2 _position;
        protected ColisionElement _boundingElement;

        #endregion

        #region Accessors

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public ColisionElement BoundingElement
        {
            get { return _boundingElement; }
            set { _boundingElement = value; }
        }

        #endregion

        protected Entity(DnK game)
            : base(game)
        {
        }

        public bool Intersects(Entity entity)
        {
            return _boundingElement.Intersects(entity.BoundingElement);
        }
    }
}
