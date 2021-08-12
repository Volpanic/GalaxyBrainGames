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

    private int currentScene = 0;
    private Transform canvasRoot;

    private void OnEnable()
    {
        currentScene = 0;
        SceneManager.sceneLoaded += FindCanvasRoot;
        FindCanvasRoot(SceneManager.GetActiveScene(), LoadSceneMode.Single);
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
        return ScenesInOrder[currentScene];
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
