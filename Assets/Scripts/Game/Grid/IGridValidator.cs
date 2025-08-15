using Game.Blocks;
using Game.Data;
using Game.Grinders;
using UnityEngine;

namespace Game.Grid
{
    public interface IGridValidator
    {
        bool IsPositionValid(int x, int y);
        bool IsPositionValid(GridPosition pos, BlockType blockType, BlockRotation rotation);
        bool IsPositionValid(Vector3 worldPos, BlockType blockType, BlockRotation rotation);
        public bool IsBlockFitsInGrinder(BlockModel blockModel, GridPosition blockPosition, GridPosition checkPosition, out GrinderModel grinderModel);
    }
}