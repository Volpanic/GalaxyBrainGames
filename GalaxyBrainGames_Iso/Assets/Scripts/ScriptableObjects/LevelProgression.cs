using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class LevelProgression : ScriptableObject
{
    public List<int> ScenesInOrder;

    public GameObject SceneTransition;

    private int currentScene = -1;
    private Transform canvasRoot;

    private void OnEnable()
    {
        currentScene = 0;
        SceneManager.sceneLoaded += FindCanvasRoot;

        if (Application.isPlaying)
        {
            FindCanvasRoot(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    private void FindCanvasRoot(Scene arg0, LoadSceneMode arg1)
    {
        canvasRoot = null;
        canvasRoot = FindObjectOfType<Canvas>()?.transform;

        if(canvasRoot != null && SceneTransition != null)
        {
            Instantiate(SceneTransition,canvasRoot);
        }
    }

    public void NewGame()
    {
        currentScene = 0;
    }

    public int GetCurrentScene()
    {
        if (currentScene < 0) currentScene = 0;
        return ScenesInOrder[currentScene];
    }

    public void SetCurrentScene(int index)
    {
        currentScene = index;
    }

    public void IncrementCurrentScene()
    {
        currentScene++;
        if(currentScene >= ScenesInOrder.Count)
        {
            currentScene = 0;
        }
    }
}
