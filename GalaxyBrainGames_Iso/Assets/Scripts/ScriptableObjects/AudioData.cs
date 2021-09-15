using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GalaxyBrain.Audio
{
    [CreateAssetMenu]
    public class AudioData : ScriptableObject
    {
        public AudioClip Sound;
        public AudioMixerGroup SoundMixer;

        public AudioType AudioType;

        [Range(0f, 1f)]
        public float Volume = 1f;

        public void Play()
        {
            if (AudioType == AudioType.Sound)
            {
                AudioManager.Instance.PlaySound(this);
            }

            if (AudioType == AudioType.Music)
            {
                AudioManager.Instance.PlayMusic(this);
            }
        }
    }
}
