using UnityEngine;
using UnityEditor;
using qASIC;

namespace Game.Editor
{
    [CustomPropertyDrawer(typeof(ToggableValue<>))]
    public class ToggableValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawToggableValue(position, property, label);
        }

        public static void DrawToggableValue(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            Rect toggleRect = new Rect(position).SetWidth(EditorGUIUtility.singleLineHeight);
            Rect fieldRect = new Rect(position).BorderLeft(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            SerializedProperty toggleProperty = property.FindPropertyRelative("enabled");
            SerializedProperty fieldProperty = property.FindPropertyRelative("value");

            EditorGUI.PropertyField(toggleRect, toggleProperty, GUIContent.none);

            using (new EditorGUI.DisabledGroupScope(!toggleProperty.boolValue))
            {
                EditorGUI.PropertyField(fieldRect, fieldProperty, GUIContent.none);
            }
        }
    }
}