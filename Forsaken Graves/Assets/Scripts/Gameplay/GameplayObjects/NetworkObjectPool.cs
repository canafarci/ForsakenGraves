using System;
using System.Collections.Generic;
using System.Linq;
using ForsakenGraves.Identifiers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace ForsakenGraves.Gameplay.GameplayObjects
{
    public class NetworkObjectPool : NetworkBehaviour
    {
        [SerializeField] private List<PoolConfigObject> PooledPrefabsList;

        private readonly HashSet<PooledObjectID> _prefabIDs = new HashSet<PooledObjectID>();
        private readonly Dictionary<PooledObjectID, ObjectPool<NetworkObject>> _pooledObjects = new Dictionary<PooledObjectID, ObjectPool<NetworkObject>>();
        
        public override void OnNetworkSpawn()
        {
            // Registers all objects in PooledPrefabsList to the cache.
            foreach (PoolConfigObject configObject in PooledPrefabsList)
            {
                RegisterPrefabInternal(configObject);
            }
        }

        public override void OnNetworkDespawn()
        {
            // Unregisters all objects in PooledPrefabsList from the cache.
            foreach (PooledObjectID prefabID in _prefabIDs)
            {
                // Unregister Netcode Spawn handlers
                GameObject prefab = PooledPrefabsList.FirstOrDefault(x => x.PooledObjectID == prefabID).Prefab;
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                
                _pooledObjects[prefabID].Clear();
            }
            _pooledObjects.Clear();
            _prefabIDs.Clear();
        }

        public void OnValidate()
        {
            for (int i = 0; i < PooledPrefabsList.Count; i++)
            {
                GameObject prefab = PooledPrefabsList[i].Prefab;
                if (prefab != null)
                {
                    Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
                }
            }
        }

        // Gets an instance of the given prefab from the pool. The prefab must be registered to the pool.
        // To spawn a NetworkObject from one of the pools, this must be called on the server, then the instance
        // returned from it must be spawned on the server. This method will then also be called on the client by the
        // PooledPrefabInstanceHandler when the client receives a spawn message for a prefab that has been registered
        // here.
        public NetworkObject GetNetworkObject(PooledObjectID pooledObjectID, Vector3 position, Quaternion rotation)
        {
            NetworkObject networkObject = _pooledObjects[pooledObjectID].Get();

            Transform networkObjectTransform = networkObject.transform;
            networkObjectTransform.position = position;
            networkObjectTransform.rotation = rotation;

            return networkObject;
        }

        // Return an object to the pool (reset objects before returning).
        public void ReturnNetworkObject(PooledObjectID pooledObjectID,  NetworkObject networkObject)
        {
            _pooledObjects[pooledObjectID].Release(networkObject);
        }

        // <summary>
        // Builds up the cache for a prefab.
        // </summary>
        private void RegisterPrefabInternal(PoolConfigObject poolConfigObject)
        {
            NetworkObject CreateFunc()
            {
                return Instantiate(poolConfigObject.Prefab).GetComponent<NetworkObject>();
            }

            void ActionOnGet(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(true);
            }

            void ActionOnRelease(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(false);
            }

            void ActionOnDestroy(NetworkObject networkObject)
            {
                Destroy(networkObject.gameObject);
            }

            _prefabIDs.Add(poolConfigObject.PooledObjectID);

            // Create the pool
            _pooledObjects[poolConfigObject.PooledObjectID] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: poolConfigObject.PrewarmCount);

            // Populate the pool
            List<NetworkObject> prewarmNetworkObjects = new List<NetworkObject>();
            for (int i = 0; i < poolConfigObject.PrewarmCount; i++)
            {
                prewarmNetworkObjects.Add(_pooledObjects[poolConfigObject.PooledObjectID].Get());
            }
            foreach (NetworkObject networkObject in prewarmNetworkObjects)
            {
                _pooledObjects[poolConfigObject.PooledObjectID].Release(networkObject);
            }

            // Register Netcode Spawn handlers
            NetworkManager.Singleton.PrefabHandler.AddHandler(poolConfigObject.Prefab, new PooledPrefabInstanceHandler(poolConfigObject.Prefab, this, poolConfigObject.PooledObjectID));
        }
    }

    [Serializable]
    internal struct PoolConfigObject
    {
        public PooledObjectID PooledObjectID;
        public GameObject Prefab;
        public int PrewarmCount;
    }

    internal class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly GameObject _prefab;
        private readonly NetworkObjectPool _pool;
        private readonly PooledObjectID _pooledObjectID;

        public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool, PooledObjectID pooledObjectID)
        {
            _prefab = prefab;
            _pool = pool;
            _pooledObjectID = pooledObjectID;
        }

        NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return _pool.GetNetworkObject(_pooledObjectID, position, rotation);
        }

        void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject)
        {
            _pool.ReturnNetworkObject(_pooledObjectID, networkObject);
        }
    }

}