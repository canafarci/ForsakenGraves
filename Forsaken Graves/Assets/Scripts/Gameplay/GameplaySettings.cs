using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay
{
    public class GameplaySettings : IStartable
    {
        public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}