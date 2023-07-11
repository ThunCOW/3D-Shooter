using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade List", menuName = "Guns/Upgrade List", order = 5)]
public class GunUpgradeListScriptableObject : ScriptableObject, System.ICloneable
{
    public List<UpgradeList> Upgrades = new List<UpgradeList>();

    public object Clone()
    {
        GunUpgradeListScriptableObject upgradeList = CreateInstance<GunUpgradeListScriptableObject>();

        upgradeList.Upgrades.AddRange(this.Upgrades);

        return upgradeList;
    }
}

[System.Serializable]
public class UpgradeList
{
    public int Level;
    public List<UpgradeWeapon> Upgrades;
}

[System.Serializable]
public class UpgradeWeapon
{
    public PlayerWeapons UpgradeType;
    public Object Upgrade;
}