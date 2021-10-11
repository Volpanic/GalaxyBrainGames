using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace GalaxyBrain.Systems
{
    [System.Serializable]
    public struct GameData
    {
        public GameData(float musicVol, float soundVol, float ambVol)
        {
            MusicVolume = musicVol;
            SoundVolume = soundVol;
            AmbianceVolume = ambVol;

            MaxLevelCompleted = 0;
        }

        [Range(0f, 1f)] public float MusicVolume;
        [Range(0f, 1f)] public float SoundVolume;
        [Range(0f, 1f)] public float AmbianceVolume;

        [Min(0)] public int MaxLevelCompleted;
    }

    [CreateAssetMenu]
    public class SaveData : ScriptableObject
    {
        private const string SAVE_FILE_NAME = "Save.sav";

        public string FilePath
        {
            get { return Application.persistentDataPath + "/" + SAVE_FILE_NAME; }
        }

        public GameData Data;

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                LoadSave();
            }
        }

        private void OnDisable()
        {
            LoadSave();
        }


        [ContextMenu("Reset Save Data")]
        public void ResetSave()
        {
            Data = new GameData(0.5f, 0.5f, 0.5f);
        }

        [ContextMenu("Save Data")]
        public void SaveGame()
        {
            string jsonData = JsonUtility.ToJson(Data);
            File.WriteAllText(FilePath, jsonData);
        }

        [ContextMenu("Load Save")]
        public void LoadSave()
        {
            if (File.Exists(FilePath))
            {
                string fileContents = File.ReadAllText(FilePath);

                Data = JsonUtility.FromJson<GameData>(fileContents);
            }
        }

        public void MarkLevelComplete(LevelProgression progression)
        {
            string currentIndex = SceneManager.GetActiveScene().name;

            int completeIndex = progression.ScenesInOrder.FindIndex(0,scn => scn == currentIndex);

            if (completeIndex >= Data.MaxLevelCompleted)
            {
                Data.MaxLevelCompleted = completeIndex + 1;
                SaveGame();
                return;
            }
        }
    }
}