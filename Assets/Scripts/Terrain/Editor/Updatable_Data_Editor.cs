using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Updatable_Data), true)]
public class Updatable_Data_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Updatable_Data Data = (Updatable_Data)target;

        if(GUILayout.Button("Update"))
        {
            Data.Notify_Of_Updated_Values();
            EditorUtility.SetDirty(target);
        }
    }
}
