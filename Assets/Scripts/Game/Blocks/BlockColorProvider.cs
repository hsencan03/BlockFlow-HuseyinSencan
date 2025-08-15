using Game.Interfaces;
using UnityEngine;

namespace Game.Blocks
{
    public class BlockColorProvider : IColorProvider<BlockColor>
    {
        public Color GetColor(BlockColor colorType)
        {
            return colorType switch
            {
                BlockColor.Blue => Color.blue,
                BlockColor.Green => Color.green,
                BlockColor.Red => Color.red,
                BlockColor.Yellow => Color.yellow,
                _ => Color.white 
            };
        }
    }
}