using Microsoft.Xna.Framework;

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
		public Vector2 pos;

		#endregion //Members

		#region Properties

		public override float X
		{
			get { return pos.X; }
			set { pos.X = value; }
		}
		
		public override float Y
		{
			get { return pos.Y; }
			set { pos.Y = value; }
		}
		
		#endregion //Properties

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLSample.Mover"/> class.
		/// </summary>
		/// <param name="myBulletManager">My bullet manager.</param>
		public Mover(IBulletManager myBulletManager) : base(myBulletManager)
		{
		}

		public void Init()
		{
			used = true;
			bulletRoot = false;
		}

        public override void Update(GameTime gameTime)
		{
			//BulletMLで自分を動かす
			base.Update(gameTime);

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

		#endregion //Methods
	}
}
