using ForsakenGraves.Identifiers;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class InputPoller : NetworkBehaviour
    {
        public InputFlags GetMovementInput()
        {
            if (!NetworkManager.IsListening) return 0;

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
            if (!NetworkManager.IsListening) return 0;
            
            return Input.GetAxis("Mouse X");
        }
        
        public float GetRotationYInput()
        {
            if (!NetworkManager.IsListening) return 0;
            
            return Input.GetAxis("Mouse Y");
        }
    }
}