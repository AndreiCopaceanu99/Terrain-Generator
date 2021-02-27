Shader "Custom/Terrain"
{
    Properties
    {
        Test_Texture("Texture", 2D) = "White"{}
        Test_Scale("Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int Max_Layer_Count = 8;
        const static float Epsilon = 1E-4;

        int Layer_Count;
        float3 Base_Colours[Max_Layer_Count];
        float Base_Start_Heights[Max_Layer_Count];
        float Base_Blends[Max_Layer_Count];
        float Base_Colour_Strenght[Max_Layer_Count];
        float Base_Texture_Scales[Max_Layer_Count];

        float Min_Height;
        float Max_Height;

        sampler2D Test_Texture;
        float Test_Scale;

        UNITY_DECLARE_TEX2DARRAY(Base_Textures);

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float Inverse_Lerp(float A, float B, float Value)
        {
            return saturate((Value - A) / (B - A));
        }

        float3 Triplanar(float3 worldPos, float Scale, float3 Blend_Axes, int Texture_Index)
        {
            float3 Scaled_World_Pos = worldPos / Scale;

            float3 X_Projection = UNITY_SAMPLE_TEX2DARRAY(Base_Textures, float3(Scaled_World_Pos.y, Scaled_World_Pos.z, Texture_Index)) * Blend_Axes.x;
            float3 Y_Projection = UNITY_SAMPLE_TEX2DARRAY(Base_Textures, float3(Scaled_World_Pos.x, Scaled_World_Pos.z, Texture_Index)) * Blend_Axes.y;
            float3 Z_Projection = UNITY_SAMPLE_TEX2DARRAY(Base_Textures, float3(Scaled_World_Pos.x, Scaled_World_Pos.y, Texture_Index)) * Blend_Axes.z;
            return X_Projection + Y_Projection + Z_Projection;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float Height_Percent = Inverse_Lerp(Min_Height, Max_Height, IN.worldPos.y);
            float3 Blend_Axes = abs(IN.worldNormal);
            Blend_Axes /= Blend_Axes.x + Blend_Axes.y + Blend_Axes.z;

            for (int i = 0; i < Layer_Count; i++)
            {
                float Draw_Strenght = Inverse_Lerp(-Base_Blends[i] / 2 - Epsilon, Base_Blends[i] / 2, Height_Percent - Base_Start_Heights[i]);

                float3 Base_Colour = Base_Colours[i] * Base_Colour_Strenght[i];
                float3 Texture_Colour = Triplanar(IN.worldPos, Base_Texture_Scales[i], Blend_Axes, i) * (1 - Base_Colour_Strenght[i]);

                o.Albedo = o.Albedo * (1 - Draw_Strenght) + (Base_Colour + Texture_Colour) * Draw_Strenght;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
