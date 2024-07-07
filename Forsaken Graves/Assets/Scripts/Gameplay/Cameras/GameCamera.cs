using ForsakenGraves.Identifiers;
using Unity.Cinemachine;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private GameCameraType _cameraType;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        public GameCameraType CameraType => _cameraType;
        public CinemachineCamera CinemachineCamera => _cinemachineCamera;
    }
}