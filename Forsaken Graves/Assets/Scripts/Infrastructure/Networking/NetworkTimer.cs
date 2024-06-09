using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Infrastructure.Networking
{
    public class NetworkTimer
    {
        private readonly float _minTimeBetweenTicks;
        
        private int _currentTick = 0;
        private float _timer;

        public int CurrentTick => _currentTick;
        
        public NetworkTimer(int tickRate)
        {
            _minTimeBetweenTicks = 1f / tickRate;
        }
        
        public void Tick(float deltaTime)
        {
            _timer += deltaTime;
        }

        public bool ShouldTick()
        {
            if (_timer < _minTimeBetweenTicks) return false;
            
            _timer -= _minTimeBetweenTicks;
            _currentTick = CurrentTick + 1;
            return true;
        }
    }
}