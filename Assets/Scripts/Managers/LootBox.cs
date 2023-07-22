using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    public int RespawnTime;

    public AudioClip PickupSound;
    
    [Range(0f, 1f)]
    public float PickUpVolume;

    private BoxCollider boxCollider;

    private AudioSource audioSource;

    private List<GameObject> boxes = new List<GameObject>();

    private PlayerGunSelector playerGunSelector;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < transform.childCount; i++)
        {
            boxes.Add(transform.GetChild(i).gameObject);
        }

        playerGunSelector = GameManager.Instance.Player.GetComponent<PlayerGunSelector>();
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        RandomLoot();
    }

    void RandomLoot()
    {
        audioSource.PlayOneShot(PickupSound, PickUpVolume);

        PlayerWeapons randomWeaponLoot;
        if (playerGunSelector.WeaponLastUnlockIndex == 0)
            randomWeaponLoot = PlayerWeapons.Uzi;
        else
            randomWeaponLoot = (PlayerWeapons)Random.Range(1, playerGunSelector.WeaponLastUnlockIndex + 1);

        GunScriptableObject gun;
        switch (randomWeaponLoot)
        {
            case PlayerWeapons.Handgun:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Handgun);
                break;
            case PlayerWeapons.Uzi:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Uzi);
                break;
            case PlayerWeapons.Shotgun:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Shotgun);
                break;
            case PlayerWeapons.Rocket:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Rocket);
                break;
            default:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Handgun);
                break;
        }
        gun.CurrentAmmo = gun.AmmoConfig.MaxAmmo;
        if (playerGunSelector.ActivePrimaryGun == gun) UIWeaponManager.Instance.ChangeAmmoText(gun.CurrentAmmo);

        foreach (GameObject box in boxes)
        {
            box.SetActive(false);
        }

        boxCollider.enabled = false;

        StartCoroutine(RespawnRoutine(RespawnTime));
    }

    IEnumerator RespawnRoutine(int RespawnTime)
    {
        yield return new WaitForSeconds(RespawnTime);

        foreach (GameObject box in boxes)
        {
            box.SetActive(true);
        }

        boxCollider.enabled = true;
    }
}
