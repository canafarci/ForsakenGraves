using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForsakenGraves.PreGame.UI.ReadyPanel
{
    public class ReadyPanelView : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _text;
        public Button ReadyButton => _readyButton;
        public TextMeshProUGUI Text => _text;
    }
}