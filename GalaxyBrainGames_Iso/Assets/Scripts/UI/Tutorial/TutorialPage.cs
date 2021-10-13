using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.UI.Tutorial
{
    public class TutorialPage : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private FadeUIGroup fadeGroup;

        public void SelectPage(bool instant)
        {
            if(instant)
            {
                group.alpha = 1;
                group.blocksRaycasts = true;
                group.interactable = true;
                return;
            }

            fadeGroup.FadeIn();
        }

        public void DeselectPage(bool instant)
        {
            if (instant)
            {
                group.alpha = 0;
                group.blocksRaycasts = true;
                group.interactable = true;
                return;
            }

            fadeGroup.FadeOut();
        }
    }
}
