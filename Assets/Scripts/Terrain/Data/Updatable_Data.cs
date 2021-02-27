using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updatable_Data : ScriptableObject
{
    public event System.Action On_Values_Updated;
    public bool Auto_Update;

    #if UNITY_EDITOR

    protected virtual void OnValidate()
    {
        if(Auto_Update)
        {
            UnityEditor.EditorApplication.update += Notify_Of_Updated_Values;
        }
    }

    public void Notify_Of_Updated_Values()
    {
        UnityEditor.EditorApplication.update -= Notify_Of_Updated_Values;
        if (On_Values_Updated != null)
        {
            On_Values_Updated();
        }
    }

    #endif
}
