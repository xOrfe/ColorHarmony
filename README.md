# Color Harmony

This package which is made for Unity engine contains some tools and utilities related to colors, gradients etc.

Inside the package, you can find 26 separate color space transformation script, both in HLSL and Shader Graph. 
Currently, only YCBCR and OKLAB converters are available for C# CPU, but I will complete all 26 converters for C# when I have free time.

The package is currently in early stage, and some utilities (those requiring strongly typed switch cases) are not fully functional yet. 
Feel free to use, edit, fork, etc., but use at your own risk.

Enjoy :c



![alt text](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/GradientX.png)

![alt text](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/ColorWheel.png)

## How to use?

Tested with Unity 2022.3.29f1 but any unity version with URP 14+ should work(i guess so).

For color wheel, create a ColourWheel object in your script and edit in editor.

For using Custom Editor GUI utilities and GradientX for shader graph, set shader graph's "Custom Editor GUI" variable as "Harmonica".

![alt text](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/CustomEditorGUI.png)

Because of "Custom Editor GUI" variable , your shader graphs GUI will be overwritten by GradientXEditorWındow.cs script.
I list the tags you can use below:

[Gradient] tag for sampling gradients in shader graph.(only for Texture2D)
[ToggleFoldout], [sub], [sub2],[sub3] ... [subn] tags for nested foldouts.(works with any data type)

![alt text](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/EditorGUITags.png)


## Package structure

```none
<root>
  ├── package.json
  ├── README.md
  ├── Editor
  │   └── ColorWheel
  │       └── ColourWheel.cs
  │       └── ColourWheelDrawer.cs
  │       └── WheelPointDefinition.cs
  │       └── ColourWheelEditorWindow.cs
  │       └── WheelPointPropertyDrawer.cs
  │   └── GradientX
  │       └── GradientXElement.cs
  │       └── GradientXEditorWındow.cs
  ├── Runtime
  │   └── ColorX.cs
  │   └── ColorSpaceType.cs
  │   └── ColorWheel
  │       └── WheelType.cs
  │       └── HarmonyType.cs
  │       └── WheelUtilities.cs
  │       └── WheelPointElement.cs
  │       └── WheelTextureProvider.cs
  │       └── ColourHarmonySettings.cs
  │   └── GradientX
  │       └── AlphaKey.cs
  │       └── ColorKey.cs
  │       └── GradientX.cs
  │       └── KeyElement.cs
  │       └── GradientType.cs
  │       └── IGradientKey.cs
  │       └── GradientSampler.cs
  │       └── KeyContainerElement.cs
  │       └── GradientXDataScriptable.cs
  │   └── Shaders
  │     └── ColorConversions.hlsl
  │     └── GetLuminance.hlsl
  │     └── LinearGradientSampler.shader
  │         └── Conversions
  │             └── /** 26 diffrent color space conversion script **/
  │   └── Sub Graphs
  │       └── /** 26 diffrent color space conversion node for shader graph **
  ├── Resources
  ├── Samples
  │   └── SampleExample.cs
```