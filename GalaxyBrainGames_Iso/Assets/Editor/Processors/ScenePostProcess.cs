using GalaxyBrain.Creatures;
using GalaxyBrain.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrainEditor.Processor
{
    public class ScenePostProcess
    {
        private const int screenshot_width = 1280;
        private const int screenshot_height = 720;

        [PostProcessSceneAttribute(2)]
        public static void OnPostprocessScene()
        {
            string[] levelProgressionAssets = AssetDatabase.FindAssets("t:LevelProgression");
            if (levelProgressionAssets.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(levelProgressionAssets[0]);
                LevelProgression levelProgression = AssetDatabase.LoadAssetAtPath<LevelProgression>(assetPath);

                //Get action point manager in current scene
                ActionPointManager apManager = UnityEngine.Object.FindObjectOfType<ActionPointManager>();

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

                        //Create scene info list if it doesn't exsist
                        if (levelProgression.SceneScreenShots == null || levelProgression.SceneScreenShots.Count != levelProgression.ScenesInOrder.Count)
                        {
                            levelProgression.SceneScreenShots = new List<Texture2D>();
                            while (levelProgression.SceneScreenShots.Count != levelProgression.ScenesInOrder.Count)
                            {
                                levelProgression.SceneScreenShots.Add(new Texture2D(2,2,TextureFormat.RGB24,false));
                            }
                        }


                        //Get current info to update
                        SceneInfo info = levelProgression.SceneInformation[currentIndex];
                        info.ActionPointsInLevel = apManager.CurrentActionPoints;
                        List<Sprite> creaturesInLevel = new List<Sprite>();

                        PlayerController[] controllers = UnityEngine.Object.FindObjectsOfType<PlayerController>();


                        for (int i = 0; i < controllers.Length; i++)
                        {
                            creaturesInLevel.Add(controllers[i].CreatureIcon);
                        }

                        info.CreaturesInLevel = creaturesInLevel.ToArray();

                        TakeLevelScreenshot(levelProgression, currentIndex,currentScene, UnityEngine.Object.FindObjectOfType<Camera>());

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

        private static void TakeLevelScreenshot(LevelProgression progression,int currentIndex,Scene currentScene, Camera camera)
        {
            RenderTexture pictureTex = new RenderTexture(screenshot_width, screenshot_height,0);
            RenderTexture oldTarget = camera.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            camera.targetTexture = pictureTex;
            camera.Render();
            RenderTexture.active = pictureTex;


            Texture2D screenshot = new Texture2D(screenshot_width, screenshot_height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, screenshot_width, screenshot_height), 0, 0);
            camera.targetTexture = oldTarget;
            RenderTexture.active = oldActive;

            GameObject.DestroyImmediate(pictureTex);

            string screenShotFolderPath = Path.Combine(Application.dataPath, "Data", "LevelScreenShots");
            CreateScreenShotFolder(screenShotFolderPath);

            byte[] imageData = screenshot.EncodeToJPG();
            string filename = $"SH_{currentScene.name}.jpg";
            string filePath = Path.Combine(screenShotFolderPath, filename);


            if (!FilesAreTheSame(filePath, screenshot))
            {
                File.WriteAllBytes(Path.Combine(screenShotFolderPath, filename), imageData);
                AssetDatabase.Refresh();

                progression.SceneScreenShots[currentIndex] = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine("Assets", "Data", "LevelScreenShots", filename));
            }
            else
            {
                if(progression.SceneScreenShots[currentIndex] == null)
                {
                    progression.SceneScreenShots[currentIndex] = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine("Assets", "Data", "LevelScreenShots", filename));
                }
            }
        }

        private static bool FilesAreTheSame(string path, Texture2D newTex)
        {
            if(!File.Exists(path))
            {
                return false;
            }

            byte[] imageData = File.ReadAllBytes(path);
            Texture2D oldTex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            oldTex.LoadImage(imageData);

            return newTex.GetPixels().ToString() == oldTex.GetPixels().ToString();
        }

        private static void CreateScreenShotFolder(string screenShotFolderPath)
        {
            if(!Directory.Exists(screenShotFolderPath))
            {
                Directory.CreateDirectory(screenShotFolderPath);
            }
        }
    }
}
