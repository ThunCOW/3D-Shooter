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
    AnimatorManager PlayerAnimatorManager;

    [SerializeField]
    private GunType PrimaryGun;
    [SerializeField]
    private GunType SecondaryGun;
    [SerializeField]
    private Transform PrimaryGunParent;
    [SerializeField]
    private Transform SecondaryGunParent;

    [SerializeField] List<GunScriptableObject> GunsSO;
    public List<GunScriptableObject> Guns;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActivePrimaryGun;
    public GunScriptableObject ActiveSecondaryGun;

    [SerializeField] private PlayerWeapons _playerWeapons;
    public PlayerWeapons PlayerWeapon
    {
        get { return _playerWeapons; }
        set 
        { 
            _playerWeapons = value;
            ChangeWeapon();
        }
    }

    public int Dual_Handgun_Unlock_Level;
    [SerializeField] private bool Dual_Handgun;
    public int Dual_Uzi_Unlock_Level;
    [SerializeField] private bool Dual_Uzi;

    private void OnValidate()
    {
        //ChangeWeapon();
    }

    private void Awake()
    {
        foreach (GunScriptableObject gun in GunsSO)
            Guns.Add(gun.Clone() as GunScriptableObject);

        GameManager.Instance.onUnlockUpgradeList += Upgrade;
    }

    private void Start()
    {
        PlayerAnimatorManager = GameManager.Instance.Player.GetComponentInChildren<AnimatorManager>();

        ChangeWeapon();
    }

    private void Update()
    {
        if (ChangeWeaponEditor)
            ChangeWeapon();
    }
    private void EquipPrimary()
    {
        GunScriptableObject gun = Guns.Find(gun => gun.ID == PrimaryGun);

        if (gun == null)
        {
            Debug.LogError($"No GunscriptableObject found for GunType :{gun})");
            return;
        }

        ActivePrimaryGun = gun;
        ActivePrimaryGun.Spawn(PrimaryGunParent, this);
        ActiveSecondaryGun = null;

        PlayerAnimatorManager.AimLayerName = ActivePrimaryGun.AnimatorLayerName;
    }

    private void EquipSecondary()
    {
        GunScriptableObject gun = Guns.Find(gun => gun.ID == SecondaryGun);

        if (gun == null)
        {
            Debug.LogError($"No GunscriptableObject found for GunType :{gun})");
            return;
        }

        ActiveSecondaryGun = gun;
        ActiveSecondaryGun.Spawn(SecondaryGunParent, this);

        PlayerAnimatorManager.AimLayerName = ActiveSecondaryGun.AnimatorLayerName;
    }

    private bool primaryShoot;
    private float lastShootTime;
    private float lastEmptyBulletTime;
    
    public void Shoot()
    {
        if (Time.time > ActivePrimaryGun.ShootConfig.FireRate + lastShootTime)
        {
            if (ActivePrimaryGun.CurrentAmmo > 0 || ActivePrimaryGun.CurrentAmmo < 0)
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
                    
                ActivePrimaryGun.CurrentAmmo--;

                lastShootTime = Time.time;
            }
            else
            {
                if (Time.time > 0.3f + lastEmptyBulletTime)
                {
                    ActivePrimaryGun.AudioConfig.PlayOutOfAmmoClip(ActivePrimaryGun.ShootingAudioSource);
                    lastEmptyBulletTime = Time.time;
                }
            }
        }
    }

    private void Upgrade(int CurrentLevel)
    {
        if(CurrentLevel == Dual_Handgun_Unlock_Level)
        {
            Dual_Handgun = true;
        }
        else if(CurrentLevel == Dual_Uzi_Unlock_Level)
        {
            Dual_Uzi = true;
        }
    }

    public bool ChangeWeaponEditor;
    public void ChangeWeapon()
    {
        if (ActivePrimaryGun != null && ActivePrimaryGun.CurrentAmmo == 0)
            lastShootTime = Time.time;

        ChangeWeaponEditor = false;
        if (PrimaryGunParent.childCount > 0) Destroy(PrimaryGunParent.GetChild(0).gameObject);
        if (SecondaryGunParent.childCount > 0) Destroy(SecondaryGunParent.GetChild(0).gameObject);
        switch(PlayerWeapon)
        {
            case PlayerWeapons.Handgun:
                PrimaryGun = GunType.P_Handgun;
                SecondaryGun = GunType.S_Handgun;
                EquipPrimary();
                if (Dual_Handgun)
                {
                    EquipSecondary();
                }
                PlayerAnimatorManager.UpdateAimLayer();
                break;
            case PlayerWeapons.Uzi:
                PrimaryGun = GunType.P_Uzi;
                SecondaryGun = GunType.S_Uzi;
                EquipPrimary();
                if (Dual_Uzi)
                    EquipSecondary();
                PlayerAnimatorManager.UpdateAimLayer();
                break;
        }
    }
}