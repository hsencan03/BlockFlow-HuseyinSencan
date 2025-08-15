using System;
using System.Collections.Generic;

namespace Game.Data
{
    [Serializable]
    public class LevelData
    {
        public int gridWidth;
        public int gridHeight;
        public string timeLimit;
        public List<BlockData> blocks;
        public List<GrinderData> grinders;
    }
}