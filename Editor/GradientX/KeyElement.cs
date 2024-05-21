using System;
using UnityEngine;
using UnityEngine.UIElements;
using XO.ColorHarmony;

[Serializable]
public class KeyElement<T> : VisualElement where T : IGradientKey, new()
{
    [SerializeField] public T KeyValue;
    public int Index { get; set; }

    public void SetTime(float time) 
    {
        time = Mathf.Clamp(time, 0, 1);
        KeyValue.Time(time);
        //StyleLength styleLength = new StyleLength(time * this.parent.resolvedStyle.width);
        this.style.left = Length.Percent(time * 100);
    }

    public float GetTime() => KeyValue.Time();
    public float Time => KeyValue.Time();
    public KeyElement(int index)
    {
        Index = index;
        
        if (this.GetType() == typeof(KeyElement<ColorKey>))
        {
            KeyValue = (T) Convert.ChangeType(new ColorKey(0.5f), typeof(T));
        }else if (this.GetType() == typeof(KeyElement<AlphaKey>))
        {
            KeyValue = (T) Convert.ChangeType(new AlphaKey(0.5f), typeof(T));
        }
        
        this.AddToClassList("key");
    }

    public KeyElement()
    {
        this.AddToClassList("key");
    }
}