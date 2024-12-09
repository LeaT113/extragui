using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExtraGUIs.Editor
{
    public static class ExtraGUI
    {
        private static readonly GUIContent ScriptableObjectSettingsFieldNewLabel = new ("New", "Create a a default instance.");
        private static readonly GUIContent ScriptableObjectSettingsFieldCloneLabel = new ("Clone", "Clone the current instance.");
        private static readonly GUIContent EditFolderIcon = new () { image = EditorGUIUtility.IconContent("d_FolderOpened Icon").image, tooltip = "Change path" };
        // TODO Create method that iterates through properties using Next() and:
        // 1. Draws fields with names from a list
        // 2. Draws fields with names excluding a list
        // 3. Draws fields until it encounters a field name


        // Fields
        public static string AssetFolderPathField(Rect rect, GUIContent label, string value)
        {
            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.LabelField(labelRect, label);


            var contentRect = EditorGUI.PrefixLabel(rect, label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var changeButtonWidth = contentRect.height;

            EditorGUI.LabelField(contentRect, GUIContent.none, EditorStyles.helpBox);

            var pathTextRect = new Rect(contentRect.x + 2f, contentRect.y, contentRect.width - 2f - changeButtonWidth - 2f, contentRect.height);
            if (GUI.Button(pathTextRect, value, EditorStyles.label))
            {
                EditorUtility.FocusProjectWindow();

                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(value);

                Selection.activeObject = obj;
            }

            var changeButtonRect = new Rect(contentRect.xMax - changeButtonWidth - 2f, contentRect.y, changeButtonWidth, rect.height);
            if (GUI.Button(changeButtonRect, EditFolderIcon, EditorStyles.iconButton))
            {
                var newPath = EditorUtility.SaveFolderPanel("Select output folder", value, "");

                if (!string.IsNullOrEmpty(newPath))
                {
                    if (newPath.StartsWith(Application.dataPath))
                    {
                        newPath = "Assets" + newPath[Application.dataPath.Length..];
                        AssetDatabase.Refresh();

                        if (AssetDatabase.IsValidFolder(newPath))
                            value = newPath;
                        else
                        {
                            EditorUtility.DisplayDialog("Non-existent path", $"'{newPath}' does not exist.", "Ok");
                        }
                    }
                    else
                        EditorUtility.DisplayDialog("Invalid path", $"'{value}' is not a valid path in this project.", "Ok");
                }
            }

            EditorGUI.indentLevel = indent;
            return value;
        }

        public static T ScriptableObjectSettingsField<T>(Rect rect, GUIContent label, T value) where T : ScriptableObject
        {
            const float buttonWidth = 50f;
            const float objectButtonGap = 2f;

            var objectFieldRect = new Rect(rect.x, rect.y, rect.width - (buttonWidth * 2f + 5f), rect.height);
            var newSO = EditorGUI.ObjectField(objectFieldRect, label, value, typeof(T), false) as T;

            var newButtonRect = new Rect(objectFieldRect.xMax + objectButtonGap, rect.y, buttonWidth, rect.height);
            if (GUI.Button(newButtonRect, ScriptableObjectSettingsFieldNewLabel, EditorStyles.miniButtonLeft))
            {
                newSO = ScriptableObject.CreateInstance<T>();
                var newPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{value.GetType().Name}.asset");

                AssetDatabase.CreateAsset(newSO, newPath);
                AssetDatabase.SaveAssets();
            }

            EditorGUI.BeginDisabledGroup(!value);
            var cloneButtonRect = new Rect(newButtonRect.xMax, rect.y, buttonWidth, rect.height);
            if (GUI.Button(cloneButtonRect, ScriptableObjectSettingsFieldCloneLabel, EditorStyles.miniButtonRight))
            {
                newSO = Object.Instantiate(value);
                var assetPath = AssetDatabase.GetAssetPath(value);
                var newPath = !assetPath.StartsWith("Assets") ? 
                        AssetDatabase.GenerateUniqueAssetPath($"Assets/My{System.IO.Path.GetFileName(assetPath)}") :
                        AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(value));

                AssetDatabase.CreateAsset(newSO, newPath);
                AssetDatabase.SaveAssets();
            }
            EditorGUI.EndDisabledGroup();

            return newSO;
        }
        
        public static void ExactMinMaxSlider(Rect rect, GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, float sliderWidthRatio = 0.7f)
        {
            const float gap = 2;
            var floatFieldWidth = (1f - sliderWidthRatio) / 2;
        
            var contentRect = EditorGUI.PrefixLabel(rect, label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            var minFieldRect = new Rect(contentRect.x, contentRect.y, contentRect.width * floatFieldWidth - gap, contentRect.height);
            var sliderRect = new Rect(minFieldRect.xMax + gap, contentRect.y, contentRect.width * sliderWidthRatio, contentRect.height);
            var maxFieldRect = new Rect(sliderRect.xMax + gap, contentRect.y, contentRect.width * floatFieldWidth - gap, contentRect.height);
            
            minValue = EditorGUI.FloatField(minFieldRect, minValue);
            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, minLimit, maxLimit);
            maxValue = EditorGUI.FloatField(maxFieldRect, maxValue);
        
            minValue = Mathf.Max(minValue, minLimit);
            maxValue = Mathf.Min(maxValue, maxLimit);
            
            EditorGUI.indentLevel = indent;
        }


        // Tools
        public static float GetPropertyHeights(SerializedProperty property, params string[] fields)
        {
            return fields.Sum(field => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(field), true) + EditorGUIUtility.standardVerticalSpacing);
        }

        public static void PropertyFields(ref Rect rect, SerializedProperty parentProperty, params (string name, GUIContent label)[] fields)
        {
            foreach (var field in fields)
            {
                var prop = parentProperty.FindPropertyRelative(field.name);
                EditorGUI.PropertyField(rect, prop, field.label, true);
                rect.y += EditorGUI.GetPropertyHeight(prop) + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public static void ConditionalPropertyFields(ref Rect rect, SerializedProperty parentProperty, (string name, GUIContent label) boolField,
            params (string name, GUIContent label)[] fields)
        {
            var boolProp = parentProperty.FindPropertyRelative(boolField.name);
            EditorGUI.PropertyField(rect, boolProp, boolField.label, true);
            rect.y += EditorGUI.GetPropertyHeight(boolProp) + EditorGUIUtility.standardVerticalSpacing;

            if (boolProp.boolValue)
            {
                EditorGUI.indentLevel++;
                PropertyFields(ref rect, parentProperty, fields);
                EditorGUI.indentLevel--;
            }
        }
    }
}