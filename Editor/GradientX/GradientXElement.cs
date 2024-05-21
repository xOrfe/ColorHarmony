using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    public class GradientXElement : VisualElement
    {
        public Texture2D Texture;

        public KeyContainerElement<AlphaKey> AlphaKeys;
        public KeyContainerElement<ColorKey> ColorKeys;

        private StyleSheet _styleSheet;

        private GradientXEditorWındow _editorWindow;

        private VisualElement _gradientMain;

        public ColorSpaceType ColorSpaceType { get; private set; }
        public GradientType GradientType { get; private set; }
        
        private EnumField _colorSpaceField;
        private EnumField _gradientTypeField;
        
        public void SetColorSpace(ColorSpaceType colorSpaceType)
        {
            ColorSpaceType = colorSpaceType;
            _colorSpaceField.value = ColorSpaceType;
        }
        public void SetGradientType(GradientType gradientType)
        {
            GradientType = gradientType;
            _gradientTypeField.value = GradientType;
        }
        public GradientXElement(GradientXEditorWındow window)
        {
            _editorWindow = window;

            _styleSheet = Resources.Load<StyleSheet>("Editor/GradientX/GradientXElement");
            this.styleSheets.Add(_styleSheet);
            this.AddToClassList("gradientx-element");
            this.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));


            var enumSpace = new VisualElement();
            this.Add(enumSpace);
            enumSpace.AddToClassList("enum-space");
            
            _colorSpaceField = new EnumField(ColorSpaceType.RGB);
            enumSpace.Add(_colorSpaceField);
            ColorSpaceType = ColorSpaceType.RGB;
            
            _gradientTypeField = new EnumField(GradientType.Interpolate);
            enumSpace.Add(_gradientTypeField);
            GradientType = GradientType.Interpolate;
            
            _colorSpaceField.RegisterValueChangedCallback(OnColorSpaceChange);
            _gradientTypeField.RegisterValueChangedCallback(OnGradientTypeChange);
            
            AlphaKeys = new KeyContainerElement<AlphaKey>(window);
            AlphaKeys.ReSampleCall = ReSample;
            ColorKeys = new KeyContainerElement<ColorKey>(window);
            ColorKeys.ReSampleCall = ReSample;

            _gradientMain = new VisualElement();
            _gradientMain.AddToClassList("gradient-single");
            _gradientMain.style.backgroundColor = new StyleColor(Color.black);
            this.Add(_gradientMain);
            _gradientMain.Add(AlphaKeys);
            _gradientMain.Add(ColorKeys);
            
            Texture = new Texture2D(256, 1);
            _gradientMain.style.backgroundImage = Texture;
        }

        private void OnColorSpaceChange(ChangeEvent<Enum> evt)
        {
            ColorSpaceType = (ColorSpaceType)evt.newValue;
            ReSample();
        }
        private void OnGradientTypeChange(ChangeEvent<Enum> evt)
        {
            GradientType = (GradientType)evt.newValue;
            ReSample();
        }
        public void ReSample()
        {
            if (ColorKeys.keys.Count == 0) return;
            
            List<KeyElement<ColorKey>> colorKeys = ColorKeys.keys.OrderBy(x => x.Time).ToList();
            List<KeyElement<AlphaKey>> alphaKeys = AlphaKeys.keys.OrderBy(x => x.Time).ToList();
            
            Texture = GradientSampler.SampleGradient(_editorWindow.GradientLength, colorKeys, alphaKeys,
                ColorSpaceType,GradientType);
            _gradientMain.style.backgroundImage = Texture;
            
            _editorWindow.DrawCall();
        }
    }
}