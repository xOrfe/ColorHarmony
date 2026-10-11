using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    [CustomEditor(typeof(GradientAsset))]
    internal sealed class GradientAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement(); ColorHarmonyStyle.Apply(root);
            root.Add(ColorHarmonyStyle.Heading("Gradient Asset"));
            root.Add(new Label("Reusable source data with an owned, baked linear RGB texture."));
            var asset = (GradientAsset)target;
            root.Add(new Button(() => GradientXEditorWindow.OpenAsset(asset)) { text = "Open GradientX" });
            var texture = new ObjectField("Baked Texture") { objectType = typeof(Texture2D), value = asset.BakedTexture }; texture.SetEnabled(false); root.Add(texture);
            var details = new Label("Resolution: " + asset.Resolution + " • " + (asset.Hdr ? "HDR" : "LDR")); root.Add(details);
            root.TrackSerializedObjectValue(serializedObject, _ =>
            {
                texture.SetValueWithoutNotify(asset.BakedTexture);
                details.text = "Resolution: " + asset.Resolution + " • " + (asset.Hdr ? "HDR" : "LDR");
            });
            return root;
        }
    }
}



