using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image WeaponImage;
    public PlayerWeapons WeaponType;

    private Vector3 startPos;

    void Awake()
    {
        startPos = WeaponImage.GetComponent<RectTransform>().anchoredPosition3D;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (WeaponImage.color.a == 0)
            return;
        WeaponImage.raycastTarget = false;

        WeaponImage.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        WeaponImage.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        WeaponImage.raycastTarget = true;

        WeaponImage.GetComponent<RectTransform>().anchoredPosition3D = startPos;
    }

    // Here, the target refers to the *object* currently being dragged, not to the *object* mouse pointing
    public void OnDrop(PointerEventData eventData)
    {
        WeaponSlot targetSlot = eventData.pointerDrag.GetComponent<WeaponSlot>();               // TargetSlot = Dragged Object
        if (targetSlot == null)                                     // this.WeaponImage = Target Object
            return;

        DropWeapon(targetSlot);
    }

    private void DropWeapon(WeaponSlot targetSlot)
    {
        Sprite targetSprite = targetSlot.WeaponImage.sprite;
        targetSlot.WeaponImage.sprite = WeaponImage.sprite;
        WeaponImage.sprite = targetSprite;

        PlayerWeapons targetWeaponType = targetSlot.WeaponType;
        targetSlot.WeaponType = WeaponType;
        WeaponType = targetWeaponType;

        Color targetColor = targetSlot.WeaponImage.color;
        targetSlot.WeaponImage.color = WeaponImage.color;
        WeaponImage.color = targetColor;
    }
}