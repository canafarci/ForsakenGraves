using ForsakenGraves.Gameplay.Data;
using UnityEngine;

namespace ForsakenGraves.Visuals.Animations
{
    public class FollowHands : MonoBehaviour
    {
        private Transform _transformToFollow;
        private PlayerConfig _playerConfig;

        public void Initialize(Transform transformToFollow, PlayerConfig playerConfig)
        {
            _transformToFollow = transformToFollow;
            _playerConfig = playerConfig;
        }

        private void FixedUpdate()
        {
            if (!_transformToFollow) return;
            
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  _transformToFollow.rotation,
                                                  Time.fixedDeltaTime * _playerConfig.HandsSlerpSpeed);
            
            transform.transform.position = Vector3.Lerp(transform.position,
                                                        _transformToFollow.position,
                                                        Time.fixedDeltaTime * _playerConfig.HandsLerpSpeed);
        }
    }
}