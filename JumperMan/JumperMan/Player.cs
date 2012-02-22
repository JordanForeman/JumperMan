using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace JumperMan
{
    class Player : Sprite
    {
        public static Game1 gameContext;

        const string Asset = "player";
        const int startX = 20;
        const int startY = 373;
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;

        public enum State { Walking, Jumping, Still }

        public int Health = 3;
        public Vector2 MoveSpeed = new Vector2(6.0f, 3.5f);
        public State CurrentState = State.Still;
        private Vector2 Dir = Vector2.Zero;

        private Vector2 StartPosition;
        private KeyboardState PreviousKeyboardState;
        private int WalkState = 0;

        public Player(Game1 g)
        {
            gameContext = g;
        }

        public void LoadContent(ContentManager theContentManager)
        {
            Position = new Vector2(startX, startY);
            base.LoadContent(theContentManager, Asset);
            Source = new Rectangle(0, 0, 28, Source.Height);
        }

        public void Update(GameTime theGameTime)
        {
            KeyboardState CurrentKeyboardState = Keyboard.GetState();
            GamePadState CurrentGamepadState = GamePad.GetState(PlayerIndex.One);
            UpdateMovement(CurrentKeyboardState, CurrentGamepadState);
            PreviousKeyboardState = CurrentKeyboardState;
            base.Update(theGameTime, MoveSpeed, Dir);
        }

        private void UpdateMovement(KeyboardState CurrentKeyboardState, GamePadState CurrentGamePadState)
        {
            //Keyboard / D-Pad controls
            if (CurrentKeyboardState.IsKeyDown(Keys.Left) || CurrentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                Position.X -= MoveSpeed.X;
                CurrentState = (CurrentState != State.Jumping) ? State.Walking : CurrentState;
            }
            if (CurrentKeyboardState.IsKeyDown(Keys.Right) || CurrentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                Position.X += MoveSpeed.X;
                CurrentState = (CurrentState != State.Jumping) ? State.Walking : CurrentState;
            }

            if (CurrentGamePadState.ThumbSticks.Left.X != 0)
            {
                Position.X += CurrentGamePadState.ThumbSticks.Left.X * MoveSpeed.X;
                CurrentState = (CurrentState != State.Jumping) ? State.Walking : CurrentState;
            }

            if (CurrentKeyboardState.IsKeyDown(Keys.Up) || CurrentKeyboardState.IsKeyDown(Keys.Space) || CurrentGamePadState.DPad.Up == ButtonState.Pressed || CurrentGamePadState.Buttons.A == ButtonState.Pressed)
            {
                UpdateJump(CurrentKeyboardState, CurrentGamePadState);
            }

            //TODO: Make crouching logic(?)
            if (CurrentKeyboardState.IsKeyDown(Keys.Down) || CurrentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                //CROUCH
            }

            if (!CurrentKeyboardState.IsKeyDown(Keys.Left) && !CurrentKeyboardState.IsKeyDown(Keys.Right) && CurrentGamePadState.DPad.Left != ButtonState.Pressed && CurrentGamePadState.DPad.Right != ButtonState.Pressed && CurrentGamePadState.ThumbSticks.Left.X == 0 && CurrentState == State.Walking)
            {
                CurrentState = State.Still;
                Source = new Rectangle(0, 0, 28, Source.Height);
            }

            if (CurrentState == Player.State.Jumping)
            {
                UpdateJump(CurrentKeyboardState, CurrentGamePadState);
            }

            if (CurrentState == State.Walking)
            {
                int x = 0;
                int width = 0;
                switch (WalkState)
                {
                    case 0:
                        x = 61;
                        width = 30;
                        WalkState += 1;
                        break;
                    case 1:
                        x = 94;
                        width = 24;
                        WalkState += 1;
                        break;
                    case 2:
                        x = 119;
                        width = 28;
                        WalkState += 1;
                        break;
                    case 3:
                        x = 0;
                        width = 28;
                        WalkState = 0;
                        break;
                }
                Source = new Rectangle(x, 0, width, Source.Height);
            }

            //Make sure that the player does not go out of bounds
            Position.X = MathHelper.Clamp(Position.X, 0, gameContext.GraphicsDevice.Viewport.Width - Size.Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, gameContext.GraphicsDevice.Viewport.Height - Size.Height);
        }

        public void UpdateJump(KeyboardState aCurrentKeyboardState, GamePadState aCurrentGamePadState)
        {
            if ((CurrentState == State.Walking || CurrentState == State.Still) && (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true || aCurrentGamePadState.DPad.Up == ButtonState.Pressed || aCurrentGamePadState.Buttons.A == ButtonState.Pressed))
            {
                Jump();
            }

            if (CurrentState == State.Jumping)
            {

                if (StartPosition.Y - Position.Y > 80)  //At peak of jump...
                { 
                    Dir.Y = MOVE_DOWN;
                    MoveSpeed.Y = 3.0f;
                }
                
                Position.Y = (Dir.Y == MOVE_DOWN) ? Position.Y + MoveSpeed.Y : Position.Y - MoveSpeed.Y; //Change Y position

                //END JUMP LOGIC
                if (Position.Y > StartPosition.Y)
                {
                    Position.Y = StartPosition.Y;
                    CurrentState = State.Still;
                    Source = new Rectangle(0, 0, 28, Source.Height);
                    Dir.Y = 0;
                    MoveSpeed.X = 6.0f;
                    MoveSpeed.Y = 3.5f;
                }
            }
        }

        //Start Jumping!
        private void Jump()
        {
            if (CurrentState != State.Jumping)
            {
                CurrentState = State.Jumping;
                Source = new Rectangle(29, 0, 31, Source.Height);
                StartPosition = Position;
                Dir.Y = MOVE_UP;
                MoveSpeed.X = 3.0f;
            }
        }
    }
}
