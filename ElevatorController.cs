using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ElevatorProcess
{
	public class ElevatorController
	{
		#region Fields

		// if the controller is visible
		bool isVisible = false;

		// floor controller
		List<Button> floorControllers = new List<Button>();

		// door controller
		Button openButton;
		Button closeButton;

		// elevator control panel
		State floorNum;

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets floor number
		/// </summary>
		public bool IsVisible
		{
			get { return isVisible; }
			set { isVisible = value; }
		}

		/// <summary>
		/// Gets and sets floor controllers
		/// </summary>
		public List<Button> FloorControllers
		{
			get { return floorControllers; }
			set { floorControllers = value; }
		}

		/// <summary>
		/// Gets open button
		/// </summary>
		public Button OpenButton
		{
			get { return openButton; }
		}

		/// <summary>
		/// Gets open button
		/// </summary>
		public Button CloseButton
		{
			get { return closeButton; }
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
		public ElevatorController(ContentManager contentManager,
			int x, int y)
		{
			floorControllers = new List<Button> ();
			for (int i = 0; i < Constants.FloorNum; i++) {
				Button button;
				if (i < Constants.FloorNum / 2)
					button = new Button (contentManager,
						(@"Graphics\Button\Button" + (char)('0' + (i + 1) / 10)) + (char)('0' + (i + 1) % 10),
						x - Constants.ButtonDistance,
						y + (i - Constants.FloorNum / 4) * Constants.ButtonDistance * 2 + 50);
				else
					button = new Button (contentManager,
						(@"Graphics\Button\Button" + (char)('0' + (i + 1) / 10)) + (char)('0' + (i + 1) % 10),
						x + Constants.ButtonDistance,
						y + (i - Constants.FloorNum * 3 / 4) * Constants.ButtonDistance * 2 + 50);
				floorControllers.Add (button);
			}

			openButton = new Button (contentManager,
				@"Graphics\Button\Open",
				x - Constants.ButtonDistance,
				y - Constants.YDistance
			);
			closeButton = new Button (contentManager,
				@"Graphics\Button\Close",
				x + Constants.ButtonDistance,
				y - Constants.YDistance
			);

		}

		#endregion

		#region Public methods

		/// <summary>
		/// Draws the elevator controller
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (isVisible) {
				foreach (Button button in floorControllers) {
					button.Draw (spriteBatch);
				}
				openButton.Draw (spriteBatch);
				closeButton.Draw (spriteBatch);
			}
		}

		#endregion
	}
}