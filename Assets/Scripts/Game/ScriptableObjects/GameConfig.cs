using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Config/Game")]
    public class GameConfig : ScriptableObject
    {
        public int totalLevels;
        [Tooltip("Number of recently played levels to remember when selecting the next one. Higher values reduce repeats.")]
        public int recentLevelMemoryCount = 2;

        public int targetFrameRate = 60;
        
        private void OnValidate()
        {
            if (totalLevels < 1)
                totalLevels = 1;

            if (recentLevelMemoryCount < 0)
                recentLevelMemoryCount = 0;

            if (recentLevelMemoryCount >= totalLevels)
                recentLevelMemoryCount = Mathf.Max(0, totalLevels - 1);

            if (targetFrameRate < 0)
                targetFrameRate = 30;
        }
    }
}