using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.AI
{
    public class ServerAICharacter : ServerCharacter
    {
        [Inject] private Blackboard _blackboard;
        [Inject] private BehaviourTreeOwner _behaviourTreeOwner;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
            {
                _behaviourTreeOwner.enabled = false;
                _blackboard.enabled = false;
                enabled = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            _behaviourTreeOwner.StopBehaviour();
            _behaviourTreeOwner.enabled = false;
            _blackboard.enabled = false;
            
            base.OnNetworkDespawn();
        }
    }
}