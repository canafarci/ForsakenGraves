using UnityEngine;
using UnityEngine.Serialization;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraTargetReference : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _cameraLookAtTarget;
        [SerializeField] private Transform _handsFollowTransform;

        public Transform CameraTransform => _cameraTransform;
        public Transform CameraLookAtTarget => _cameraLookAtTarget;
        public Transform HandsFollowTransform => _handsFollowTransform;
    }
}