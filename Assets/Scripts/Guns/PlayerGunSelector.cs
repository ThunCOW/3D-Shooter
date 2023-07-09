using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GunType is being used to find the gun from the Guns list, like an ID
public enum GunType
{
    P_Handgun, S_Handgun,
    P_Uzi, S_Uzi,
}

// Player weapons are the main weapons player will see and select
public enum PlayerWeapons
{
    Handgun,
    Uzi
}

public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField]
    private GunType PrimaryGun;
    [SerializeField]
    private GunType SecondaryGun;
    [SerializeField]
    private Transform PrimaryGunParent;
    [SerializeField]
    private Transform SecondaryGunParent;
    [SerializeField]
    private List<GunScriptableObject> Guns;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActivePrimaryGun;
    public GunScriptableObject ActiveSecondaryGun;

    private PlayerWeapons _playerWeapons;
    public PlayerWeapons PlayerWeapon
    {
        get { return _playerWeapons; }
        set 
        { 
            _playerWeapons = value;
            ChangeWeapon();
        }
    }

    private void Start()
    {
        EquipPrimary();
        EquipSecondary();
    }

    private void EquipPrimary()
    {
        GunScriptableObject gun = Guns.Find(gun => gun.Type == PrimaryGun);

        if (gun == null)
        {
            Debug.LogError($"No GunscriptableObject found for GunType :{gun})");
            return;
        }

        ActivePrimaryGun = gun;
        gun.Spawn(PrimaryGunParent, this);
    }

    private void EquipSecondary()
    {
        GunScriptableObject gun = Guns.Find(gun => gun.Type == SecondaryGun);

        if (gun == null)
        {
            Debug.LogError($"No GunscriptableObject found for GunType :{gun})");
            return;
        }

        ActiveSecondaryGun = gun;
        gun.Spawn(SecondaryGunParent, this);
    }

    private bool primaryShoot;
    public void Shoot()
    {
        if (ActiveSecondaryGun != null)
        {
            if (primaryShoot)
            {
                primaryShoot = false;
                ActivePrimaryGun.Shoot();
            }
            else
            {
                primaryShoot = true;
                ActiveSecondaryGun.Shoot();
            }
        }
        else
            ActivePrimaryGun.Shoot();
    }

    public void ChangeWeapon()
    {
        
    }
}