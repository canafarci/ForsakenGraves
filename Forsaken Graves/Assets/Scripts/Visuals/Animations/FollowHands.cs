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
        private Vector3 _handsPosition;
        
        public void Initialize(Transform transformToFollow, PlayerConfig playerConfig)
        {
            _transformToFollow = transformToFollow;
            _playerConfig = playerConfig;
        }

        private void Awake()
        {
            NetworkTicker.OnNetworkTick += NetworkTick;
            _handsPosition = transform.position;
        }

        private void NetworkTick(int currentTick)
        {
            if (!_transformToFollow) return;
            
            _handsPosition = Vector3.SmoothDamp(_handsPosition,
                                                _transformToFollow.position,
                                                ref _horizontalVelocity,
                                                NetworkTicker.TickRate * _playerConfig.HandsLerpSpeed);
            
           // transform.rotation = _transformToFollow.rotation;
            
            // transform.rotation = Quaternion.Slerp(transform.rotation,
            //                                       _transformToFollow.rotation,
            //                                       NetworkTicker.TickRate * _playerConfig.HandsSlerpSpeed);
            
        }

        private void Update()
        {
            if (!_transformToFollow) return;
            
            // Vector3 transformPosition = Vector3.SmoothDamp(transform.position,
            //                             _transformToFollow.position,
            //                             ref _horizontalVelocity,
            //                             Time.smoothDeltaTime * _playerConfig.HandsLerpSpeed);
            
            //transform.position = _transformToFollow.position;
            //transform.rotation = _transformToFollow.rotation;
            
            // transform.position = Vector3.Lerp(transform.position,
            //                                   _transformToFollow.position,
            //                                   Time.fixedDeltaTime * _playerConfig.HandsLerpSpeed);

            Quaternion transformRotation = Quaternion.Slerp(transform.rotation,
                                                            _transformToFollow.rotation,
                                                            Time.smoothDeltaTime * _playerConfig.HandsSlerpSpeed);
            
            transform.SetPositionAndRotation(_handsPosition, transformRotation);
        }

        private void OnDestroy()
        {
            NetworkTicker.OnNetworkTick -= NetworkTick;
        }
    }
}