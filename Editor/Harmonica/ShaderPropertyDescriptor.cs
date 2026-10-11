using System;
using System.Text.RegularExpressions;

namespace XO.ColorHarmony.Editor
{
    public sealed class ShaderPropertyDescriptor
    {
        private static readonly Regex Tags = new Regex(@"\[([^\]]+)\]", RegexOptions.Compiled);
        private static readonly Regex DepthTag = new Regex(@"^sub(\d*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string Label { get; }
        public bool IsGradient { get; }
        public bool IsToggleFoldout { get; }
        public int Depth { get; }
        public string Diagnostic { get; }

        public ShaderPropertyDescriptor(string displayName)
        {
            displayName = displayName ?? string.Empty;
            int depth = 0; bool gradient = false, foldout = false; string diagnostic = null;
            string label = Tags.Replace(displayName, match =>
            {
                string tag = match.Groups[1].Value.Trim();
                if (tag.Equals("Gradient", StringComparison.OrdinalIgnoreCase)) { gradient = true; return string.Empty; }
                if (tag.Equals("ToggleFoldout", StringComparison.OrdinalIgnoreCase)) { foldout = true; return string.Empty; }
                Match nesting = DepthTag.Match(tag);
                if (nesting.Success)
                {
                    if (nesting.Groups[1].Value.Length == 0) depth = 1;
                    else if (!int.TryParse(nesting.Groups[1].Value, out depth) || depth < 1 || depth > 16) { depth = 0; diagnostic = "Foldout depth must be between 1 and 16."; }
                    return string.Empty;
                }
                return match.Value; // Unknown annotations stay visible and remain usable by other drawers.
            });
            Label = label.Trim(); IsGradient = gradient; IsToggleFoldout = foldout; Depth = depth;
            Diagnostic = gradient && foldout ? "Gradient and ToggleFoldout cannot be combined." : diagnostic;
        }
    }
}
