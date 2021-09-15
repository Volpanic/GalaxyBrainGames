using GalaxyBrain.Systems;
using GalaxyBrain.Utility.Extnesion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

        [Header("Audio")]
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup soundEffectMixer;

        private bool initilized = false;

        private void Start()
        {
            SetInitalSliderValues();
            SetMixerVolume();
            initilized = true;
        }

        private void SetMixerVolume()
        {
            if (musicMixer != null)
            {
                //Out of 80 because that's the 
                musicMixer.audioMixer.SetFloat("musicVolume", musicVolumeSlider.value.Remap(0f, 100f, -80f, 0f));
                musicMixer.audioMixer.SetFloat("sfxVolume",   soundVolumeSlider.value.Remap(0f, 100f, -80f, 0f));
            }
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

            SetMixerVolume();
        }

        public void SaveChanges()
        {
            SetMixerVolume();
            saveData.SaveGame();
        }
    }
}