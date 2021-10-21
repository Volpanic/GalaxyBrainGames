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
    }

    public class DialougeBox : MonoBehaviour // Handels fading in letters of the dialogue box.
    {
        public TextMeshProUGUI DialougeText;
        public float DelayBetweenLetter = 0.1f;
        public Transform DialougeDoneIndecator;
        [SerializeField] private Image dialougePortrait;
        [SerializeField] private RectTransform dialougeBoxRect;

        private float timer = 0;
        private int maxCharecter = 1;
        private int messageCount = 0;

        private RectTransform portraitTransform;

        [SerializeField]
        private List<DialougeSequence> playingSequence;

        [SerializeField] private UnityEvent onDialougeComplete;

        private bool active = true;

        //Portrait Effects
        private float portraitTimer = 0;
        private Vector3 startPortraitPositon;
        private Vector3 targetPortraitPositon;
        private Vector2 targetPortraitPivot;
        private Sprite targetPortraitSprite;
        private Color targetPortraitColor;

        private void Start()
        {
            portraitTransform = dialougePortrait.GetComponent<RectTransform>();

            startPortraitPositon = portraitTransform.position;
            targetPortraitPositon = portraitTransform.position;
            targetPortraitPivot = portraitTransform.pivot;
            targetPortraitSprite = dialougePortrait.sprite;
            targetPortraitColor = Color.white;

            if (playingSequence != null && playingSequence.Count > 0)
            {
                DialougeText.text = playingSequence[0].DialougeText;
                UpdateDialougePortrait(playingSequence[0]);
                portraitTimer = 0.9f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            TypewritterEffect();
            UpdatePortraitEffects(0.1f);
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

        private void UpdatePortraitEffects(float duration = 1f)
        {
            if(portraitTimer <= duration)
            {
                portraitTimer += Time.deltaTime;

                Vector3 targetPos;
                targetPos.x = Mathf.Lerp(startPortraitPositon.x, targetPortraitPositon.x, portraitTimer/duration);
                targetPos.y = Mathf.Lerp(startPortraitPositon.y, targetPortraitPositon.y, portraitTimer/duration);
                targetPos.z = Mathf.Lerp(startPortraitPositon.z, targetPortraitPositon.z, portraitTimer/duration);

                portraitTransform.position = targetPos;

                ////First Half
                //if (portraitTimer <= duration * 0.5f)
                //{
                //    targetPortraitColor.a = Mathf.Lerp(1f,0f,portraitTimer / duration);
                //}
                //else
                //{
                //    dialougePortrait.sprite = targetPortraitSprite;
                //    targetPortraitColor.a = Mathf.Lerp(.5f, 1f,(portraitTimer / duration));
                //}

                //dialougePortrait.color = targetPortraitColor;
            }
        }

        private void UpdateDialougePortrait(DialougeSequence sequence)
        {
            startPortraitPositon = portraitTransform.position;
            targetPortraitSprite = sequence.SpeakerPortrait;

            float xPos = Mathf.Lerp(0, dialougeBoxRect.rect.size.x * 0.5f, sequence.NormalizedPortraitPosition);

            targetPortraitPivot = new Vector2(sequence.NormalizedPortraitPosition, portraitTransform.pivot.y);

            targetPortraitPositon = new Vector3(xPos, portraitTransform.position.y, portraitTransform.position.z);

            dialougePortrait.sprite = sequence.SpeakerPortrait;

            portraitTimer = 0;
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