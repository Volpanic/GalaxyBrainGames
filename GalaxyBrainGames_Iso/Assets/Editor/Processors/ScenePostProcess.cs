using GalaxyBrain.Creatures;
using GalaxyBrain.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrainEditor.Processor
{
    public class ScenePostProcess
    {
        [PostProcessSceneAttribute(2)]
        public static void OnPostprocessScene()
        {
            string[] levelProgressionAssets = AssetDatabase.FindAssets("t:LevelProgression");
            if (levelProgressionAssets.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(levelProgressionAssets[0]);
                LevelProgression levelProgression = AssetDatabase.LoadAssetAtPath<LevelProgression>(assetPath);

                //Get action point manager in current scene
                ActionPointManager apManager = Object.FindObjectOfType<ActionPointManager>();

                //If it exsists, means it's a gameplay scene
                if (levelProgression && apManager != null)
                {
                    var currentScene = SceneManager.GetActiveScene();

                    //Make sure were running in a scene that's in the final build
                    if (levelProgression.ScenesInOrder.Contains(currentScene.name))
                    {
                        int currentIndex = levelProgression.ScenesInOrder.FindIndex((x) => x == currentScene.name);

                        //Create scene info list if it doesn't exsist
                        if (levelProgression.SceneInformation == null || levelProgression.SceneInformation.Count != levelProgression.ScenesInOrder.Count)
                        {
                            levelProgression.SceneInformation = new List<SceneInfo>();
                            while (levelProgression.SceneInformation.Count != levelProgression.ScenesInOrder.Count)
                            {
                                levelProgression.SceneInformation.Add(new SceneInfo());
                            }
                        }

                        //Get current info to update
                        SceneInfo info = levelProgression.SceneInformation[currentIndex];
                        info.ActionPointsInLevel = apManager.CurrentActionPoints;
                        List<Sprite> creaturesInLevel = new List<Sprite>();

                        PlayerController[] controllers = Object.FindObjectsOfType<PlayerController>();

                        for (int i = 0; i < controllers.Length; i++)
                        {
                            creaturesInLevel.Add(controllers[i].CreatureIcon);
                        }

                        info.CreaturesInLevel = creaturesInLevel.ToArray();

                        //Check if anything is different before we edit the progression
                        SceneInfo oldInfo = levelProgression.SceneInformation[currentIndex];
                        if(oldInfo.ActionPointsInLevel != info.ActionPointsInLevel || oldInfo.CreaturesInLevel != info.CreaturesInLevel)
                        {
                            levelProgression.SceneInformation[currentIndex] = info;
                        }
                    }
                }
            }
        }
    }
}
