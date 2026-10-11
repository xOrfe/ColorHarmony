using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    /// <summary>Re-resolves the property on every operation, including after Undo and array changes.</summary>
    internal sealed class GradientPropertySession
    {
        private readonly UnityEngine.Object target;
        private readonly string path;
        public GradientPropertySession(UnityEngine.Object target, string path) { this.target = target; this.path = path; }
        public GradientX Read()
        {
            if (!target) return null;
            using (var serialized = new SerializedObject(target)) return GradientSerialization.Read(serialized.FindProperty(path));
        }
        public void Write(GradientX model)
        {
            if (!target) return;
            using (var serialized = new SerializedObject(target))
            {
                SerializedProperty property = serialized.FindProperty(path);
                if (property == null) return;
                GradientSerialization.Write(property, model);
                serialized.ApplyModifiedProperties();
            }
        }
    }
}


