using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Controls
{
    class InputHandler : Microsoft.Xna.Framework.GameComponent
    {
        #region Mouse Field Region

        static MouseState mouseState;
        static MouseState lastMouseState;

        #endregion

        #region Keyboard Field Region

        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;

        #endregion

        #region Game Pad Field Region

        static GamePadState[] gamePadStates;
        static GamePadState[] lastGamePadStates;

        #endregion

        #region Mouse Property Region

        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }

        #endregion

        #region Keyboard Property Region

        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        #endregion

        #region Game Pad Property Region

        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }

        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }

        #endregion

        #region Constructor Region

        public InputHandler(Game game)
            : base(game)
        {
            mouseState = Mouse.GetState();

            keyboardState = Keyboard.GetState();

            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);
        }

        #endregion

        #region XNA methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastGamePadStates = (GamePadState[])gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);

            base.Update(gameTime);
        }

        #endregion

        #region General Method Region

        public static void Flush()
        {
            lastMouseState = mouseState;
            lastKeyboardState = keyboardState;
        }

        #endregion

        #region Mouse Region

        public static bool Scroll()
        {
            return mouseState.ScrollWheelValue == lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollUp()
        {
            return mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollDown()
        {
            return mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
        }

        #endregion

        #region Keyboard Region

        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) &&
                lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool HavePressedKey()
        {
            return keyboardState != lastKeyboardState;
        }

        public static Keys[] GetPressedKeys()
        {
            return keyboardState.GetPressedKeys();
        }

        #endregion

        #region Game Pad Region

        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) &&
                lastGamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) &&
                lastGamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool HavePressedButton(PlayerIndex index)
        {
            return gamePadStates[(int)index] != lastGamePadStates[(int)index];
        }

        public static Buttons[] GetPressedButton(PlayerIndex index)
        {
            return Enum.GetValues(typeof(Buttons)).Cast<Buttons>().Where(button => ButtonPressed(button, index)).ToArray();
        }

        #endregion
    }
}
