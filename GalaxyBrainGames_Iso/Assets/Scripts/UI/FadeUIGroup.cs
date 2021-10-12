using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.UI
{
    public class FadeUIGroup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField, Min(0)] private float fadeDuration = 0;
        [SerializeField] private bool fadeInOnStart;

        private float timer = 0;
        private bool fadeIn;

        private float alphaStart = 0;
        private float alphaEnd = 1;

        private void Start()
        {
            enabled = false;

            if (fadeInOnStart)
            {
                FadeIn();
            }
        }

        public void FadeOut()
        {
            group.alpha = 1;

            //If duration is 0 do it instantly
            if (fadeDuration == 0)
            {
                DisableGroup();
                return;
            }

            timer = 0;
            fadeIn = false;
            enabled = true;

            alphaStart = 1;
            alphaEnd = 0;
        }

        public void FadeIn()
        {
            group.alpha = 0;

            //If duration is 0 do it instantly
            if (fadeDuration == 0)
            {
                EnableGroup();
                return;
            }

            timer = 0;
            fadeIn = true;
            enabled = true;

            alphaStart = 0;
            alphaEnd = 1;
        }

        private void EnableGroup()
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        private void DisableGroup()
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            group.alpha = Mathf.Lerp(alphaStart, alphaEnd, timer / fadeDuration);

            if(timer >= fadeDuration)
            {
                if (fadeIn) EnableGroup();
                else DisableGroup();
                enabled = false;
            }
        }
    }
}
