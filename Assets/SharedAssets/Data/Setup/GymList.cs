using System.Collections.Generic;
using UnityEngine;

public class GymList : ScriptableObject
{
    public List<string> gymList = new List<string>();

    public void AddGym(string gymName)
    {
        gymList.Add(gymName);
    }

    public void RemoveGym(string gymName)
    {
        gymList.Remove(gymName);
    }

    public void ClearGymList()
    {
        gymList.Clear();
    }
}
