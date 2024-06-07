using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForsakenGraves.Gameplay.UI
{
    public class LobbyCreationView : MonoBehaviour
    {
        [SerializeField] private InputField _lobbyNameInputField;
        [SerializeField] private Button _createLobbyButton;
        [SerializeField] Toggle _isPrivateToggle;
        
        public event EventHandler<CreateLobbyClickedEventArgs> OnCreateLobbyClicked;

        private void Start()
        {
            _createLobbyButton.onClick.AddListener(FireOnCreateLobbyClickedEvent);
        }

        private void FireOnCreateLobbyClickedEvent()
        {
            OnCreateLobbyClicked?.Invoke(this, new CreateLobbyClickedEventArgs()
                                               {
                                                   LobbyName = _lobbyNameInputField.text,
                                                   IsPrivate =  _isPrivateToggle.isOn
                                               });
        }

        private void OnDestroy()
        {
            _createLobbyButton.onClick.RemoveAllListeners();
        }
    }

    public class CreateLobbyClickedEventArgs
    {
        public string LobbyName;
        public bool IsPrivate;
    }
}