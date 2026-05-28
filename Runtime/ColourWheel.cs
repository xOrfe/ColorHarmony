using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    [Serializable]
    public class ColourWheel
    {
        public const int MaxPointCount = 8;

        [SerializeField] private string wheelName = "New Wheel";
        [SerializeField] private WheelType wheelType = WheelType.Standard;
        [SerializeField] private HarmonyType harmonyType = HarmonyType.Custom;
        [SerializeField, Range(0f, 1f)] private float brightness = 1f;
        [SerializeField] private int pointCount = 1;
        [SerializeField] private bool isPointsLocked = true;
        [SerializeField] private ColorWheelCoordinate anchorCoordinate = new ColorWheelCoordinate(0f, 1f);
        [SerializeField] private List<WheelPointDefinition> wheelPointDefinitions = new List<WheelPointDefinition>(MaxPointCount);

        public string WheelName
        {
            get => wheelName;
            set => wheelName = value;
        }

        public WheelType WheelType => wheelType;
        public HarmonyType HarmonyType => harmonyType;
        public float Brightness => brightness;
        public int PointCount => Mathf.Clamp(pointCount, 0, MaxPointCount);
        public bool IsPointsLocked => isPointsLocked;
        public ColorWheelCoordinate AnchorCoordinate => anchorCoordinate;
        public IReadOnlyList<WheelPointDefinition> Points => wheelPointDefinitions;

        public ColourWheel()
        {
            EnsurePoints();
            SetPointCount(1);
            Refresh();
        }

        public void SetWheel(WheelType value)
        {
            wheelType = value;
            Refresh();
        }

        public void SetHarmony(HarmonyType value)
        {
            harmonyType = value;
            ColorHarmonyUtility.Apply(this, harmonyType);
        }

        public void SetBrightness(float value)
        {
            brightness = Mathf.Clamp01(value);
            for (int i = 0; i < PointCount; i++)
            {
                wheelPointDefinitions[i].SetBrightness(brightness, wheelType, false);
            }

            Refresh();
        }

        public void SetPointsLocked(bool value)
        {
            isPointsLocked = value;
        }

        public void MovePoint(int index, ColorWheelCoordinate coordinate)
        {
            EnsurePoints();
            if (index < 0 || index >= PointCount)
            {
                return;
            }

            if (isPointsLocked)
            {
                anchorCoordinate = coordinate;
                ColorHarmonyUtility.Apply(this, harmonyType);
                return;
            }

            SetPointCoordinate(index, coordinate);
        }

        public void AddPoint()
        {
            SetPointCount(PointCount + 1);
            int index = PointCount - 1;
            SetPointCoordinate(index, anchorCoordinate.Offset(index / (float)Mathf.Max(1, PointCount)));
        }

        public void RemovePoint()
        {
            SetPointCount(PointCount - 1);
        }

        public void SetPointCount(int value)
        {
            EnsurePoints();
            pointCount = Mathf.Clamp(value, 1, MaxPointCount);
            for (int i = 0; i < wheelPointDefinitions.Count; i++)
            {
                wheelPointDefinitions[i].SetVisible(i < pointCount);
                if (i >= pointCount)
                {
                    wheelPointDefinitions[i].Clear(wheelType);
                }
            }
        }

        public void SetPointCoordinate(int index, ColorWheelCoordinate coordinate, bool invoke = true)
        {
            EnsurePoints();
            if (index < 0 || index >= MaxPointCount)
            {
                return;
            }

            wheelPointDefinitions[index].SetCoordinate(coordinate, wheelType, invoke);
        }

        public void Refresh()
        {
            EnsurePoints();
            for (int i = 0; i < PointCount; i++)
            {
                wheelPointDefinitions[i].Evaluate(wheelType);
            }
        }

        private void EnsurePoints()
        {
            wheelPointDefinitions ??= new List<WheelPointDefinition>(MaxPointCount);
            while (wheelPointDefinitions.Count < MaxPointCount)
            {
                wheelPointDefinitions.Add(new WheelPointDefinition());
            }

            if (pointCount <= 0)
            {
                pointCount = 1;
            }
        }
    }
}
