using System.Reflection;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace ForsakenGraves.Infrastructure.Netcode
{
    //on instances which have authority, transform can be updated instanty.
    //server reconcialiation is done on AnticipatedPlayerController class.
    public class ClientPredictedNetworkTransform : NetworkTransform
    {
        private MethodInfo _updateInterpolationMethodInfo;

        protected override void Awake()
        {
            base.Awake();
            
            // Get the MethodInfo object for the UpdateInterpolation method
            _updateInterpolationMethodInfo = typeof(NetworkTransform).GetMethod("UpdateInterpolation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        
        public override void OnUpdate()
        {
            // If not spawned or this instance has authority, exit early
            if (!IsSpawned || CanCommitToTransform || IsOwner)
            {
                return;
            }
            
            //interpolate and apply authoritative data on non-authoritative instaces
            _updateInterpolationMethodInfo.Invoke(this, null);
            ApplyAuthoritativeState();
        }
        
        //update other clients from server data only
        protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
        {
            base.OnSynchronize(ref serializer);
            if (!CanCommitToTransform)
            {
                ApplyAuthoritativeState();
            }
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ApplyAuthoritativeState();
        }
    }
}