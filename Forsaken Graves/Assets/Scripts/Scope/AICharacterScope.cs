using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Gameplay.Data;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class AICharacterScope : LifetimeScope
    {
        //components
        [SerializeField ] private NetworkCharacterHealth _characterHealth;
        [SerializeField ] private Blackboard _blackboard;
        [SerializeField ] private BehaviourTreeOwner _behaviourTreeOwner;
        
        //data
        [SerializeField ] private CharacterConfig _characterConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_characterHealth);
            builder.RegisterInstance(_characterConfig);
            builder.RegisterInstance(_blackboard);
            builder.RegisterInstance(_behaviourTreeOwner);
        }
    }
}