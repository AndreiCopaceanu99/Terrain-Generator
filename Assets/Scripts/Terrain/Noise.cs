using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum Normalise_Mode { Local, Global };

    public static float[,] Generate_Noise_Map(int Map_Width, int Map_Height, Noise_Settings Settings, Vector2 Sample_Centre)
    {
        float[,] Noise_Map = new float[Map_Width, Map_Height];

        System.Random prng = new System.Random(Settings.Seed);
        Vector2[] Octave_Offsets = new Vector2[Settings.Octaves];

        float Max_Possible_Height = 0;
        float Amplitude = 1;
        float Frequency = 1;

        for (int i = 0; i < Settings.Octaves; i++)
        {
            float Offset_X = prng.Next(-100000, 100000) + Settings.Offset.x + Sample_Centre.x;
            float Offset_Y = prng.Next(-100000, 100000) - Settings.Offset.y - Sample_Centre.y;
            Octave_Offsets[i] = new Vector2(Offset_X, Offset_Y);

            Max_Possible_Height += Amplitude;
            Amplitude *= Settings.Persistance;
        }

        float Max_Local_Noise_Height = float.MinValue;
        float Min_Local_Noise_Height = float.MaxValue;

        float Half_Width = Map_Width / 2f;
        float Half_Height = Map_Height / 2f;

        for(int y = 0; y < Map_Height; y++)
        {
            for(int x = 0; x < Map_Width; x++)
            {
                Amplitude = 1;
                Frequency = 1;
                float Noise_Height = 0;

                for (int i = 0; i < Settings.Octaves; i++)
                {
                    float Sample_X = (x-Half_Width + Octave_Offsets[i].x) / Settings.Scale * Frequency;
                    float Sample_Y = (y-Half_Height + Octave_Offsets[i].y) / Settings.Scale * Frequency;

                    float Perlin_Value = Mathf.PerlinNoise(Sample_X, Sample_Y) * 2 - 1;
                    Noise_Height += Perlin_Value * Amplitude;

                    Amplitude *= Settings.Persistance;
                    Frequency *= Settings.Lacunarity;
                }

                if(Noise_Height > Max_Local_Noise_Height)
                {
                    Max_Local_Noise_Height = Noise_Height;
                }
                if(Noise_Height < Min_Local_Noise_Height)
                {
                    Min_Local_Noise_Height = Noise_Height;
                }
                Noise_Map[x, y] = Noise_Height;

                if (Settings.Normalise_Mode == Normalise_Mode.Global)
                {
                    float Normalised_Height = (Noise_Map[x, y] + 1) / (Max_Possible_Height / 0.9f);
                    Noise_Map[x, y] = Mathf.Clamp(Normalised_Height, 0, int.MaxValue);
                }
            }
        }
        if (Settings.Normalise_Mode == Normalise_Mode.Local)
        {
            for (int y = 0; y < Map_Height; y++)
            {
                for (int x = 0; x < Map_Width; x++)
                {
                    Noise_Map[x, y] = Mathf.InverseLerp(Min_Local_Noise_Height, Max_Local_Noise_Height, Noise_Map[x, y]); 
                }
            }
        }

        return Noise_Map;
    }
}

[System.Serializable]
public class Noise_Settings
{
    public Noise.Normalise_Mode Normalise_Mode;

    public float Scale = 50;

    public int Octaves = 6;
    [Range(0, 1)]
    public float Persistance = 6f;
    public float Lacunarity = 2;

    public int Seed;
    public Vector2 Offset;

    public void Validate_Values()
    {
        Scale = Mathf.Max(Scale, 0.01f);
        Octaves = Mathf.Max(Octaves, 1);
        Lacunarity = Mathf.Max(Lacunarity, 1);
        Persistance = Mathf.Clamp01(Persistance);
    }
}
