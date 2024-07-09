using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Infrastructure.Networking
{
    public class NetworkTicker : IInitializable, IFixedTickable, ITickable
    {
        [Inject] private NetworkManager _networkManager;
        
        private int _networkTickRate;
        private NetworkTimer _networkTimer;

        public static float TickRate;
        public static event Action<int> OnNetworkTick;

        public void Initialize()
        {
            _networkTickRate = (int)_networkManager.NetworkTickSystem.TickRate;
            _networkTimer = new NetworkTimer(_networkTickRate);
            TickRate = 1f / _networkManager.NetworkTickSystem.TickRate;
        }
        
        public void Tick()
        {
            _networkTimer.Tick(Time.deltaTime);
        }

        public void FixedTick()
        {
            while (_networkTimer.ShouldTick())
            {
                NetworkTick(_networkTimer.CurrentTick);
                Debug.Log("TICKED");
            }
        }

        private void NetworkTick(int tick)
        {
            OnNetworkTick?.Invoke(tick);
        }

    }
}