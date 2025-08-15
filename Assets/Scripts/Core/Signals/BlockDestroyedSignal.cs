using Game.Audio;
using Game.Blocks;
using Game.Data;
using Game.Grinders;

namespace Core.Signals
{
    public readonly struct BlockDestroyedSignal : ISignalSfxPlayer
    {
        public BlockModel Block { get; }
        public GrinderModel Grinder { get; }
        public float AnimDurationSec { get; }
        
        public float DelaySeconds => AnimDurationSec;
        public SfxId Id => SfxId.BlockDestroyed;
        
        public BlockDestroyedSignal(BlockModel block, GrinderModel grinder, float animDurationSec)
        {
            Block = block;
            Grinder = grinder;
            AnimDurationSec = animDurationSec;
        }
    }
}