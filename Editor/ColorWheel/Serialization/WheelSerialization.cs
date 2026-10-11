using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    internal static class WheelSerialization
    {
        public static ColourWheel Read(SerializedProperty property)
        {
            ColourWheel source = SerializedPropertyUtility.GetManagedValue<ColourWheel>(property);
            if (source == null) return null;
            var copy = CloneForEditing(source);
            copy.Refresh();
            return copy;
        }

        public static ColourWheel CloneForEditing(ColourWheel source)
        {
            var copy = JsonUtility.FromJson<ColourWheel>(JsonUtility.ToJson(source));
            foreach (WheelPointDefinition point in copy.Points)
                if (point != null) point.onUpdate = new UnityEngine.Events.UnityEvent<Color>();
            return copy;
        }

        public static void Write(SerializedProperty property, ColourWheel wheel)
        {
            property.FindPropertyRelative("wheelName").stringValue = wheel.WheelName;
            property.FindPropertyRelative("wheelType").intValue = (int)wheel.WheelType;
            property.FindPropertyRelative("harmonyType").intValue = (int)wheel.HarmonyType;
            property.FindPropertyRelative("brightness").floatValue = wheel.Brightness;
            property.FindPropertyRelative("pointCount").intValue = wheel.PointCount;
            property.FindPropertyRelative("isPointsLocked").boolValue = wheel.IsPointsLocked;
            WriteCoordinate(property.FindPropertyRelative("anchorCoordinate"), wheel.AnchorCoordinate);
            SerializedProperty points = property.FindPropertyRelative("wheelPointDefinitions");
            points.arraySize = ColourWheel.MaxPointCount;
            for (int i = 0; i < points.arraySize; i++)
            {
                SerializedProperty point = points.GetArrayElementAtIndex(i);
                WriteCoordinate(point.FindPropertyRelative("coordinate"), wheel.Points[i].Coordinate);
                point.FindPropertyRelative("brightness").floatValue = wheel.Points[i].Brightness;
                point.FindPropertyRelative("visible").boolValue = i < wheel.PointCount;
                point.FindPropertyRelative("color").colorValue = wheel.Points[i].Color;
                // Existing UnityEvents are preserved on each serialized point.
            }
        }

        private static void WriteCoordinate(SerializedProperty property, ColorWheelCoordinate coordinate)
        {
            property.FindPropertyRelative("hue").floatValue = coordinate.Hue;
            property.FindPropertyRelative("radius").floatValue = coordinate.Radius;
        }
    }
}

