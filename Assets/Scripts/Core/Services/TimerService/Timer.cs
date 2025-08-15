using System;

namespace Core.Services.TimerService
{
    public class Timer
    {
        public string ID { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public TimeSpan Durations { get; private set; }
        
        public TimeSpan RemainingTime { get;  private set; }
        public float TickInterval { get; private set; }
        
        public event Action OnTick; 
        public event Action OnTimerCompleted;
        
        private float elapsedTime;

        public Timer(TimerParams parameters)
        {
            ID = parameters.ID;
            Durations = parameters.Durations;
            TickInterval = parameters.TickIntervalSeconds;
            RemainingTime = Durations;
            elapsedTime = 0f;   
            IsPaused = false;
            IsCompleted = false;
            IsRunning = false;

            if (parameters.AutoStart)
            {
                Start();
            }
        }
        
        public void Start()
        {
            if(IsRunning || IsCompleted)
                return;
            
            IsRunning = true;
        }
        
        public void Update(float deltaTime)
        {
            if (!IsRunning || IsPaused || IsCompleted)
            {
                return;
            }

            RemainingTime -= TimeSpan.FromSeconds(deltaTime);
            elapsedTime += deltaTime;
            if (RemainingTime <= TimeSpan.Zero)
            {
                IsCompleted = true;
                IsRunning = false;
                RemainingTime = TimeSpan.Zero;
                OnTimerCompleted?.Invoke();
            }
            else if (elapsedTime >= TickInterval)
            {
                OnTick?.Invoke();
                elapsedTime = 0f;
            }
        }
        
        public void Pause()
        {
            if(IsCompleted)
                return;
            
            IsPaused = true;
            IsRunning = false;
        }
        
        public void Resume()
        {
            if(IsCompleted)
                return;
            
            IsRunning = true;
            IsPaused = false;
        }
    }
}