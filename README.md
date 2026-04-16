# Color Harmony

A Unity toolkit for working with color spaces, gradients, and editor utilities.

---

## Overview

Color Harmony provides tools for color space conversion, gradient editing, and custom Shader Graph interfaces.

Features include:

- 26 color space converters (HLSL + Shader Graph)
- YCbCr and OKLab CPU converters (in progress)
- Custom Editor GUI for Shader Graph
- Interactive Color Wheel
- GradientX system for gradient editing

> Note: This project is still in early development. Some features may change or be unstable.

---

## Previews

**GradientX — gradient editing inside Unity**

![GradientX](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/GradientX.png)

**Color Wheel — color harmony tool**

![ColorWheel](https://raw.githubusercontent.com/xOrfe/ColorHarmony/main/Images/ColorWheel.png)

---

## Getting Started

- Tested with **Unity 2022.3.29f1**
- Requires **URP 14+** (or newer with Shader Graph support)

---

## Usage

### Color Wheel

1. Create a `ColourWheel` object in your script.
2. Adjust its properties through the Unity Inspector.

---

### GradientX (Shader Graph)

1. Open your Shader Graph.
2. In the Graph Inspector, set `Custom Editor GUI` to `Harmonica`.

---

### Supported Shader Graph Tags

You can use the following tags in your shader properties:

- `[Gradient]` — Gradient Texture2D sampler  
- `[ToggleFoldout]`, `[sub]`, `[sub2]`, etc. — Nested foldouts  

---

## Project Structure
