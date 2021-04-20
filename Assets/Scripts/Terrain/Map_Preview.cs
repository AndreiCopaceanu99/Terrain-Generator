using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map_Preview : MonoBehaviour
{
    public Renderer Texture_Renderer;
    public MeshFilter Mesh_Filter;
    public MeshRenderer Mesh_Renderer;

    public enum Draw_Mode { Noise_Map, Mesh, Falloff_Map };
    public Draw_Mode draw_Mode;
    
    public enum Terrain_Type { Plain, Hill, Mountain, Default, Test };
    public Terrain_Type terrain_Type;
    [SerializeField]
    Mesh_Info[] mesh_Infos;
    public Mesh_Info Active_Mesh_Type;

    public Material Terrain_Material;

    [Range(0, Mesh_Settings.Num_Supported_LODs - 1)]
    public int Editor_Preview_LOD;

    public bool Auto_Update;

    [SerializeField]
    Slider Terrain;

    [SerializeField]
    Dropdown Terrain_Type_UI;

    private void Update()
    {
        for (int i = 0; i < mesh_Infos.Length; i++)
        {
            if (Terrain_Type_UI.options[Terrain_Type_UI.value].text == mesh_Infos[i].Terrain_Name)
            {
                Active_Mesh_Type = mesh_Infos[i];
            }
        }

        Active_Mesh_Type.heightmap_Settings.noise_Settings.Seed = (int)Terrain.value;

        Active_Mesh_Type.texture_Data.Apply_To_Material(Terrain_Material);

        Active_Mesh_Type.texture_Data.Update_Mesh_Heights(Terrain_Material, Active_Mesh_Type.heightmap_Settings.Min_Height, Active_Mesh_Type.heightmap_Settings.Max_Height);
        Heightmap heightmap = Heightmap_Generator.Generate_Heightmap(Active_Mesh_Type.mesh_Settings.Num_Verts_Per_Line, Active_Mesh_Type.mesh_Settings.Num_Verts_Per_Line, Active_Mesh_Type.heightmap_Settings, Vector2.zero);

        if (draw_Mode == Draw_Mode.Noise_Map)
        {
            Draw_Texture(Texture_Generator.Texture_From_Heightmap(heightmap));
        }
        else if (draw_Mode == Draw_Mode.Mesh)
        {
            Draw_Mesh(Mesh_Generator.Generate_Terrain_Mesh(heightmap.Values, Active_Mesh_Type.mesh_Settings, Editor_Preview_LOD));
        }
        else if (draw_Mode == Draw_Mode.Falloff_Map)
        {
            Draw_Texture(Texture_Generator.Texture_From_Heightmap(new Heightmap(Falloff_Generator.Generate_Falloff_Map(Active_Mesh_Type.mesh_Settings.Num_Verts_Per_Line), 0, 1)));
        }
    }

    public void Draw_Map_In_Editor()
    {
        for (int i = 0; i < mesh_Infos.Length; i++)
        {
            if(terrain_Type.ToString() == mesh_Infos[i].Terrain_Name)
            {
                Active_Mesh_Type = mesh_Infos[i];
            }
        }

        Active_Mesh_Type.texture_Data.Apply_To_Material(Terrain_Material);

        Active_Mesh_Type.texture_Data.Update_Mesh_Heights(Terrain_Material, Active_Mesh_Type.heightmap_Settings.Min_Height, Active_Mesh_Type.heightmap_Settings.Max_Height);
        Heightmap heightmap = Heightmap_Generator.Generate_Heightmap(Active_Mesh_Type.mesh_Settings.Num_Verts_Per_Line, Active_Mesh_Type.mesh_Settings.Num_Verts_Per_Line, Active_Mesh_Type.heightmap_Settings, Vector2.zero);
        
        if (draw_Mode == Draw_Mode.Noise_Map)
        {
           Draw_Texture(Texture_Generator.Texture_From_Heightmap(heightmap));
        }
        else if (draw_Mode == Draw_Mode.Mesh)
        {
            Draw_Mesh(Mesh_Generator.Generate_Terrain_Mesh(heightmap.Values, Active_Mesh_Type.mesh_Settings, Editor_Preview_LOD));
        }
        else if (draw_Mode == Draw_Mode.Falloff_Map)
        {
            Draw_Texture(Texture_Generator.Texture_From_Heightmap(new Heightmap(Falloff_Generator.Generate_Falloff_Map(Active_Mesh_Type.mesh_Settings.Num_Verts_Per_Line),0,1)));
        }
    }

    public void Draw_Texture(Texture2D Texture)
    {
        Texture_Renderer.sharedMaterial.mainTexture = Texture;
        Texture_Renderer.transform.localScale = new Vector3(Texture.width, 1, Texture.height) / 10f;

        Texture_Renderer.gameObject.SetActive(true);
        Mesh_Filter.gameObject.SetActive(false);
    }

    public void Draw_Mesh(Mesh_Data mesh_Data)
    {
        Mesh_Filter.sharedMesh = mesh_Data.Create_Mesh();

        Texture_Renderer.gameObject.SetActive(false);
        Mesh_Filter.gameObject.SetActive(true);
    }

    void On_Values_Updated()
    {
        if (!Application.isPlaying)
        {
            Draw_Map_In_Editor();
        }
    }

    void On_Texture_Values_Update()
    {
        Active_Mesh_Type.texture_Data.Apply_To_Material(Terrain_Material);
    }

    private void OnValidate()
    {
        if (Active_Mesh_Type.mesh_Settings != null)
        {
            Active_Mesh_Type.mesh_Settings.On_Values_Updated -= On_Values_Updated;
            Active_Mesh_Type.mesh_Settings.On_Values_Updated += On_Values_Updated;
        }

        if (Active_Mesh_Type.heightmap_Settings != null)
        {
            Active_Mesh_Type.heightmap_Settings.On_Values_Updated -= On_Values_Updated;
            Active_Mesh_Type.heightmap_Settings.On_Values_Updated += On_Values_Updated;
        }

        if (Active_Mesh_Type.texture_Data != null)
        {
            Active_Mesh_Type.texture_Data.On_Values_Updated -= On_Texture_Values_Update;
            Active_Mesh_Type.texture_Data.On_Values_Updated += On_Texture_Values_Update;
        }
    }
}

[System.Serializable]
public struct Mesh_Info
{
    public string Terrain_Name;
    public Mesh_Settings mesh_Settings;
    public Heightmap_Settings heightmap_Settings;
    public Texture_Data texture_Data;
}
