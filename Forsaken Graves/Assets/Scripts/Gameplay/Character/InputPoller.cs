using System.Collections.Generic;
using ForsakenGraves.Identifiers;
using ForsakenGraves.Infrastructure.Data;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    public class InputPoller : NetworkBehaviour
    {
        private FrameHistory<InputFlags>_historicalInput = new();

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
            
            _historicalInput.Add(NetworkManager.LocalTime.Time, input);
            return input;
        }

        public void RemoveBefore(double time)
        {
            _historicalInput.RemoveBefore(time);
        }

        public List<FrameHistory<InputFlags>.ItemFrameData> GetHistory()
        {
            return _historicalInput.GetHistory();
        }
        
        public void Clear()
        {
            _historicalInput.Clear();
        }
    }
}