using GalaxyBrain.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyBrainEditor.Attributes
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class ScenePropertyDrawer : PropertyDrawer
    {
        private const float VERTICAL_PADDING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + (VERTICAL_PADDING*2.5f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                string[] oldScene = AssetDatabase.FindAssets(property.stringValue);

                if (oldScene.Length != 0)
                {
                    SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(oldScene[0]));

                    Rect topRect = position;
                    topRect.height  = EditorGUIUtility.singleLineHeight;
                    topRect.y += VERTICAL_PADDING;

                    SceneAsset targetScene = (SceneAsset)EditorGUI.ObjectField(topRect, property.displayName, scene, typeof(SceneAsset), false);

                    if (scene != targetScene && targetScene != null)
                    {
                        property.stringValue = targetScene.name;
                    }

                    topRect.y += EditorGUIUtility.singleLineHeight + (VERTICAL_PADDING*0.5f);

                    //Check for build index
                    if (!SceneInBuildSettings(targetScene))
                    {
                        if (GUI.Button(topRect, new GUIContent("Add to Build", "Adds the scene to the build settings")))
                        {
                            AddSceneToBuildSettings(targetScene);
                        }
                    }
                    else 
                    {
                        if (targetScene != null)
                        {
                            GUI.Label(topRect, new GUIContent("Scene is in build settings."), EditorStyles.centeredGreyMiniLabel);
                        }
                    }
                }
            }
        }

        private void AddSceneToBuildSettings(SceneAsset scene)
        {
            if (scene == null) return;

            if(!SceneInBuildSettings(scene))
            {
                string path = AssetDatabase.GetAssetPath(scene);
                List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();

                buildScenes.Add(new EditorBuildSettingsScene(path,true));

                EditorBuildSettings.scenes = buildScenes.ToArray();
               
            }
        }

        private bool SceneInBuildSettings(SceneAsset scene)
        {
            if (scene == null) return false;

            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
            string path = AssetDatabase.GetAssetPath(scene);

            for (int i = 0; i < buildScenes.Length; i++)
            {
                if (path == buildScenes[i].path)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
