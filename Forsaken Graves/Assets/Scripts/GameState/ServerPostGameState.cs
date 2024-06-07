using ForsakenGraves.Connection.Data;
using ForsakenGraves.Connection.Utilities;
using ForsakenGraves.Gameplay.State;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.GameState
{
    public class ServerPostGameState : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _postGameResultText;
        public TextMeshProUGUI PostGameResultText => _postGameResultText;

        [Inject] private PersistentGameplayState _persistentGameplayState;
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
            }
            else
            {
                SessionManager<SessionPlayerData>.Instance.OnSessionEnded();
                
                SetPostGameTextClientRpc(_persistentGameplayState.IsGameWon);
            }
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void SetPostGameTextClientRpc(bool hasWonGame)
        {
            if (hasWonGame)
            {
                _postGameResultText.text = "GAME WON!";
            }
            else
            {
                _postGameResultText.text = "GAME LOST!";
            }
        }
    }
}