using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Texture_Data : Updatable_Data
{
    const int Texture_Size = 512;
    const TextureFormat Texture_Format = TextureFormat.RGB565;

    public Layer[] Layers;

    float Saved_Min_Height;
    float Saved_Max_Height;

    public void Apply_To_Material(Material material)
    {
        material.SetInt("Layer_Count", Layers.Length);
        material.SetColorArray("Base_Colours", Layers.Select(x => x.Tint).ToArray());
        material.SetFloatArray("Base_Start_Heights", Layers.Select(x => x.Start_Height).ToArray());
        material.SetFloatArray("Base_Blends", Layers.Select(x => x.Blend_Strength).ToArray());
        material.SetFloatArray("Base_Colour_Strenght", Layers.Select(x => x.Tint_Strenght).ToArray());
        material.SetFloatArray("Base_Texture_Scales", Layers.Select(x => x.Texture_Scale).ToArray());
        Texture2DArray Textures_Array = Generate_Texture_Array(Layers.Select(x => x.texture).ToArray());
        material.SetTexture("Base_Textures", Textures_Array);


        Update_Mesh_Heights(material, Saved_Min_Height, Saved_Max_Height);
    }

    public void Update_Mesh_Heights(Material material, float Min_Height, float Max_Height)
    {
        Saved_Max_Height = Max_Height;
        Saved_Min_Height = Min_Height;

        material.SetFloat("Min_Height", Min_Height);
        material.SetFloat("Max_Height", Max_Height);
    }

    Texture2DArray Generate_Texture_Array(Texture2D[] Textures)
    {
        Texture2DArray Texture_Array = new Texture2DArray(Texture_Size, Texture_Size, Textures.Length, Texture_Format, true);
        for(int i = 0; i < Textures.Length; i++)
        {
            Texture_Array.SetPixels(Textures[i].GetPixels(), i);
        }
        Texture_Array.Apply();
        return Texture_Array;
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color Tint;
        [Range(0,1)]
        public float Tint_Strenght;
        [Range(0, 1)]
        public float Start_Height;
        [Range(0, 1)]
        public float Blend_Strength;
        public float Texture_Scale;
    }
}
