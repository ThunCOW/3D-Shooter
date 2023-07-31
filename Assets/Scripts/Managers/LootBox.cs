using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    void OnTriggerEnter(Collider other)
    {
        RandomLoot();
    }

    void RandomLoot()
    {
        audioSource.PlayOneShot(PickupSound, PickUpVolume);

        PlayerWeapons randomWeaponLoot;
        if ((int)playerGunSelector.WeaponLastUnlock == 0)
            randomWeaponLoot = PlayerWeapons.Uzi;
        else
            randomWeaponLoot = (PlayerWeapons)Random.Range(1, (int)playerGunSelector.WeaponLastUnlock + 1);

        GunScriptableObject gun;
        switch (randomWeaponLoot)
        {
            case PlayerWeapons.Handgun:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Handgun);
                Debug.LogWarning("Shouldn't enter here");
                break;
            case PlayerWeapons.Uzi:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Uzi);
                if ((int)playerGunSelector.WeaponLastUnlock != 0) SpawnLootNotification("Uzi Ammo is replenished");
                break;
            case PlayerWeapons.Shotgun:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Shotgun);
                SpawnLootNotification("Shotgun Ammo is replenished");
                break;
            case PlayerWeapons.Rocket:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Rocket);
                SpawnLootNotification("Rocket Launcher Ammo is replenished");
                break;
            default:
                gun = playerGunSelector.Guns.Find(gun => gun.ID == GunType.P_Handgun);
                Debug.LogWarning("Shouldn't enter here");
                break;
        }
        if ((int)playerGunSelector.WeaponLastUnlock == 0) SpawnLootNotification("Handgun Ammo Is Full");
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

    // TODO: Make a centralized spawnnotificationmanager where scripts can call functions to spawn notifications
    private void SpawnLootNotification(string message)
    {
        UIUpgradeList.Instance.activeNotificationCount++;

        GameObject UpgradePopup = Instantiate(UIUpgradeList.Instance.UpgradePopupPrefab, UIUpgradeList.Instance.UpgradeNotificationSpawnParent.transform, false);
        TMP_Text popupText = UpgradePopup.GetComponentInChildren<TMP_Text>();
        popupText.text = message;
        popupText.color = Color.white;

        UpgradePopup.transform.localPosition = new Vector3(UpgradePopup.transform.localPosition.x, UpgradePopup.transform.localPosition.y + UIUpgradeList.Instance.activeNotificationCount * 20, UpgradePopup.transform.localPosition.z);

        StartCoroutine(UIUpgradeList.Instance.TextDisappearSlowly(popupText.gameObject, 1.5f, 1.5f));
    }
}
