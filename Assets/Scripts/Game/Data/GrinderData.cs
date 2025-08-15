using Game.Blocks;

namespace Game.Data
{
    [System.Serializable]
    public class GrinderData
    {
        public int x;
        public int y;
        
        public int length;
        public BlockColor color;
        public bool isVertical;
    }
}