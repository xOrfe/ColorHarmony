using UnityEditor;
using UnityEngine;


namespace XO.ColorHarmony
{
    [CustomPropertyDrawer(typeof(WheelPointDefinition))]
    public class WheelPointPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent content = new GUIContent("ghgff");
            if (GUILayout.Button(content))
            {
                Debug.Log("adsfsd");
            }
        }
    }
}
