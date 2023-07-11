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

        PlayerWeapons randomWeaponLoot = (PlayerWeapons)Random.Range(0, System.Enum.GetNames(typeof(PlayerWeapons)).Length);
        
        GunScriptableObject gun;

        switch (randomWeaponLoot)
        {
            case PlayerWeapons.Handgun:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Handgun);
                gun.CurrentAmmo = gun.AmmoConfig.MaxAmmo;
                break;
            case PlayerWeapons.Uzi:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Uzi);
                gun.CurrentAmmo = gun.AmmoConfig.MaxAmmo;
                break;
        }

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