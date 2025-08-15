using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Services.TimerService
{
    public class TimerService : ITimerService, IInitializable, ITickable, IDisposable
    {
        private IDictionary<string, Timer> activeTimers;
        private IList<string> disposedTimers;
        
        public Timer Create(TimerParams parameters)
        {
            if (parameters.Durations.TotalMilliseconds <= 0)
            {
                throw new ArgumentException("Duration must be greater than zero.", nameof(parameters.Durations));
            }

            Timer timer = new Timer(parameters);
            if (!activeTimers.TryAdd(parameters.ID, timer))
            {
                throw new InvalidOperationException($"Timer with ID {parameters.ID} already exists.");
            }

            return timer;
        }

        public Timer GetTimer(string id)
        {
            if (activeTimers.TryGetValue(id, out Timer timer))
            {
                return timer;
            }

            throw new KeyNotFoundException($"No timer found with ID {id}.");
        }

        public bool DisposeTimer(string id)
        {
            if (activeTimers.ContainsKey(id))
            {
                disposedTimers.Add(id);
                return true;
            }
            return false;
        }

        public void Initialize()
        {
            activeTimers = new Dictionary<string, Timer>();
            disposedTimers = new List<string>();
        }
        
        public void Tick()
        {
            if(activeTimers == null || activeTimers.Count == 0)
                return;

            foreach (string id in disposedTimers)
            {
                activeTimers.Remove(id);
            }
            disposedTimers.Clear();
            
            foreach (Timer timer in activeTimers.Values)
            {
                timer.Update(Time.deltaTime);
            }
        }

        public void Dispose()
        {
            activeTimers.Clear();
        }
    }
}