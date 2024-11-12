using UnityEditor;
using UnityEngine;

namespace ExtraGUIs.Editor.PropertyDrawers
{
    [CustomPropertyDrawer (typeof(OptionalProperty), true)]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var enabledProp = property.FindPropertyRelative("enabled");
            var valueProp = property.FindPropertyRelative("value");
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            const float checkboxWidth = 15f;
            const float checkboxGap = 2f;
            var checkboxRect = new Rect(position.x, position.y, checkboxWidth, position.height);
            var fieldRect = new Rect(position.x + checkboxWidth + checkboxGap, position.y, position.width - checkboxWidth - checkboxGap, position.height);
            
            enabledProp.boolValue = EditorGUI.Toggle(checkboxRect, enabledProp.boolValue);
            
            EditorGUI.BeginDisabledGroup(!enabledProp.boolValue);
            EditorGUI.PropertyField(fieldRect, valueProp, GUIContent.none);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
            
            EditorGUI.indentLevel = indent;
        }
    }
}