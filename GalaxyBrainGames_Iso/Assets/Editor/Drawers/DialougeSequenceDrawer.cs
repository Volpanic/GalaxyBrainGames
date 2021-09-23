using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GalaxyBrain.UI;

namespace GalaxyBrainEditor.Drawers
{
    [CustomPropertyDrawer(typeof(DialougeSequence))]
    public class DialougeSequenceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propertyHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded) propertyHeight += EditorGUIUtility.singleLineHeight*7;

            return propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty diaText       = property.FindPropertyRelative("DialougeText");
            SerializedProperty portrait      = property.FindPropertyRelative("SpeakerPortrait");
            SerializedProperty normalPortPos = property.FindPropertyRelative("NormalizedPortraitPosition");

            position.height = EditorGUIUtility.singleLineHeight;

            string foldoutName = (!property.isExpanded) ? diaText.stringValue : diaText.displayName;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, foldoutName);

            if(property.isExpanded)
            {
                //Portrait Sprite
                GUI.enabled = false;
                position.y += position.height;
                EditorGUI.PropertyField(position,portrait);

                //Portrait Pos
                position.y += position.height;
                EditorGUI.LabelField(position,"Portrait Position on text box",EditorStyles.centeredGreyMiniLabel);
                position.y += position.height;
                normalPortPos.floatValue = GUI.HorizontalSlider(position,normalPortPos.floatValue,0f,1f);
                GUI.enabled = true;

                //Text box
                position.y += position.height;
                position.height *= 3;
                diaText.stringValue = EditorGUI.TextArea(position, diaText.stringValue);
            }
        }
    }
}
