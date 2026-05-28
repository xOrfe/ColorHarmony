using System;
using UnityEngine;
using UnityEngine.Events;

namespace XO.ColorHarmony
{
    [Serializable]
    public class WheelPointDefinition
    {
        [SerializeField] private ColorWheelCoordinate coordinate = new ColorWheelCoordinate(0f, 1f);
        [SerializeField, Range(0f, 1f)] private float brightness = 1f;
        [SerializeField] private bool visible;
        [SerializeField] private Color color = Color.white;

        public UnityEvent<Color> onUpdate = new UnityEvent<Color>();

        public ColorWheelCoordinate Coordinate => coordinate;
        public ColorWheelCoordinate WheelCoordinate => coordinate;
        public float Brightness => brightness;
        public bool Visible => visible;
        public Color Color => color;

        public void SetCoordinate(ColorWheelCoordinate value, WheelType wheelType, bool invoke = true)
        {
            coordinate = value;
            Evaluate(wheelType, invoke);
        }

        public void SetBrightness(float value, WheelType wheelType, bool invoke = true)
        {
            brightness = Mathf.Clamp01(value);
            Evaluate(wheelType, invoke);
        }

        public void SetBrightness(float value, bool invoke = true)
        {
            brightness = Mathf.Clamp01(value);
            if (invoke)
            {
                onUpdate?.Invoke(color);
            }
        }

        public void SetVisible(bool value)
        {
            visible = value;
        }

        public void Evaluate(WheelType wheelType, bool invoke = true)
        {
            color = ColorWheelUtility.Evaluate(wheelType, coordinate, brightness);
            if (invoke)
            {
                onUpdate?.Invoke(color);
            }
        }

        public void Clear(WheelType wheelType)
        {
            coordinate = new ColorWheelCoordinate(0f, 1f);
            brightness = 1f;
            visible = false;
            Evaluate(wheelType, false);
        }
    }
}
