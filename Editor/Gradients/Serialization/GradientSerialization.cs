using System;
using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    internal static class GradientSerialization
    {
        public static GradientX Read(SerializedProperty property)
        {
            if (property == null) return null;
            var model = new GradientX
            {
                InterpolationColorSpace = (ColorSpaceType)property.FindPropertyRelative("interpolationColorSpace").intValue,
                GradientType = (GradientType)property.FindPropertyRelative("gradientType").intValue,
                ShortestHue = property.FindPropertyRelative("shortestHue")?.boolValue ?? false
            };
            SerializedProperty colors = property.FindPropertyRelative("colorKeys");
            model.ColorKeys = new ColorKey[colors.arraySize];
            for (int i = 0; i < colors.arraySize; i++)
            {
                SerializedProperty key = colors.GetArrayElementAtIndex(i);
                model.ColorKeys[i] = new ColorKey(key.FindPropertyRelative("time").floatValue, key.FindPropertyRelative("color").colorValue, key.FindPropertyRelative("brightness").floatValue);
            }
            SerializedProperty alphas = property.FindPropertyRelative("alphaKeys");
            model.AlphaKeys = new AlphaKey[alphas.arraySize];
            for (int i = 0; i < alphas.arraySize; i++)
            {
                SerializedProperty key = alphas.GetArrayElementAtIndex(i);
                model.AlphaKeys[i] = new AlphaKey(key.FindPropertyRelative("time").floatValue, key.FindPropertyRelative("alpha").floatValue);
            }
            return model;
        }

        public static void Write(SerializedProperty property, GradientX model)
        {
            property.FindPropertyRelative("interpolationColorSpace").intValue = (int)model.InterpolationColorSpace;
            property.FindPropertyRelative("gradientType").intValue = (int)model.GradientType;
            property.FindPropertyRelative("shortestHue").boolValue = model.ShortestHue;
            SerializedProperty colors = property.FindPropertyRelative("colorKeys");
            colors.arraySize = model.ColorKeys.Length;
            for (int i = 0; i < colors.arraySize; i++)
            {
                SerializedProperty key = colors.GetArrayElementAtIndex(i);
                key.FindPropertyRelative("time").floatValue = model.ColorKeys[i].Time();
                key.FindPropertyRelative("color").colorValue = model.ColorKeys[i].Color();
                key.FindPropertyRelative("brightness").floatValue = model.ColorKeys[i].Brightness();
            }
            SerializedProperty alphas = property.FindPropertyRelative("alphaKeys");
            alphas.arraySize = model.AlphaKeys.Length;
            for (int i = 0; i < alphas.arraySize; i++)
            {
                SerializedProperty key = alphas.GetArrayElementAtIndex(i);
                key.FindPropertyRelative("time").floatValue = model.AlphaKeys[i].Time();
                key.FindPropertyRelative("alpha").floatValue = model.AlphaKeys[i].Alpha();
            }
        }
    }

}


