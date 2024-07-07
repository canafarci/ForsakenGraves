using System;
using System.Collections.Generic;
using ForsakenGraves.Identifiers;
using NodeCanvas.Tasks.Actions;
using Sirenix.Utilities;
using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraController : IInitializable, ITickable, IDisposable
    {
        private  CameraTargetReference _cameraTargetReference;
        private readonly Dictionary<GameCameraType, GameCamera> _cameraLookup = new();
        private HandsSpawnTransform _handsSpawnTransform;

        public void SetCameraTargetReference(CameraTargetReference cameraTargetReference)
        {
            _cameraTargetReference = cameraTargetReference;
        }
        
        public void SetHandsTransform(HandsSpawnTransform handsSpawnTransform)
        {
            _handsSpawnTransform = handsSpawnTransform;
        }
        
        
        public void Initialize()
        {
            GameCamera[] cameras = MonoBehaviour.FindObjectsByType<GameCamera>(FindObjectsSortMode.None);
            cameras.ForEach(x => _cameraLookup[x.CameraType] = x);
        }
        
        public void Tick()
        {
            throw new NotImplementedException();
        }
        
        public void SetGameplayCamera()
        {
            GameCamera gameplayCamera = _cameraLookup[GameCameraType.GamePlay];
            
            gameplayCamera.CinemachineCamera.Follow = _cameraTargetReference.CameraPosTarget;
            gameplayCamera.CinemachineCamera.LookAt = _cameraTargetReference.CameraLookAtTarget;
        }

        public void Dispose()
        {
            _cameraLookup.Clear();
        }
    }
}