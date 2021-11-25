using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Volpanic.Easing;
using Volpanic.UITweening;

namespace GalaxyBrain.UI
{
    public class LevelInfoBox : MonoBehaviour
    {
        [SerializeField] private LevelProgression levelProgression;
        [SerializeField] private CanvasGroup infoGroup;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private GridLayoutGroup creaturesInLevelgroup;
        [SerializeField] private RectTransform folderInfoBox;
        [SerializeField] private RawImage levelPreview;

        private string targetScene;
        private bool locked;
        private Fade fade;

        private Vector3 originalFolderPosition;
        private EffectBuilder folderEffect;

        private const float infoBoxTweenDistance = 32;

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
            originalFolderPosition = folderInfoBox.position;

            //Tweening
            folderEffect = new EffectBuilder(this);
            folderInfoBox.position = originalFolderPosition + (Vector3.down * infoBoxTweenDistance);
            folderInfoBox.localScale = Vector3.zero;
            folderEffect.AddEffect(new MoveRectEffect(folderInfoBox, originalFolderPosition, false, null, new TweenData(0.35f, false, Easingf.InOutBack)));
            folderEffect.AddEffect(new ScaleRectEffect(folderInfoBox, Vector3.one, null, new TweenData(0.35f, false, Easingf.OutExpo)));
        }

        public void SetInfo(int levelNum, string targetScene, bool levelLocked)
        {
            levelNameText.text = $"Level {levelNum}";
            this.targetScene = targetScene;

            if (levelProgression.SceneInformation != null && (levelNum - 1) < levelProgression.SceneInformation.Count)
            {
                SceneInfo info = levelProgression.SceneInformation[levelNum - 1];
                actionPointsText.text = info.ActionPointsInLevel.ToString();
                CreateCreatureIcons(info.CreaturesInLevel);
            }
            else
            {
                actionPointsText.text = "Level info is generated when built";
                CreateCreatureIcons(null);
            }

            if (levelProgression.SceneScreenShots != null && (levelNum - 1) < levelProgression.SceneScreenShots.Count)
                levelPreview.texture = levelProgression.SceneScreenShots[levelNum - 1];

            infoGroup.interactable = true;
            infoGroup.blocksRaycasts = true;
            infoGroup.alpha = 1;

            folderInfoBox.position = originalFolderPosition + (Vector3.down * infoBoxTweenDistance);
            folderInfoBox.localScale = Vector3.zero;
            folderEffect.ExecuteEvents();
        }

        private void CreateCreatureIcons(Sprite[] creaturesInLevel)
        {
            //Clear Children
            foreach (Transform child in creaturesInLevelgroup.transform)
            {
                Destroy(child.gameObject);
            }


            if (creaturesInLevel == null) return;

            for (int i = 0; i < creaturesInLevel.Length; i++)
            {
                Sprite creatureIcon = creaturesInLevel[i];
                if (creatureIcon != null)
                {
                    GameObject go = new GameObject();
                    Image sprite = go.AddComponent<Image>();
                    sprite.sprite = creatureIcon;
                    sprite.preserveAspect =  true;
                    go.transform.SetParent(creaturesInLevelgroup.transform, false);
                }
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
