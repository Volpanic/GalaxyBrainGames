using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GalaxyBrain.UI.Tutorial
{
    public class TutorialManger : MonoBehaviour
    {
        [SerializeField] private Button previousPageButton;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private TextMeshProUGUI pageProgressionText;

        private int currentPage = 0;
        private TutorialPage[] pages;

        private void Awake()
        {
            GetTutorialPages();
            SetPageButtonEvents();
            UpdateUI();
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        private void SetPageButtonEvents()
        {
            previousPageButton.onClick.AddListener(() => DecrementPage());
            nextPageButton.onClick.AddListener(()     => IncrementPage());
        }

        private void UpdateUI()
        {
            UpdatePageButtonVisibility();
            UpdatePageProgressionText();
            SetActivePages();
        }

        private void SetActivePages()
        {
            for(int i = 0; i < pages.Length; i++)
            {
                if (i == currentPage) pages[i].SelectPage(true);
                else pages[i].DeselectPage(true);
            }
        }

        private void UpdatePageProgressionText()
        {
            pageProgressionText.text = $"{currentPage+1}/{pages.Length}";
        }

        private void UpdatePageButtonVisibility()
        {
            //Disable buttons if no more menus to look through
            if (currentPage == 0) previousPageButton.gameObject.SetActive(false);
            else previousPageButton.gameObject.SetActive(true);

            if (currentPage >= pages.Length-1) nextPageButton.gameObject.SetActive(false);
            else nextPageButton.gameObject.SetActive(true);
        }

        private void GetTutorialPages()
        {
            pages = GetComponentsInChildren<TutorialPage>();
        }

        public void IncrementPage()
        {
            //Button shouldn't be active if this will go out of bounds
            currentPage += 1;
            UpdateUI();
        }   
        
        public void DecrementPage()
        {
            //Button shouldn't be active if this will go out of bounds
            currentPage -= 1;
            UpdateUI();
        }
    }
}
