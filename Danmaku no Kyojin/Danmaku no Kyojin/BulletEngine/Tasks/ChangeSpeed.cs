using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This task changes the speed a little bit every frame.
	/// </summary>
	public class BulletMLChangeSpeed : BulletMLTask
	{
		#region Members

		/// <summary>
		/// The amount to change speed every frame
		/// </summary>
		private float SpeedChange { get; set; }

		/// <summary>
		/// How long to run this task... measured in frames
		/// </summary>
		private int Duration { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating whether this is the <see cref="BulletMLLib.BulletMLAccel"/> initial run.
		/// </summary>
		/// <value><c>true</c> if initial run; otherwise, <c>false</c>.</value>
		private bool InitialRun { get; set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLTask"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public BulletMLChangeSpeed(BulletMLNode node, BulletMLTask owner) : base(node, owner)
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
			InitialRun = true;
			Duration = (int)Node.GetChildValue(ENodeName.term, this);
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			if (InitialRun)
			{
				InitialRun = false;

				switch (Node.GetChild(ENodeName.speed).NodeType)
				{
					case ENodeType.sequence:
					{
						SpeedChange = Node.GetChildValue(ENodeName.speed, this);
					}
					break;

					case ENodeType.relative:
					{
						SpeedChange = Node.GetChildValue(ENodeName.speed, this) / Duration;
					}
					break;

					default:
					{
						SpeedChange = (Node.GetChildValue(ENodeName.speed, this) - bullet.Velocity) / Duration;
					}
					break;
				}
			}

			bullet.Velocity += SpeedChange;

			Duration--;
			if (Duration <= 0)
			{
				TaskFinished = true;
				return ERunStatus.End;
			}
			else
			{
				return ERunStatus.Continue;
			}
		}

		#endregion //Methods
	}
}