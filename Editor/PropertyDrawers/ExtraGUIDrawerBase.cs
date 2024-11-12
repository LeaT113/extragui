using UnityEditor;
using UnityEngine;

namespace ExtraGUIs.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(UseExtraGUIDrawer))]
    public class ExtraGUIDrawerBase : PropertyDrawer
    {
        private static readonly ExtraGUIShaderDrawer ExtraGUIShaderDrawer = new();
        
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue is Shader)
                return ExtraGUIShaderDrawer.GetPropertyHeight(property, label);

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.type == "PPtr<$Shader>")
            {
                ExtraGUIShaderDrawer.OnGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
                Debug.LogWarning($"[ExtraGUI] No custom drawer for {property.propertyPath}.");
            }
        }
    }
}