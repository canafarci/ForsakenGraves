using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.Application
{
    public class ApplicationSettings : IStartable
    {
        public void Start()
        {
            UnityEngine.Application.targetFrameRate = 60;
        }
    }
}