using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class InputPoller
    {
        public Vector3 GetMovementInput()
        {
            Vector3 movementInput = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                movementInput += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movementInput -= Vector3.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movementInput -= Vector3.right;
            }

            if (Input.GetKey(KeyCode.D))
            {
                movementInput += Vector3.right;
            }
            
            return movementInput;
        }
        
        public bool GetShootingInput()
        {
            if (Input.GetMouseButtonDown(0))
                return true;
            
            return false;
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