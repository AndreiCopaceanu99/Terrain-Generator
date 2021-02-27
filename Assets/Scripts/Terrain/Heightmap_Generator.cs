using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Heightmap_Generator
{
    public static Heightmap Generate_Heightmap(int Width, int Height, Heightmap_Settings Settings, Vector2 Sample_Centre)
    {
        float[,] Values = Noise.Generate_Noise_Map(Width, Height, Settings.noise_Settings, Sample_Centre);

        AnimationCurve Height_Curve_Threadsafe = new AnimationCurve(Settings.Height_Curve.keys);

        float Min_Value = float.MaxValue;
        float Max_Value = float.MinValue;

        for(int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                Values[i, j] *= Height_Curve_Threadsafe.Evaluate(Values[i, j]) * Settings.Height_Multiplier;

                if(Values[i,j] > Max_Value)
                {
                    Max_Value = Values[i, j];
                }
                if(Values[i, j] < Min_Value)
                {
                    Min_Value = Values[i, j];
                }
            }
        }

        return new Heightmap(Values, Min_Value, Max_Value);
    }
}

public struct Heightmap
{
    public readonly float[,] Values;
    public readonly float Min_Value;
    public readonly float Max_Value;

    public Heightmap(float[,] Values, float Min_Value, float Max_Value)
    {
        this.Values = Values;
        this.Min_Value = Min_Value;
        this.Max_Value = Max_Value;
    }
}
