using System;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This task sets the direction of a bullet
	/// </summary>
	public class BulletMLSetDirection : BulletMLTask
	{
		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLTask"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public BulletMLSetDirection(BulletMLNode node, BulletMLTask owner) : base(node, owner)
		{
			Debug.Assert(null != Node);
			Debug.Assert(null != Owner);
		}

		public override ERunStatus Run(Bullet bullet)
		{
			ENodeType ENodeType = Node.NodeType;
			float value = (float)(Node.GetValue(this) * Math.PI / 180);

			switch (ENodeType)
			{
				case ENodeType.sequence:
				{
					bullet.Direction = bullet.GetFireData().srcDir + value;
				}
				break;
				case ENodeType.absolute:
				{
					bullet.Direction = value;
				}
				break;
				case ENodeType.relative:
				{
					bullet.Direction = bullet.Direction + value;
				}
				break;
				default:
				{
					bullet.Direction = bullet.GetAimDir() + value;
				}
				break;
			}

			TaskFinished = true;
			return ERunStatus.End;
		}

		#endregion //Methods
	}
}