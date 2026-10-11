using UnityEngine;
using XO.ColorHarmony;

public sealed class ColorHarmonyExample : MonoBehaviour
{
    [SerializeField] private GradientX gradient = new GradientX();
    [SerializeField] private ColourWheel palette = new ColourWheel();
    [SerializeField] private GradientAsset bakedGradient;

    public Color Evaluate(float time) => gradient.Evaluate(time);

    public void SampleMany(Color[] destination)
    {
        // Reuse one snapshot for a batch. Make a new snapshot after changing keys.
        gradient.CreateSnapshot().Sample(destination);
    }

    public Texture2D GradientTexture => bakedGradient ? bakedGradient.BakedTexture : null;
}
