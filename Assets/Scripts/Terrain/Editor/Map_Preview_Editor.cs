using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Map_Preview))]
public class Map_Preview_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        Map_Preview Map_Prev = (Map_Preview)target;

        if(DrawDefaultInspector())
        {
            if (Map_Prev.Auto_Update)
            {
                Map_Prev.Draw_Map_In_Editor();
            }
        }

        if(GUILayout.Button("Generate"))
        {
            Map_Prev.Draw_Map_In_Editor();
        }
    }
}
