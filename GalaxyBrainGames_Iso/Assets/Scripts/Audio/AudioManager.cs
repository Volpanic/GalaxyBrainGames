using GalaxyBrain.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Dictionary<string, AudioClip> soundLookup;
        public ObjectPooler<AudioSource> objectPooler;

        private static AudioManager instance;

        public static AudioManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = GenerateInstance();
                }
                return instance;
            }
        }

        private AudioSource musicSource;

        /// <summary>
        /// Generates a singleton instance if we don't have one
        /// </summary>
        /// <returns></returns>
        private static AudioManager GenerateInstance()
        {
            GameObject go = new GameObject("Audio Manager");
            AudioManager manager = go.AddComponent<AudioManager>();

            GameObject.DontDestroyOnLoad(go);

            return manager;
        }

        private void Awake()
        {
            objectPooler = new ObjectPooler<AudioSource>(20, false);

            //Setup music source
            musicSource = objectPooler.GetRawGameobject();
            musicSource.gameObject.name = "Music Source";

            musicSource.loop = true;
        }

        public AudioSource GetSoundObject()
        {
            ObjectPooler<AudioSource>.ObjectPoolItem go = objectPooler.GetPooledObject();

            return go.component;
        }

        public AudioSource PlaySound(AudioData sound)
        {
            AudioSource source = GetSoundObject();

            source.clip = sound.Sound;
            source.outputAudioMixerGroup = sound.SoundMixer;
            source.volume = sound.Volume;
            source.Play();
            source.gameObject.SetActive(true);
            StartCoroutine(DeactivateAfteTime(source.gameObject,source.clip.length));

            return source;
        }

        public AudioSource PlayMusic(AudioData sound)
        {
            if (sound.Sound != musicSource.clip)
            {
                musicSource.clip = sound.Sound;
                musicSource.volume = sound.Volume;
                musicSource.outputAudioMixerGroup = sound.SoundMixer;
                musicSource.Play();
            }

            return musicSource;
        }

        private IEnumerator DeactivateAfteTime(GameObject obj,float time)
        {
            yield return new WaitForSeconds(time);

            obj.SetActive(false);
        }
    }
}
