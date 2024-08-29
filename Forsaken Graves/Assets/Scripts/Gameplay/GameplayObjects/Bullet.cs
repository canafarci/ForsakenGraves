using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using ForsakenGraves.Identifiers;
using Unity.Netcode;
using VContainer;

namespace ForsakenGraves.Gameplay.GameplayObjects
{
    public class Bullet : NetworkBehaviour
    {
        [Inject] private NetworkObjectPool _networkObjectPool;
        [SerializeField] private Rigidbody Rigidbody;
        [SerializeField] private TrailRenderer TrailRenderer;
        
        private const float RETURN_DELAY = 1f;
        private float _returnToPoolTimer = 0;

        private void OnEnable()
        {
            _returnToPoolTimer = RETURN_DELAY;
            ClearTrail();
        }
        
        //reset position occurs after object is enabled, wait one frame for clearing the trail
        private  void ClearTrail()
        {
            TrailRenderer.Clear();
            TrailRenderer.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _networkObjectPool.ReturnNetworkObject(PooledObjectID.Bullet, NetworkObject);
        }

        private void Update()
        {
            if (!TrailRenderer.enabled) return;
            
            transform.position += transform.forward * (Time.deltaTime * 200f);
            
            _returnToPoolTimer -= Time.deltaTime;
            if (_returnToPoolTimer <= 0)
            {
                ReturnToPool();
            }
        }

        private void OnDisable()
        {
            TrailRenderer.enabled = false;
        }
    }
}