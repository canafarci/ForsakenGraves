using System;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Infrastructure.Networking;
using UnityEngine;

namespace ForsakenGraves.Visuals.Animations
{
    public class FollowHands : MonoBehaviour
    {
        private Transform _transformToFollow;
        private PlayerConfig _playerConfig;
        private Vector3 velocity = Vector3.zero;


        public void Initialize(Transform transformToFollow, PlayerConfig playerConfig)
        {
            _transformToFollow = transformToFollow;
            _playerConfig = playerConfig;
        }

        private void Awake()
        {
            NetworkTicker.OnNetworkTick += NetworkTick;
        }

        private async void NetworkTick(int currentTick)
        {
            if (!_transformToFollow) return;

            //await UniTask.Delay(10);
            
            // transform.rotation = Quaternion.Slerp(transform.rotation,
            //                                      _transformToFollow.rotation,
            //                                      NetworkTicker.TickRate * _playerConfig.HandsSlerpSpeed);
            
            transform.position = Vector3.SmoothDamp(transform.position,
                                                  _transformToFollow.position,
                                                  ref velocity,
                                                  NetworkTicker.TickRate * _playerConfig.HandsLerpSpeed);
        }

        private void OnDestroy()
        {
            NetworkTicker.OnNetworkTick -= NetworkTick;
        }
    }
}