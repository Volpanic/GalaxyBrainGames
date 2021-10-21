using GalaxyBrain.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Audio
{
    /// <summary>
    /// A component that manages audio in the game
    /// This component is a singleton created when the Instance is called in the getter
    /// This allow for easy playback of sound effects and switching of music.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public Dictionary<string, AudioClip> SoundLookup;
        public ObjectPooler<AudioSource> ObjectPooler;

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
            ObjectPooler = new ObjectPooler<AudioSource>(20, true);

            //Setup music source
            musicSource = ObjectPooler.GetRawGameobject();
            musicSource.gameObject.name = "Music Source";

            musicSource.loop = true;
        }

        public AudioSource GetSoundObject()
        {
            ObjectPooler<AudioSource>.ObjectPoolItem go = ObjectPooler.GetPooledObject();

            return go.component;
        }

        public AudioSource PlaySound(AudioData sound)
        {
            AudioSource source = GetSoundObject();

            //Set pooled objects info
            source.clip = sound.Sound;
            source.outputAudioMixerGroup = sound.SoundMixer;
            source.volume = sound.Volume;
            source.gameObject.SetActive(true);
            source.Play();
            StartCoroutine(DeactivateAfteTime(source.gameObject,source.clip.length));

            return source;
        }

        public AudioSource PlayMusic(AudioData sound)
        {
            if (sound.Sound != musicSource.clip)
            {
                //Set music sources info
                musicSource.clip = sound.Sound;
                musicSource.volume = sound.Volume;
                musicSource.outputAudioMixerGroup = sound.SoundMixer;
                musicSource.Play();
            }

            return musicSource;
        }

        // Used to disable audio sources after there sound has played.
        private IEnumerator DeactivateAfteTime(GameObject obj,float time)
        {
            yield return new WaitForSeconds(time);

            obj.SetActive(false);
        }
    }
}
