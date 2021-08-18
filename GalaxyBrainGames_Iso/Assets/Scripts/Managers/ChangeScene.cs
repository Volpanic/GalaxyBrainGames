using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Fade fade;
    [SerializeField] private LevelProgression levelProg;

    private int targetIndex = 0;

    public void Changescene()
    {
        if (fade.FadeDone)
        {
            fade.FadeOut(ChangeSceneAfterFade);
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
        targetIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(targetIndex);
    }

    public void ResetScene()
    {
        if (fade.FadeDone)
        {
            fade.FadeOut(ResetSceneAfterFade);
        }
    }
}
