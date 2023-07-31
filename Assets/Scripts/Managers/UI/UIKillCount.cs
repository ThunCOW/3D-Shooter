using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIKillCount : MonoBehaviour
{
    private TMP_Text killCountText;

    void Awake()
    {
        killCountText = GetComponent<TMP_Text>();    
    }

    void OnEnable()
    {
        killCountText.text = "Kill Count : " + GameManager.Instance.KillCount.ToString();
    }
}
