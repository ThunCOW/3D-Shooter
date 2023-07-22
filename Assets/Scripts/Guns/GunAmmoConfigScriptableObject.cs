using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 5)]
public class GunAmmoConfigScriptableObject : ScriptableObject, System.ICloneable
{
    public int MaxAmmo = 100;
    public int BulletPerShot = 1;

    public object Clone()
    {
        GunAmmoConfigScriptableObject config = CreateInstance<GunAmmoConfigScriptableObject>();

        config.MaxAmmo = MaxAmmo;
        config.BulletPerShot = BulletPerShot;

        return config;
    }
}
