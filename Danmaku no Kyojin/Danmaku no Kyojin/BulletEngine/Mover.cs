using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// 弾や敵オブジェクト（自身が弾源になる場合も、弾源から呼び出される場合もあります）
	/// </summary>
	class Mover : Bullet
	{
		#region Members

		public bool used;
		public bool bulletRoot;

		#endregion //Members

		#region Properties
		
		#endregion //Properties

		#region Methods

		/// <summary>
        /// Initializes a new instance of the <see cref="Danmaku_no_Kyojin.BulletEngine.Mover"/> class.
		/// </summary>
		/// <param name="myBulletManager">My bullet manager.</param>
		public Mover(DnK game, IBulletManager myBulletManager) : base(game, myBulletManager)
		{
            Position = Vector2.Zero;
            Rotation = 0f;

            IsAlive = true;
		}

        public override void Initialize()
        {
			used = true;
			bulletRoot = false;

            base.Initialize();
		}

	    protected override void LoadContent()
	    {
            Sprite = Game.Content.Load<Texture2D>(@"Graphics/Sprites/ball");

            Center = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);

            CollisionBox = new CollisionCircle(this, Vector2.Zero, Sprite.Width / 2f);

            base.LoadContent();
	    }

	    public override void Update(GameTime gameTime)
		{
			//BulletMLで自分を動かす
			base.Update(gameTime);

	        if (bulletRoot)
	            used = false;

            if (X < 0 || X > Config.Resolution.X || Y < 0 || Y > Config.Resolution.Y)
			{
				used = false;
			}
		}

		/// BulletMLの弾幕定義を自分にセット
		public void SetBullet(BulletMLNode tree)
		{
			InitTop(tree);
		}

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Center, 1f, SpriteEffects.None, 0f);

            if (Config.DisplayCollisionBoxes)
            {
                CollisionBox.Draw(Game.SpriteBatch);
            }

            base.Draw(gameTime);
        }

		#endregion //Methods
	}
}
