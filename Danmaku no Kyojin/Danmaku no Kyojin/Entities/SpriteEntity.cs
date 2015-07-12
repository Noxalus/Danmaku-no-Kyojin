using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class SpriteEntity : Entity
    {
        #region Fields

        private Texture2D _sprite;

        #endregion

        protected SpriteEntity(DnK gameRef) : base(gameRef)
        {
        }

        protected Texture2D Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                Size = new Point(value.Width, value.Height);
                Origin = new Vector2(value.Width / 2f, value.Height / 2f);
            }
        }
    }
}
