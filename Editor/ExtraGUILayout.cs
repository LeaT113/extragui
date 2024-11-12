using UnityEditor;
using UnityEngine;

namespace ExtraGUIs.Editor
{
    public static class ExtraGUILayout
    {
        // TODO Functions here should just be wrappers for ExtraGUI functions that take a Rect. The rect in Layout should be obtained by calling EditorGUILayout.GetControlRect();
        
        public static void IndentedLabel(string text, GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
            GUILayout.Label(text, style);
            GUILayout.EndHorizontal();
        }
        
        public static bool FoldoutHeader(bool display, string title)
        {
            const float verticalPadding = 2f;
            Color backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            
            var rect = GUILayoutUtility.GetRect(16f, EditorGUIUtility.singleLineHeight + verticalPadding, GUIStyle.none);

            EditorGUI.DrawRect(rect, backgroundColor);
            
            Color topLineColor = new Color(0.1f, 0.1f, 0.1f);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), topLineColor);

            Color bottomLineColor = new Color(0.2f, 0.2f, 0.2f);
            if (display)
                EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), bottomLineColor);    
            
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fixedHeight = EditorGUIUtility.singleLineHeight + verticalPadding,
                contentOffset = new Vector2(2, 0),
            };

            Rect foldoutRect = new Rect(rect.x + 5, rect.y, rect.width - 2, rect.height);
            display = EditorGUI.Foldout(foldoutRect, display, title, true, foldoutStyle);
            
            return display;
        }

        // Fields
        public static string AssetFolderPathField(GUIContent label, string value)
        {
            var rect = EditorGUILayout.GetControlRect();
            return ExtraGUI.AssetFolderPathField(rect, label, value);
        }

        public static T ScriptableObjectSettingsField<T>(GUIContent label, T value) where T : ScriptableObject
        {
            var rect = EditorGUILayout.GetControlRect();
            return ExtraGUI.ScriptableObjectSettingsField(rect, label, value);
        }
    }
}