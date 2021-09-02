using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyBrain.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private SaveData saveData;

        [Header("Option components")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider soundVolumeSlider;
        [SerializeField] private Slider ambianceVolumeSlider;

        private bool initilized = false;

        private void Start()
        {
            SetInitalSliderValues();
            initilized = true;
        }

        private void SetInitalSliderValues()
        {
            if (saveData == null) return;

            if (musicVolumeSlider != null) musicVolumeSlider.value = saveData.Data.MusicVolume * 100;
            if (soundVolumeSlider != null) soundVolumeSlider.value = saveData.Data.SoundVolume * 100;
            if (ambianceVolumeSlider != null) ambianceVolumeSlider.value = saveData.Data.AmbianceVolume * 100;
        }

        public void OnValueChanged()
        {
            if (saveData == null || !initilized) return;

            if (musicVolumeSlider != null) saveData.Data.MusicVolume = musicVolumeSlider.value / 100;
            if (soundVolumeSlider != null) saveData.Data.SoundVolume = soundVolumeSlider.value / 100;
            if (ambianceVolumeSlider != null) saveData.Data.AmbianceVolume = ambianceVolumeSlider.value / 100;
        }

        public void SaveChanges()
        {
            saveData.SaveGame();
        }
    }
}