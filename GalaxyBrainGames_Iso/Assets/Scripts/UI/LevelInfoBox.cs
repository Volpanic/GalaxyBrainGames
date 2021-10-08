using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.UI
{
    public class LevelInfoBox : MonoBehaviour
    {
        [SerializeField] private CanvasGroup infoGroup;
        [SerializeField] private TextMeshProUGUI levelNameText;

        private string targetScene;
        private bool locked;
        private Fade fade;

        private void Awake()
        {
            infoGroup.interactable = false;
            infoGroup.blocksRaycasts = false;
            infoGroup.alpha = 0;
        }

        private void Start()
        {
            //Slow but only happens once
            fade = FindObjectOfType<Fade>();
        }

        public void SetInfo(int levelNum, string targetScene, bool levelLocked)
        {
            levelNameText.text = $"Level {levelNum}";
            this.targetScene = targetScene;

            infoGroup.interactable = true;
            infoGroup.blocksRaycasts = true;
            infoGroup.alpha = 1;
        }

        public void CloseBox()
        {
            infoGroup.interactable = false;
            infoGroup.blocksRaycasts = false;
            infoGroup.alpha = 0;
        }

        public void StartLevel()
        {
            if (!locked)
            {
                fade.FadeOut(() => {SceneManager.LoadScene(targetScene);});
            }
        }
    }
}