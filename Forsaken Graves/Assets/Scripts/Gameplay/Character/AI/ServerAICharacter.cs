using NodeCanvas.BehaviourTrees;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.AI
{
    public class ServerAICharacter : NetworkBehaviour
    {
        [SerializeField] private BehaviourTreeOwner _behaviourTreeOwner;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                _behaviourTreeOwner.enabled = false;
                enabled = false;
            }
        }
    }
}