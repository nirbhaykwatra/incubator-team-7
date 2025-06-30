using System.Collections.Generic;
using UnityEngine;

public abstract class LUT<T> : ScriptableObject
{
    public List<T> Values;
    
    public T GetObjectAtIndex(int key)
    {
        return Values[key];
    }

    public int GetObjectIndex(T value)
    {
        return Values.IndexOf(value);
    }
    
    public void Add(T value)
    {
        Values.Add(value);
    }
    
    public void Remove(T value)
    {
        Values.Remove(value);
    }
    
    public void Clear()
    {
        Values.Clear();
    }
    
    public bool Contains(T value)
    {
        return Values.Contains(value);
    }
}
