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

        protected readonly DnK Game;
        protected Vector2 Position;
        protected ColisionElement BoundingElement;

        public bool IsAlive { get; set; }

        #endregion

        #region Accessors

        public Vector2 GetPosition()
        {
            return Position;
        }

        public ColisionElement GetBoundingElement()
        {
            return BoundingElement;
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
            return BoundingElement.Intersects(entity.BoundingElement);
        }
    }
}
