using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    public class GradientXEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset gradientXMain = default;
        [SerializeField] private VisualTreeAsset colorKeyField = default;
        [SerializeField] private VisualTreeAsset alphaKeyField = default;

        private VisualElement _root;

        private List<GradientXElement> _gradients;
        private VisualElement _colorField;
        private VisualElement _alphaField;

        private bool _drawCall;
        private bool _copyGradientXDataCall;

        private static Material _targetMaterial;
        private static string _targetPropertyName;
        private static string _targetPropertyReferenceName;
        private static UnityEngine.Object _targetObject;
        private static string _targetPropertyPath;

        public static int _gradientLength;
        private static bool _isExposed;
        public int GradientLength => _gradientLength;

        private bool _doNotSave;
        public static void ExposeWindow(Material targetMaterial, string targetPropertyName,
            string targetPropertyOnlyName)
        {
            if(_isExposed) return;
            _isExposed = true;
            _targetMaterial = targetMaterial;
            _targetPropertyName = targetPropertyName;
            _targetPropertyReferenceName = targetPropertyOnlyName;
            _targetObject = null;
            _targetPropertyPath = null;

            _gradientLength = 512;

            GradientXEditorWindow wnd = GetWindow<GradientXEditorWindow>();
            wnd.titleContent = new GUIContent("GradientX");
            wnd.maxSize = new Vector2(720f, 1440f);
            wnd.minSize = new Vector2(400f, 450f);
        }

        public static void ExposeWindow(UnityEngine.Object targetObject, string targetPropertyPath)
        {
            if (_isExposed) return;
            _isExposed = true;
            _targetMaterial = null;
            _targetPropertyName = targetPropertyPath;
            _targetPropertyReferenceName = null;
            _targetObject = targetObject;
            _targetPropertyPath = targetPropertyPath;

            _gradientLength = 512;

            GradientXEditorWindow wnd = GetWindow<GradientXEditorWindow>();
            wnd.titleContent = new GUIContent("GradientX");
            wnd.maxSize = new Vector2(720f, 1440f);
            wnd.minSize = new Vector2(400f, 450f);
        }

        public void CreateGUI()
        {
            if (_targetMaterial == null && _targetObject == null)
            {
                _doNotSave = true;
                this.Close();
                return;
            }

            _root = rootVisualElement;

            VisualElement gradientXMain = this.gradientXMain.Instantiate();
            _root.Add(gradientXMain);

            _root.Q("GradientContainer").Add(CreateListView());

            _root.Q<Button>("AddGradientButton").clicked += OnClickAddGradient;
            _root.Q<Button>("RemoveGradientButton").clicked += OnClickRemoveGradient;

            _copyGradientXDataCall = true;
        }

        private void Update()
        {
            if (_copyGradientXDataCall) CopyGradientXData();
            if (_drawCall) Draw();
        }

        private void OnDisable()
        {
            _isExposed = false;
            SaveGradientXData();
        }

        public void DrawCall() => _drawCall = true;

        private void Draw()
        {
            if (_targetMaterial == null)
            {
                SaveGradientXData();
                _drawCall = false;
                return;
            }

            Texture2D texture = LoadGradientTexture();

            for (int i = 0; i < _gradientLength; i++)
            {
                Color col = new Color();
                foreach (var gradient in _gradients)
                {
                    col += gradient.Texture.GetPixel(i, 0);
                }

                col /= _gradients.Count;

                texture.SetPixel(i, 0, col);
            }

            texture.Apply();
            _targetMaterial.SetTexture(_targetPropertyReferenceName, texture);
            EditorUtility.SetDirty(_targetMaterial);
            _drawCall = false;
        }

        public void BindKeyValue(KeyElement<ColorKey> key, Action reSampleCall)
        {
            if (_colorField != null) _colorField.Clear();
            if (_alphaField != null) _alphaField.Clear();

            _colorField = this.colorKeyField.Instantiate();
            _root.Q<VisualElement>("Inputs").Add(_colorField);

            ColorField color = (_colorField.Q("Color") as ColorField);
            Slider brightness = (_colorField.Q("Brightness") as Slider);
            Slider pos = (_colorField.Q("Position") as Slider);

            color.value = key.KeyValue.Color();
            brightness.value = key.KeyValue.Brightness();
            pos.value = key.KeyValue.Time();

            color.RegisterValueChangedCallback(
                (evt) =>
                {
                    key.KeyValue.Color(evt.newValue);
                    key.style.backgroundColor = evt.newValue;
                    reSampleCall.Invoke();
                });
            brightness.RegisterValueChangedCallback(
                (evt) =>
                {
                    key.KeyValue.Brightness(evt.newValue);
                    reSampleCall.Invoke();
                });
            pos.RegisterValueChangedCallback(
                (evt) =>
                {
                    key.SetTime(evt.newValue);
                    reSampleCall.Invoke();
                });
        }

        public void BindKeyValue(KeyElement<AlphaKey> key, Action reSampleCall)
        {
            if (_colorField != null) _colorField.Clear();
            if (_alphaField != null) _alphaField.Clear();

            _colorField = this.alphaKeyField.Instantiate();
            _root.Q<VisualElement>("Inputs").Add(_colorField);

            Slider alpha = (_colorField.Q("Alpha") as Slider);
            Slider pos = (_colorField.Q("Position") as Slider);

            alpha.value = key.KeyValue.Alpha();
            pos.value = key.KeyValue.Time();

            alpha.RegisterValueChangedCallback(
                (evt) =>
                {
                    key.KeyValue.Alpha(evt.newValue);
                    key.style.backgroundColor = new Color(1, 1, 1, evt.newValue);
                    reSampleCall.Invoke();
                });

            pos.RegisterValueChangedCallback(
                (evt) =>
                {
                    key.SetTime(evt.newValue);
                    reSampleCall.Invoke();
                });
        }

        private Texture2D LoadGradientTexture()
        {
            string path = AssetDatabase.GetAssetPath(_targetMaterial);
            string targetName = "GradientTexture {" + _targetPropertyName + "}";

            var assets = AssetDatabase.LoadAllAssetsAtPath(path);

            Texture2D texture;

            foreach (var asset in assets)

            {
                if (asset.name == targetName)
                {
                    texture = asset as Texture2D;
                    if (texture == null)
                        texture = CreateGradientTexture(path);
                    return texture;
                }
            }

            texture = CreateGradientTexture(path);

            return texture;
        }

        private Texture2D CreateGradientTexture(string path)
        {
            var texture = new Texture2D(_gradientLength, 1, TextureFormat.ARGB32, false)
            {
                name = "GradientTexture {" + _targetPropertyName + "}",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            AssetDatabase.AddObjectToAsset(texture, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
            return texture;
        }

        private void CopyGradientXData()
        {
            if (_targetObject != null)
            {
                CopySerializedGradientXData();
                return;
            }

            var gradientXDataScriptable = LoadGradientXDataScriptable();
            _gradients = new List<GradientXElement>();

            if (gradientXDataScriptable.Gradients.Count == 0)
            {
                gradientXDataScriptable.Gradients.Add(new GradientXData());
            }

            foreach (var gradientXData in gradientXDataScriptable.Gradients)
            {
                AddGradientElement(gradientXData);
            }

            _copyGradientXDataCall = false;
        }

        private void CopySerializedGradientXData()
        {
            _gradients = new List<GradientXElement>();

            SerializedObject serializedObject = new SerializedObject(_targetObject);
            SerializedProperty property = serializedObject.FindProperty(_targetPropertyPath);
            if (property != null)
            {
                AddGradientElement(ReadGradientXData(property));
            }

            _copyGradientXDataCall = false;
        }

        private void AddGradientElement(GradientXData gradientXData)
        {
            GradientXElement element = new GradientXElement(this);
            element.SetColorSpace(gradientXData.ColorSpaceType);
            element.SetGradientType(gradientXData.GradientType);

            foreach (var alphaKeyData in gradientXData.AlphaKeys)
            {
                var alphaKey = element.AlphaKeys.AddKey(alphaKeyData.Time);
                alphaKey.KeyValue.Time(alphaKeyData.Time);
                alphaKey.KeyValue.Alpha(alphaKeyData.Alpha);
                alphaKey.style.backgroundColor = new Color(1, 1, 1, alphaKeyData.Alpha);
            }

            foreach (var colorKeyData in gradientXData.ColorKeys)
            {
                var colorKey = element.ColorKeys.AddKey(colorKeyData.Time);
                colorKey.KeyValue.Color(colorKeyData.Color);
                colorKey.style.backgroundColor = colorKeyData.Color;
                colorKey.KeyValue.Brightness(colorKeyData.Brightness);
            }

            _gradients.Add(element);
            element.ReSample();
            _root.Q("GradientContainer").Add(_gradients[^1]);
        }

        private static GradientXData ReadGradientXData(SerializedProperty property)
        {
            SerializedProperty colorKeys = property.FindPropertyRelative("colorKeys");
            SerializedProperty alphaKeys = property.FindPropertyRelative("alphaKeys");

            List<ColorKeyData> colorKeyDatas = new List<ColorKeyData>();
            for (int i = 0; i < colorKeys.arraySize; i++)
            {
                SerializedProperty key = colorKeys.GetArrayElementAtIndex(i);
                colorKeyDatas.Add(new ColorKeyData(
                    key.FindPropertyRelative("color").colorValue,
                    key.FindPropertyRelative("brightness").floatValue,
                    key.FindPropertyRelative("time").floatValue));
            }

            List<AlphaKeyData> alphaKeyDatas = new List<AlphaKeyData>();
            for (int i = 0; i < alphaKeys.arraySize; i++)
            {
                SerializedProperty key = alphaKeys.GetArrayElementAtIndex(i);
                alphaKeyDatas.Add(new AlphaKeyData(
                    key.FindPropertyRelative("alpha").floatValue,
                    key.FindPropertyRelative("time").floatValue));
            }

            if (colorKeyDatas.Count == 0)
            {
                colorKeyDatas.Add(new ColorKeyData(Color.black, 1f, 0f));
                colorKeyDatas.Add(new ColorKeyData(Color.white, 1f, 1f));
            }

            if (alphaKeyDatas.Count == 0)
            {
                alphaKeyDatas.Add(new AlphaKeyData(1f, 0f));
                alphaKeyDatas.Add(new AlphaKeyData(1f, 1f));
            }

            return new GradientXData(
                alphaKeyDatas,
                colorKeyDatas,
                (ColorSpaceType)property.FindPropertyRelative("interpolationColorSpace").enumValueIndex,
                (GradientType)property.FindPropertyRelative("gradientType").enumValueIndex);
        }

        private void SaveGradientXData()
        {
            if (_doNotSave) return;
            if (_targetObject != null)
            {
                SaveSerializedGradientXData();
                return;
            }
            
            GradientXDataScriptable data = LoadGradientXDataScriptable();
            
            data.Gradients.Clear();

            foreach (var gradient in _gradients)
            {
                List<AlphaKeyData> alphaKeyDatas = new List<AlphaKeyData>();
                foreach (var alphaKey in gradient.AlphaKeys.keys)
                {
                    AlphaKeyData alphaKeyData = new AlphaKeyData(alphaKey.KeyValue.Alpha(), alphaKey.KeyValue.Time());
                    alphaKeyDatas.Add(alphaKeyData);
                }

                List<ColorKeyData> colorKeyDatas = new List<ColorKeyData>();
                foreach (var colorKey in gradient.ColorKeys.keys)
                {
                    ColorKeyData colorKeyData = new ColorKeyData(colorKey.KeyValue.Color(),
                        colorKey.KeyValue.Brightness(), colorKey.KeyValue.Time());
                    colorKeyDatas.Add(colorKeyData);
                }

                GradientXData gradientXData = new GradientXData(alphaKeyDatas, colorKeyDatas, gradient.ColorSpaceType,
                    gradient.GradientType);
                data.Gradients.Add(gradientXData);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        private void SaveSerializedGradientXData()
        {
            if (_gradients == null || _gradients.Count == 0 || _targetObject == null) return;

            SerializedObject serializedObject = new SerializedObject(_targetObject);
            SerializedProperty property = serializedObject.FindProperty(_targetPropertyPath);
            if (property == null) return;

            Undo.RecordObject(_targetObject, "Edit GradientX");
            WriteGradientXData(property, _gradients[0]);
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_targetObject);
        }

        private static void WriteGradientXData(SerializedProperty property, GradientXElement gradient)
        {
            property.FindPropertyRelative("interpolationColorSpace").enumValueIndex = (int)gradient.ColorSpaceType;
            property.FindPropertyRelative("gradientType").enumValueIndex = (int)gradient.GradientType;

            SerializedProperty colorKeys = property.FindPropertyRelative("colorKeys");
            colorKeys.arraySize = gradient.ColorKeys.keys.Count;
            for (int i = 0; i < gradient.ColorKeys.keys.Count; i++)
            {
                SerializedProperty key = colorKeys.GetArrayElementAtIndex(i);
                ColorKey colorKey = gradient.ColorKeys.keys[i].KeyValue;
                key.FindPropertyRelative("time").floatValue = colorKey.Time();
                key.FindPropertyRelative("color").colorValue = colorKey.Color();
                key.FindPropertyRelative("brightness").floatValue = colorKey.Brightness();
            }

            SerializedProperty alphaKeys = property.FindPropertyRelative("alphaKeys");
            alphaKeys.arraySize = gradient.AlphaKeys.keys.Count;
            for (int i = 0; i < gradient.AlphaKeys.keys.Count; i++)
            {
                SerializedProperty key = alphaKeys.GetArrayElementAtIndex(i);
                AlphaKey alphaKey = gradient.AlphaKeys.keys[i].KeyValue;
                key.FindPropertyRelative("time").floatValue = alphaKey.Time();
                key.FindPropertyRelative("alpha").floatValue = alphaKey.Alpha();
            }
        }

        private GradientXDataScriptable LoadGradientXDataScriptable()
        {
            string path = AssetDatabase.GetAssetPath(_targetMaterial);
            string targetName = "GradientData {" + _targetPropertyName + "}";

            var assets = AssetDatabase.LoadAllAssetsAtPath(path);

            GradientXDataScriptable data;
            
            
            foreach (var asset in assets)
            {
                if (asset == null || asset.name != targetName) continue;
                data = asset as GradientXDataScriptable;
                return data;
            }

            data = CreateGradientXDataScriptable(path);

            return data;
        }

        private GradientXDataScriptable CreateGradientXDataScriptable(string path)
        {
            Debug.Log("Create Gradient");
            var data = ScriptableObject.CreateInstance<GradientXDataScriptable>();
            data.name = "GradientData {" + _targetPropertyName + "}";
            AssetDatabase.AddObjectToAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
            return data;
        }

        private ListView CreateListView()
        {
            // Make the hierarchy for the row, the ListView will virtualize these so that it only creates enough elements to cover what is displayed.
            Func<VisualElement> makeItem = () =>
            {
                var root = new VisualElement();
                var label = new GradientXElement(this);
                root.Add(label);
                return root;
            };

            // Bind the data
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                // The element has a single Label as a children, so we can use a simple type query to fetch it.
                var label = e.Q<GradientXElement>();
                label = _gradients[i];
            };

            var listView = new ListView(_gradients, 30, makeItem, bindItem);

            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            listView.showBorder = true;
            listView.selectionType = SelectionType.Multiple;
            listView.style.flexGrow = 1.0f;
            return listView;
        }

        private void OnClickAddGradient()
        {
            if (_targetObject != null) return;
            if (_gradients.Count >= 3) return;
            _gradients.Add(new GradientXElement(this));
            _root.Q("GradientContainer").Add(_gradients[^1]);
            DrawCall();
        }

        private void OnClickRemoveGradient()
        {
            if (_targetObject != null) return;
            if (_gradients.Count <= 1) return;
            _root.Q("GradientContainer").Remove(_gradients[^1]);
            _gradients.RemoveAt(_gradients.Count - 1);
            DrawCall();
        }
    }
}
