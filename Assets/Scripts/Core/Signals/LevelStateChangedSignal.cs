using Game.Audio;
using Game.Enums;

namespace Core.Signals
{
    public readonly struct LevelStateChangedSignal : ISignalSfxPlayer
    {
        public readonly int LevelIndex;
        public readonly LevelState State;
        public float DelaySeconds => 0.5f;
        public SfxId Id { get; }
        
        public LevelStateChangedSignal(int levelIndex, LevelState state)
        {
            LevelIndex = levelIndex;
            State = state;
            
            Id = State switch
            {
                LevelState.Completed => SfxId.LevelComplete,
                _ => SfxId.None
            };
        }

    }
}