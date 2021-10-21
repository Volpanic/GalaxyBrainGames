using System;
using UnityEngine;

namespace GalaxyBrain.UI
{
    /// <summary>
    /// Fades a UI canvas in and out and invokes an event when it's completed
    /// </summary>
    public class Fade : MonoBehaviour
    {
        //Fade info
        [SerializeField] private CanvasGroup FadeInGroup;
        [SerializeField] private bool fadeIn = true;

        //This action is invoked when a fade is completed, it then clears itself.
        private Action afterFade;

        public bool FadeDone
        {
            get
            {
                if (fadeIn) return FadeInGroup.alpha == 0;
                else return FadeInGroup.alpha == 1;
            }
        }

        private void Start()
        {
            FadeInGroup.alpha = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (fadeIn)
            {
                FadeInGroup.alpha = Mathf.MoveTowards(FadeInGroup.alpha, 0, Time.unscaledDeltaTime);
                if (FadeInGroup.alpha == 0) FadeComplete();
            }
            else
            {
                FadeInGroup.alpha = Mathf.MoveTowards(FadeInGroup.alpha, 1, Time.unscaledDeltaTime);
                if (FadeInGroup.alpha == 1) FadeComplete();
            }
        }

        private void FadeComplete()
        {
            if (FadeInGroup.alpha == 0) FadeInGroup.blocksRaycasts = false;
            else FadeInGroup.blocksRaycasts = true;
            afterFade?.Invoke();
            afterFade = null;
            enabled = false;
        }

        //Actions chain an event to happen when the fade is completed.
        public void FadeIn(Action fadeCompleteEvent = null)
        {
            FadeInGroup.blocksRaycasts = true;
            FadeInGroup.alpha = 1;
            fadeIn = true;
            enabled = true;
            afterFade = fadeCompleteEvent;
        }

        public void FadeOut(Action fadeCompleteEvent = null)
        {
            FadeInGroup.blocksRaycasts = true;
            FadeInGroup.alpha = 0;
            fadeIn = false;
            enabled = true;
            afterFade = fadeCompleteEvent;
        }
    }
}
