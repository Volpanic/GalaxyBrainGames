using UnityEngine;

namespace GalaxyBrain.Audio
{
    /// <summary>
    /// Plays a AudioData when at the start of the scene
    /// Then disables itself.
    /// </summary>
    public class LevelMusic : MonoBehaviour
    {
        [SerializeField] private AudioData songToPlay;

        public void Awake()
        {
            songToPlay?.Play();
            enabled = false;
        }
    }
}
