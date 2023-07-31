using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class UIUpgradeList : MonoBehaviour
{
    public static UIUpgradeList Instance;

    [Header("********* Editor Filled *************")]
    [SerializeField] GameObject UpgradeList;
    
    [Header("************ Bar Variables **********")]
    public GameObject ParentList;
    public GameObject UpgradeBarUI;
    public float BarWidth;
    public float BarHeight;
    public float ListHeight;

    [Header("************ Upgrade List ***********")]
    public bool CreateList;
    public GameObject UpgradePopupPrefab;
    public GameObject UpgradeNotificationSpawnParent;
    public List<UpgradeContext> Upgrades;

    // Private Fields
    private RectTransform blackBackground;
    private int currentUpgradeIndex = 0;
    [HideInInspector] public int activeNotificationCount;
    private List<Transform> UpgradeListUI = new List<Transform>();
    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            for (int i = 1; i < ParentList.transform.childCount; i++)
                UpgradeListUI.Add(ParentList.transform.GetChild(i));

            blackBackground = ParentList.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

            blackBackground.transform.SetAsLastSibling();
            UpgradeListUI[0].SetAsLastSibling();
            
            UnlockUpgrade();
        }

    }
    void Update()
    {
        CreateUpgradeList();
    }

    public void UnlockUpgrade()
    {
        if (currentUpgradeIndex > Upgrades.Count - 1)
            return;

        // TODO : index out of range error here for the last upgrade
        if (ScoreManager.Instance.CurrentScoreLevel + 1 == Upgrades[currentUpgradeIndex].Level)
        {
            UpgradeListUI[currentUpgradeIndex].SetAsLastSibling();
            SpawnNotification();
            currentUpgradeIndex++;
        }
    }

    public void SpawnNotification()
    {
        activeNotificationCount++;
        
        GameObject UpgradePopup = Instantiate(UpgradePopupPrefab, UpgradeNotificationSpawnParent.transform, false);
        TMP_Text popupText = UpgradePopup.GetComponentInChildren<TMP_Text>();
        popupText.text = Upgrades[currentUpgradeIndex].Context;
        popupText.color = Upgrades[currentUpgradeIndex].ContextColor;

        UpgradePopup.transform.localPosition = new Vector3(UpgradePopup.transform.localPosition.x, UpgradePopup.transform.localPosition.y + activeNotificationCount * 20, UpgradePopup.transform.localPosition.z);

        StartCoroutine(TextDisappearSlowly(popupText.gameObject, 1.5f, 1.5f));
    }

    public IEnumerator TextDisappearSlowly(GameObject TextSpawn, float waitForWithoutAlphaChange = 0, float disappearTime = 1.5f, float speed = 10)
    {
        TMP_Text tempText = TextSpawn.GetComponent<TMP_Text>();
        Color tempColor = tempText.color;

        float wait = waitForWithoutAlphaChange;
        // First Continue Without Changing Alpha
        yield return new WaitForSeconds(waitForWithoutAlphaChange);
        /*while (wait > 0)
        {
            wait -= Time.deltaTime;

            TextSpawn.transform.position = new Vector3(TextSpawn.transform.position.x, TextSpawn.transform.position.y - (speed * Time.deltaTime), TextSpawn.transform.position.z);

            yield return new WaitForFixedUpdate();
        }*/

        wait = (tempColor.a / 1) * disappearTime;
        // Start Changing Alpha
        while (wait > 0)
        {
            wait -= Time.deltaTime;

            float percentage = waitForWithoutAlphaChange == 0 ? (wait / disappearTime) * 0.65f : (wait / disappearTime);

            tempColor.a = percentage;
            tempText.color = tempColor;

            TextSpawn.transform.position = new Vector3(TextSpawn.transform.position.x, TextSpawn.transform.position.y - (speed * Time.deltaTime), TextSpawn.transform.position.z);

            yield return new WaitForFixedUpdate();
        }
        Destroy(TextSpawn);

        activeNotificationCount--;
    }
    
    public void Show(bool active)
    {
        UpgradeList.gameObject.SetActive(active);
    }



    private void CreateUpgradeList()
    {
        if (CreateList)
        {
            CreateList = false;

            float currentHeight = 0;
            float currentWidth = 0;
            int i = ParentList.transform.childCount == 1 ? 0 : ParentList.transform.childCount - 1;
            for (; i >= 1; i--)
                DestroyImmediate(ParentList.transform.GetChild(i).gameObject);

            ParentList.transform.SetParent(ParentList.transform.parent);
            foreach (UpgradeContext context in Upgrades)
            {
                if (Mathf.Abs(currentHeight) < ListHeight)
                {
                    GameObject go = Instantiate(UpgradeBarUI, ParentList.transform);
                    go.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = context.Level.ToString();
                    go.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = context.Context;
                    go.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().color = context.ContextColor;
                    go.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentWidth, currentHeight, 0);
                    currentHeight -= BarHeight;
                }
                else
                {
                    currentHeight = 0;
                    currentWidth += BarWidth;
                    GameObject go = Instantiate(UpgradeBarUI, ParentList.transform);
                    go.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = context.Level.ToString();
                    go.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = context.Context;
                    go.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().color = context.ContextColor;
                    go.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentWidth, currentHeight, 0);
                    currentHeight -= BarHeight;
                }
            }

            blackBackground = this.ParentList.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            blackBackground.anchoredPosition = new Vector2((currentWidth + BarWidth) / 2, -ListHeight / 2);
            blackBackground.sizeDelta = new Vector2(currentWidth + BarWidth, ListHeight);
        }
    }

}

[System.Serializable]
public class UpgradeContext
{
    public int Level;
    public string Context;
    public Color ContextColor = Color.white;
}