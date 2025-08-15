using System;

namespace Game.Blocks.Movement
{
    public class MovementStrategyFactory
    {
        public IMovementStrategy Create(BlockModel blockModel)
        {
            if (blockModel.State == BlockState.Frozen)
                return new FrozenMovementStrategy();
            
            return blockModel.MovementType switch
            {
                BlockMovementType.Free => new FreeMovement(),
                BlockMovementType.LeftRight => new HorizontalMovement(),
                BlockMovementType.UpAndDown => new VerticalMovement(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}