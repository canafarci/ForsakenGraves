using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class CrossSceneService : IStartable
    {
        [Inject] private IPublisher<int> _publisher;
        public void Print(string message)
        {
            Debug.Log(message);
        }

        public void Start()
        {
            _publisher.Publish(44);
        }
    }
}