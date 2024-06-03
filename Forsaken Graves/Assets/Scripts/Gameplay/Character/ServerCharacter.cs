using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            ulong clientID = OwnerClientId;
            name = $"Client {clientID}'s Gameplay Character";

            transform.position = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        }
    }
}