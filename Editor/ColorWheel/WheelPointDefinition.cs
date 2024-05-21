using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    [Serializable]
    public class WheelPointDefinition
    {
        
        [SerializeField] public Vector2 wheelCoordinate;
        public Vector2 WheelCoordinate => wheelCoordinate;

        [SerializeField] public Vector2 wheelCoordinateDiffWithCoordinateFactor;
        public Vector2 WheelCoordinateDiffWithCoordinateFactor => wheelCoordinateDiffWithCoordinateFactor;

        [Range(0, 1)] [SerializeField] public float brightness;
        public float Brightness => brightness;

        [Range(0, 1)] private float _wheelBrightness;

        private Color _color;
        public Color Color => _color;

        public WheelPointElement WheelPointElement;
        public VisualElement ExposedColor;
        public VisualElement ExposedData;
        
        public UnityEvent<Color> onUpdate;

        private Texture2D _wheelTexture;

        private float _lastEvaluateColorTime;

        [SerializeField] public bool visible;
        public bool Visible => visible;
        
        public WheelPointDefinition()
        {           
            Init();
        }

        private void Init()
        {
            _lastEvaluateColorTime = DateTime.Now.Millisecond;
            brightness = 1;
            _color = new Color();
            wheelCoordinate = new Vector2();
            wheelCoordinateDiffWithCoordinateFactor = new Vector2();
        }
        public void OnBrightnessChange()
        {
            EvaluateColor();
        }

        public void UpdateWheelCoordinate(Vector2 coordinateFactor)
        {
            wheelCoordinate.x = wheelCoordinateDiffWithCoordinateFactor.Angle() + coordinateFactor.Angle();
            wheelCoordinate.y = wheelCoordinateDiffWithCoordinateFactor.Radius() + coordinateFactor.Radius();
            if (wheelCoordinate.Radius() > 165)
            {
                wheelCoordinate.y = 165;
            }

            Vector2 newPos = WheelUtilities.AngleToPos(wheelCoordinate);

#if UNITY_EDITOR
            SetMargins(newPos);
#endif

            EvaluateColor();
        }

        private void SetMargins(Vector2 localPos)
        {
            WheelPointElement.style.marginLeft = localPos.x - 28;
            WheelPointElement.style.marginTop = localPos.y - 28;
        }

        public void SetBrightness(float newBrightness)
        {
            this.brightness = newBrightness;
            EvaluateColor();
        }

        public void SetTexture(Texture2D texture2D)
        {
            _wheelTexture = texture2D;
            EvaluateColor();
        }

        private void EvaluateColor()
        {
            if (_wheelTexture == null) return;


            Vector2Int pixelCoord =
                new Vector2Int((int)(WheelPointElement.style.marginLeft.value.value + 170 + 28),
                    (int)(WheelPointElement.style.marginTop.value.value + 170 + 28));

            pixelCoord.x = Mathf.CeilToInt(MathX.Remap(pixelCoord.x, 0, 340, 0, 256));
            pixelCoord.y = Mathf.CeilToInt(MathX.Remap(pixelCoord.y, 0, 340, 0, 256));

            pixelCoord.y = Mathf.Abs(pixelCoord.y - 256);

            _color = _wheelTexture.GetPixel(pixelCoord.x, pixelCoord.y);

            /*
            WheelUtilities.Col hsb = WheelUtilities.RgbToHsb(new WheelUtilities.Col(_color.r,_color.g,_color.b));
            Debug.Log(hsb);

            hsb.z = brightness;
            WheelUtilities.Col rgb = WheelUtilities.HsbToRgb(hsb);

            _color.r = (float)rgb.x;
            _color.g = (float)rgb.y;
            _color.b = (float)rgb.z;
            */

            _color.r -= _color.r * (1 - brightness);
            _color.g -= _color.g * (1 - brightness);
            _color.b -= _color.b * (1 - brightness);

            //TODO Max x fps / second limiter
            var dt = Time.realtimeSinceStartup - _lastEvaluateColorTime;
            if (dt < 0) _lastEvaluateColorTime = Time.realtimeSinceStartup;
            if (dt < ColourHarmonySettings.instance.DesiredDelta) return;
            _lastEvaluateColorTime = Time.realtimeSinceStartup;

#if UNITY_EDITOR
            UpdateGuiColors();
#endif
            onUpdate.Invoke(_color);
        }

        private void UpdateGuiColors()
        {
            StyleColor sc = new StyleColor(_color);
            WheelPointElement.style.backgroundColor = sc;
            ExposedColor.style.backgroundColor = sc;
        }

        public void ExposePointData()
        {
            ExposedData.style.display = DisplayStyle.Flex;
        }

        public void SetVisible(bool visible)
        {
            DisplayStyle ds = visible ? DisplayStyle.Flex : DisplayStyle.None;
            this.visible = visible;
            if(WheelPointElement != null)WheelPointElement.style.display = ds;
            if (ExposedColor != null) ExposedColor.style.display = ds;
        }

        public void Clear()
        {
            Init();
        }
    }
}