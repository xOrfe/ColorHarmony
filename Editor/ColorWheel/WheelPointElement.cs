using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    public class WheelPointElement : VisualElement
    {
        public int Index;

        public WheelPointElement()
        {
            
        }
        
        public WheelPointElement(int index)
        {
            Index = index;
        }
    }
}
