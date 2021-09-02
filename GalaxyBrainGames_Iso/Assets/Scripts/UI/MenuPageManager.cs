using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.UI
{
    public class MenuPageManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup currentInterfacePage;
        [SerializeField, Min(0.1f)] private float pageFadeDuration;

        private CanvasGroup oldInterfacePage;
        private float timer = 0;
        private int currentPage = 0;

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
            group.alpha = 0;
        }

        private void EnableCanvasGroup(CanvasGroup group)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
            group.alpha = 1;
        }

        public void ChangeMenuPage(CanvasGroup group)
        {
            DisableCanvasGroup(currentInterfacePage);
            EnableCanvasGroup(group);

            oldInterfacePage = currentInterfacePage;
            currentInterfacePage = group;
        }
    }
}