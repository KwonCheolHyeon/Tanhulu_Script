using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TotalAchievementScoreText : MonoBehaviour
{
    private string totalScoretext;
    public TextMeshProUGUI totalScore;

    void Start()
    {
        totalScoretext = AchievementControlManager.Instance.GetTotalScore().ToString();

        totalScore.text = totalScoretext;
    }

}
