using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class Entity : ICollidable
    {
        #region Fields

        protected DnK Game;
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
        {
            Game = game;
        }

        public virtual void Initialize()
        {
            LoadContent();
        }

        protected virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public bool Intersects(Entity entity)
        {
            return _boundingElement.Intersects(entity.BoundingElement);
        }
    }
}
