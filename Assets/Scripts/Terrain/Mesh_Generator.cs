using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mesh_Generator 
{
    public static Mesh_Data Generate_Terrain_Mesh(float[,] Heightmap, Mesh_Settings mesh_Settings, int Level_of_Detail)
    {
        int Skip_Increment = (Level_of_Detail == 0) ? 1 : Level_of_Detail * 2;

        int Num_Verts_Per_Line = mesh_Settings.Num_Verts_Per_Line;

        Vector2 Top_Left = new Vector2(-1, 1) * mesh_Settings.Mesh_World_Size / 2f;

        Mesh_Data mesh_Data = new Mesh_Data(Num_Verts_Per_Line, Skip_Increment, mesh_Settings.Use_Flat_Shading);

        int[,] Vertex_Indices_Map = new int[Num_Verts_Per_Line, Num_Verts_Per_Line];
        int Mesh_Vertex_Index = 0;
        int Out_Of_Mesh_Vertex = -1;

        for (int y = 0; y < Num_Verts_Per_Line; y ++)
        {
            for (int x = 0; x < Num_Verts_Per_Line; x ++)
            {
                bool Is_Out_Of_Mesh_Vertex = y == 0 || y == Num_Verts_Per_Line - 1 || x == 0 || x == Num_Verts_Per_Line - 1;
                bool Is_Skipped_Vertex = x > 2 && x < Num_Verts_Per_Line - 3 && y > 2 && y < Num_Verts_Per_Line - 3 && ((x - 2) % Skip_Increment != 0 || (y - 2) % Skip_Increment != 0);

                if(Is_Out_Of_Mesh_Vertex)
                {
                    Vertex_Indices_Map[x, y] = Out_Of_Mesh_Vertex;
                    Out_Of_Mesh_Vertex--;
                }
                else if(!Is_Skipped_Vertex)
                {
                    Vertex_Indices_Map[x, y] = Mesh_Vertex_Index;
                    Mesh_Vertex_Index++;
                }
            }
        }

        for (int y = 0; y < Num_Verts_Per_Line; y ++)
        {
            for (int x = 0; x < Num_Verts_Per_Line; x ++)
            {
                bool Is_Skipped_Vertex = x > 2 && x < Num_Verts_Per_Line - 3 && y > 2 && y < Num_Verts_Per_Line - 3 && ((x - 2) % Skip_Increment != 0 || (y - 2) % Skip_Increment != 0);

                if (!Is_Skipped_Vertex)
                {
                    bool Is_Out_Of_Mesh_Vertex = y == 0 || y == Num_Verts_Per_Line - 1 || x == 0 || x == Num_Verts_Per_Line - 1;
                    bool Is_Mesh_Edge_Vertex = (y == 1 || y == Num_Verts_Per_Line - 2 || x == 1 || x == Num_Verts_Per_Line - 2) && !Is_Out_Of_Mesh_Vertex;
                    bool Is_Main_Vertex = (x - 2) % Skip_Increment == 0 && (y - 2) % Skip_Increment == 0 && !Is_Out_Of_Mesh_Vertex && !Is_Mesh_Edge_Vertex;
                    bool Is_Edge_Connection_Vertex = (y == 2 || y == Num_Verts_Per_Line - 3 || x == 2 || x == Num_Verts_Per_Line - 3) && !Is_Out_Of_Mesh_Vertex && !Is_Mesh_Edge_Vertex && !Is_Main_Vertex;

                    int Vertex_Index = Vertex_Indices_Map[x, y];
                    Vector2 Percent = new Vector2(x - 1, y - 1) / (Num_Verts_Per_Line - 3);
                    Vector2 Vertex_Position2D = Top_Left + new Vector2(Percent.x, -Percent.y) * mesh_Settings.Mesh_World_Size;
                    float Height = Heightmap[x, y];

                    if(Is_Edge_Connection_Vertex)
                    {
                        bool Is_Vertical = x == 2 || x == Num_Verts_Per_Line - 3;
                        int Dst_To_Main_Vertex_A = ((Is_Vertical) ? y - 2 : x - 2) % Skip_Increment;
                        int Dst_To_Main_Vertex_B = Skip_Increment - Dst_To_Main_Vertex_A;
                        float Dst_Percent_From_A_To_B = Dst_To_Main_Vertex_A / (float)Skip_Increment;

                        float Height_Of_Main_Vertex_A = Heightmap[(Is_Vertical) ? x : x - Dst_To_Main_Vertex_A, (Is_Vertical) ? y - Dst_To_Main_Vertex_A : y];
                        float Height_Of_Main_Vertex_B = Heightmap[(Is_Vertical) ? x : x + Dst_To_Main_Vertex_B, (Is_Vertical) ? y + Dst_To_Main_Vertex_B : y];

                        Height = Height_Of_Main_Vertex_A * (1 - Dst_Percent_From_A_To_B) + Height_Of_Main_Vertex_B * Dst_Percent_From_A_To_B;
                    }

                    mesh_Data.Add_Vertex(new Vector3(Vertex_Position2D.x, Height, Vertex_Position2D.y), Percent, Vertex_Index);

                    bool Create_Triangle = x < Num_Verts_Per_Line - 1 && y < Num_Verts_Per_Line - 1 && (!Is_Edge_Connection_Vertex || (x != 2 && y != 2));

                    if (Create_Triangle)
                    {
                        int Current_Increment = (Is_Main_Vertex && x != Num_Verts_Per_Line - 3 && y != Num_Verts_Per_Line - 3) ? Skip_Increment : 1;

                        int A = Vertex_Indices_Map[x, y];
                        int B = Vertex_Indices_Map[x + Current_Increment, y];
                        int C = Vertex_Indices_Map[x, y + Current_Increment];
                        int D = Vertex_Indices_Map[x + Current_Increment, y + Current_Increment];
                        mesh_Data.Add_Triangle(A, D, C);
                        mesh_Data.Add_Triangle(D, A, B);
                    }
                }
            }
        }

        mesh_Data.Process_Mesh();

        return mesh_Data;
    }
}

public class Mesh_Data
{
    Vector3[] Vertices;
    int[] Triangles;
    Vector2[] UVs;
    Vector3[] Baked_Normals;

    Vector3[] Out_Of_Mesh_Vertices;
    int[] Out_Of_Mesh_Triangles;

    int Triangle_Index;
    int Out_Of_Mesh_Triangle_Index;

    bool Use_Flat_Shading;

    public Mesh_Data(int Num_Verts_Per_Line, int Skip_Increment, bool Use_Flat_Shading)
    {
        this.Use_Flat_Shading = Use_Flat_Shading;

        int Num_Mesh_Edge_Vertices = (Num_Verts_Per_Line - 2) * 4 - 4;
        int Num_Edge_Connection_Vertices = (Skip_Increment - 1) * (Num_Verts_Per_Line - 5) / Skip_Increment * 4;
        int Num_Main_Vertices_Per_Line = (Num_Verts_Per_Line - 5) / Skip_Increment + 1;
        int Num_Main_Vertices = Num_Verts_Per_Line * Num_Verts_Per_Line;

        Vertices = new Vector3[Num_Mesh_Edge_Vertices + Num_Edge_Connection_Vertices + Num_Main_Vertices];
        UVs = new Vector2[Vertices.Length];

        int Num_Mesh_Edge_Triangles = 8 * (Num_Verts_Per_Line - 4);
        int Num_Main_Triangles = (Num_Main_Vertices_Per_Line - 1) * (Num_Main_Vertices_Per_Line - 1) * 2;
        Triangles = new int[(Num_Mesh_Edge_Triangles + Num_Main_Triangles) * 3];

        Out_Of_Mesh_Vertices = new Vector3[Num_Verts_Per_Line * 4 - 4];
        Out_Of_Mesh_Triangles = new int[24 * (Num_Verts_Per_Line - 2)];
    }

    public void Add_Vertex(Vector3 Vertex_Position, Vector2 UV, int Vertex_Index)
    {
        if(Vertex_Index < 0)
        {
            Out_Of_Mesh_Vertices[-Vertex_Index - 1] = Vertex_Position;
        }
        else
        {
            Vertices[Vertex_Index] = Vertex_Position;
            UVs[Vertex_Index] = UV;
        }
    }

    public void Add_Triangle(int A, int B, int C)
    {
        if (A < 0 || B < 0 || C < 0)
        {
            Out_Of_Mesh_Triangles[Out_Of_Mesh_Triangle_Index] = A;
            Out_Of_Mesh_Triangles[Out_Of_Mesh_Triangle_Index + 1] = B;
            Out_Of_Mesh_Triangles[Out_Of_Mesh_Triangle_Index + 2] = C;
            Out_Of_Mesh_Triangle_Index += 3;
        }
        else
        {
            Triangles[Triangle_Index] = A;
            Triangles[Triangle_Index + 1] = B;
            Triangles[Triangle_Index + 2] = C;
            Triangle_Index += 3;
        }
    }

    Vector3[] Calculate_Normals()
    {
        Vector3[] Vertex_Normals = new Vector3[Vertices.Length];
        int Triangle_Count = Triangles.Length / 3;
        for(int i = 0; i < Triangle_Count; i++)
        {
            int Normal_Trianle_Index = i * 3;
            int Vertex_Intex_A = Triangles[Normal_Trianle_Index];
            int Vertex_Intex_B = Triangles[Normal_Trianle_Index + 1];
            int Vertex_Intex_C = Triangles[Normal_Trianle_Index + 2];

            Vector3 Triangle_Normal = Surface_Normal_From_Indices(Vertex_Intex_A, Vertex_Intex_B, Vertex_Intex_C);
            Vertex_Normals[Vertex_Intex_A] += Triangle_Normal;
            Vertex_Normals[Vertex_Intex_B] += Triangle_Normal;
            Vertex_Normals[Vertex_Intex_C] += Triangle_Normal;
        }

        int Border_Triangle_Count = Out_Of_Mesh_Triangles.Length / 3;
        for (int i = 0; i < Border_Triangle_Count; i++)
        {
            int Normal_Trianle_Index = i * 3;
            int Vertex_Intex_A = Out_Of_Mesh_Triangles[Normal_Trianle_Index];
            int Vertex_Intex_B = Out_Of_Mesh_Triangles[Normal_Trianle_Index + 1];
            int Vertex_Intex_C = Out_Of_Mesh_Triangles[Normal_Trianle_Index + 2];

            Vector3 Triangle_Normal = Surface_Normal_From_Indices(Vertex_Intex_A, Vertex_Intex_B, Vertex_Intex_C);
            if (Vertex_Intex_A >= 0)
            {
                Vertex_Normals[Vertex_Intex_A] += Triangle_Normal;
            }
            if (Vertex_Intex_B >= 0)
            {
                Vertex_Normals[Vertex_Intex_B] += Triangle_Normal;
            }
            if (Vertex_Intex_C >= 0)
            {
                Vertex_Normals[Vertex_Intex_C] += Triangle_Normal;
            }
        }

        for (int i = 0; i < Vertex_Normals.Length; i++)
        {
            Vertex_Normals[i].Normalize();
        }

        return Vertex_Normals;
    }

    Vector3 Surface_Normal_From_Indices(int Index_A, int Index_B, int Index_C)
    {
        Vector3 Point_A = (Index_A < 0) ? Out_Of_Mesh_Vertices[-Index_A - 1] : Vertices[Index_A];
        Vector3 Point_B = (Index_B < 0) ? Out_Of_Mesh_Vertices[-Index_B - 1] : Vertices[Index_B];
        Vector3 Point_C = (Index_C < 0) ? Out_Of_Mesh_Vertices[-Index_C - 1] : Vertices[Index_C];

        Vector3 Side_AB = Point_B - Point_A;
        Vector3 Side_AC = Point_C - Point_A;
        return Vector3.Cross(Side_AB, Side_AC).normalized;
    }

    public void Process_Mesh()
    {
        if(Use_Flat_Shading)
        {
            Flat_Shading();
        }
        else
        {
            Bake_Normals();
        }
    }

    void Bake_Normals()
    {
        Baked_Normals = Calculate_Normals();
    }

    void Flat_Shading()
    {
        Vector3[] Flat_Shaded_Vertices = new Vector3[Triangles.Length];
        Vector2[] Flat_Shaded_UVs = new Vector2[Triangles.Length];

        for(int i = 0; i < Triangles.Length; i++)
        {
            Flat_Shaded_Vertices[i] = Vertices[Triangles[i]];
            Flat_Shaded_UVs[i] = UVs[Triangles[i]];
            Triangles[i] = i;
        }

        Vertices = Flat_Shaded_Vertices;
        UVs = Flat_Shaded_UVs;
    }

    public Mesh Create_Mesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVs;
        if (Use_Flat_Shading)
        {
            mesh.RecalculateNormals();

        }
        else
        {
            mesh.normals = Baked_Normals;
        }
        return mesh;
    }
}
