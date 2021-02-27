using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture_Generator
{
    public static Texture2D Texture_From_Colour_Map(Color[] Colour_Map, int Width, int Height)
    {
        Texture2D Texture = new Texture2D(Width, Height);
        Texture.filterMode = FilterMode.Point;
        Texture.wrapMode = TextureWrapMode.Clamp;
        Texture.SetPixels(Colour_Map);
        Texture.Apply();
        return Texture;
    }

    public static Texture2D Texture_From_Heightmap(Heightmap Heightmap)
    {
        int Width = Heightmap.Values.GetLength(0);
        int Height = Heightmap.Values.GetLength(1);

        Color[] Colour_Map = new Color[Width * Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Colour_Map[y * Width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(Heightmap.Min_Value, Heightmap.Max_Value, Heightmap.Values[x, y]));
            }
        }

        return Texture_From_Colour_Map(Colour_Map, Width, Height);
    }
}
