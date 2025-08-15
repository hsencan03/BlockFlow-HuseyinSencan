namespace Core.Services.TimerService
{
    public interface ITimerService
    { 
        Timer Create(TimerParams parameters); 
        Timer GetTimer(string id);
        bool DisposeTimer(string id);
    }
}