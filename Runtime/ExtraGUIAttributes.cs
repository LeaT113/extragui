using UnityEngine;

namespace ExtraGUIs.Editor
{
    public class UseExtraGUIDrawer : PropertyAttribute
    { }

    // TODO Maybe [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ContainsReferences : PropertyAttribute
    {
        public ContainsReferences() : base(true)
        { }
    }
    
    public class ReferenceList : PropertyAttribute
    {
        public ReferenceList() : base(true)
        { }
    }
}