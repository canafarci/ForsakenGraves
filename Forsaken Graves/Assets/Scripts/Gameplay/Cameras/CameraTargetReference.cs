using UnityEngine;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraTargetReference : MonoBehaviour
    {
        [SerializeField] private Transform _cameraPosTarget;
        [SerializeField] private Transform _cameraLookAtTarget;

        public Transform CameraPosTarget => _cameraPosTarget;
        public Transform CameraLookAtTarget => _cameraLookAtTarget;
    }
}