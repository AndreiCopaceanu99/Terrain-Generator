using UnityEngine;

public class Terrain_Chunk
{
    const float Collider_Generation_Distance_Threshold = 5;

    public event System.Action<Terrain_Chunk, bool> On_Visibility_Changed;
    public Vector2 Coord;

    GameObject Mesh_Object;
    Vector2 Sample_Centre;
    Bounds Bounds;

    MeshRenderer Mesh_Renderer;
    MeshFilter Mesh_Filter;
    MeshCollider Mesh_Collider;

    LOD_Info[] Detail_Levels;
    LOD_Mesh[] LOD_Meshes;
    int Collider_LOD_Index;

    Heightmap Heightmap;
    bool Heightmap_Received;
    int Previous_LOD_Index = -1;
    bool Has_Set_Collider;
    float Max_View_Dst;

    Heightmap_Settings heightmap_Settings;
    Mesh_Settings mesh_Settings;
    Transform Viewer;

    public Terrain_Chunk(Vector2 Coord, Heightmap_Settings heightmap_Settings, Mesh_Settings mesh_Settings, LOD_Info[] Detail_Levels, int Collider_LOD_Index, Transform Parent, Transform Viewer, Material material)
    {
        this.Coord = Coord;
        this.Detail_Levels = Detail_Levels;
        this.Collider_LOD_Index = Collider_LOD_Index;
        this.heightmap_Settings = heightmap_Settings;
        this.mesh_Settings = mesh_Settings;
        this.Viewer = Viewer;

        Sample_Centre = Coord * mesh_Settings.Mesh_World_Size / mesh_Settings.Mesh_Scale;
        Vector2 Position = Coord * mesh_Settings.Mesh_World_Size;
        Bounds = new Bounds(Position, Vector2.one * mesh_Settings.Mesh_World_Size);

        Mesh_Object = new GameObject("Terrain Chunk");
        Mesh_Renderer = Mesh_Object.AddComponent<MeshRenderer>();
        Mesh_Filter = Mesh_Object.AddComponent<MeshFilter>();
        Mesh_Collider = Mesh_Object.AddComponent<MeshCollider>();
        Mesh_Renderer.material = material;

        Mesh_Object.transform.position = new Vector3(Position.x, 0, Position.y);
        Mesh_Object.transform.parent = Parent;
        Set_Visible(false);

        LOD_Meshes = new LOD_Mesh[Detail_Levels.Length];
        for (int i = 0; i < Detail_Levels.Length; i++)
        {
            LOD_Meshes[i] = new LOD_Mesh(Detail_Levels[i].LOD);
            LOD_Meshes[i].Update_Callback += Update_Terrain_Chunk;
            if (i == Collider_LOD_Index)
            {
                LOD_Meshes[i].Update_Callback += Update_Collision_Mesh;
            }
        }

        Max_View_Dst = Detail_Levels[Detail_Levels.Length - 1].Visible_Dst_Threshold;
    }

    public void Load()
    {
        Threaded_Data_Requester.Request_Data(() => Heightmap_Generator.Generate_Heightmap(mesh_Settings.Num_Verts_Per_Line, mesh_Settings.Num_Verts_Per_Line, heightmap_Settings, Sample_Centre), On_Heightmap_Data_Received);
    }

    void On_Heightmap_Data_Received(object Heightmap_Object)
    {
        this.Heightmap = (Heightmap)Heightmap_Object;
        Heightmap_Received = true;

        Update_Terrain_Chunk();
    }

    Vector2 Viewer_Position
    {
        get
        {
            return new Vector2(Viewer.position.x, Viewer.position.z);
        }
    }

    public void Update_Terrain_Chunk()
    {
        if (Heightmap_Received)
        {
            float Viewer_Dst_From_Nearest_Edge = Mathf.Sqrt(Bounds.SqrDistance(Viewer_Position));

            bool Was_Visible = Is_Visible();
            bool Visible = Viewer_Dst_From_Nearest_Edge <= Max_View_Dst;

            if (Visible)
            {
                int LOD_Index = 0;

                for (int i = 0; i < Detail_Levels.Length - 1; i++)
                {
                    if (Viewer_Dst_From_Nearest_Edge > Detail_Levels[i].Visible_Dst_Threshold)
                    {
                        LOD_Index = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (LOD_Index != Previous_LOD_Index)
                {
                    LOD_Mesh lod_Mesh = LOD_Meshes[LOD_Index];
                    if (lod_Mesh.Has_Mesh)
                    {
                        Previous_LOD_Index = LOD_Index;
                        Mesh_Filter.mesh = lod_Mesh.mesh;
                    }
                    else if (!lod_Mesh.Has_Requested_Mesh)
                    {
                        lod_Mesh.Request_Mesh(Heightmap, mesh_Settings);
                    }
                }
            }

            if (Was_Visible != Visible)
            {
                Set_Visible(Visible);
                if (On_Visibility_Changed != null)
                {
                    On_Visibility_Changed(this, Visible);
                }
            }
        }
    }

    public void Update_Collision_Mesh()
    {
        if (!Has_Set_Collider)
        {
            float SQR_Dst_From_Viewer_To_Edge = Bounds.SqrDistance(Viewer_Position);

            {
                if (!LOD_Meshes[Collider_LOD_Index].Has_Requested_Mesh)
                {
                    LOD_Meshes[Collider_LOD_Index].Request_Mesh(Heightmap, mesh_Settings);
                }
            }
            if (SQR_Dst_From_Viewer_To_Edge < Detail_Levels[Collider_LOD_Index].SQR_Visible_Dst_Threshold)

                if (SQR_Dst_From_Viewer_To_Edge < Collider_Generation_Distance_Threshold * Collider_Generation_Distance_Threshold)
                {
                    if (LOD_Meshes[Collider_LOD_Index].Has_Mesh)
                    {
                        Mesh_Collider.sharedMesh = LOD_Meshes[Collider_LOD_Index].mesh;
                        Has_Set_Collider = true;
                    }
                }
        }
    }

    public void Set_Visible(bool Visible)
    {
        Mesh_Object.SetActive(Visible);
    }

    public bool Is_Visible()
    {
        return Mesh_Object.activeSelf;
    }
}

class LOD_Mesh
{
    public Mesh mesh;
    public bool Has_Requested_Mesh;
    public bool Has_Mesh;
    int LOD;
    public event System.Action Update_Callback;

    public LOD_Mesh(int LOD)
    {
        this.LOD = LOD;
    }

    void On_Mesh_Data_Received(object Mesh_Data_Object)
    {
        mesh = ((Mesh_Data)Mesh_Data_Object).Create_Mesh();
        Has_Mesh = true;

        Update_Callback();
    }

    public void Request_Mesh(Heightmap heightmap, Mesh_Settings mesh_Settings)
    {
        Has_Requested_Mesh = true;
        Threaded_Data_Requester.Request_Data(() => Mesh_Generator.Generate_Terrain_Mesh(heightmap.Values, mesh_Settings, LOD), On_Mesh_Data_Received);
    }
}
