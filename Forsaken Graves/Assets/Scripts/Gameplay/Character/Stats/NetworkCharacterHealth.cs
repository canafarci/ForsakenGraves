using System;
using ForsakenGraves.Gameplay.Data;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Character.Stats
{
    public class NetworkCharacterHealth : NetworkBehaviour, ITargetable, IConfigurable
    {
        [SerializeField] private CharacterConfig _characterConfig;
        private NetworkVariable<float> _characterHealth;

        public NetworkVariable<float> Health => _characterHealth;
        
        public event Action<float> OnCharacterDamaged;

        public override void OnNetworkSpawn()
        {
            _characterHealth.OnValueChanged += HealthChangedHandler;
        }
        
        //TODO listen on client animator
        private void HealthChangedHandler(float previousValue, float newValue)
        {
            _characterHealth.Value = newValue;

            if (_characterHealth.Value < 0f)
            {
                Debug.Log("CHARACTER DIED");
                //TODO play animation
            }
        }
        
        public void Damage(float damage)
        {
            OnCharacterDamaged?.Invoke(damage);
        }
        
        //called before spawn
        public void Configure()
        {
            _characterHealth = new NetworkVariable<float>(_characterConfig.Health);
        }

        public override void OnNetworkDespawn()
        {
            _characterHealth.OnValueChanged -= HealthChangedHandler;
        }
    }
}