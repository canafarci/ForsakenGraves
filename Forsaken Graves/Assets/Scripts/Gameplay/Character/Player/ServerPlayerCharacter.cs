using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Player
{
    public class ServerPlayerCharacter : ServerCharacter
    {
        [SerializeField] private GameObject _camera;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            name = $"Client {OwnerClientId}'s Gameplay Character";
            
            if (!IsOwner) //Destroy camera if not the owner
                Destroy(_camera);
        }
    }
}