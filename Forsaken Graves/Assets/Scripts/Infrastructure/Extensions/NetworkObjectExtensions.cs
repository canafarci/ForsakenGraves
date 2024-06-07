using ForsakenGraves.Gameplay.Character.Stats;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Infrastructure.Extensions
{
    public static class NetworkObjectExtensions
    {
        public static void Configure(this NetworkObject networkObject)
        {
            Transform transform = networkObject.transform;

            foreach (IConfigurable configurable in transform.GetComponentsInChildren<IConfigurable>())
            {
                configurable.Configure();
            }
        }
    }
}