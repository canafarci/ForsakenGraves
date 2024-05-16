using System;
using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.Test
{
    public class TestSubscriber : IDisposable, IInitializable
    {
        private readonly TestService _service;

        public TestSubscriber(TestService service)
        {
            _service = service;
        }
        
        public void Initialize()
        {

        }
        
        public void Dispose()
        {

        }
    }
}