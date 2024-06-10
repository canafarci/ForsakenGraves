using System;
using ForsakenGraves.Gameplay.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace ForsakenGraves.Gameplay.Character.Stats
{
    public class NetworkCharacterHealth : NetworkBehaviour, ITargetable, IConfigurable
    {
        [Inject] private CharacterConfig _characterConfig;
        
        public NetworkVariable<float> CharacterHealth;
        public event Action<float> OnCharacterDamaged;
        
        //called before spawn
        public void Configure()
        {
            CharacterHealth = new NetworkVariable<float>(_characterConfig.Health);
        }

        public override void OnNetworkSpawn()
        {
            CharacterHealth.OnValueChanged += HealthChangedHandler;
        }
        
        //TODO listen on client animator
        private void HealthChangedHandler(float previousValue, float newValue)
        {
            CharacterHealth.Value = newValue;
            Debug.Log($"HEALTH CHANGED TO: {newValue}");

            if (CharacterHealth.Value < 0f)
            {
                Debug.Log("CHARACTER DIED");
                //TODO play animation
            }
        }
        
        public void Damage(float damage)
        {
            Debug.Log($"GO {gameObject.name} received {damage} damage on client {OwnerClientId}");
            OnCharacterDamaged?.Invoke(damage);
        }
        
        public override void OnNetworkDespawn()
        {
            CharacterHealth.OnValueChanged -= HealthChangedHandler;
        }
    }
}