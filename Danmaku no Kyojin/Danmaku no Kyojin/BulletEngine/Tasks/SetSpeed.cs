using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This action sets the velocity of a bullet
	/// </summary>
	public class BulletMLSetSpeed : BulletMLTask
	{
		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLTask"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public BulletMLSetSpeed(BulletMLNode node, BulletMLTask owner) : base(node, owner)
		{
			Debug.Assert(null != Node);
			Debug.Assert(null != Owner);
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			bullet.Velocity = Node.GetValue(this);
			TaskFinished = true;
			return ERunStatus.End;
		}

		#endregion //Methods
	}
}