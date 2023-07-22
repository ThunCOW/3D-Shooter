using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GunType is being used to find the gun from the Guns list, like an ID
public enum GunType
{
    P_Handgun, S_Handgun,
    P_Uzi, S_Uzi,
    P_Shotgun,
    P_Rocket,
}

// Player weapons are the main weapons player will see and select
public enum PlayerWeapons
{
    Handgun,
    Uzi,
    Shotgun,
    Rocket,
}

public class PlayerGunSelector : MonoBehaviour
{
    AnimatorManager PlayerAnimatorManager;
    
    [Header("************Editor Filled*************")]
    [SerializeField] private Transform PrimaryGunParent;
    [SerializeField] private Transform SecondaryGunParent;

    [Space]
    [Header("**************Runtime Filled*****************")]

    [SerializeField] private GunType PrimaryGun;
    [SerializeField] private GunType SecondaryGun;
    [Space]
    public GunScriptableObject ActivePrimaryGun;
    public GunScriptableObject ActiveSecondaryGun;
    [Space]
    [SerializeField] List<GunScriptableObject> GunsSO;
    public List<GunScriptableObject> Guns;

    [SerializeField] private PlayerWeapons _playerWeapons;
    public PlayerWeapons PlayerWeapon
    {
        get { return _playerWeapons; }
        set 
        {
            _playerWeapons = value;
            EquipWeapon();
        }
    }

    [Header("************Weapon Unlock Variables**********")]
    public int Uzi_Unlock_Level;
    public int Shotgun_Unlock_Level;
    public int Rocket_Launcher_Unlock_Level;
    public int Dual_Handgun_Unlock_Level;
    public int Dual_Uzi_Unlock_Level;

    //**************** Runtime Filled ****************
    public int WeaponLastUnlockIndex;                               // The equivelant of PlayerWeapons in index, consider it like the last unlocked index in PlayerWeapons, starts with 0 which is only handgun

    [Header("*************Weapon Unlock Editor Variables***********")]
    [SerializeField] private bool Dual_Handgun;
    [SerializeField] private bool Dual_Uzi;

    private void Awake()
    {
        foreach (GunScriptableObject gun in GunsSO)
            Guns.Add(gun.Clone() as GunScriptableObject);
    }

    private void Start()
    {
        PlayerAnimatorManager = GameManager.Instance.Player.GetComponentInChildren<AnimatorManager>();

        EquipWeapon();

        ScoreManager.Instance.onUnlockUpgradeList += UnlockWeapon;
    }

    private void Update()
    {
        if (ChangeWeaponOnRuntime) EquipWeapon();
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

        UIWeaponManager.Instance.ChangeWeapon(ActivePrimaryGun.Sprite, ActivePrimaryGun.CurrentAmmo);
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

    public bool ChangeWeaponOnRuntime;
    private void EquipWeapon()
    {
        //if (ActivePrimaryGun != null && ActivePrimaryGun.CurrentAmmo == 0) lastShootTime = Time.time;

        ChangeWeaponOnRuntime = false;
        if (PrimaryGunParent.childCount > 0) Destroy(PrimaryGunParent.GetChild(0).gameObject);
        if (SecondaryGunParent.childCount > 0) Destroy(SecondaryGunParent.GetChild(0).gameObject);
        switch (PlayerWeapon)
        {
            case PlayerWeapons.Handgun:
                PrimaryGun = GunType.P_Handgun;
                SecondaryGun = GunType.S_Handgun;
                EquipPrimary();
                PlayerAnimatorManager.UpdateAimLayer();
                if (Dual_Handgun)
                    EquipSecondary();
                break;
            case PlayerWeapons.Uzi:
                PrimaryGun = GunType.P_Uzi;
                SecondaryGun = GunType.S_Uzi;
                EquipPrimary();
                PlayerAnimatorManager.UpdateAimLayer();
                if (Dual_Uzi)
                    EquipSecondary();
                break;
            case PlayerWeapons.Shotgun:
                PrimaryGun = GunType.P_Shotgun;
                EquipPrimary();
                break;
            case PlayerWeapons.Rocket:
                PrimaryGun = GunType.P_Rocket;
                EquipPrimary();
                break;
            default:
                Debug.LogWarning("Add Weapon Here");
                break;
        }
        PlayerAnimatorManager.UpdateAimLayer();
    }

    private bool primaryShoot;
    private float lastEmptyBulletTime;
    public void Shoot()
    {
        if (Time.time > ActivePrimaryGun.ShootConfig.FireRate + ActivePrimaryGun.lastShootTime)
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
                UIWeaponManager.Instance.ChangeAmmoText(ActivePrimaryGun.CurrentAmmo);

                ActivePrimaryGun.lastShootTime = Time.time;
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

    private void UnlockWeapon(int CurrentLevel)
    {
        if (CurrentLevel == Uzi_Unlock_Level)
        {
            WeaponLastUnlockIndex = 1;
        }
        else if(CurrentLevel == Shotgun_Unlock_Level)
        {
            WeaponLastUnlockIndex = 2;
        }
        else if(CurrentLevel == Rocket_Launcher_Unlock_Level)
        {
            WeaponLastUnlockIndex = 3;
        }

        if(CurrentLevel == Dual_Handgun_Unlock_Level)
        {
            Dual_Handgun = true;
            if (ActivePrimaryGun.WeaponType == PlayerWeapons.Handgun)
                SelectWeapon((int)PlayerWeapons.Handgun);
        }
        else if(CurrentLevel == Dual_Uzi_Unlock_Level)
        {
            Dual_Uzi = true;
            if (ActivePrimaryGun.WeaponType == PlayerWeapons.Uzi)
                SelectWeapon((int)PlayerWeapons.Uzi);
        }
    }

    public void ScrollWeapon(int indexChange)
    {
        if (WeaponLastUnlockIndex == 0)
            return;
        PlayerWeapon = (PlayerWeapons)mod((((int)PlayerWeapon) + indexChange), WeaponLastUnlockIndex);
    }

    public void SelectWeapon(int index)
    {
        if (index <= WeaponLastUnlockIndex)
            PlayerWeapon = (PlayerWeapons)index;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}