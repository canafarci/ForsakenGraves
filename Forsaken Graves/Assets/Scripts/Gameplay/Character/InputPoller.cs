using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    public class InputPoller : NetworkBehaviour
    {
        public InputFlags GetInput()
        {
            if (!NetworkManager.IsListening )
            {
                return 0;
            }

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
    }
}