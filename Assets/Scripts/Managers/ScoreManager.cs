using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int KillCount;

    [Space]
    [Header("*********Level To Score Variables************")]
    public AnimationCurve LevelToScoreCurve; 
    public List<int> NextScoreLevelScoreRequirementList;            // Filled in OnValidate
    public int BaseStartRequiredScore;
    public int ScoreRequiredForLevelHundred;
    
    [Space]
    public AnimationCurve ScoreLevelDecreaseTimeCurve;
    
    [Space]
    [Header("***********Score Values Editor View**************")]
    [SerializeField] private int _MaxScoreLevel;
    public int MaxScoreLevel
    {
        get { return _MaxScoreLevel; }
        set
        {
            _MaxScoreLevel = value;
            onUnlockUpgradeList?.Invoke(MaxScoreLevel + 1);
        }
    }
    [SerializeField] private int _CurrentScoreLevel;
    public int CurrentScoreLevel
    {
        get { return _CurrentScoreLevel; }
        set
        {
            _CurrentScoreLevel = value;

            UIScoreManager.Instance.ChangeScoreLevel(value + 1);
        }
    }
    
    [Space]
    [SerializeField] private float _CurrentScore;
    public float CurrentScore
    {
        get { return _CurrentScore; }
        private set 
        { 
            _CurrentScore = value;
        }
    }
    public int LastLevelScoreRequirement;
    public int NextLevelScoreRequirement;



    // Hidden Fields
    public delegate void OnUnlockUpgradeList(int level);
    public OnUnlockUpgradeList onUnlockUpgradeList;
    
    private float ScoreLevelDecreaseTime;

    void OnValidate()
    {
        NextScoreLevelScoreRequirementList.Clear();
        ScoreRequiredForLevelHundred = 0;

        Keyframe lastFrame = LevelToScoreCurve[LevelToScoreCurve.length - 1];
        int lastKeyTime = (int)lastFrame.time;
        for (int i = 0; i < lastKeyTime; i++)
        {
            NextScoreLevelScoreRequirementList.Add((int)LevelToScoreCurve.Evaluate(i));
            ScoreRequiredForLevelHundred += NextScoreLevelScoreRequirementList[i];
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ScoreLevelDecreaseTime = ScoreLevelDecreaseTimeCurve.Evaluate(0);
        NextLevelScoreRequirement = NextScoreLevelScoreRequirementList[0];
    }

    public void IncreaseScore(int Point)
    {
        KillCount++;

        CurrentScore += Point;

        while (CurrentScore >= NextLevelScoreRequirement)
        {
            CurrentScoreLevel++;

            LastLevelScoreRequirement = NextLevelScoreRequirement;
            NextLevelScoreRequirement += NextScoreLevelScoreRequirementList[CurrentScoreLevel];

            if (CurrentScoreLevel > MaxScoreLevel) MaxScoreLevel = CurrentScoreLevel;

            ScoreLevelDecreaseTime = ScoreLevelDecreaseTimeCurve.Evaluate(CurrentScoreLevel);
        }
        UIScoreManager.Instance.ChangeBarPercentage(CurrentScore, NextLevelScoreRequirement, LastLevelScoreRequirement);
    }

    void Update()
    {
        DecreaseScoreLevel();
    }

    void DecreaseScoreLevel()
    {
        if (CurrentScore <= 0)
        {
            CurrentScore = 0;
            return;
        }
        else
        {
            CurrentScore -= (NextLevelScoreRequirement - LastLevelScoreRequirement) * Time.deltaTime / ScoreLevelDecreaseTime;

            if (CurrentScoreLevel > 0)
            {
                if (CurrentScore <= LastLevelScoreRequirement)
                {
                    CurrentScoreLevel--;

                    NextLevelScoreRequirement = LastLevelScoreRequirement;
                    if (CurrentScoreLevel == 0)
                        LastLevelScoreRequirement = 0;
                    else
                        LastLevelScoreRequirement -= NextScoreLevelScoreRequirementList[CurrentScoreLevel];


                    ScoreLevelDecreaseTime = ScoreLevelDecreaseTimeCurve.Evaluate(CurrentScoreLevel);
                }
            }
            UIScoreManager.Instance.ChangeBarPercentage(CurrentScore, NextLevelScoreRequirement, LastLevelScoreRequirement);
        }
    }
}