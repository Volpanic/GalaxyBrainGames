using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Fade fadeManager;
        [SerializeField, Min(0)] int targetScene = 1;
        [SerializeField] GameEvent changeScene;

        private bool shouldChangeScene = true;

        //Triggered by button on click event, starts fadeout.
        public void PlayGame()
        {
            if (shouldChangeScene)
            {
                changeScene?.Raise();
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