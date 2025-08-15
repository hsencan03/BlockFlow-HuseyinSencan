using Core.Data;
using UnityEngine;

namespace Core.Services.SaveService
{
    public class PlayerPrefsSaveService<T> : ISaveService<T> where T : GameSaveData
    {
        private const string SaveKey = "GameSaveData";

        public void Save(T data)
        {
            try
            {
                string json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(SaveKey, json);
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game data to PlayerPrefs: {e.Message}");
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
                string json = PlayerPrefs.GetString(SaveKey);
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game data from PlayerPrefs: {e.Message}");
                return null;
            }
        }

        public bool HasSave()
        {
            return PlayerPrefs.HasKey(SaveKey);
        }

        public void DeleteSave()
        {
            if (HasSave())
            {
                PlayerPrefs.DeleteKey(SaveKey);
                PlayerPrefs.Save();
            }
        }
    }
}