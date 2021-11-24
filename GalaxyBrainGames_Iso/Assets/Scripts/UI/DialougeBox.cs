using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Volpanic.Easing;

namespace GalaxyBrain.UI
{
    [System.Serializable]
    public struct DialougeSequence
    {
        [TextArea(3, 5)]
        public string DialougeText;

        public Sprite SpeakerPortrait;
        public float NormalizedPortraitPosition;
        public UnityEvent EndOfLineEvent;
    }

    public class DialougeBox : MonoBehaviour // Handels fading in letters of the dialogue box.
    {
        public TextMeshProUGUI DialougeText;
        public float DelayBetweenLetter = 0.1f;
        public Transform DialougeDoneIndecator;
        [SerializeField] private Image dialougePortrait;

        private float timer = 0;
        private int maxCharecter = 1;
        private int messageCount = 0;

        [SerializeField]
        private List<DialougeSequence> playingSequence;

        [SerializeField] private UnityEvent onDialougeComplete;

        private bool active = true;

        private void Start()
        {
            if (playingSequence != null && playingSequence.Count > 0)
            {
                DialougeText.text = playingSequence[0].DialougeText;
                DialougeText.ForceMeshUpdate();
                UpdateDialougePortrait(playingSequence[0]);
                DialougeText.maxVisibleCharacters = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            TypewritterEffect();
        }

        private void TypewritterEffect()
        {
            // If Active update code
            if (active)
            {
                DialougeLineUpdate();
            }
            else
            {
                DialougeLineFinished();
            }

            DialougeText.maxVisibleCharacters = maxCharecter;
        }

        private void DialougeLineUpdate()
        {
            //Turn off the done idecator 
            DialougeDoneIndecator.gameObject.SetActive(false);

            timer += Time.deltaTime;
            UpdateDialougePortrait(playingSequence[messageCount]);

            // Clicked before Dialouge is done
            if (Input.GetMouseButtonDown(0))
            {
                maxCharecter = DialougeText.textInfo.characterCount - 2;
                timer = DelayBetweenLetter * 1.1f;
            }

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
        }

        //Runs when the line of dialogue has concluded
        private void DialougeLineFinished()
        {
            //Turn on the done indicator 
            DialougeDoneIndecator.gameObject.SetActive(true);

            //Text should be done, so close if clicked
            if (Input.GetMouseButtonDown(0))
            {
                playingSequence[messageCount].EndOfLineEvent?.Invoke();
                messageCount++;

                //Reset values and play dialouge
                active = true;
                maxCharecter = 1;
                timer = 0;

                if (messageCount > playingSequence.Count - 1)
                {
                    active = false;
                    onDialougeComplete.Invoke();
                    gameObject.SetActive(false);
                }
                else // Set new message info
                {
                    //Set the text to be the first message
                    DialougeText.text = playingSequence[messageCount].DialougeText;
                    UpdateDialougePortrait(playingSequence[messageCount]);
                }


            }
        }

        private void UpdateDialougePortrait(DialougeSequence sequence)
        {
            dialougePortrait.sprite = sequence.SpeakerPortrait;
        }

        public void PlayDialouge(DialougeSequence playThis)
        {
            //Reset values and play dialouge
            active = true;
            maxCharecter = 1;
            timer = 0;
            messageCount = 0;

            //Make sure the box is active
            gameObject.SetActive(true);
        }
    }
}