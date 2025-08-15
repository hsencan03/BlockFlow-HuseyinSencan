using System;

namespace Core.Data
{
    [Serializable]
    public class GameSaveData
    {
        public int levelIndex;
        public bool hasPlayedAllLevels;
    }
}