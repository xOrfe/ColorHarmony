namespace XO.ColorHarmony
{
    [System.Serializable]
    public enum ColorSpaceType
    {
        RGB,
        HCV,
        HCY,
        HSL,
        HSV,
        [UnityEngine.InspectorName("OKLab")] OKLAB,
        [UnityEngine.InspectorName("xyY")] XYY,
        XYZ,
        [UnityEngine.InspectorName("YCbCr")] YCBCR
    }
}
