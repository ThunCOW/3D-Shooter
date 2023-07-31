using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvWeaponManager : MonoBehaviour
{
    public static InvWeaponManager Instance;

    public GameObject WeaponSlotsParent;
    [Space]

    public bool PopulateWeaponSlots;
    public List<WeaponSlot> WeaponSlots;

    public List<GunScriptableObject> GunSO;

    private int currentSlotIndex;
    private PlayerWeapons lastUnlockedWeapon;

    void OnValidate()
    {
        if (PopulateWeaponSlots)
        {
            PopulateWeaponSlots = false;
            
            InitializeWeaponSlots();
        }
    }

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;    
    }

    private void Start()
    {
        InitializeWeaponSlots();

        for (int i = 1; i < WeaponSlots.Count; i++)
        {
            Color transparent = Color.white;
            transparent.a = 0;
            WeaponSlots[i].WeaponImage.color = transparent;
        }
    }

    private void InitializeWeaponSlots()
    {
        WeaponSlots.Clear();
        WeaponSlots.AddRange(GetComponentsInChildren<WeaponSlot>(true));

        for (int i = 0; i < WeaponSlots.Count; i++)
        {
            if (i > GunSO.Count - 1)
            {
                Debug.LogWarning("GunSO's for InventoryManager is not set! There is " + (i + 1 - GunSO.Count) + " more slot than gun!");
                break;
            }
            WeaponSlots[i].WeaponType = GunSO[i].WeaponType;
            WeaponSlots[i].WeaponImage.sprite = GunSO[i].InventorySprite;
            WeaponSlots[i].WeaponImage.color = Color.white;
        }
    }

    public void UnlockWeapon(PlayerWeapons unlockedWeapon)
    {
        foreach(WeaponSlot slot in WeaponSlots)
        {
            if (slot.WeaponType == unlockedWeapon)
            {
                slot.WeaponImage.color = Color.white;
                lastUnlockedWeapon = unlockedWeapon;
            }
        }
        //WeaponSlots[index].WeaponImage.color = Color.white;
    }

    public PlayerWeapons ScrollWeapon(int indexChange, int WeaponLastUnlockIndex)
    {
        currentSlotIndex += indexChange;
        return WeaponSlots[mod(currentSlotIndex, WeaponLastUnlockIndex + 1)].WeaponType;
    }

    /*public PlayerWeapons SelectWeapon(int index)
    {
        currentSlotIndex = index;
        return (PlayerWeapons)WeaponSlots[currentSlotIndex].WeaponIndex;
    }*/

    public PlayerWeapons SelectWeapon(int index)
    {
        if ((int)lastUnlockedWeapon >= (int)WeaponSlots[index].WeaponType)
        {
            currentSlotIndex = index;
            return WeaponSlots[currentSlotIndex].WeaponType;
        }
        else
            return PlayerWeapons.Locked;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void Show(bool active)
    {
        WeaponSlotsParent.gameObject.SetActive(active);
    }
}
