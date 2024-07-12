using System;
using ForsakenGraves.Infrastructure.Networking;
using Unity.Cinemachine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Cameras
{
    public class CameraTicker : IInitializable, IDisposable
    {
        // [Inject] private CinemachineBrain _cinemachineBrain;
        //
        // private int _networkTickRate;
        //
        public void Initialize()
        {
        //     NetworkTicker.OnNetworkTick += NetworkTick;
        }
        //
        // private void NetworkTick(int currentTick)
        // {
        //     ManualUpdateCamera();
        // }
        //
        // private void ManualUpdateCamera()
        // {
        //     _cinemachineBrain.ManualUpdate();
        // }
        //
        public void Dispose()
        {
        //     NetworkTicker.OnNetworkTick -= NetworkTick;
        }
    }
}