using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// 弾や敵オブジェクト（自身が弾源になる場合も、弾源から呼び出される場合もあります）
	/// </summary>
	class Mover : Bullet
	{
		#region Members

		public bool Used;
		public bool BulletRoot;

        private Random _rand = new Random(DateTime.Now.Millisecond);

		#endregion //Members

		#region Properties
		
		#endregion //Properties

		#region Methods

	    /// <summary>
	    /// Initializes a new instance of the <see cref="Danmaku_no_Kyojin.BulletEngine.Mover"/> class.
	    /// </summary>
	    /// <param name="gameRef">Game reference</param>
	    /// <param name="myBulletManager">My bullet manager.</param>
	    public Mover(DnK gameRef, IBulletManager myBulletManager) : base(gameRef, myBulletManager)
		{
            Position = Vector2.Zero;
            Rotation = 0f;

            IsAlive = true;
		}

        public override void Initialize()
        {
			Used = true;
			BulletRoot = false;

            base.Initialize();
		}

	    protected override void LoadContent()
	    {
            Sprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/boss_bullet_type3");
            CollisionBoxes.Add(new CollisionCircle(this, Vector2.Zero, Sprite.Height / 2f));
            Origin = new Vector2(Sprite.Height / 2f, Sprite.Height / 2f);

            base.LoadContent();
	    }

	    public override void Update(GameTime gameTime)
		{
			//BulletMLで自分を動かす
			base.Update(gameTime);

	        if (BulletRoot)
	            Used = false;

            if (X < 0 || X > Config.GameArea.X || Y < 0 || Y > Config.GameArea.Y)
			{
				Used = false;
			}
		}

		/// BulletMLの弾幕定義を自分にセット
		public void SetBullet(BulletMLNode tree)
		{
			InitTop(tree);
		}

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Draw(Sprite, Position, null, Color.White, Direction, Origin, 1f, SpriteEffects.None, 0f);

            if (Config.DisplayCollisionBoxes)
                CollisionBoxes.Draw(GameRef.SpriteBatch);

            base.Draw(gameTime);
        }

		#endregion //Methods
	}
}
