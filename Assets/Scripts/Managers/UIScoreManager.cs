using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreManager : MonoBehaviour
{
    public static UIScoreManager Instance;

    public TMP_Text ScoreText;

    public Image Bar;
    public RectTransform SliderButton;
    public float BarFillMaxAmount;
    [Range(0f, 1f)]
    public float BarPercentage;

    public void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        
        Instance = this;
    }

    public void ChangeScoreLevel(int newLevel)
    {
        ScoreText.text = newLevel.ToString();
    }

    public void ChangeBarPercentage(float CurrentScore, float MaxScore, float MinScore)
    {
        BarPercentage = (CurrentScore - MinScore) / (MaxScore - MinScore);
        float buttonAngle = BarPercentage * 360 * Bar.fillAmount;
        SliderButton.localEulerAngles = new Vector3(0, 0, buttonAngle);
    }
}