using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] LevelProgression levelProgression;
       
        private bool shouldChangeScene = true;
        private Fade fade;

        private void Start()
        {
            //Created dynamically, so we need to find it dynamically
            //It's cached and the title screen so the performance cost is fine
            fade = FindObjectOfType<Fade>();
        }

        //Triggered by button on click event, starts fadeout.
        public void PlayGame()
        {
            if (shouldChangeScene)
            {
                fade.FadeOut(() => SceneManager.LoadScene(levelProgression.ScenesInOrder[0]));
                shouldChangeScene = false;
            }
        }

        //Triggered by button on click event, quits game.
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}