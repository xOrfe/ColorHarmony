using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XO.ColorHarmony;

public class KeyContainerElement<T> : VisualElement where T : IGradientKey, new()
{
    public List<KeyElement<T>> keys;
    
    private GradientXEditorWındow _window;
    
    private bool _isKeySelected;
    private int _selectedKey;
    
    public Action ReSampleCall;

    public KeyContainerElement(GradientXEditorWındow window)
    {
        _window = window;
        keys = new List<KeyElement<T>>();

        this.RegisterCallback<PointerDownEvent>(OnPointerDown);
        this.RegisterCallback<PointerUpEvent>(OnPointerUp);
        this.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        
        this.AddToClassList("key-container");
    }

    public KeyElement<T> AddKey(float time)
    {
        var key = new KeyElement<T>(keys.Count);
        
        keys.Add(key);
        this.Add(key);
        key.SetTime(time);
        key.focusable = true;

        key.RegisterCallback<PointerDownEvent>(
            (evt) => {
                if (evt.button != 0)
                {
                    return;
                }

                SelectKey(key.Index);
            }
        );
        SortKeys();
        ReSampleCall.Invoke();
        return key;
    }

    private void SortKeys()
    {
        //TODO
    }
    private void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0 || _isKeySelected || keys.Count >= 8)
        {
            return;
        }
        
        float time = GetKeyPosition(evt.localPosition.x);
        KeyElement<T> key = AddKey(time);
        
        SelectKey(key.Index);

    }
    
    private void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0)
        {
            return;
        }

        ReleaseKey();
        
    }
    
    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isKeySelected)
        {
            return;
        }

        int selectedKey = _selectedKey;
        
        float time = GetKeyPosition(evt.localPosition.x);
        keys[selectedKey].SetTime(time);
        
        float height = this.resolvedStyle.height;

        if (Mathf.Abs(evt.localPosition.y - (height/2)) > height/2.2f)
        {
            ReleaseKey();
            DeleteKey(selectedKey);
        }
        
        SortKeys();
        ReSampleCall.Invoke();
    }
    
    public void SelectKey(int index)
    {
        if(_isKeySelected) return;
        
        _isKeySelected = true;
        _selectedKey = index;
        KeyElement<T> key = keys[_selectedKey];
        key.BringToFront();
        key.Focus();
        
        if(key is KeyElement<ColorKey>)
            _window.BindKeyValue(key as KeyElement<ColorKey>,ReSampleCall);
        else if(key is KeyElement<AlphaKey>)
            _window.BindKeyValue(key as KeyElement<AlphaKey>,ReSampleCall);

    }
    
    public void ReleaseKey()
    {
        if(!_isKeySelected) return;
        
        //keys[_selectedKey].Blur();
        
        _isKeySelected = false;
        _selectedKey = -1;
    }

    public void DeleteKey(int index)
    {
        if(keys.Count == 1) return;
        
        this.Remove(keys[index]);
        keys.RemoveAt(index);
        
        for (int i = index; i < keys.Count; i++)
        {
            keys[i].Index = i;
        }
    }
    
    private float GetKeyPosition(float pos)
    {
        pos = pos / this.resolvedStyle.width;
        return pos;
    }
}
