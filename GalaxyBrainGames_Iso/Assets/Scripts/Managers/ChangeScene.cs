using GalaxyBrain.Systems;
using GalaxyBrain.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.Managers
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private Fade fade;
        [SerializeField] private LevelProgression levelProg;

        private string targetIndex = "";

        public void Changescene()
        {
            if (fade.FadeDone)
            {
                fade.FadeOut(ChangeSceneAfterFade);
            }
        }

        public void ResetScene()
        {
            if (fade.FadeDone)
            {
                fade.FadeOut(ResetSceneAfterFade);
            }
        }

        private void ChangeSceneAfterFade()
        {
            levelProg.IncrementCurrentScene();
            targetIndex = levelProg.GetCurrentScene();

            SceneManager.LoadScene(targetIndex);
        }

        private void ResetSceneAfterFade()
        {
            targetIndex = SceneManager.GetActiveScene().name;

            SceneManager.LoadScene(targetIndex);
        }
    }
}