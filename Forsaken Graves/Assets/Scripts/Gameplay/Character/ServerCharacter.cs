using Unity.Netcode;

namespace ForsakenGraves.Gameplay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            ulong clientID = OwnerClientId;
            name = $"Client {clientID}'s Gameplay Character";
        }
    }
}