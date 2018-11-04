using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ObservableProperty<T>
{
    public delegate void ObservablePropertyValueChanged(T oldValue, T newValue);
    public event ObservablePropertyValueChanged ValueChangedEvent;
    
    private T currentValue;

    public T Value
    {
        get { return currentValue; }
        set
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, value))
            {
                T prevVal = currentValue;
                
                currentValue = value;

                if (ValueChangedEvent != null)
                    ValueChangedEvent(prevVal, currentValue);
            }
        }
    }

    public void Register(ObservablePropertyValueChanged callback)
    {
        ValueChangedEvent += callback;
    }

    public void RegisterAndCall(ObservablePropertyValueChanged callback)
    {
        ValueChangedEvent += callback;
        callback(Value, Value);
    }

    public void UnRegister(ObservablePropertyValueChanged callback)
    {
        ValueChangedEvent -= callback;
    }

    public ObservableProperty(T initialValue)
    {
        Value = initialValue;
    }

    public void ForceUpdate(T value)
    {
        T prevVal = currentValue;
        
        currentValue = value;

        if (ValueChangedEvent != null)
            ValueChangedEvent(prevVal, currentValue);
    }
}
