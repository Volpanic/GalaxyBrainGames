using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyBrain.UI
{
    [System.Serializable]
    public struct DialougeSequence
    {
        [TextArea(3, 5)]
        public string DialougeText;
    }

    public class DialougeBox : MonoBehaviour // Handels fading in letters of the dialogue box.
    {
        public TextMeshProUGUI DialougeText;
        public float DelayBetweenLetter = 0.1f;
        public Transform DialougeDoneIndecator;

        private float timer = 0;
        private int maxCharecter = 1;
        private int messageCount = 0;

        [SerializeField]
        private List<DialougeSequence> playingSequence;
        private byte CharecterAlpha = 0;
        private bool active = true;

        private void Start()
        {
            if (playingSequence != null && playingSequence.Count > 0)
            {
                DialougeText.text = playingSequence[0].DialougeText;
            }
        }

        // Update is called once per frame
        void Update()
        {
            TypewritterEffect();
        }

        private void TypewritterEffect()
        {
            Color32[] colors = DialougeText.textInfo.meshInfo[0].colors32;

            // If Active update code
            if (active && colors != null && colors.Length > 0)
            {
                DialougeLineUpdate(colors);
            }
            else
            {
                DialougeLineFinished();
            }

            DialougeText.maxVisibleCharacters = maxCharecter;
        }

        private void DialougeLineUpdate(Color32[] colors)
        {
            //Turn off the done idecator 
            DialougeDoneIndecator.gameObject.SetActive(false);

            timer += Time.deltaTime;

            while (timer > DelayBetweenLetter)
            {
                timer -= DelayBetweenLetter;

                //Increment Charecter
                maxCharecter++;

                if (maxCharecter >= DialougeText.textInfo.characterCount)
                {
                    active = false;
                    break;
                }

                if (DelayBetweenLetter <= 0) break;
            }

            DialougeText.UpdateVertexData();
        }

        //Runs when the line of dialogue has concluded
        private void DialougeLineFinished()
        {
            //Turn on the done indicator 
            DialougeDoneIndecator.gameObject.SetActive(true);

            //Text should be done, so close if clicked
            if (Input.GetMouseButtonDown(0))
            {
                messageCount++;

                //Reset values and play dialouge
                active = true;
                maxCharecter = 1;
                CharecterAlpha = 0;
                timer = 0;

                if (messageCount > playingSequence.Count - 1)
                {
                    active = false;
                    gameObject.SetActive(false);
                }
                else // Set new message info
                {
                    //Set the text to be the first message
                    DialougeText.text = playingSequence[messageCount].DialougeText;
                }
            }
        }

        public void PlayDialouge(DialougeSequence playThis)
        {
            //Reset values and play dialouge
            active = true;
            maxCharecter = 1;
            CharecterAlpha = 0;
            timer = 0;
            messageCount = 0;

            //Make sure the box is active
            gameObject.SetActive(true);
        }
    }
}