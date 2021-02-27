using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Heightmap_Settings : Updatable_Data
{
    public Noise_Settings noise_Settings;

    public bool Use_Falloff;

    public float Height_Multiplier;
    public AnimationCurve Height_Curve;

    public float Min_Height
    {
        get
        {
            return Height_Multiplier * Height_Curve.Evaluate(0);
        }
    }

    public float Max_Height
    {
        get
        {
            return Height_Multiplier * Height_Curve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noise_Settings.Validate_Values();
        base.OnValidate();
    }

    #endif
}
