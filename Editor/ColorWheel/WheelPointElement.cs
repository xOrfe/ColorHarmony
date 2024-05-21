using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    public class WheelPointElement : VisualElement
    {
        public int Index;
        
        #region UXML
        [Preserve]
        public new class UxmlFactory : UxmlFactory<WheelPointElement, UxmlTraits>
        {
        }
 
        [Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
        #endregion

        public WheelPointElement()
        {
            
        }
        
        public WheelPointElement(int index)
        {
            Index = index;
        }
    }
}
