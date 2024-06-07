using ForsakenGraves.Identifiers;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class InputPoller
    {
        public bool GetShootingInput()
        {
            if (Input.GetMouseButtonDown(0))
                return true;
            
            return false;
        }
        
        public InputFlags GetMovementInput()
        {
            InputFlags input = 0;
            if (Input.GetKey(KeyCode.W))
            {
                input |= InputFlags.Up;
            }

            if (Input.GetKey(KeyCode.A))
            {
                input |= InputFlags.Left;
            }

            if (Input.GetKey(KeyCode.S))
            {
                input |= InputFlags.Down;
            }

            if (Input.GetKey(KeyCode.D))
            {
                input |= InputFlags.Right;
            }
            
            return input;
        }

        public float GetRotationXInput()
        {
            return Input.GetAxis("Mouse X");
        }
        
        public float GetRotationYInput()
        {
            return Input.GetAxis("Mouse Y");
        }
    }
}