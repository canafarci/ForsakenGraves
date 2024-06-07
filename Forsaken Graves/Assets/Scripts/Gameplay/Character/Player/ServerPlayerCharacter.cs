using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ServerPlayerCharacter : ServerCharacter
    {
        [SerializeField] private GameObject _camera;

        public override void OnNetworkSpawn()
        {
            ulong clientID = OwnerClientId;
            name = $"Client {clientID}'s Gameplay Character";

            transform.position = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            
            if (!IsOwner) //Destroy camera if not the owner
                Destroy(_camera);
        }
    }
}