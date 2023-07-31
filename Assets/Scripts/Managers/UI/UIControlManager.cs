using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlManager : MonoBehaviour
{
    public static UIControlManager Instance;

    public GameObject PausedScreen;

    // ************* Private Fields
    private bool showUpgradeList;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;
    }

    void Start()
    {
        PauseGame(false);
    }

    private void Update()
    {
        KeyboardControls();
    }

    private void KeyboardControls()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame(true);
                Time.timeScale = 0f;
            }
            else
            {
                PauseGame(false);
                Time.timeScale = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (Time.timeScale == 0)
            {
                showUpgradeList = !showUpgradeList;
                UIUpgradeList.Instance.Show(showUpgradeList);
                InvWeaponManager.Instance.Show(!showUpgradeList);
            }
        }
    }

    private void PauseGame(bool Pause)
    {
        GameManager.Instance.PauseGame = Pause;

        PausedScreen.SetActive(Pause);
        UIUpgradeList.Instance.Show(false);
        InvWeaponManager.Instance.Show(true);
        
        showUpgradeList = false;
    }
}
