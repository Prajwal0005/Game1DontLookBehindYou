using System.IO;
using UnityEngine;
using DontLookBehindYou.Core;

namespace DontLookBehindYou.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private string saveFilePath;

        public SaveData CurrentSaveData { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
            CurrentSaveData = new SaveData();
        }

        public void SaveGame()
        {
            try
            {
                // In a real scenario, we would gather data from Player, Inventory, etc. here
                // CurrentSaveData.playerPosition = ...
                
                string json = JsonUtility.ToJson(CurrentSaveData, true);
                File.WriteAllText(saveFilePath, json);
                
                Debug.Log($"[SaveManager] Game saved successfully to {saveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
            }
        }

        public bool LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(saveFilePath);
                    CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
                    
                    Debug.Log($"[SaveManager] Game loaded successfully from {saveFilePath}");
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
                    return false;
                }
            }
            else
            {
                Debug.Log("[SaveManager] No save file found. Starting fresh.");
                CurrentSaveData = new SaveData();
                return false;
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                CurrentSaveData = new SaveData();
                Debug.Log("[SaveManager] Save file deleted.");
            }
        }
    }
}
