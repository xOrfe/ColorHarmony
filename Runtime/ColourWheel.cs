using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace XO.ColorHarmony
{
    [Serializable]
    public class ColourWheel
    {
        [Range(0, 1)] [SerializeField] private float test;

        public VisualElement Root;
        
        [SerializeField] public List<WheelPointDefinition> wheelPointDefinitions;

        private VisualElement _wheelVisualElement;
        public VisualElement WheelVisualElement
        {
            get
            {
                if (_wheelVisualElement == null)
                {
                    _wheelVisualElement = Root.Query("ColourWheel");
                }
                return _wheelVisualElement;
            }
            set => _wheelVisualElement = value;
        }

        private VisualElement _lockButtonElement;
        private VisualElement LockButtonElement
        {
            get
            {
                if (_lockButtonElement == null)
                {
                    _lockButtonElement = Root.Query("LockButton");
                }
                return _lockButtonElement;
            }
            set => _lockButtonElement = value;
        }
        
        [SerializeField] public WheelType wheelType;
        public WheelType WheelType => wheelType;

        [SerializeField] public HarmonyType harmonyType;
        public HarmonyType HarmonyType => harmonyType;
        
        private bool _isDragging;
        public bool IsDragging => _isDragging;

        [SerializeField] private float brightness;
        public float Brightness => brightness;

        [SerializeField] public int pointCount;
        public int PointCount => pointCount;

        [SerializeField] public bool isPointsLocked;
        
        private int _draggingPointIndex;

        [SerializeField] public Vector2 dynamicWheelCoordinateFactor;
        public Vector2 DynamicWheelCoordinateFactor => dynamicWheelCoordinateFactor;
        
        private bool _angleOrRadiusClock;
        
        private RenderTexture _wheelRenderTexture;
        private Texture2D _wheelTexture2D;

        public ColourWheel()
        {
            pointCount = 0;
            
            dynamicWheelCoordinateFactor = new Vector2(0,0);
            
            wheelPointDefinitions = new List<WheelPointDefinition>();
            
            _isDragging = false;
            _draggingPointIndex = -1;
            isPointsLocked = true;

            for (int i = 0; i < 8; i++)
            {
                bool vis = i <= pointCount - 1;
                WheelPointDefinition pd = new WheelPointDefinition();
                wheelPointDefinitions.Add(pd);
            }
            
        }

        private void ReSerialize()
        {
            AssetDatabase.ForceReserializeAssets();
        }
        
        public void StartDragging(int wheelIndex)
        {
            if (_isDragging)
            {
                return;
            }

            _draggingPointIndex = wheelIndex;
            _isDragging = true;
        }
        
        private void StopDragging()
        {
            _draggingPointIndex = -1;
            _isDragging = false;
        }
        
        public void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_isDragging || _draggingPointIndex < 0)
            {
                return;
            }
            
            Vector3 distanceToCenter = new Vector3(evt.localPosition.x - (float)(WheelVisualElement.resolvedStyle.width / 2f),
                evt.localPosition.y - (float)(WheelVisualElement.resolvedStyle.height / 2f), 0);
            float length = Vector3.Distance(Vector3.zero,distanceToCenter);
            
            if (length > 165)
            {
                StopDragging();
                return;
            }
            
            Vector2 newWheelCoordinate = WheelUtilities.CalculateWheelCoordinate(distanceToCenter);

            _angleOrRadiusClock = !_angleOrRadiusClock;
            
            if (isPointsLocked)
            {
                
                if(_angleOrRadiusClock) dynamicWheelCoordinateFactor.x += newWheelCoordinate.Angle() - 
                                                                              wheelPointDefinitions[_draggingPointIndex].WheelCoordinate.Angle();
                else dynamicWheelCoordinateFactor.y += newWheelCoordinate.Radius() - 
                                                             wheelPointDefinitions[_draggingPointIndex].WheelCoordinate.Radius();

                foreach (var pd in wheelPointDefinitions)
                    pd.UpdateWheelCoordinate(dynamicWheelCoordinateFactor);
            }
            else
            {
                var pd = wheelPointDefinitions[_draggingPointIndex];

                if(_angleOrRadiusClock) pd.wheelCoordinateDiffWithCoordinateFactor.x += newWheelCoordinate.Angle() - pd.WheelCoordinate.Angle();
                else pd.wheelCoordinateDiffWithCoordinateFactor.y += newWheelCoordinate.Radius() - pd.WheelCoordinate.Radius();


                pd.UpdateWheelCoordinate(dynamicWheelCoordinateFactor);
            }
        }
        
        public void OnPointerUp(PointerUpEvent evt)
        {
            StopDragging();
        }


        public void SetWheel(WheelType wheelType)
        {
            this.wheelType = wheelType;
            if (_wheelRenderTexture == null)
            {
                _wheelRenderTexture = Resources.Load<RenderTexture>("Wheel");
            }
            var tmpWheelRenderTexture = new RenderTexture(256, 256, 24);
            tmpWheelRenderTexture.enableRandomWrite = true;
            tmpWheelRenderTexture.Create();
            WheelTextureProvider.GetWheel(this.wheelType, tmpWheelRenderTexture);

            
            Graphics.Blit(tmpWheelRenderTexture, _wheelRenderTexture);
            _wheelTexture2D = _wheelRenderTexture.ToTexture2D();

            switch (this.wheelType)
            {
                case WheelType.Standard:
                    break;
                case WheelType.C:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.wheelType), this.wheelType, null);
            }

            foreach (var pd in wheelPointDefinitions)
            {
                pd.SetTexture(_wheelTexture2D);
            }
            
        }

        public void SetWheel(ChangeEvent<Enum> evt) => SetWheel((WheelType)evt.newValue);

        public void SetHarmony(HarmonyType harmonyType)
        {
            this.harmonyType = harmonyType;
            switch (this.harmonyType)
            {
                case HarmonyType.Analogus:
                    break;
                case HarmonyType.Monochromatic:
                    break;
                case HarmonyType.Triad:
                    break;
                case HarmonyType.Complementary:
                    break;
                case HarmonyType.SplitComplementary:
                    break;
                case HarmonyType.DoubleSplitComplementary:
                    break;
                case HarmonyType.Square:
                    break;
                case HarmonyType.Compound:
                    break;
                case HarmonyType.Shades:
                    break;
                case HarmonyType.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.harmonyType), this.harmonyType, null);
            }
        }

        public void SetHarmony(ChangeEvent<Enum> evt) => SetHarmony((HarmonyType)evt.newValue);

        public void UnExposePoints()
        {
            foreach (var pd in wheelPointDefinitions)
            {
                pd.ExposedData.style.display = DisplayStyle.None;
            }
        }
        
        public void OnBrightnessChange(ChangeEvent<float> evt)
        {
            brightness = evt.newValue;
        }
        
        public void OnLockButtonClicked()
        {
            isPointsLocked = !isPointsLocked;

            string path = isPointsLocked ? "locked" : "unlocked";
            StyleBackground styleBackground = new StyleBackground(Resources.Load<Sprite>("Visuals/" + path));
            
            LockButtonElement.style.backgroundImage = styleBackground;
        }

        public void OnAddPointButtonClicked()
        {
            if(pointCount == 8) return;
            WheelPointDefinition pd = wheelPointDefinitions[pointCount];
            pd.SetVisible(true);
            pointCount++;

            pd.wheelCoordinateDiffWithCoordinateFactor = dynamicWheelCoordinateFactor * -1;
            pd.UpdateWheelCoordinate(DynamicWheelCoordinateFactor);
        }
        public void OnRemovePointButtonClicked()
        {
            if(pointCount == 1) return;
            WheelPointDefinition pd = wheelPointDefinitions[pointCount-1];
            pd.SetVisible(false);
            pointCount--;
            pd.Clear();

            
        }
    }
}


