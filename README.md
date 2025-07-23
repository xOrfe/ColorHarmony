# 🎨 Color Harmony  
> ✨ A magical color playground for Unity ✨  
> 26 color space converters, vibrant gradients, and a whole lot of uwu~ 💖

---

## 🌟 Overview

Color Harmony brings ✨color space transformations✨, cute utilities, and custom editors to your Unity projects.

Inside, you'll find:
- 🔁 26 color space converters (HLSL + Shader Graph)
- 🧠 YCbCr and OKLab CPU converters (more coming!)
- 🌈 Custom Editor GUI for Shader Graph
- 🎡 Interactive Color Wheel
- 💎 GradientX system for glowing gradients

⚠️ Still in early development — some features may sparkle unexpectedly 🌠  
Use, fork, remix... but use with love (and caution)! 💕

---

## 📸 Previews

**GradientX — beautiful gradient editing right inside Unity!**

![GradientX](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/GradientX.png)

**Color Wheel — for harmony and color balance wizards 🔮**

![ColorWheel](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/ColorWheel.png)

---

## 🚀 Getting Started

> ✅ Tested with **Unity 2022.3.29f1**  
> ✅ Requires **URP 14+** (or newer with Shader Graph support)

---

### 🎡 Using the Color Wheel

1. Create a `ColourWheel` object in your script.
2. Tweak it through the Unity Inspector.
3. Achieve perfect chromatic harmony uwu~ ✨

---

### 🌈 Using GradientX in Shader Graph

1. Open your Shader Graph.
2. In the Graph Inspector, set `Custom Editor GUI` to `Harmonica`.

That's it! GradientX will now take over with its own custom editor.

![CustomEditorGUI](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/CustomEditorGUI.png)

---

### 🏷️ Supported Tags for Shader Graph

You can add the following tags to your shader graph properties:

- `[Gradient]` → Gradient Texture2D sampler 🎨  
- `[ToggleFoldout]`, `[sub]`, `[sub2]`, … → Nested foldouts 💫

![EditorGUITags](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/EditorGUITags.png)

---

## 📁 Project Structure

A quick peek into the rainbow-colored codebase:

```none
<root>
  ├── package.json
  ├── README.md
  ├── Editor
  │   ├── ColorX.cs
  │   ├── ColorSpaceType.cs
  │   ├── ColorWheel/
  │   │   ├── ColourWheel.cs, Drawer, EditorWindow, etc.
  │   ├── GradientX/
  │   │   ├── GradientX.cs, Keys, EditorWindow, etc.
  │   ├── Shaders/
  │   │   ├── 26 HLSL conversion files
  │   ├── Sub Graphs/
  │   │   ├── 26 Shader Graph nodes
  ├── Resources/
  ├── Samples/
  │   ├── SampleExample.cs
````

---

## 💌 Final Notes

Feel free to:

* 🌟 Fork the project
* 🎮 Experiment with the color tools
* ✨ Create magical shader setups

C# converters for all 26 spaces are on the roadmap 💪
Stay tuned, and stay colorful\~ 🌸

> Made with 💖 by [xOrfe](https://github.com/xOrfe)

---

🦄🌈 *Color Harmony is love. Color Harmony is life.* 🌈🦄