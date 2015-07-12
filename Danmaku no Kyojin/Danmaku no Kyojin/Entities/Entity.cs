using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class Entity : ICollidable
    {
        #region Fields

        protected readonly DnK GameRef;
        private Vector2 _position;
        private float _rotation;
        private Vector2 _origin;
        private Vector2 _scale;
        private Point _size;

        public bool IsAlive { get; set; }

        #endregion

        #region Accessors

        public float X
        {
            get { return Position.X; }
            set { _position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { _position.Y = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        // Used as PositionDelegate for BulletEngine
        public Vector2 GetPosition()
        {
            return _position;
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public virtual CollisionElements CollisionBoxes { get; set; }

        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public Point Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        #endregion

        protected Entity(DnK gameRef)
        {
            GameRef = gameRef;

            CollisionBoxes = new CollisionElements();
            _scale = new Vector2(1f, 1f);
            IsAlive = true;
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
                CollisionBoxes.Draw(GameRef.SpriteBatch);
        }

        public virtual bool Intersects(Entity entity)
        {
            return CollisionBoxes.Intersects(entity.CollisionBoxes);
        }
    }
}
