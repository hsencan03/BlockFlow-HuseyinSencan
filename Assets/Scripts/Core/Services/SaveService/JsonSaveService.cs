using System.IO;
using Core.Data;
using UnityEngine;

namespace Core.Services.SaveService
{
    public class JsonSaveService<T> : ISaveService<T> where T : class
    {
        private const string SaveFileName = "gamesave.json";
        
        private readonly string saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        public void Save(T data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(saveFilePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game data to JSON: {e.Message}");
            }
        }

        public T Load()
        {
            if (!HasSave())
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game data from JSON: {e.Message}");
                return null;
            }
        }

        public bool HasSave()
        {
            return File.Exists(saveFilePath);
        }

        public void DeleteSave()
        {
            if (HasSave())
            {
                File.Delete(saveFilePath);
            }
        }
    }
}