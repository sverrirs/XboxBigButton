using System;

namespace XboxBigButton
{
    [Flags, Serializable]
    public enum Buttons
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        Start = 16,
        Back = 32,
        // After this line the values do not match up with the bytes passed by the USB device anymore
        BigButton = 64,
        Home = 128,
        A = 256,
        B = 512,
        X = 1024,
        Y = 2048   
    }

    /*public static class ButtonsExtensions
    {
        /// <summary>
        /// Checks if the supplied check buttons are currently pressed (note that other buttons can be pressed)
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsPressed(this Buttons currentState, Buttons check)
        {
            return (currentState & check) == check;
        }

        /// <summary>
        /// Checks if the supplied check buttons are the exclusively pressed buttons
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsPressedOnly(this Buttons currentState, Buttons check)
        {
            return (currentState ^ check) == check;
        }

        /// <summary>
        /// Checks if the supplied check buttons are not pressed. All buttons will have to be unpressed for this to return true
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsReleased(this Buttons currentState, Buttons check)
        {
            return (currentState & check) != check;
        }
    }*/
}