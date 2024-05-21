using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class ColourHarmonySettings : ScriptableSingleton<ColourHarmonySettings>
{
    public int UpdateWheelPerSecond
    {
        set
        {
            if (value <= 0) Debug.LogError("UpdateWheelPerSecond cannot be zero or negative");
            UpdateWheelPerSecond = value;
            DesiredDelta = 1f / UpdateWheelPerSecond;
        }
        get => UpdateWheelPerSecond;
    }

    public float DesiredDelta { get; private set; }
}
