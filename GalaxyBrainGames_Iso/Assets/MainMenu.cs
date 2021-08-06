using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Fade fadeManager;

    private bool shouldChangeScene;

    private void Update()
    {
        if(shouldChangeScene && fadeManager.FadeDone)
        {
            SceneManager.LoadScene(1);
        }
    }

    //Triggered by button on click event, starts fadeout.
    public void PlayGame()
    {
        if(fadeManager.FadeDone)
        {
            fadeManager.FadeOut();
            shouldChangeScene = true;
        }
    }

    //Triggered by button on click event, quits game.
    public void ExitGame()
    {
        Application.Quit();
    }
}
