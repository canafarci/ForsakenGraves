using ForsakenGraves.Connection.ConnectionStates;
using UnityEngine;
using VContainer;

namespace ForsakenGraves.Connection
{
    public class ConnectionStateManager : MonoBehaviour
    {
        ConnectionState _currentState;

        
        //[Inject] NetworkManager _networkManager;
        [SerializeField] private int _maxConnectedPlayers;
        public int MaxConnectedPlayers => _maxConnectedPlayers;

        public void StartHostLobby(string displayName)
        {
            throw new System.NotImplementedException();
        }
    }
}