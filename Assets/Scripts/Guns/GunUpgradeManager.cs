using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class GunUpgradeManager : MonoBehaviour
{
    [SerializeField] private GunUpgradeListScriptableObject GunUpgradeList;
    
    private List<GunScriptableObject> guns = new List<GunScriptableObject>();

    private void Awake()
    {
        
        GameManager.Instance.onUnlockUpgradeList += Upgrade;
        
        guns = GameManager.Instance.Player.GetComponent<PlayerGunSelector>().Guns;

        GunUpgradeList = GunUpgradeList.Clone() as GunUpgradeListScriptableObject;
    }
    private void Start()
    {

    }

    private void Upgrade(int CurrentLevel)
    {
        if (CurrentLevel >= GunUpgradeList.Upgrades[0].Level)
        {
            GunScriptableObject gun;
            switch (GunUpgradeList.Upgrades[0].Upgrades[0].UpgradeType)
            {
                case PlayerWeapons.Handgun:
                    
                    foreach (UpgradeWeapon WeaponUpgrade in GunUpgradeList.Upgrades[0].Upgrades)
                    {
                        // Primary
                        gun = guns.Find(gun => gun.ID == GunType.P_Handgun);
                        UpgradeWeapon(WeaponUpgrade, gun);
                        // Secondary
                        gun = guns.Find(gun => gun.ID == GunType.S_Handgun);
                        UpgradeWeapon(WeaponUpgrade, gun);
                    }
                    break;

                case PlayerWeapons.Uzi:

                    foreach (UpgradeWeapon WeaponUpgrade in GunUpgradeList.Upgrades[0].Upgrades)
                    {
                        
                    }

                    break;
            }
            
            GunUpgradeList.Upgrades.RemoveAt(0);
        }
    }

    private void UpgradeWeapon<T>(T Upgrade, GunScriptableObject Gun)
    {
        Type type = Gun.GetType();
        
        foreach (FieldInfo field in type.GetFields())
        {
            if (field.FieldType == Upgrade.GetType())
            {
                field.SetValue(Gun, Upgrade);
                break;
            }
        }
    }
}