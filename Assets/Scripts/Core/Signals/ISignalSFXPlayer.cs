using Game.Audio;

namespace Core.Signals
{
    public interface ISignalSfxPlayer
    {
        public float DelaySeconds { get; }
        public SfxId Id { get; }
    }
}