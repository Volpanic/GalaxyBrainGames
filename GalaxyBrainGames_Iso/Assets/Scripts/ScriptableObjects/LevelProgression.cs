using GalaxyBrain.Attributes;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrain.Systems
{
    [System.Serializable]
    public struct SceneInfo
    {
        public int ActionPointsInLevel;
        public string CreaturesInLevel;
    }

    /// <summary>
    /// A scirptable object that stores the level order of the game.
    /// Also used to determine what level comes next.
    /// </summary>
    [CreateAssetMenu]
    public class LevelProgression : ScriptableObject
    {
        [Scene]
        public List<string> ScenesInOrder;

        [ReadOnly]
        public List<SceneInfo> SceneInformation;

        public GameObject SceneTransition;

        private Transform canvasRoot;

#if UNITY_EDITOR
        private void OnValidate()
        {
            //If a scene is deleted, remove it from the level progression
            for(int i = 0; i < ScenesInOrder.Count; i++)
            {
                if(AssetDatabase.FindAssets(ScenesInOrder[i]).Length <= 0)
                {
                    ScenesInOrder.RemoveAt(i);
                    i--;
                }
            }
        }
#endif

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

            return Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(0));
        }

        [ContextMenu("Print Info")]
        public void LoadSave()
        {
            foreach(SceneInfo scene in SceneInformation)
            {
                Debug.Log(scene.ActionPointsInLevel + " : " + scene.CreaturesInLevel);
            }
        }
    }
}