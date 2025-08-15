using System.Collections.Generic;
using Game.Blocks;
using Game.Data;
using Game.Grid;

namespace Game.Grinders
{
    public class GrinderModel 
    {
        public BlockColor Color { get; }
        public List<GridPosition> Positions { get; private set; }
        public int Length { get; private set; }

        public bool IsVertical { get; private set; }

        public GrinderModel(GrinderData data)
        {
            Color = data.color;
            Length = data.length;
            Positions = new List<GridPosition>(Length);
            IsVertical = data.isVertical;
        }

        public void AddPosition(GridPosition position)
        {
            Positions.Add(position);
        }
    }
}