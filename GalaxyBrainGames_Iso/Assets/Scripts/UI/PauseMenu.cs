using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainGroup;
        [SerializeField] private MenuPageManager menuManager;

        private Fade levelFade;

        private void OnEnable()
        {
            Time.timeScale = 0;
            menuManager.ChangeMenuPage(mainGroup);
        }

        private void Start()
        {
            levelFade = Object.FindObjectOfType<Fade>();
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        //Menu Functions
        public void Resume()
        {
            gameObject.SetActive(false);
        }

        public void MainMenu()
        {
            mainGroup.interactable = false;
            levelFade?.FadeOut(() => SceneManager.LoadScene(1));
        }
    }
}
