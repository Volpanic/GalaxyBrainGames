using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.UI
{
    public class LevelBaseUI : MonoBehaviour
    {
        [SerializeField] private GameEvent levelCompleteEvent;

        private bool done = false;
        private DialougeBox dialogue;

        private void Start()
        {
            dialogue = GetComponentInChildren<DialougeBox>(true);
            if (dialogue != null) dialogue.gameObject.SetActive(false);
        }

        public void WispCollected()
        {
            if (done) return;
            done = true;

            if(dialogue != null)
            {
                dialogue.gameObject.SetActive(true);
            }
            else
            {
                levelCompleteEvent?.Raise();
            }
        }
    }
}
