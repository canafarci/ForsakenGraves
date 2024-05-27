using UnityEngine;
using UnityEngine.UI;

namespace ForsakenGraves.Gameplay.UI
{
    public class LobbyJoiningView : MonoBehaviour
    {
        [SerializeField] private Button _quickJoinButton;

        public Button QuickJoinButton => _quickJoinButton;
    }
}