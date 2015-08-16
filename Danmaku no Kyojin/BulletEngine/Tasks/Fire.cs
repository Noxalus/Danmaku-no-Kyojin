using System;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// A task to shoot a bullet
	/// </summary>
	public class BulletMLFire : BulletMLTask
	{
		#region Members

		/// <summary>
		/// The node we are going to use to set the direction of any bullets shot with this task
		/// </summary>
		/// <value>The dir node.</value>
		private BulletMLNode DirNode { get; set; }

		/// <summary>
		/// The node we are going to use to set the speed of any bullets shot with this task
		/// </summary>
		/// <value>The speed node.</value>
		private BulletMLNode SpeedNode { get; set; }

		/// <summary>
		/// A reference node this task will use to set bullets shot with this task
		/// </summary>
		/// <value>The reference node.</value>
		private BulletMLNode RefNode { get; set; }

		/// <summary>
		/// A bullet node this task will use to set any bullets shot from this task
		/// </summary>
		/// <value>The bullet node.</value>
		private BulletMLNode BulletNode { get; set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLFire"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public BulletMLFire(BulletMLNode node, BulletMLTask owner) : base(node, owner)
		{
			Debug.Assert(null != Node);
			Debug.Assert(null != Owner);

			//First try and get the nodes striaght from our node
			DirNode = node.GetChild(ENodeName.direction);
			SpeedNode = node.GetChild(ENodeName.speed);
			RefNode = node.GetChild(ENodeName.bulletRef);
			BulletNode = node.GetChild(ENodeName.bullet);

			//ok fire nodes HAVE TO have either a ref or a bullet node
			Debug.Assert((null != RefNode) || (null != BulletNode));

			//what we gonna use to set the direction if we couldnt find a node?
			if (null == DirNode)
			{
				if (null != RefNode)
				{
					//Get the direction from the reference node
					DirNode = RefNode.GetChild(ENodeName.direction);
				}
				else if (BulletNode != null)
				{
					//Set teh driection from teh bullet node
					DirNode = BulletNode.GetChild(ENodeName.direction);
				}
			}

			//what we gonna use to set teh speed if we couldnt find a node?
			if (null == SpeedNode)
			{
				if (null != RefNode)
				{
					//set the speed from teh refernce node
					SpeedNode = RefNode.GetChild(ENodeName.speed);
				}
				else if (null != BulletNode)
				{
					//set teh speed from the bullet node
					SpeedNode = BulletNode.GetChild(ENodeName.speed);
				}
			}
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			//Find which direction to shoot the new bullet
			if (DirNode != null)
			{
				//get the direction continade in the node
				float newBulletDirection = (int)DirNode.GetValue(this) * (float)Math.PI / (float)180;
				switch (DirNode.NodeType)
				{
					case ENodeType.sequence:
					{
						bullet.GetFireData().srcDir += newBulletDirection;
					}
					break;

					case ENodeType.absolute:
					{
						bullet.GetFireData().srcDir = newBulletDirection;
					}
					break;

					case ENodeType.relative:
					{
						bullet.GetFireData().srcDir = newBulletDirection + bullet.Direction;
					}
					break;

					default:
					{
						bullet.GetFireData().srcDir = newBulletDirection + bullet.GetAimDir();
					}
					break;
				}
			}
			else
			{
				//otherwise if no direction node, aim the bullet at the bad guy
				bullet.GetFireData().srcDir = bullet.GetAimDir();
			}

			//Create the new bullet
			Bullet newBullet = bullet.MyBulletManager.CreateBullet();

			if (newBullet == null)
			{
				//wtf did you do???
				TaskFinished = true;
				return ERunStatus.End;
			}

			//initialize the bullet from a reference node, or our bullet node
			if (RefNode != null)
			{
				//Add an empty task to the bullet and populate it with all the params
				BulletMLTask bulletBlankTask = newBullet.CreateTask();

				//Add all the params to the new task we just added to that bullet
				for (int i = 0; i < RefNode.ChildNodes.Count; i++)
				{
					bulletBlankTask.ParamList.Add(RefNode.ChildNodes[i].GetValue(this));
				}

				//init the bullet now that all our stuff is prepopulated
				BulletMLNode subNode = bullet.MyNode.GetRootNode().FindLabelNode(RefNode.Label, ENodeName.bullet);
				newBullet.Init(subNode);
			}
			else
			{
				//if there is no ref node, there has to be  bullet node
				newBullet.Init(BulletNode);
			}

			//set the location of the new bullet
			newBullet.X = bullet.X;
			newBullet.Y = bullet.Y;

			//set the owner of the new bullet to this dude
			newBullet._tasks[0].Owner = this;

			//set the direction of the new bullet
			newBullet.Direction = bullet.GetFireData().srcDir;

			//Has the speed for new bullets been set in the source bullet yet?
			if (!bullet.GetFireData().speedInit && newBullet.GetFireData().speedInit)
			{
				bullet.GetFireData().srcSpeed = newBullet.Velocity;
				bullet.GetFireData().speedInit = true;
			}
			else
			{
				//find the speed for new bullets and store it in the source bullet
				if (SpeedNode != null)
				{
					//Get the speed from a speed node
					float newBulletSpeed = SpeedNode.GetValue(this);
					if (SpeedNode.NodeType == ENodeType.sequence || SpeedNode.NodeType == ENodeType.relative)
					{
						bullet.GetFireData().srcSpeed += newBulletSpeed;
					}
					else
					{
						bullet.GetFireData().srcSpeed = newBulletSpeed;
					}
				}
				else
				{
					if (!newBullet.GetFireData().speedInit)
					{
						bullet.GetFireData().srcSpeed = 1;
					}
					else
					{
						bullet.GetFireData().srcSpeed = newBullet.Velocity;
					}
				}
			}

			newBullet.GetFireData().speedInit = false;
			newBullet.Velocity = bullet.GetFireData().srcSpeed;

			TaskFinished = true;
			return ERunStatus.End;
		}

		#endregion //Methods
	}
}