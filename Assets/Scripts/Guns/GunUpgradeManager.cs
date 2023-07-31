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
        guns = GameManager.Instance.Player.GetComponent<PlayerGunSelector>().Guns;

        GunUpgradeList = GunUpgradeList.Clone() as GunUpgradeListScriptableObject;
    }
    private void Start()
    {
        ScoreManager.Instance.onUnlockUpgradeList += Upgrade;
    }

    private void Upgrade(int CurrentLevel)
    {
        if (GunUpgradeList.Upgrades.Count == 0)
            return;

        if (CurrentLevel >= GunUpgradeList.Upgrades[0].Level)
        {
            UIUpgradeList.Instance.UnlockUpgrade();

            GunScriptableObject gun;
            switch (GunUpgradeList.Upgrades[0].Upgrades[0].UpgradeType)
            {
                case PlayerWeapons.Handgun:
                    foreach (UpgradeWeapon WeaponUpgrade in GunUpgradeList.Upgrades[0].Upgrades)
                    {
                        // Primary
                        gun = guns.Find(gun => gun.ID == GunType.P_Handgun);
                        UpgradeWeapon(WeaponUpgrade.Upgrade, gun);
                        // Secondary
                        gun = guns.Find(gun => gun.ID == GunType.S_Handgun);
                        UpgradeWeapon(WeaponUpgrade.Upgrade, gun);
                    }
                    break;

                case PlayerWeapons.Uzi:
                    foreach (UpgradeWeapon WeaponUpgrade in GunUpgradeList.Upgrades[0].Upgrades)
                    {
                        // Primary
                        gun = guns.Find(gun => gun.ID == GunType.P_Uzi);
                        UpgradeWeapon(WeaponUpgrade.Upgrade, gun);
                        // Secondary
                        gun = guns.Find(gun => gun.ID == GunType.S_Uzi);
                        UpgradeWeapon(WeaponUpgrade.Upgrade, gun);
                    }
                    break;

                case PlayerWeapons.Shotgun:
                    foreach (UpgradeWeapon WeaponUpgrade in GunUpgradeList.Upgrades[0].Upgrades)
                    {
                        // Primary
                        gun = guns.Find(gun => gun.ID == GunType.P_Shotgun);
                        UpgradeWeapon(WeaponUpgrade.Upgrade, gun);
                    }
                    break;

                case PlayerWeapons.Rocket:
                    foreach (UpgradeWeapon WeaponUpgrade in GunUpgradeList.Upgrades[0].Upgrades)
                    {
                        // Primary
                        gun = guns.Find(gun => gun.ID == GunType.P_Rocket);
                        UpgradeWeapon(WeaponUpgrade.Upgrade, gun);
                    }
                    break;
            }
            GunUpgradeList.Upgrades.RemoveAt(0);
        }

        if (GunUpgradeList.Upgrades.Count != 0 && GunUpgradeList.Upgrades[0].Level < CurrentLevel) Upgrade(CurrentLevel);
    }

    private void UpgradeWeapon<T>(T Upgrade, GunScriptableObject Gun)
    {
        Type type = Gun.GetType();
        
        foreach (FieldInfo field in type.GetFields())
        {
            //Debug.Log(field.FieldType + " VS " + Upgrade.GetType());
            if (field.FieldType == Upgrade.GetType())
            {
                field.SetValue(Gun, Upgrade);
                break;
            }
        }
    }
}