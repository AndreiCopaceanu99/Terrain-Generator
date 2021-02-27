using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Mesh_Settings : Updatable_Data
{
    public const int Num_Supported_LODs = 5;
    public const int Num_Supported_Chunk_Sizes = 9;
    public const int Num_Supported_Flatshaded_Chunk_Sizes = 3;

    public static readonly int[] Supported_Chunks_Sizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float Mesh_Scale = 2f;

    public bool Use_Flat_Shading;

    [Range(0, Num_Supported_Chunk_Sizes - 1)]
    public int Chunk_Size_Index;
    [Range(0, Num_Supported_Flatshaded_Chunk_Sizes - 1)]
    public int Flatshaded_Chunk_Size_Index;

    // Num verts per line of mesh rendered at LOD = 0. Include the 2 extra verts that are excluded from final mesh, but used for calculating normals
    public int Num_Verts_Per_Line
    {
        get
        {
            return Supported_Chunks_Sizes[(Use_Flat_Shading) ? Flatshaded_Chunk_Size_Index : Chunk_Size_Index] + 5;
        }
    }

    public float Mesh_World_Size
    {
        get
        {
            return (Num_Verts_Per_Line - 3) * Mesh_Scale;
        }
    }
}
