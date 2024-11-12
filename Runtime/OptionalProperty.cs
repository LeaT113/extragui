using System;
using ExtraGUIs.Editor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExtraGUIs
{
    [Serializable]
    public class OptionalProperty
    {
        [FormerlySerializedAs("overridden")] public bool enabled;
    }

    [Serializable]
    public class OptionalInt : OptionalProperty
    {
        public int value;
        public int? NullableValue => enabled ? value : null;
    }
    
    [Serializable]
    public class OptionalFloat : OptionalProperty
    {
        public float value;
        public float? NullableValue => enabled ? value : null;
        
        public static implicit operator OptionalFloat(float f) => new () {value = f};
    }
    
    [Serializable]
    public class OptionalString : OptionalProperty
    {
        public string value;
        public string NullableValue => enabled ? value : null;
        
        public static implicit operator OptionalString(string v) => new () {value = v};
    }

    [Serializable]
    public class OptionalShader : OptionalProperty
    {
        [UseExtraGUIDrawer]
        public Shader value;
        public Shader NullableValue => enabled ? value : null;
        
        public static implicit operator OptionalShader(Shader v) => new () {value = v};
    }
    
    [Serializable]
    public class OptionalMaterial : OptionalProperty
    {
        public Material value;
        public Material NullableValue => enabled ? value : null;
        
        public static implicit operator OptionalMaterial(Material m) => new() { value = m };
    }
}