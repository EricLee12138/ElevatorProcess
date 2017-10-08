using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

namespace ElevatorProcess
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		List<Elevator> elevators = new List<Elevator> ();

		// go-up-and-down buttons
		List<List<Button>> goUpButtons = new List<List<Button>> ();
		List<List<Button>> goDownButtons = new List<List<Button>> ();
		List<Button> floorOptionButtons = new List<Button> ();

		// click support
		ButtonState prev = ButtonState.Released;

		// long press support
		int pressTime = 0;

		// algorithm support
		int currentFloor = 1;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";

			// set resolution
			graphics.PreferredBackBufferWidth = Constants.WindowWidth;
			graphics.PreferredBackBufferHeight = Constants.WindowHeight;

			//set mouse visible
			IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
            
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			//TODO: use this.Content to load your game content here
			for (int i = 0; i < Constants.ElevatorNum; i++) {
				Elevator elevator = new Elevator (Content,
					i + 1,
					Constants.elevatorOffset + i * Constants.ElevatorDistance,
					Constants.WindowHeight / 2);
				elevators.Add (elevator);
			}

			for (int i = 0; i < Constants.FloorNum; i++) {
				Button button = new Button (Content,
					(@"Graphics\Button\Button" + (char)('0' + (i + 1) / 10)) + (char)('0' + (i + 1) % 10),
					Constants.WindowWidth / 2 + (i - Constants.FloorNum / 2) * 2 * Constants.ButtonDistance,
					Constants.WindowHeight - 50);
				floorOptionButtons.Add (button);
			}
			floorOptionButtons [0].ButtonState = ElevatorButtonState.Pressed;

			// load go-up-and-down buttons
			for (int i = 0; i < Constants.FloorNum; i++) {
				List<Button> buttons = new List<Button> ();

				if (i != Constants.FloorNum - 1) {
					for (int j = 0; j < Constants.ElevatorNum; j++) {
						Button button = new Button (Content,
							                @"Graphics\Button\GoUp",
							                i + 1,
							                Constants.goUoDownButtonOffset + j * Constants.ElevatorDistance,
							                Constants.WindowHeight / 2);
						buttons.Add (button);
					}
					goUpButtons.Add (buttons);
				}

				if (i != 0) {
					buttons = new List<Button> ();
					for (int j = 0; j < Constants.ElevatorNum; j++) {
						Button button = new Button (Content,
							                @"Graphics\Button\GoDown",
							                i + 1,
							                Constants.goUoDownButtonOffset + j * Constants.ElevatorDistance,
							                Constants.WindowHeight / 2 + Constants.ButtonDistance);
						buttons.Add (button);
					}
					goDownButtons.Add (buttons);
				}
			}
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif
            
			MouseState mouse = Mouse.GetState();

			// mouse left button click detection
			if (mouse.LeftButton == ButtonState.Pressed && prev == ButtonState.Released) {
				// go-up buttons pressed detection
				if (currentFloor != Constants.FloorNum)
					for (int i = 0; i < goUpButtons[currentFloor - 1].Count; i++) {
						Button button = goUpButtons[currentFloor - 1][i];
						if (button.DrawRectangle.Contains (mouse.X, mouse.Y)) {
							if (button.ButtonState == ElevatorButtonState.Released)
								foreach (Button button_c in goUpButtons[currentFloor - 1]) {
									button_c.ButtonState = ElevatorButtonState.Pressed;
								}
						
							foreach (Elevator elevator in elevators) {
								/* * * * * * * * */
								/* * * * * * * * */
								if (elevator.FloorNum == currentFloor &&
								    elevator.ElevatorDirection == ElevatorDirection.Stop) {
									elevator.Priority = 1;
								} else {
									if (elevator.FloorNum < currentFloor &&
									    elevator.DestinationExistHigherThan (currentFloor)) {
										elevator.Priority = 2;
									} else {
										if (elevator.FloorNum < currentFloor &&
										    elevator.ElevatorDirection == ElevatorDirection.Stop) {
											elevator.Priority = 3;
										} else {
											if (elevator.FloorNum > currentFloor &&
											    elevator.ElevatorDirection == ElevatorDirection.Stop) {
												elevator.Priority = 3;
											} else {
												if (elevator.FloorNum < currentFloor &&
												    !elevator.DestinationExistHigherThan (currentFloor)) {
													elevator.Priority = 5;
												} else
													elevator.Priority = 6;
											}
										}
									}
								}
								/* * * * * * * * */
								/* * * * * * * * */
							}

							elevators.Sort ((elevator1, elevator2) =>
								Math.Abs (elevator1.ID - (i + 1)) - Math.Abs (elevator2.ID - (i + 1)));
							elevators.Sort ((elevator1, elevator2) =>
								Math.Abs (elevator1.FloorNum - currentFloor) - Math.Abs (elevator2.FloorNum - currentFloor));
							elevators.Sort ((elevator1, elevator2) =>
								elevator1.Priority - elevator2.Priority);
							if (elevators [0].Priority != 6) {
								if (elevators [0].Priority == 1) {
									elevators [0].IsOpen = true;
									elevators [0].ElapsedWaitingTime = 0;
								} else {
									elevators [0].Destinations.Add (currentFloor);
								}
							}
						}
					}
				// go-down buttons pressed detection
				if (currentFloor != 1)
					for (int i = 0; i < goDownButtons[currentFloor - 2].Count; i++) {
						Button button = goDownButtons [currentFloor - 2] [i];
						if (button.DrawRectangle.Contains (mouse.X, mouse.Y)) {
							if (button.ButtonState == ElevatorButtonState.Released)
								foreach (Button button_c in goDownButtons[currentFloor - 2]) {
									button_c.ButtonState = ElevatorButtonState.Pressed;
								}
						
							foreach (Elevator elevator in elevators) {
								/* * * * * * * * */
								/* * * * * * * * */
								if (elevator.FloorNum == currentFloor &&
								   elevator.ElevatorDirection == ElevatorDirection.Stop) {
									elevator.Priority = 1;
								} else {
									if (elevator.FloorNum > currentFloor &&
									   elevator.DestinationExistLowerThan (currentFloor)) {
										elevator.Priority = 2;
									} else {
										if (elevator.FloorNum > currentFloor &&
										   elevator.ElevatorDirection == ElevatorDirection.Stop) {
											elevator.Priority = 3;
										} else {
											if (elevator.FloorNum < currentFloor &&
											   elevator.ElevatorDirection == ElevatorDirection.Stop) {
												elevator.Priority = 3;
											} else {
												if (elevator.FloorNum > currentFloor &&
												   !elevator.DestinationExistLowerThan (currentFloor)) {
													elevator.Priority = 5;
												} else
													elevator.Priority = 6;
											}
										}
									}
								}
								/* * * * * * * * */
								/* * * * * * * * */
							}

							elevators.Sort ((elevator1, elevator2) =>
								Math.Abs (elevator1.ID - (i + 1)) - Math.Abs (elevator2.ID - (i + 1)));
							elevators.Sort ((elevator1, elevator2) =>
								Math.Abs (elevator1.FloorNum - currentFloor) - Math.Abs (elevator2.FloorNum - currentFloor));
							elevators.Sort ((elevator1, elevator2) =>
								elevator1.Priority - elevator2.Priority);
							if (elevators [0].Priority != 6) {
								if (elevators [0].Priority == 1) {
									elevators [0].IsOpen = true;
									elevators [0].ElapsedWaitingTime = 0;
								} else {
//								elevators [0].IsOpen = false;
									elevators [0].Destinations.Add (currentFloor);
								}
							}
						}
					}

				// elevator doors opened detection
				// and door-open or door-close button pressed detection
				// and destination floor number buttons pressed detection
				foreach (Elevator elevator in elevators) {
					if (elevator.ElevatorController.OpenButton.DrawRectangle.Contains (mouse.X, mouse.Y)) {
						elevator.ElapsedWaitingTime = 0;
					}
					if (elevator.ElevatorController.CloseButton.DrawRectangle.Contains (mouse.X, mouse.Y)) {
						elevator.ElapsedWaitingTime = Constants.ElevatorWaitingDuration;
					}

					for (int i = 0; i < elevator.ElevatorController.FloorControllers.Count; i++) {
						if (elevator.ElevatorController.FloorControllers[i].DrawRectangle.Contains (mouse.X, mouse.Y) &&
							i != currentFloor - 1 &&
							elevator.ElevatorController.IsVisible) {
							elevator.ElevatorController.FloorControllers[i].ButtonState = ElevatorButtonState.Pressed;
							elevator.Destinations.Add (i + 1);
						}

					}
				}

				// current floor number chosen detection
				for (int i = 0; i < Constants.FloorNum; i++)
					if (floorOptionButtons [i].DrawRectangle.Contains (mouse.X, mouse.Y))
						currentFloor = i + 1;

				for (int i = 0; i < Constants.FloorNum; i++) {
					if (currentFloor == i + 1)
						floorOptionButtons [i].ButtonState = ElevatorButtonState.Pressed;
					else
						floorOptionButtons [i].ButtonState = ElevatorButtonState.Released;
				}

				// long-press delay reset
				pressTime = 0;
			}

			// mouse left button long press detection
			if (mouse.LeftButton == ButtonState.Pressed && prev == ButtonState.Pressed) {
				pressTime += gameTime.ElapsedGameTime.Milliseconds;
				if (pressTime > Constants.LongPressDelay) {
					foreach (Button button in goUpButtons[currentFloor - 1]) {
						if (button.DrawRectangle.Contains (mouse.X, mouse.Y)) {
							if (button.ButtonState == ElevatorButtonState.Pressed)
								foreach (Button button_c in goUpButtons[currentFloor - 1]) {
									button_c.ButtonState = ElevatorButtonState.Released;
								}
						}
					}

					foreach (Button button in goDownButtons[currentFloor - 1]) {
						if (button.DrawRectangle.Contains (mouse.X, mouse.Y)) {
							if (button.ButtonState == ElevatorButtonState.Pressed)
								foreach (Button button_c in goDownButtons[currentFloor - 1]) {
									button_c.ButtonState = ElevatorButtonState.Released;
								}
						}
					}
				}
			}

			// previous mouse left button state
			prev = mouse.LeftButton;

			foreach (Elevator elevator in elevators) {
				if (elevator.FloorNum == currentFloor &&
					elevator.ElevatorDirection == ElevatorDirection.Stop) {
					if (currentFloor != Constants.FloorNum)
						foreach (Button button in goUpButtons[currentFloor - 1])
							button.ButtonState = ElevatorButtonState.Released;
					if (currentFloor != 1)
						foreach (Button button in goDownButtons[currentFloor - 2])
							button.ButtonState = ElevatorButtonState.Released;
				}
				elevator.Update (gameTime, spriteBatch, currentFloor);
			}
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.LightGray);
            
			spriteBatch.Begin();

			foreach (Elevator elevator in elevators) {
				elevator.Draw (spriteBatch, currentFloor);
			}
				
			foreach (List<Button> buttons in goUpButtons) {
				foreach (Button button in buttons) {
					if (button.FloorNum == currentFloor)
						button.Draw (spriteBatch);
				}
			}

			foreach (List<Button> buttons in goDownButtons) {
				foreach (Button button in buttons) {
					if (button.FloorNum == currentFloor)
						button.Draw (spriteBatch);
				}
			}

			foreach (Button button in floorOptionButtons) {
				button.Draw (spriteBatch);
			}

			spriteBatch.End ();
            
			base.Draw (gameTime);
		}
	}
}

