using NodeCanvas.BehaviourTrees;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.AI
{
    public class ServerAICharacter : ServerCharacter
    {
        [SerializeField] private BehaviourTreeOwner _behaviourTreeOwner;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
            {
                _behaviourTreeOwner.enabled = false;
                enabled = false;
            }
        }
    }
}