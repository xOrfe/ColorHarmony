````md
# 🌈💖 Color Harmony 💖🌈  
> A magical color playground for Unity — full of gradients, rainbows, unicorns and love~ 🦄🎨✨

Bored of boring colors? Want more ✨ harmony ✨ in your shader graphs and scripts?  
Color Harmony is here to bless your Unity project with **26 dazzling color space converters**, glowing gradients, and a fabulous color wheel~ 💅🌸

---

## ✨ What's Inside?

🧪 This enchanting package includes:  
- **26 color space transformation spells** (both HLSL + Shader Graph) 🧙‍♂️  
- **C# converters** for YCbCr and OKLab (more coming soon~ I pawmise! 🐾)  
- **Custom Editor GUIs** to decorate your shader graphs like magical gardens 🌻  
- **Color Wheel system** for vibrant, customizable harmony visuals 🎡  

⚠️ Still a baby dragon 🐉 (early stage)! Some utilities that require strongly typed switch cases are still hibernating.  
Use at your own risk (but with love)! 💖

---

## 🌟 Screenshots from the Dream World

![GradientX](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/GradientX.png)

*~ Taste the rainbow inside your shader graph ~*

![ColorWheel](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/ColorWheel.png)

*~ Spin the wheel of color fate ~*

---

## 🧙‍♀️ How to Use?

> ✨ *Tested with Unity 2022.3.29f1, URP 14+ is recommended for best sparkle power* ✨

### 🎡 Color Wheel
1. Create a `ColourWheel` object in your script.
2. Tweak settings via the Unity Inspector for instant harmony magic 🌼

### 🧁 Custom Editor GUI + GradientX  
To use the ✨ GradientX ✨ system in Shader Graph:
1. Set **Custom Editor GUI** on your Shader Graph to `Harmonica`  
2. Watch it transform like a glittery chameleon~ 💫

![EditorGUI](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/CustomEditorGUI.png)


### 🏷️ Magical Tags You Can Use:
- `[Gradient]` → Add delicious gradient sampling (Texture2D only 🍬)  
- `[ToggleFoldout]`, `[sub]`, `[sub2]`, ... `[subN]` → Create cozy nested foldouts 🧺  
Perfect for making your editor GUI feel like a warm blanket on a rainy day~ ☔

![Tags](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/EditorGUITags.png)


---

## 📁 Package Structure

Here's a peek into the treasure chest 🎁:

```none
<root>
  ├── package.json
  ├── README.md
  ├── Editor
  │   ├── ColorX.cs
  │   ├── ColorSpaceType.cs
  │   ├── ColorWheel
  │   │   ├── WheelType.cs
  │   │   ├── HarmonyType.cs
  │   │   ├── WheelUtilities.cs
  │   │   ├── WheelPointElement.cs
  │   │   ├── WheelTextureProvider.cs
  │   │   ├── ColourHarmonySettings.cs
  │   │   ├── ColourWheel.cs
  │   │   ├── ColourWheelDrawer.cs
  │   │   ├── WheelPointDefinition.cs
  │   │   ├── ColourWheelEditorWindow.cs
  │   │   ├── WheelPointPropertyDrawer.cs
  │   ├── GradientX
  │   │   ├── AlphaKey.cs
  │   │   ├── ColorKey.cs
  │   │   ├── GradientX.cs
  │   │   ├── KeyElement.cs
  │   │   ├── GradientType.cs
  │   │   ├── IGradientKey.cs
  │   │   ├── GradientSampler.cs
  │   │   ├── KeyContainerElement.cs
  │   │   ├── GradientXEditorWındow.cs
  │   │   ├── GradientXDataScriptable.cs
  │   ├── Shaders
  │   │   ├── ColorConversions.hlsl
  │   │   ├── GetLuminance.hlsl
  │   │   ├── LinearGradientSampler.shader
  │   │   └── Conversions
  │   │       └── /** 26 magical color space scripts **/
  │   ├── Sub Graphs
  │       └── /** Shader Graph nodes for every conversion **/
  ├── Resources
  ├── Samples
  │   └── SampleExample.cs
````

---

## 💌 Final Words

🌟 Fork it, break it, remix it, sparkle it!
This package is for color lovers, shader nerds, and dreamers\~
More updates and C# color spells will arrive when the stars align ✨

> Stay magical, stay colorful\~
> Made with ♥ by xOrfe

---

🦄🌈💖 *color harmony is love. color harmony is life.* 💖🌈🦄

```

---

İstersen bu dosyayı `README.md` olarak dışa aktarılmış şekilde de gönderebilirim.  
Bir sonraki adımda bunu Unity paketinle birleştirebiliriz ya da sana özel bir logo bile tasarlayabiliriz! İster misin? 🌟
```
