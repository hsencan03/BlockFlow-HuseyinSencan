using Game.Blocks.Movement;
using Game.Data;
using Game.Grid;
using UnityEngine;

namespace Game.Blocks
{
    public class BlockModel
    {
        public BlockColor Color { get; }
        public BlockType Type { get; }

        public BlockState State { get; private set; }
        public int GrindAttempts { get; private set; }
        public BlockMovementType MovementType { get; }
        public BlockRotation Rotation { get; }
        public GridPosition Position { get; private set; }
        public GameObject Object { get; private set; }
        
        private GridSystem grid;

        public BlockModel(GridSystem grid, GridPosition position, BlockData data)
        {
            this.grid = grid;
            Color = data.color;
            Type = data.type;
            State = data.state;
            GrindAttempts = data.grindAttempts;
            MovementType = data.movementType;
            Rotation = data.rotation;
            Position = position;
        }
        
        public void SetPosition(GridPosition position)
        {
            grid.SetBlockInGridTiles(Position, Type, Rotation, null);
            Position = position;
            grid.SetBlockInGridTiles(position, Type, Rotation, this);
        }

        public void OnBlockGrind()
        {
            GrindAttempts--;
            if (GrindAttempts == 0)
            {
                State = BlockState.Normal;
            }
        }

        public void SetObject(GameObject blockObject)
        {
            Object = blockObject;
        }

        public void DestroyObject()
        {
            if (Object != null)
            { 
                grid.SetBlockInGridTiles(Position, Type, Rotation, null);
                UnityEngine.Object.Destroy(Object);
                Object = null;
            }
        }
    }
}