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

        [SerializeField] private Toggle fullscreenToggle;

        [Header("Audio")]
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup soundEffectMixer;

        private bool initilized = false;

        private void Start()
        {
            SetInitalSliderValues();
            SetMixerVolume();
            SetFullscreenToggle();
            SetFullscreenMode();
            initilized = true;
        }

        private void SetFullscreenMode()
        {
            if (saveData.Data.Fullscreen)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        private void SetFullscreenToggle()
        {
            fullscreenToggle.isOn = saveData.Data.Fullscreen;
        }

        private void SetMixerVolume()
        {
            if (musicMixer != null)
            {
                //Remapped to -80f and 0 because that how the mixer handles it.
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

        public void FullscreenToggleChanged()
        {
            saveData.Data.Fullscreen = fullscreenToggle.isOn;
            SetFullscreenMode();
            saveData.SaveGame();
        }

        public void SaveChanges()
        {
            SetMixerVolume();
            saveData.SaveGame();
        }
    }
}