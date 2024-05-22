using System;
using System.Collections.Generic;
using ForsakenGraves.Infrastructure.Data;
using UnityEngine;
using VContainer.Unity;

namespace ForsakenGraves.Infrastructure
{
    public class UpdateRunner : ITickable, IDisposable
    {
        private readonly Queue<Action> _pendingHandlers = new();
        private readonly HashSet<Action<float>> _subscribers = new();
        private readonly Dictionary<Action<float>, UpdateSubscriberData> _subscriberData = new();

        
        public void Subscribe(Action<float> onUpdate, float updatePeriod)
        {
            //check for unsubbable funcs
            if (onUpdate == null) return;
            if (onUpdate.Target == null) return; //dont sub local funcs
            if (onUpdate.Method.ToString().Contains("<")) return; //dont sub to lambda funcs
            
            //check for duplicate funcs
            if (_subscribers.Contains(onUpdate)) return;
            
            //sub
            _pendingHandlers.Enqueue(() =>
                 {
                     if (_subscribers.Add(onUpdate))
                     {
                         _subscriberData.Add(onUpdate,
                                             new UpdateSubscriberData()
                                             {
                                                 Period = updatePeriod,
                                                 NextCallTime = 0,
                                                 LastCallTime = Time.time
                                             });
                     }
                 }
             );
        }
        public void Unsubscribe(Action<float> onUpdate)
        {
            _pendingHandlers.Enqueue(() =>
                 {
                     _subscribers.Remove(onUpdate);
                     _subscriberData.Remove(onUpdate);
                 });
        }

        public void Tick()
        {
            while (_pendingHandlers.Count > 0)
            {
                _pendingHandlers.Dequeue()?.Invoke();
            }

            CheckAndInvokeSubscriberMethods();
        }

        private void CheckAndInvokeSubscriberMethods()
        {
            foreach (var subscriber in _subscribers)
            {
                UpdateSubscriberData subscriberData = _subscriberData[subscriber];

                if (Time.time >= subscriberData.NextCallTime)
                {
                    subscriber.Invoke(Time.time - subscriberData.LastCallTime);
                    subscriberData.LastCallTime = Time.time;
                    subscriberData.NextCallTime = Time.time + subscriberData.Period;
                }
            }
        }

        public void Dispose()
        {
            _pendingHandlers.Clear();
            _subscribers.Clear();
            _subscriberData.Clear();
        }
    }
}