using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.UI
{
    public class MenuPageManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup currentInterfacePage;
        [SerializeField] private float pageFadeDuration;

        private CanvasGroup oldInterfacePage;

        private void Start()
        {
            if (currentInterfacePage != null)
            {
                EnableCanvasGroup(currentInterfacePage);
            }
        }

        private void DisableCanvasGroup(CanvasGroup group)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        private void EnableCanvasGroup(CanvasGroup group)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        public void ChangeMenuPage(CanvasGroup group)
        {
            DisableCanvasGroup(currentInterfacePage);
            EnableCanvasGroup(group);

            if (pageFadeDuration > 0)
            {
                StartCoroutine(FadeBetweenPages(currentInterfacePage, group, pageFadeDuration));
            }
            else
            {
                ChangeGroup(currentInterfacePage, group);
            }
            oldInterfacePage = currentInterfacePage;
            currentInterfacePage = group;
        }

        private void ChangeGroup(CanvasGroup oldGroup, CanvasGroup newGroup)
        {
            DisableCanvasGroup(oldGroup);
            EnableCanvasGroup(newGroup);
            oldGroup.alpha = 0;
            newGroup.alpha = 1;
        }

        //We use a Coroutine here so we only update when needed
        private IEnumerator FadeBetweenPages(CanvasGroup oldGroup, CanvasGroup newGroup, float duration)
        {
            float timer = 0;

            DisableCanvasGroup(oldGroup);
            DisableCanvasGroup(newGroup);
            float alpha = 0;

            while(timer <= duration)
            {
                timer += Time.deltaTime;
                alpha = timer / duration;

                oldGroup.alpha = 1f - alpha;
                newGroup.alpha = alpha;

                yield return null;
            }

            EnableCanvasGroup(newGroup);

            yield return null;
        }
    }
}