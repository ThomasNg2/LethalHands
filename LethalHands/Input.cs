using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace LethalHands
{
    internal class Input
    {

        public class SquareUpInput : LcInputActions
        {
            public static SquareUpInput Instance = new SquareUpInput();

            [InputAction(kbmPath: "<Keyboard>/j", Name = "Square up")]
            public InputAction SquareUpKey { get; set; }
        }
    }
}
