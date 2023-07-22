using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponManager : MonoBehaviour
{
    public static UIWeaponManager Instance;

    public Image WeaponImage;
    public Image AmmoInfiniteImage;
    public TMP_Text AmmoText;
    [SerializeField] private int _AmmoCount;
    public int AmmoCount
    {
        get { return _AmmoCount; }
        set 
        { 
            if (value <= -1)
            {
                AmmoText.gameObject.SetActive(false);
                AmmoInfiniteImage.gameObject.SetActive(true);
            }
            else
            {
                AmmoText.text = value.ToString();
                AmmoText.gameObject.SetActive(true);
                AmmoInfiniteImage.gameObject.SetActive(false);
                _AmmoCount = value;
            }
        }
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }

    public void ChangeWeapon(Sprite WeaponSprite, int Ammo)
    {
        WeaponImage.sprite = WeaponSprite;
        AmmoCount = Ammo;
    }

    public void ChangeAmmoText(int Ammo)
    {
        AmmoCount = Ammo;
    }
}
