using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ExtraGUIs.Editor.PropertyDrawers
{
    public class ExtraGUIShaderDrawer : PropertyDrawer
    {
        private Shader _pickedShader;
        private string _pickedShaderPropertyPath;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var shader = property.objectReferenceValue as Shader;
            
            EditorGUI.BeginProperty(position, label, property);

            bool wasEnabled = GUI.enabled;
            position = EditorGUI.PrefixLabel(position, label);

            if (EditorGUI.DropdownButton(position, new GUIContent(shader?.name ?? "No Shader Selected"), FocusType.Keyboard, EditorStyles.miniPullDown))
            {
                var dropdown = new ShaderSelectionDropdown(s =>
                {
                    _pickedShader = s;
                    _pickedShaderPropertyPath = property.propertyPath;
                });
                dropdown.Show(position);
                GUIUtility.ExitGUI();
            }

            if (_pickedShader && property.propertyPath == _pickedShaderPropertyPath)
            {
                property.objectReferenceValue = _pickedShader;
                _pickedShader = null;
            }
            
            GUI.enabled = wasEnabled;
            
            EditorGUI.EndProperty();
        }
    }
    
    public class ShaderSelectionDropdown : AdvancedDropdown
    {
        private readonly Action<Shader> _onShaderSelected;

        // TODO Better search (match native) + "None" option

        public ShaderSelectionDropdown(Action<Shader> onShaderSelected)
            : base(new AdvancedDropdownState())
        {
            minimumSize = new Vector2(270, 308);
            _onShaderSelected = onShaderSelected;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Shaders");
            
            var shaders = AssetDatabase.FindAssets("t:Shader")
                .Select(g => AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(g)))
                .Where(s => s)
                .OrderBy(s => s.name);
            // TODO shaders.AddRange(GetBuiltinShaders());

            foreach (var shader in shaders)
                AddShaderToTree(root, shader.name.Split('/'), shader);

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is ShaderDropdownItem shaderItem)
                _onShaderSelected(shaderItem.Shader);
        }

        private static void AddShaderToTree(AdvancedDropdownItem parent, string[] path, Shader shader)
        {
            if (path.Length == 0)
                return;
            
            // Shader itself
            if (path.Length == 1)
            {
                parent.AddChild(new ShaderDropdownItem(path[0], shader));
                return;
            }
            
            // Group
            var groupName = path[0];
            var groupItem = parent.children.FirstOrDefault(c => c.name == groupName);
            if (groupItem == null)
            {
                groupItem = new AdvancedDropdownItem(groupName);
                parent.AddChild(groupItem);
            }
            
            AddShaderToTree(groupItem, path.Skip(1).ToArray(), shader);
        }
    }

    
    internal class ShaderDropdownItem : AdvancedDropdownItem
    {
        public Shader Shader { get; private set; }

        public ShaderDropdownItem(string name, Shader shader) : base(name)
        {
            Shader = shader;
        }
    }
}