using System;
using Game.Blocks;
using Game.Blocks.Movement;

namespace Game.Data
{
    [Serializable]
    public class BlockData
    {
        public int x;             
        public int y;    
        public BlockColor color;
        public BlockType type;
        public BlockState state;
        public int grindAttempts;
        public BlockMovementType movementType;
        public BlockRotation rotation;
    }
}