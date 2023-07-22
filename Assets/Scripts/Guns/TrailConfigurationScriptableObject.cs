using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Gun Trail Configuration", order = 4)]
public class TrailConfigurationScriptableObject : ScriptableObject, System.ICloneable
{
    public GameObject ProjectileObject;
    public ProjectileType ProjectileType;
    /*
    */
    public Material Material;
    public AnimationCurve WidthCurve;
    public float Duration = 0.2f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;
    public float MissDistance = 50f;
    public float SimulationSpeed = 500f;

    public object Clone()
    {
        TrailConfigurationScriptableObject config = CreateInstance<TrailConfigurationScriptableObject>();

        Utilities.CopyValues(this, config);

        return config;
    }
}

public enum ProjectileType
{
    Trail,
    Rocket,
    Nade
}