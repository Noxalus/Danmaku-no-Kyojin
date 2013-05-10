using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This task adds acceleration to a bullet.
	/// </summary>
	public class BulletMLAccel : BulletMLTask
	{
		#region Members

		/// <summary>
		/// How long to run this task... measured in frames
		/// </summary>
		private int Duration { get; set; }

		/// <summary>
		/// The direction to accelerate in 
		/// </summary>
		private Vector2 _Acceleration = Vector2.Zero;
		
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
		public BulletMLAccel(BulletMLNode node, BulletMLTask owner) : base(node, owner)
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
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			//is this the fisrt time running this task?
			if (InitialRun)
			{
				//if this is the first time, set the accelerataion we are gonna add to the bullet
				InitialRun = false;
				Duration = (int)Node.GetChildValue(ENodeName.term, this);
				switch (Node.NodeType)
				{
					case ENodeType.sequence:
						{
						_Acceleration.X = Node.GetChildValue(ENodeName.horizontal, this);
						_Acceleration.Y = Node.GetChildValue(ENodeName.vertical, this);
						}
						break;

					case ENodeType.relative:
						{
						_Acceleration.X = Node.GetChildValue(ENodeName.horizontal, this) / Duration;
						_Acceleration.Y = Node.GetChildValue(ENodeName.vertical, this) / Duration;
						}
						break;

					default:
						{
						_Acceleration.X = (Node.GetChildValue(ENodeName.horizontal, this) - bullet.Acceleration.X) / Duration;
						_Acceleration.Y = (Node.GetChildValue(ENodeName.vertical, this) - bullet.Acceleration.Y) / Duration;
						}
						break;
				}
			}

			//Add the acceleration to the bullet
			bullet.Acceleration += _Acceleration;

			//decrement the amount if time left to run and return End when this task is finished
			Duration--;
			if (Duration <= 0)
			{
				TaskFinished = true;
				return ERunStatus.End;
			}
			else 
			{
				//since this task isn't finished, run it again next time
				return ERunStatus.Continue;
			}
		}

		#endregion //Methods
	}
}