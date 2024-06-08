using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class InputPoller
    {
        private FrameHistory<InputFlags> _inputHistory = new();
        public FrameHistory<InputFlags> InputHistory => _inputHistory;

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
            
            _inputHistory.Add(NetworkManager.Singleton.LocalTime.Time, input);
            return input;
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