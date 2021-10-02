using GalaxyBrain.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.Systems
{
    [CreateAssetMenu]
    public class LevelProgression : ScriptableObject
    {
        [Scene]
        public List<string> ScenesInOrder;

        public GameObject SceneTransition;

        private Transform canvasRoot;

        private void OnEnable()
        {
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

            if (canvasRoot != null && SceneTransition != null)
            {
                Instantiate(SceneTransition, canvasRoot);
            }
        }

        public void NewGame()
        {

        }

        public string GetNextScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;

            for(int i = 1; i < ScenesInOrder.Count; i++)
            {
                if(ScenesInOrder[i-1] == currentScene)
                {
                    return ScenesInOrder[i];
                }
            }

            return SceneManager.GetSceneByBuildIndex(0).name;
        }
    }
}