using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Audio
{
    [CreateAssetMenu]
    public class AudioData : ScriptableObject
    {
        public AudioClip Sound;

        public AudioType AudioType;

        [Range(0f, 1f)]
        public float Volume = 1f;

        public void Play()
        {
            AudioManager.Instance.PlaySound(this);
        }
    }
}
