using System;

namespace Core.Services.TimerService
{
    public struct TimerParams
    {
        public readonly string ID;
        public TimeSpan Durations;
        public readonly bool AutoStart;
        public readonly float TickIntervalSeconds;

        public TimerParams(string id, TimeSpan durations, bool autoStart = true, float tickIntervalSeconds = 0.5f)
        {
            ID = id;
            Durations = durations;
            AutoStart = autoStart;
            TickIntervalSeconds = tickIntervalSeconds;
        }
    }
}