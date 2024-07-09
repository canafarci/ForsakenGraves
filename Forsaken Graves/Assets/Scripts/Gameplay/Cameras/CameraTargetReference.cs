using UnityEngine;
using UnityEngine.Serialization;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraTargetReference : MonoBehaviour
    {
        [SerializeField] private Transform _cameraPosTarget;
        [SerializeField] private Transform _cameraLookAtTarget;
        [SerializeField] private Transform _handsFollowTransform;

        public Transform CameraPosTarget => _cameraPosTarget;
        public Transform CameraLookAtTarget => _cameraLookAtTarget;
        public Transform HandsFollowTransform => _handsFollowTransform;
    }
}