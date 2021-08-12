using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SaveData : ScriptableObject
{
    private const string SAVE_FILE_PATH = "Save.sav";

    private float musicVolume = 0.5f;
    private float soundVolume = 0.5f;
    private float ambianceVolume = 0.5f;

    private int maxLevelCompleted = 0;

    public void ResetSave()
    {
        musicVolume = 0.5f;
        soundVolume = 0.5f;
        ambianceVolume = 0.5f;

        maxLevelCompleted = 0;
    }

    public void SaveGame()
    {

    }

    public void LoadSave()
    {
        
    }
}
