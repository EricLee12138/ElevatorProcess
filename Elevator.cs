using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ElevatorProcess
{
	/// <summary>
	/// A class for an elevator
	/// </summary>
	public class Elevator
	{
		#region Fields
		// elevator id
		int id;

		// destination
		List<int> destinations = new List<int>();

		// current elevator direction
		State directionState;
		ElevatorDirection elevatorDirection;

		// current floor state
		State floorState;
		int floorNum = 1;

		// elevator controller
		ElevatorController elevatorController;

		// elevator drawing support
		State elevatorState;
//		Texture2D elevatorSprite;
//		Rectangle elevatorDrawRectangle;

		// elevator delay time support
		int elapsedMovingTime = 0;
		int elapsedWaitingTime = Constants.ElevatorWaitingDuration;

		// door open or close
		bool isOpen = false;

		// elevator responding priority
		int priority = 0;

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets floor number
		/// </summary>
		public int ID
		{
			get { return id; }
		}

		/// <summary>
		/// Gets and sets floor number
		/// </summary>
		public int FloorNum
		{
			get { return floorNum; }
			set { floorNum = value; }
		}

		/// <summary>
		/// Gets and sets elevator direction
		/// </summary>
		public ElevatorDirection ElevatorDirection
		{
			get { return elevatorDirection; }
			set { elevatorDirection = value; }
		}

		/// <summary>
		/// Gets the collision rectangle for the elevator
		/// </summary>
		public Rectangle DrawRectangle
		{
			get { return elevatorState.DrawRectangle; }
		}

		/// <summary>
		/// Gets the collision rectangle for the elevator
		/// </summary>
		public ElevatorController ElevatorController
		{
			get { return elevatorController; }
		}

		public List<int> Destinations
		{
			get { return destinations; }
			set { destinations = value; }
		}

		public bool IsOpen
		{
			get { return isOpen; }
			set { isOpen = value; }
		}

		public int ElapsedWaitingTime
		{
			get { return elapsedWaitingTime; }
			set { elapsedWaitingTime = value; }
		}

		public int Priority
		{
			get { return priority; }
			set { priority = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an elevator centered on the given x and y with the
		///  given velocity
		/// </summary>
		/// <param name="contentManager">the content manager for loading content</param>
		/// <param name="spriteName">the name of the sprite for the teddy bear</param>
		/// <param name="x">the x location of the center of the teddy bear</param>
		/// <param name="y">the y location of the center of the teddy bear</param>
		/// <param name="id">the identifing number or the elevator</param>
		public Elevator(ContentManager contentManager,
			int id,
			int x, int y)
		{
			this.elevatorDirection = ElevatorDirection.Stop;
			this.id = id;
			LoadContent(contentManager,
				@"Graphics\Elevator",
				@"Graphics\State\Stop",
				(@"Graphics\Number\" + (char)('0' + floorNum / 10)) + (char)('0' + floorNum % 10),
				x, y);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Loads the content for the elevator
		/// </summary>
		/// <param name="contentManager">the content manager to use</param>
		/// <param name="spriteName">the name of the sprite for the teddy bear</param>
		/// <param name="x">the x location of the center of the teddy bear</param>
		/// <param name="y">the y location of the center of the teddy bear</param>
		private void LoadContent(ContentManager contentManager,
			string elevatorSpriteName,
			string stateSpriteName,
			string floorSpriteName,
			int x, int y)
		{
			// load content and set remainder of draw rectangle
			elevatorState = new State (contentManager,
				elevatorSpriteName,
				x,
				y);
			directionState = new State (contentManager, stateSpriteName,
				x + Constants.XDistance,
				y - Constants.YDistance);
			floorState = new State (contentManager, floorSpriteName,
				x - Constants.XDistance,
				y - Constants.YDistance);
			elevatorController = new ElevatorController (contentManager,
				Constants.WindowWidth - Constants.elevatorOffset * 3 / 2,
				Constants.WindowHeight / 2);
		}


		private void Move(GameTime gameTime, SpriteBatch spriteBatch){
			destinations.Sort ((x, y) => x.CompareTo (y));
			if (destinations [0] > floorNum)
				elevatorDirection = ElevatorDirection.Up;
			else if (destinations [0] < floorNum) {
				destinations.Sort ((x, y) => -x.CompareTo (y));
				elevatorDirection = ElevatorDirection.Down;
			}

			if (destinations.Count > 0) {
				elapsedMovingTime += gameTime.ElapsedGameTime.Milliseconds;
				if (elapsedMovingTime > Constants.ElevatorMovingDuration) {
					elapsedMovingTime = 0;
					if (elevatorDirection == ElevatorDirection.Up) {
						if (floorNum == destinations [0]) {
							elevatorController.FloorControllers [destinations [0] - 1].ButtonState = ElevatorButtonState.Released;
							destinations.RemoveAt (0);
							elevatorDirection = ElevatorDirection.Stop;
							isOpen = true;
							elapsedWaitingTime = 0;
						} else
							floorNum++;
					} else if (elevatorDirection == ElevatorDirection.Down) {
						if (floorNum == destinations [0]) {
							elevatorController.FloorControllers [destinations [0] - 1].ButtonState = ElevatorButtonState.Released;
							destinations.RemoveAt (0);
							elevatorDirection = ElevatorDirection.Stop;
							isOpen = true;
							elapsedWaitingTime = 0;
						} else
							floorNum--;
					} else {
						elevatorController.FloorControllers [destinations [0] - 1].ButtonState = ElevatorButtonState.Released;
						destinations.RemoveAt (0);
						elevatorDirection = ElevatorDirection.Stop;
						isOpen = true;
						elapsedWaitingTime = 0;
					}
					floorState.Update (gameTime,
						spriteBatch,
						(@"Graphics\Number\" + (char)('0' + floorNum / 10)) + (char)('0' + floorNum % 10));
				}
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Draws the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch, int currentFloor)
		{
			elevatorState.Draw (spriteBatch);
			directionState.Draw (spriteBatch);
			floorState.Draw (spriteBatch);
			if (floorNum == currentFloor)
				elevatorController.Draw (spriteBatch);
		}

		/// <summary>
		/// Updates the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Update(GameTime gameTime, SpriteBatch spriteBatch, int currentFloor)
		{
			if (elevatorDirection == ElevatorDirection.Up) {
				directionState.FlashingDelay = 500;
				directionState.Update (gameTime, spriteBatch, @"Graphics\State\Up");
			} else if (elevatorDirection == ElevatorDirection.Down) {
				directionState.FlashingDelay = 500;
				directionState.Update (gameTime, spriteBatch, @"Graphics\State\Down");
			} else {
				directionState.FlashingDelay = 0;
				directionState.Update (gameTime, spriteBatch, @"Graphics\State\Stop");
			}

			if (isOpen) {
				if (floorNum == currentFloor)
					elevatorState.Update (gameTime, spriteBatch, @"Graphics\Elevator_Open");
				else
					elevatorState.Update (gameTime, spriteBatch, @"Graphics\Elevator");
				elevatorController.IsVisible = true;
				foreach (Button button in elevatorController.FloorControllers)
					button.Update (spriteBatch);
				
				elapsedWaitingTime += gameTime.ElapsedGameTime.Milliseconds;
				if (elapsedWaitingTime > Constants.ElevatorWaitingDuration) {
					elapsedWaitingTime = 0;
					isOpen = false;
				}
			} else {
				elevatorState.Update (gameTime, spriteBatch, @"Graphics\Elevator");
				elevatorController.IsVisible = false;
				if (destinations.Count > 0)
					Move (gameTime, spriteBatch);
			}
		}

		public bool DestinationExistHigherThan(int current){
			foreach (int dest in destinations) {
				if (dest >= current)
					return true;
			}
			return false;
		}

		public bool DestinationExistLowerThan(int current){
			foreach (int dest in destinations) {
				if (dest <= current)
					return true;
			}
			return false;
		}

		#endregion

	}
}

