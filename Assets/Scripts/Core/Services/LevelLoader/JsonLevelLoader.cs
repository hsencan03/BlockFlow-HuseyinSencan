using Cysharp.Threading.Tasks;
using Game.Data;
using UnityEngine;

namespace Core.Services.LevelLoader
{
    public class JsonLevelLoader : ILevelLoader
    {
        public async UniTask<LevelData> LoadLevelAsync(string levelName)
        {
            var resourceRequest = Resources.LoadAsync<TextAsset>(Constants.LevelFileName + levelName);
            await resourceRequest;

            TextAsset textAsset = resourceRequest.asset as TextAsset;

            if (textAsset == null)
            {
                Debug.LogError($"Level JSON '{levelName}' not found in Resources/Levels/");
                return null;
            }

            LevelData levelData = JsonUtility.FromJson<LevelData>(textAsset.text);
            if (levelData == null)
            {
                Debug.LogError($"Failed to deserialize level JSON '{levelName}'");
            }

            return levelData;
        }
    }
}