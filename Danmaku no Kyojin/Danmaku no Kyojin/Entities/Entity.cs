using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class Entity : ICollidable
    {
        #region Fields

        protected readonly DnK Game;
        protected Vector2 Position;
        protected float Rotation;
        protected CollisionElement CollisionBox;
        protected Vector2 Center;
        protected Texture2D Sprite;
        protected Point Size;

        public bool IsAlive { get; set; }

        #endregion

        #region Accessors

        public Vector2 GetPosition()
        {
            return Position;
        }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public float GetRotation()
        {
            return Rotation;
        }

        public CollisionElement GetBoundingElement()
        {
            return CollisionBox;
        }

        public Vector2 GetOrigin()
        {
            return new Vector2(Position.X - Sprite.Width / 2, Position.Y - Sprite.Height / 2);
        }

        public Point GetSize()
        {
            return new Point(Sprite.Width, Sprite.Height);
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
            if (Config.DisplayCollisionBoxes)
                CollisionBox.Draw(Game.SpriteBatch);
        }

        public bool Intersects(Entity entity)
        {
            return CollisionBox.Intersects(entity.CollisionBox);
        }
    }
}
