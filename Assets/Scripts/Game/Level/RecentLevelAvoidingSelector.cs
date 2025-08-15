using System;
using System.Linq;
using Core.Data;
using Game.ScriptableObjects;

namespace Game.Level
{
    public class RecentLevelAvoidingSelector : ILevelSelector
    {
        private readonly int[] lastPlayedLevels;
        private int currentIndex;
        
        private readonly GameConfig gameConfig;

        public RecentLevelAvoidingSelector(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
            lastPlayedLevels = Enumerable.Repeat(-1, gameConfig.recentLevelMemoryCount).ToArray();
            currentIndex = 0;
        }

        public int GetNextLevel(GameSaveData saveData)
        {
            if (saveData.hasPlayedAllLevels)
            {
                return SelectRandomLevel(gameConfig.totalLevels);
            }

            int nextLevel = ++saveData.levelIndex;
            AddLevelInLastPlayed(nextLevel);

            if (saveData.levelIndex + 1 == gameConfig.totalLevels)
            {
                saveData.hasPlayedAllLevels = true;
            }

            return nextLevel;
        }

        private int SelectRandomLevel(int totalLevels)
        {
            int[] temp = new int[totalLevels];
            int count = 0;

            for (int i = 0; i < totalLevels; i++)
            {
                bool recentlyPlayed = false;
                for (int j = 0; j < gameConfig.recentLevelMemoryCount; j++)
                {
                    if (lastPlayedLevels[j] == i)
                    {
                        recentlyPlayed = true;
                        break;
                    }
                }

                if (!recentlyPlayed)
                {
                    temp[count++] = i;
                }
            }

            if (count == 0)
            {
                Array.Copy(lastPlayedLevels, temp, gameConfig.recentLevelMemoryCount);
                count = gameConfig.recentLevelMemoryCount;
            }

            int selected = temp[UnityEngine.Random.Range(0, count)];
            AddLevelInLastPlayed(selected);
            return selected;
        }

        private void AddLevelInLastPlayed(int level)
        {
            lastPlayedLevels[currentIndex] = level;
            currentIndex = (currentIndex + 1) % gameConfig.recentLevelMemoryCount;
        }
    }
}