using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ElevatorProcess
{
	public class State
	{
		#region Fields
		// flashing support
		int elapsedGameTime = 0;
		int flashingDelay = 0;

		// drawing support
		string spriteName;
		Texture2D sprite;
		Rectangle drawRectangle;
		ContentManager contentManager;

		#endregion

		#region Properties

		/// <summary>
		/// Gets and sets falshing delay time
		/// </summary>
		public int FlashingDelay
		{
			get { return flashingDelay; }
			set { flashingDelay = value; }
		}

		/// <summary>
		/// Gets the collision rectangle for the state sprite
		/// </summary>
		public Rectangle DrawRectangle
		{
			get { return drawRectangle; }
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
		public State(ContentManager contentManager,
			string spriteName,
			int x, int y)
		{
			this.contentManager = contentManager;
			LoadContent(spriteName, x, y);
		}

		public State(ContentManager contentManager,
			string spriteName,
			int flashingDelay,
			int x, int y)
		{			
			this.contentManager = contentManager;
			this.flashingDelay = flashingDelay;
			this.spriteName = spriteName;
			LoadContent(this.spriteName, x, y);
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
		private void LoadContent(string spriteName,
			int x, int y)
		{
			// load content and set remainder of draw rectangle
			sprite = contentManager.Load<Texture2D>(spriteName);
			drawRectangle = new Rectangle (x - sprite.Width / 2,
				y - sprite.Height / 2,
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
			spriteBatch.Draw (sprite, drawRectangle, Color.White);
		}
			
		/// <summary>
		/// Updates the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Update(GameTime gameTime, SpriteBatch spriteBatch)
		{
			elapsedGameTime += (int)gameTime.ElapsedGameTime.Milliseconds;
			if (elapsedGameTime < flashingDelay) {
				drawRectangle.Y -= 1;
			} else if (elapsedGameTime > flashingDelay &&
			         elapsedGameTime < flashingDelay * 2) {
				drawRectangle.Y += 1;
			} else if (elapsedGameTime > flashingDelay * 2) {
				elapsedGameTime = 0;
			}

		}

		/// <summary>
		/// Updates the elevator
		/// </summary>
		/// <param name="spriteBatch">the sprite batch to use</param>
		public void Update(GameTime gameTime, SpriteBatch spriteBatch, string newSpriteName)
		{
			LoadContent (newSpriteName,
				drawRectangle.Center.X,
				drawRectangle.Center.Y);
			elapsedGameTime += (int)gameTime.ElapsedGameTime.Milliseconds;
			if (elapsedGameTime < flashingDelay) {
				drawRectangle.Y -= 1;
			} else if (elapsedGameTime > flashingDelay &&
				elapsedGameTime < flashingDelay * 2) {
				drawRectangle.Y += 1;
			} else if (elapsedGameTime > flashingDelay * 2) {
				elapsedGameTime = 0;
			}
		}

		#endregion
	}
}

