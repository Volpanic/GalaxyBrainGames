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
            SerializedProperty endOfLineEvent = property.FindPropertyRelative("EndOfLineEvent");

            if (property.isExpanded)
            {
                propertyHeight += EditorGUIUtility.singleLineHeight * 7;

                if(endOfLineEvent.isExpanded)
                {
                    propertyHeight += EditorGUIUtility.singleLineHeight * 5.5f;
                }
            }

            return propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty diaText        = property.FindPropertyRelative("DialougeText");
            SerializedProperty portrait       = property.FindPropertyRelative("SpeakerPortrait");
            SerializedProperty normalPortPos  = property.FindPropertyRelative("NormalizedPortraitPosition");
            SerializedProperty endOfLineEvent = property.FindPropertyRelative("EndOfLineEvent");

            position.height = EditorGUIUtility.singleLineHeight;

            string foldoutName = (!property.isExpanded) ? diaText.stringValue : diaText.displayName;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, foldoutName);

            if(property.isExpanded)
            {
                //Portrait Sprite
                position.y += position.height;
                EditorGUI.PropertyField(position,portrait);

                //Portrait Pos
                position.y += position.height;
                EditorGUI.LabelField(position,"Portrait Position on text box",EditorStyles.centeredGreyMiniLabel);
                position.y += position.height;
                normalPortPos.floatValue = Mathf.Round(GUI.HorizontalSlider(position,normalPortPos.floatValue,0f,1f) * 10f) / 10f;

                //Text box
                position.y += position.height;
                position.height *= 3;
                diaText.stringValue = EditorGUI.TextArea(position, diaText.stringValue);

                //Event foldout
                position.height /= 3;
                position.y += position.height * 3;
                EditorGUI.indentLevel = 1;
                endOfLineEvent.isExpanded = EditorGUI.Foldout(EditorGUI.IndentedRect(position), endOfLineEvent.isExpanded, "End of Line Event");

                position.y += position.height * 1;

                if (endOfLineEvent.isExpanded)
                {
                    EditorGUI.PropertyField(EditorGUI.IndentedRect(position), endOfLineEvent);
                }
                EditorGUI.indentLevel = 0;
            }
        }
    }
}
