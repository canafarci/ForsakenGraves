using Unity.Collections;
using Unity.Netcode;

namespace ForsakenGraves.Gameplay.Data
{
    public class NetworkPlayerVisualData : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> DisplayName = new NetworkVariable<FixedString32Bytes>();
        public NetworkVariable<int> AvatarIndex = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            DisplayName.OnValueChanged += OnDisplayNameChanged;
            AvatarIndex.OnValueChanged += OnAvatarIndexChanged;
        }

        private void OnAvatarIndexChanged(int previousValue, int newValue)
        {
            if (AvatarIndex.Value != newValue)
                AvatarIndex.Value = newValue;
        }

        private void OnDisplayNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            if (DisplayName.Value != newValue)
                DisplayName.Value = newValue;
        }

        public override void OnNetworkDespawn()
        {
            DisplayName.OnValueChanged -= OnDisplayNameChanged;
            AvatarIndex.OnValueChanged -= OnAvatarIndexChanged;
        }
    }
}