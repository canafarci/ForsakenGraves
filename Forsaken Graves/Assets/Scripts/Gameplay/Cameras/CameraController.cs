using System;
using System.Collections.Generic;
using ForsakenGraves.Identifiers;
using Sirenix.Utilities;
using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraController// : IInitializable, IDisposable
    {
        private  CameraTargetReference _cameraTargetReference;
        private readonly Dictionary<GameCameraType, GameCamera> _cameraLookup = new();

        // public void SetCameraTargetReference(CameraTargetReference cameraTargetReference)
        // {
        //     _cameraTargetReference = cameraTargetReference;
        // }
        //
        // public void Initialize()
        // {
        //     GameCamera[] cameras = MonoBehaviour.FindObjectsByType<GameCamera>(FindObjectsSortMode.None);
        //     cameras.ForEach(x => _cameraLookup[x.CameraType] = x);
        // }
        //
        // public void SetGameplayCamera()
        // {
        //     GameCamera gameplayCamera = _cameraLookup[GameCameraType.GamePlay];
        //     
        //     gameplayCamera.CinemachineCamera.Follow = _cameraTargetReference.CameraPosTarget;
        //     gameplayCamera.CinemachineCamera.LookAt = _cameraTargetReference.CameraLookAtTarget;
        // }
        //
        // public void Dispose()
        // {
        //     _cameraLookup.Clear();
        // }
    }
}