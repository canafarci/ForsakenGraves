using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.MainMenu
{
    public class MainMenuSettings : IStartable
    {
        public void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}