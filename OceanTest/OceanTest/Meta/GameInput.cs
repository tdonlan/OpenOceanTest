using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


//class that contains both the most  keyboard input (can be used to record fighting game style combos), 
//and the mappings of keys to game "buttons" - abstract away the keyboard vs gamepad
namespace OceanTest
{
    public class GameInput
    {
        public PlayerIndex gamePlayerIndex { get; set; }
        public string playerName;


        public GamePadState currentGamePadState;
        private const float MoveStickScale = 1.0f;


        public KeyboardState currentKeyboardState;

        KeyboardState LastKeyboardStates;
        GamePadState LastGamePadStates;

        public float HMovement;
        public float VMovement;

        
      


        public GameInput()
        {
          
        }

    


        public void getInput(GameTime GT)
        {

            LastKeyboardStates = currentKeyboardState;
            LastGamePadStates = currentGamePadState;

            currentGamePadState = GamePad.GetState(gamePlayerIndex);
            currentKeyboardState = Keyboard.GetState();

            HMovement = currentGamePadState.ThumbSticks.Left.X * MoveStickScale;


            VMovement = currentGamePadState.ThumbSticks.Left.Y * -MoveStickScale;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
                VMovement = 1.0f;

            if (currentKeyboardState.IsKeyDown(Keys.Up))
                VMovement = -1.0f;

            if (currentKeyboardState.IsKeyDown(Keys.Left))
                HMovement = -1.0f;

            if (currentKeyboardState.IsKeyDown(Keys.Right))
                HMovement = 1.0f;

            //normalize movement
            if (HMovement != 0 && VMovement != 0)
            {
                Vector2 movement = new Vector2(HMovement, VMovement);

                movement.Normalize();
                HMovement = movement.X;
                VMovement = movement.Y;
            }

           

        }

        public void updateButton(ButtonMap butt, GameTime GT)
        {
            if (IsNewButtonPress(butt.B) || IsNewKeyPress(butt.K))
            {
                butt.NewPress(GT);
            }
            else if (currentGamePadState.IsButtonDown(butt.B) || currentKeyboardState.IsKeyDown(butt.K))
            {
                butt.Press(GT);
            }
            else
            {
                butt.Release(GT);
            }
        }

        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) &&
                        LastKeyboardStates.IsKeyUp(key));
        }

        public bool IsNewButtonPress(Buttons button)
        {
            return (currentGamePadState.IsButtonDown(button) &&
                    LastGamePadStates.IsButtonUp(button));

        }
    }

    //stored what a game button is mapped to, if it is currently pressed, and the gametime in ticks that it was last pressed.
    public class ButtonMap
    {
        string name;
        public Keys K;
        public Buttons B;

        public bool isPressed;
        public bool isNewPress;

        TimeSpan timePressed;

        public ButtonMap(string name, Keys k, Buttons b)
        {
            this.name = name;
            K = k;
            B = b;
        }


        public void NewPress(GameTime GT)
        {
            isNewPress = true;
            isPressed = true;
            timePressed = GT.ElapsedGameTime;
        }

        public void Press(GameTime GT)
        {
            isPressed = true;
            isNewPress = false;
            timePressed += GT.ElapsedGameTime;

        }

        public void Release(GameTime gt)
        {
            isNewPress = false;
            isPressed = false;
            timePressed = TimeSpan.Zero;
        }


    }
}
