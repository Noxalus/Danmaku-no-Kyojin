using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This is the bullet class that outside assemblies will interact with.
	/// Just inherit from this class and override the abstract functions!
	/// </summary>
	public abstract class Bullet
	{
		#region Members

		/// <summary>
		/// The direction this bullet is travelling.  Measured as an angle in radians
		/// </summary>
		private float _direction;

		/// <summary>
		/// A bullet manager that manages this bullet.
		/// </summary>
		/// <value>My bullet manager.</value>
		private readonly IBulletManager _bulletManager;

		/// <summary>
		/// A list of tasks that will define this bullets behavior
		/// </summary>
		internal List<BulletMLTask> _tasks;

		/// <summary>
		/// The fire data objects.  There is one of these for each top level task node in the _tasks list
		/// </summary>
		private List<FireData> _fireData;

		/// <summary>
		/// The tree node that describes this bullet.  These are shared between multiple bullets
		/// </summary>
		public BulletMLNode MyNode { get; private set; }

		/// <summary>
		/// Index of the current active task
		/// </summary>
		private int _activeTaskNum = 0;

		//TODO: do a task factory, we are going to be creating a LOT of those little dudes

		#endregion //Members

		#region Properties

		/// <summary>
		/// The acceleration of this bullet
		/// </summary>
		/// <value>The accel, in pixels/frame^2</value>
		public Vector2 Acceleration { get; set; }

		/// <summary>
		/// Gets or sets the velocity
		/// </summary>
		/// <value>The velocity, in pixels/frame</value>
		public float Velocity { get; set; }

		/// <summary>
		/// Abstract property to get the X location of this bullet.
		/// measured in pixels from upper left
		/// </summary>
		/// <value>The horizontrla position.</value>
		public abstract float X
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the y parameter of the location
		/// measured in pixels from upper left
		/// </summary>
		/// <value>The vertical position.</value>
		public abstract float Y
		{
			get;
			set;
		}

		/// <summary>
		/// Gets my bullet manager.
		/// </summary>
		/// <value>My bullet manager.</value>
		public IBulletManager MyBulletManager
		{
			get
			{
				return _bulletManager;
			}
		}

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction in radians.</value>
		public float Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;

				//keep the direction between 0-360
				if (_direction > 2 * Math.PI)
				{
					_direction -= (float)(2 * Math.PI);
				}
				else if (_direction < 0)
				{
					_direction += (float)(2 * Math.PI);
				}
			}
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.Bullet"/> class.
		/// </summary>
		/// <param name="myBulletManager">My bullet manager.</param>
		public Bullet(IBulletManager myBulletManager)
		{
			//grba the bullet manager for this dude
			Debug.Assert(null != myBulletManager);
			_bulletManager = myBulletManager;

			Acceleration = Vector2.Zero;

			_tasks = new List<BulletMLTask>();
			_fireData = new List<FireData>();
		}

		/// <summary>
		/// This is the method that should be used to create tasks for this dude, since they have to sync up with firedata objects
		/// </summary>
		/// <returns>An empty task</returns>
		public BulletMLTask CreateTask()
		{
			BulletMLTask task = new BulletMLTask(null, null);
			_tasks.Add(task);
			_fireData.Add(new FireData());
			return task;
		}

		/// <summary>
		/// Initialize this bullet with a top level node
		/// </summary>
		/// <param name="rootNode">This is a top level node... find the first "top" node and use it to define this bullet</param>
		public void InitTop(BulletMLNode rootNode)
		{
			Debug.Assert(null != rootNode);

			//clear everything out
			_tasks.Clear();
			_fireData.Clear();
			_activeTaskNum = 0;

			//Grab that top level node
			MyNode = rootNode;

			//okay find the item labelled 'top'
			BulletMLNode topNode = rootNode.FindLabelNode("top", ENodeName.action);
			if (topNode != null)
			{
				//We found a top node, add a task for it, also add a firedata for the task
				BulletMLTask task = CreateTask();

				//parse the nodes into the task list
				task.Parse(topNode, this);
			}
			else
			{
				//ok there is no 'top' node, so that means we have a list of 'top#' nodes
				for (int i = 1; i < 10; i++)
				{
					topNode = rootNode.FindLabelNode("top" + i, ENodeName.action);
					if (topNode != null)
					{
						//found a top num node, add a task and firedata for it
						BulletMLTask task = CreateTask();

						//parse the nodes into the task list
						task.Parse(topNode, this);
					}
				}
			}
		}
		
		/// <summary>
		/// This bullet is fired from another bullet, initialize it from the node that fired it
		/// </summary>
		/// <param name="subNode">Sub node that defines this bullet</param>
		public void Init(BulletMLNode subNode)
		{
			Debug.Assert(null != subNode);
			
			//clear everything out
			_activeTaskNum = 0;
			
			//Grab that top level node
			MyNode = subNode;

			//Either get the first task, or create a task for the node
			//If this dude is from a FireRef task, there will already be a plain task in there with all the required params
			BulletMLTask task = ((0 != _tasks.Count) ? _tasks[0] : CreateTask());

			//parse the nodes into the task list
			task.Parse(subNode, this);
		}

		/// <summary>
		/// Update this bullet.  Called once every 1/60th of a second during runtime
		/// </summary>
        public virtual void Update(GameTime gameTime)
		{
			//Flag to tell whether or not this bullet has finished all its tasks
			for (int i = 0; i < _tasks.Count; i++)
			{
				_activeTaskNum = i;
				_tasks[i].Run(this);
			}

			//only do this stuff if the bullet isn't done, cuz sin/cosin are expensive
			X += Acceleration.X + (float)(Math.Sin(Direction) * Velocity * ((float)gameTime.ElapsedGameTime.TotalSeconds * 150));
            Y += Acceleration.Y + (float)(-Math.Cos(Direction) * Velocity * ((float)gameTime.ElapsedGameTime.TotalSeconds * 150));
		}

		/// <summary>
		/// Get the direction to aim that bullet
		/// </summary>
		/// <returns>angle to target the bullet</returns>
		public float GetAimDir()
		{
			//get the player position so we can aim at that little fucker
			Debug.Assert(null != MyBulletManager);
			Vector2 shipPos = MyBulletManager.PlayerPosition(this);

			//TODO: this function doesn't seem to work... bullets sometimes just spin around in circles?

			//get the angle at that dude
			float val = (float)Math.Atan2((shipPos.X - X), -(shipPos.Y - Y));
			return val;
		}

		/// <summary>
		/// Gets the fire data for the current active task
		/// </summary>
		/// <returns>The fire data.</returns>
		public FireData GetFireData()
		{
			Debug.Assert(_fireData.Count == _tasks.Count);
			Debug.Assert(_activeTaskNum < _fireData.Count);
			return _fireData[_activeTaskNum];
		}

		#endregion //Methods
	}
}
