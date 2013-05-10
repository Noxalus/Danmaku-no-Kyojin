using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This task pauses for a specified amount of time before resuming
	/// </summary>
	public class BulletMLWait : BulletMLTask
	{
		#region Members

		/// <summary>
		/// How long to run this task... measured in frames
		/// This task will pause until the durection runs out, then resume running tasks
		/// </summary>
		private int Duration { get; set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLTask"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public BulletMLWait(BulletMLNode node, BulletMLTask owner) : base(node, owner)
		{
			Debug.Assert(null != Node);
			Debug.Assert(null != Owner);
		}

		/// <summary>
		/// Init this task and all its sub tasks. 
		/// This method should be called AFTER the nodes are parsed, but BEFORE run is called.
		/// </summary>
		protected override void Init()
		{
			base.Init();
			Duration = (int)Node.GetValue(this) + 1;
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			Duration--;
			if (Duration >= 0)
			{
				return ERunStatus.Stop;
			}
			else
			{
				TaskFinished = true;
				return ERunStatus.End;
			}
		}

		#endregion //Methods
	}
}