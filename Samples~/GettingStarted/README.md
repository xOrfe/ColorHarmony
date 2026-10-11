# Getting Started

Import this sample through Package Manager, then add ColorHarmonyExample to a GameObject.

Use the Inspector's Edit GradientX and Open Color Wheel buttons. Property edits use normal Unity serialization and Undo. For material textures, open Window > Color Harmony > GradientX, edit the gradient and choose Apply / Bake to save a GradientAsset under Assets.

In a Shader Graph, set Custom Editor GUI to Harmonica or XO.ColorHarmony.Editor.Harmonica. Add an exposed Texture2D property with display name [Gradient] Ramp and reference name _Ramp. Sample that texture at UV (t, 0.5). The editor opens from the material's gradient row.

RGB gradient data and baked textures are linear. Color Wheel palette values and copied HEX text are sRGB; the wheel-to-gradient action converts them to linear RGB.
