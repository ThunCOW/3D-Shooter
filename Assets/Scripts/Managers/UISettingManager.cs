using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingManager : MonoBehaviour
{
    public GameObject PausedText;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                PausedText.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                PausedText.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}
