using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ElevatorProcess
{
	/// <summary>
	/// A class for buttons concerning an elevator
	/// </summary>
	public class Button
	{
		#region Fields

		// button state
		ElevatorButtonState buttonstate = ElevatorButtonState.Released;

		// floor number
		int floorNum = 0;

		// drawing support
		Texture2D sprite;
		Rectangle drawRectangle;

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an elevator teddy bear centered on the given x and y with the
		///  given velocity
		/// </summary>
		/// <param name="contentManager">the content manager for loading content</param>
		/// <param name="spriteName">the name of the sprite for the teddy bear</param>
		/// <param name="x">the x location of the center of the teddy bear</param>
		/// <param name="y">the y location of the center of the teddy bear</param>
		/// <param name="id">the identifing number or the elevator</param>
		public Button(ContentManager contentManager,
			string spriteName,
			int x, int y)
		{			
			LoadContent(contentManager, spriteName, x, y);
		}

		public Button(ContentManager contentManager,
			string spriteName,
			int floorNum,
			int x, int y)
		{			
			this.floorNum = floorNum;
			LoadContent(contentManager, spriteName, x, y);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets elevator button state
		/// </summary>
		public ElevatorButtonState ButtonState
		{
			get { return buttonstate; }
			set { buttonstate = value; }
		}

		/// <summary>
		/// Gets the collision rectangle for the button
		/// </summary>
		public Rectangle DrawRectangle
		{
			get { return drawRectangle; }
		}

		/// <summary>
		/// Gets the current floor number for the button
		/// </summary>
		public int FloorNum
		{
			get { return floorNum; }
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
			string spriteName,
			int x, int y)
		{
			// load content and set remainder of draw rectangle
			sprite = contentManager.Load<Texture2D>(spriteName);

			drawRectangle = new Rectangle(x - sprite.Width / 2,
				y - Constants.ButtonDistance - sprite.Height / 2,
				sprite.Width,
				sprite.Height);
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Draws the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			switch (buttonstate) {
			case ElevatorButtonState.Released:
				spriteBatch.Draw (sprite, drawRectangle, Color.White);
				break;

			case ElevatorButtonState.Pressed:
				spriteBatch.Draw (sprite, drawRectangle, Color.GreenYellow);
				break;
			}
		}

		/// <summary>
		/// Draws the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Draw(SpriteBatch spriteBatch, Color color)
		{
			spriteBatch.Draw (sprite, drawRectangle, color);
		}

		/// <summary>
		/// Draws the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Update(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin ();
			Draw (spriteBatch);
			spriteBatch.End ();
		}

		#endregion

	}
}

