using ForsakenGraves.Gameplay.Character.Player;
using ForsakenGraves.Gameplay.Data;
using ForsakenGraves.Infrastructure.Networking;
using UnityEngine;

namespace ForsakenGraves.Visuals.Animations
{
    public class FollowHands : MonoBehaviour
    {
        private Transform _transformToFollow;
        private PlayerConfig _playerConfig;
        private AnticipatedPlayerController _anticipatedPlayerController;
        
        private Vector3 _horizontalVelocity = Vector3.zero;
        private Vector3 _handsPosition;

        public void Initialize(Transform transformToFollow, 
                               PlayerConfig playerConfig,
                               AnticipatedPlayerController anticipatedPlayerController)
        {
            _transformToFollow = transformToFollow;
            _playerConfig = playerConfig;
            _anticipatedPlayerController = anticipatedPlayerController;

            _anticipatedPlayerController.OnTransformUpdated += UpdatePosition;
        }

        private void UpdatePosition()
        {
            if (!_transformToFollow) return;
            
            transform.SetPositionAndRotation(_transformToFollow.position,  _transformToFollow.rotation);
        }

        private void OnDestroy()
        {
            _anticipatedPlayerController.OnTransformUpdated -= UpdatePosition;
        }
    }
}