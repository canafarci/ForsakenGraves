using UnityEngine;
using UnityEngine.UI;

namespace ForsakenGraves.PreGame.UI.AvatarSelect
{
    public class AvatarSelectView : MonoBehaviour
    {
        [SerializeField] private Button _changeAvatarButton;
        public Button ChangeAvatarButton => _changeAvatarButton;
    }
}