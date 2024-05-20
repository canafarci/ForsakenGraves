using UnityEngine;
using VContainer;

namespace ForsakenGraves.Connection
{
    public class ConnectionManager : MonoBehaviour
    {
        //[Inject] NetworkManager _networkManager;
        [SerializeField] private int _maxConnectedPlayers;
        public int MaxConnectedPlayers => _maxConnectedPlayers;
    }
}