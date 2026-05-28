using System;
using System.Reflection;
using UnityEditor;

namespace XO.ColorHarmony
{
    internal static class SerializedPropertyUtility
    {
        public static T GetManagedValue<T>(SerializedProperty property) where T : class
        {
            object current = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");

            foreach (string element in path.Split('.'))
            {
                if (current == null)
                {
                    return null;
                }

                int bracketIndex = element.IndexOf("[", StringComparison.Ordinal);
                if (bracketIndex >= 0)
                {
                    string fieldName = element.Substring(0, bracketIndex);
                    int index = int.Parse(element.Substring(bracketIndex + 1, element.Length - bracketIndex - 2));
                    current = GetFieldValue(current, fieldName);
                    current = ((System.Collections.IList)current)?[index];
                }
                else
                {
                    current = GetFieldValue(current, element);
                }
            }

            return current as T;
        }

        private static object GetFieldValue(object source, string fieldName)
        {
            Type type = source.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
