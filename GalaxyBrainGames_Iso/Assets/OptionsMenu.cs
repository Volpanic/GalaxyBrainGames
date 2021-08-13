using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private SaveData saveData;

    [Header("Option components")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider ambianceVolumeSlider;

    private void Start()
    {
        SetInitalSliderValues();
    }

    private void SetInitalSliderValues()
    {
        if (saveData == null) return;

        if (musicVolumeSlider != null) musicVolumeSlider.value = saveData.Data.MusicVolume;
        if (soundVolumeSlider != null) soundVolumeSlider.value = saveData.Data.SoundVolume;
        if (ambianceVolumeSlider != null) ambianceVolumeSlider.value = saveData.Data.AmbianceVolume;
    }

    public void OnValueChanged()
    {
        if (saveData == null) return;

        if (musicVolumeSlider != null) saveData.Data.MusicVolume = musicVolumeSlider.value;
        if (soundVolumeSlider != null) saveData.Data.SoundVolume = soundVolumeSlider.value;
        if (ambianceVolumeSlider != null) saveData.Data.AmbianceVolume = ambianceVolumeSlider.value;
    }

    public void SaveChanges()
    {
        saveData.SaveGame();
    }
}
