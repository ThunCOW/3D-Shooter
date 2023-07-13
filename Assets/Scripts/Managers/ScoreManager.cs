using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int KillCount;

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

    //public List<int> NextScoreLevelPointRequirement;
    public int NextScoreLevelPointRequirement;

    public int MaxScoreLevel;
    public int CurrentScoreLevel;

    public AnimationCurve ScoreLevelDecreaseTimeCurve;
    private float ScoreLevelDecreaseTime;

    [Space]
    [Header("UI ")]
    public TMP_Text Text_CurrentScoreLevel;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ScoreLevelDecreaseTime = ScoreLevelDecreaseTimeCurve.Evaluate(0);    
    }

    public void IncreaseScore(int Point)
    {
        KillCount++;

        CurrentScore += Point;

        if (CurrentScore >= NextScoreLevelPointRequirement)
        {
            CurrentScoreLevel++;
            NextScoreLevelPointRequirement += CurrentScoreLevel * 100;
            if (CurrentScoreLevel > MaxScoreLevel) MaxScoreLevel = CurrentScoreLevel;

            ScoreLevelDecreaseTime = ScoreLevelDecreaseTimeCurve.Evaluate(CurrentScoreLevel);
        }
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
            CurrentScore -= 1000 * Time.deltaTime / ScoreLevelDecreaseTime;

            if (CurrentScoreLevel > 0)
            {
                if (CurrentScore <= NextScoreLevelPointRequirement - (CurrentScoreLevel * 100))
                {
                    CurrentScoreLevel--;

                    NextScoreLevelPointRequirement -= CurrentScoreLevel * 100;

                    ScoreLevelDecreaseTime = ScoreLevelDecreaseTimeCurve.Evaluate(CurrentScoreLevel);
                }
            }
        }
    }
}