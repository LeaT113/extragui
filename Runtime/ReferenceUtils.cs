using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExtraGUIs.Editor;
using UnityEngine;

namespace ExtraGUIs
{
    public static class ReferenceUtils
    {
        public static object DeepCopyObject(object original)
        {
            if (original == null)
                return null;

            var json = JsonUtility.ToJson(original);
            return JsonUtility.FromJson(json, original.GetType());
        }
        
        public static Type GetDefaultConcreteType(Type baseType)
        {
            if (baseType == null)
                return null;

            // Base type is already concrete
            if (!baseType.IsAbstract && !baseType.IsInterface)
                return baseType;
            
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .FirstOrDefault(type => baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);
        }

        public static void ValidateReferences(object obj)
        {
            DeduplicateAndInitializeReferences(obj, new Dictionary<int,object>());
        }
        
        
        
        // Private
        private static object DeduplicateAndInitializeReferences(object obj, Dictionary<int, object> visited)
        {
            if (obj == null)
                return null;

            var type = obj.GetType();
            if (type == typeof(string) || type is { IsPrimitive: true } or { IsEnum: true })
                return obj;

            // Deduplicate based on hash
            var objectHash = RuntimeHelpers.GetHashCode(obj);
            if (visited.ContainsKey(objectHash))
            {
                var copy = DeepCopyObject(obj);
                visited[objectHash] = copy;
                return copy;
            }

            visited[objectHash] = obj;

            // Process fields of object
            var referenceFields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsDefined(typeof(ContainsReferences), true));
            foreach (var field in referenceFields)
            {
                var fieldValue = field.GetValue(obj);

                // Initialize nulls from abstract base classes
                if (fieldValue == null) 
                {
                    fieldValue = Activator.CreateInstance(GetDefaultConcreteType(field.FieldType));
                    field.SetValue(obj, fieldValue);
                    continue;
                }

                // Process enumerable fields
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    if (fieldValue is not IList list)
                    {
                        Debug.LogError($"[ExtraGUI] Field type {field.FieldType} is not supported for reference deduplication.");
                        continue;
                    }
                    
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i] == null)
                        {
                            var baseType = GetDefaultConcreteType(field.FieldType);
                            list[i] = i > 0 && list[i - 1] != null ? DeepCopyObject(list[i - 1]) : Activator.CreateInstance(GetDefaultConcreteType(field.FieldType.GetGenericArguments
                                ().First()));
                        }
                        else
                        {
                            var newItem = DeduplicateAndInitializeReferences(list[i], visited);
                            if (!ReferenceEquals(newItem, list[i]))
                                list[i] = newItem;
                        }
                    }
                }
                else
                {
                    var newFieldValue = DeduplicateAndInitializeReferences(fieldValue, visited);
                    if (!ReferenceEquals(newFieldValue, fieldValue))
                        field.SetValue(obj, newFieldValue);
                }
            }

            return obj;
        }

    }
}