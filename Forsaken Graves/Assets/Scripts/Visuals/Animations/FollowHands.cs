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
        private Vector3 _horizontalVelocity = Vector3.zero;


        public void Initialize(Transform transformToFollow, PlayerConfig playerConfig)
        {
            _transformToFollow = transformToFollow;
            _playerConfig = playerConfig;
        }

        private void Awake()
        {
            NetworkTicker.OnNetworkTick += NetworkTick;
        }

        private void NetworkTick(int currentTick)
        {
            if (!_transformToFollow) return;
            

        }

        private void FixedUpdate()
        {
            if (!_transformToFollow) return;

            transform.position = _transformToFollow.position;
            transform.rotation = _transformToFollow.rotation;
            
            // transform.position = Vector3.Lerp(transform.position,
            //                                   _transformToFollow.position,
            //                                   Time.fixedDeltaTime * _playerConfig.HandsLerpSpeed);
            //
            // transform.rotation = Quaternion.Slerp(transform.rotation,
            //                                       _transformToFollow.rotation,
            //                                       Time.fixedDeltaTime * _playerConfig.HandsSlerpSpeed);
        }

        private void OnDestroy()
        {
            NetworkTicker.OnNetworkTick -= NetworkTick;
        }
    }
}