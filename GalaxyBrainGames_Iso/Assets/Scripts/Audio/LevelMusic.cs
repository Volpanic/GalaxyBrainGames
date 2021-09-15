using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Audio
{
    public class LevelMusic : MonoBehaviour
    {
        [SerializeField] private AudioData songToPlay;

        public void Awake()
        {
            songToPlay?.Play();
        }
    }
}
