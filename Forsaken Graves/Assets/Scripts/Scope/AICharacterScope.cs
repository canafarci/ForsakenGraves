using ForsakenGraves.Gameplay.Character.Stats;
using ForsakenGraves.Gameplay.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Scope
{
    public class AICharacterScope : LifetimeScope
    {
        [SerializeField ] private NetworkCharacterHealth _characterHealth;
        
        //data
        [SerializeField ] private CharacterConfig _characterConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_characterHealth);
            builder.RegisterInstance(_characterConfig);
        }
    }
}