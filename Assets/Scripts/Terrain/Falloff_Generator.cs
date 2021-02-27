using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Falloff_Generator
{
    public static float[,] Generate_Falloff_Map(int Size)
    {
        float[,] Map = new float[Size, Size];

        for(int i = 0; i < Size; i++)
        {
            for(int j = 0; j < Size; j++)
            {
                float X = i / (float)Size * 2 - 1;
                float Y = j / (float)Size * 2 - 1;

                float Value = Mathf.Max(Mathf.Abs(X), Mathf.Abs(Y));
                Map[i, j] = Evaluate(Value);
            }
        }

        return Map;
    }

    static float Evaluate(float Value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(Value, a) / (Mathf.Pow(Value, a) + Mathf.Pow(b - b * Value, a));
    }
}
