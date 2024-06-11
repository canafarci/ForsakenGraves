using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class InputPoller
    {
        private InputFlags _lastInput;

        public InputFlags GetMovementInput()
        {
            InputFlags movementInput = 0;
            
            if (Input.GetKey(KeyCode.W)) 
                movementInput |= InputFlags.Up;

            if (Input.GetKey(KeyCode.S)) 
                movementInput |= InputFlags.Down;

            if (Input.GetKey(KeyCode.A)) 
                movementInput |= InputFlags.Left;

            if (Input.GetKey(KeyCode.D)) 
                movementInput |= InputFlags.Right;

            if (Input.GetKeyDown(KeyCode.Space))
                movementInput |= InputFlags.Jump;
            
            //in order to simulate getkeydown in nonregular tick rate, 
            //check last input and remove if same input is present
            InputFlags lastInput = _lastInput;
            _lastInput = movementInput;
            
            if ((lastInput & InputFlags.Jump) != 0)
            {
                movementInput &= ~InputFlags.Jump;
            }
            
            return movementInput;
        }
        
        public bool GetShootingInput()
        {
            if (Input.GetMouseButtonDown(0))
                return true;
            
            return false;
        }

        public bool GetJumpInput()
        {
            return Input.GetKeyDown(KeyCode.Space);
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