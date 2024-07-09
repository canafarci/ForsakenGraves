using Cysharp.Threading.Tasks;
using ForsakenGraves.Infrastructure.Networking;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraTicker //: IInitializable, ITickable
    {
        // [Inject] private NetworkManager _networkManager;
        // [Inject] private CinemachineBrain _cinemachineBrain;
        //
        // private NetworkTimer _networkTimer;
        // private int _networkTickRate;
        //
        // public void Initialize()
        // {
        //     _networkTickRate = (int)_networkManager.NetworkTickSystem.TickRate;
        //     _networkTimer = new NetworkTimer(_networkTickRate);
        // }
        //
        // public void Tick()
        // {
        //     _networkTimer.Tick(Time.deltaTime);
        //
        //     while (_networkTimer.ShouldTick())
        //     {
        //         ManualUpdateCamera();
        //     }
        // }
        //
        // private async void ManualUpdateCamera()
        // {
        //     await UniTask.Delay(2);
        //     _cinemachineBrain.ManualUpdate();
        // }
    }
}