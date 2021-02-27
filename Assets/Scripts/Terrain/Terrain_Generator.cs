using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain_Generator : MonoBehaviour
{
    const float Viwer_Move_Threshold_For_Chunk_Update = 25f;
    const float SQR_Viwer_Move_Threshold_For_Chunk_Update = Viwer_Move_Threshold_For_Chunk_Update * Viwer_Move_Threshold_For_Chunk_Update;

    public int Collider_LOD_Index;
    public LOD_Info[] Detail_Levels;

    public Mesh_Settings mesh_Settings;
    public Heightmap_Settings heightmap_Settings;
    public Texture_Data texture_Settings;

    public Transform Viewer;
    public Transform Parent;
    public Material Map_Material;

    Vector2 Viewer_Position;
    Vector2 Viewer_Position_Old;
    float Mesh_World_Size;
    int Chunks_Visible_In_View_Dst;

    Dictionary<Vector2, Terrain_Chunk> Terrain_Chunk_Dictionary = new Dictionary<Vector2, Terrain_Chunk>();
    List<Terrain_Chunk> Visible_Terrain_Chunks = new List<Terrain_Chunk>();

    private void Start()
    {
        texture_Settings.Apply_To_Material(Map_Material);
        texture_Settings.Update_Mesh_Heights(Map_Material, heightmap_Settings.Min_Height, heightmap_Settings.Max_Height);

        float Max_View_Dst = Detail_Levels[Detail_Levels.Length - 1].Visible_Dst_Threshold;
        Mesh_World_Size = mesh_Settings.Mesh_World_Size;
        Chunks_Visible_In_View_Dst = Mathf.RoundToInt(Max_View_Dst / Mesh_World_Size);

        Update_Visible_Chunks();
    }

    private void Update()
    {
        Viewer_Position = new Vector2(Viewer.position.x, Viewer.position.z);

        if(Viewer_Position != Viewer_Position_Old)
        {
            foreach(Terrain_Chunk Chunk in Visible_Terrain_Chunks)
            {
                Chunk.Update_Collision_Mesh();
            }
        }

        if((Viewer_Position_Old - Viewer_Position).sqrMagnitude > SQR_Viwer_Move_Threshold_For_Chunk_Update)
        {
            Viewer_Position_Old = Viewer_Position;
            Update_Visible_Chunks();
        }
    }

    void Update_Visible_Chunks()
    {
        HashSet<Vector2> Already_Updated_Chunk_Coords = new HashSet<Vector2>();
        for(int i = Visible_Terrain_Chunks.Count - 1; i >= 0; i--)
        {
            Already_Updated_Chunk_Coords.Add(Visible_Terrain_Chunks[i].Coord);
            Visible_Terrain_Chunks[i].Update_Terrain_Chunk();
        }

        int Current_Chunk_Cord_X = Mathf.RoundToInt(Viewer_Position.x / Mesh_World_Size);
        int Current_Chunk_Cord_Y = Mathf.RoundToInt(Viewer_Position.y / Mesh_World_Size);

        for(int Y_Offset = -Chunks_Visible_In_View_Dst; Y_Offset <= Chunks_Visible_In_View_Dst; Y_Offset++)
        {
            for (int X_Offset = -Chunks_Visible_In_View_Dst; X_Offset <= Chunks_Visible_In_View_Dst; X_Offset++)
            {
                Vector2 Viewed_Chunk_Coord = new Vector2(Current_Chunk_Cord_X + X_Offset, Current_Chunk_Cord_Y + Y_Offset);

                if (!Already_Updated_Chunk_Coords.Contains(Viewed_Chunk_Coord))
                {
                    if (Terrain_Chunk_Dictionary.ContainsKey(Viewed_Chunk_Coord))
                    {
                        Terrain_Chunk_Dictionary[Viewed_Chunk_Coord].Update_Terrain_Chunk();
                    }
                    else
                    {
                        Terrain_Chunk New_Chunk = new Terrain_Chunk(Viewed_Chunk_Coord, heightmap_Settings, mesh_Settings, Detail_Levels, Collider_LOD_Index, transform, Viewer, Map_Material);
                        Terrain_Chunk_Dictionary.Add(Viewed_Chunk_Coord, New_Chunk);
                        New_Chunk.On_Visibility_Changed += On_Terrain_Chunk_Visibility_Changed;
                        New_Chunk.Load();
                    }
                }
            }    
        }
    }

    void On_Terrain_Chunk_Visibility_Changed(Terrain_Chunk Chunk, bool Is_Visible)
    {
        if(Is_Visible)
        {
            Visible_Terrain_Chunks.Add(Chunk);
        }
        else
        {
            Visible_Terrain_Chunks.Remove(Chunk);
        }
    }
}

[System.Serializable]
public struct LOD_Info
{
    [Range(0, Mesh_Settings.Num_Supported_LODs - 1)]
    public int LOD;
    public float Visible_Dst_Threshold;

    public float SQR_Visible_Dst_Threshold
    {
        get
        {
            return Visible_Dst_Threshold * Visible_Dst_Threshold;
        }
    }
}
