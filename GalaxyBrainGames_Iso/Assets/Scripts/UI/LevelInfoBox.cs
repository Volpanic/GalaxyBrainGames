using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GalaxyBrain.UI
{
    public class LevelInfoBox : MonoBehaviour
    {
        [SerializeField] private LevelProgression levelProgression;
        [SerializeField] private CanvasGroup infoGroup;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private GridLayoutGroup creaturesInLevelgroup;

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

            SceneInfo info = levelProgression.SceneInformation[levelNum - 1];
            actionPointsText.text = info.ActionPointsInLevel.ToString();
            CreateCreatureIcons(info.CreaturesInLevel);

            infoGroup.interactable = true;
            infoGroup.blocksRaycasts = true;
            infoGroup.alpha = 1;
        }

        private void CreateCreatureIcons(Sprite[] creaturesInLevel)
        {
            //Clear Children
            foreach (Transform child in creaturesInLevelgroup.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < creaturesInLevel.Length; i++)
            {
                GameObject go = new GameObject();
                go.AddComponent<Image>().sprite = creaturesInLevel[i];
                go.transform.SetParent(creaturesInLevelgroup.transform,false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)creaturesInLevelgroup.transform);
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
