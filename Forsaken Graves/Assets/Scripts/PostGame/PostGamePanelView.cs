using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForsakenGraves.PostGame
{
    public class PostGamePanelView : MonoBehaviour
    {
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _mainMenuButton;

        public Button ReplayButton => _replayButton;
        public Button MainMenuButton => _mainMenuButton;
    }
}