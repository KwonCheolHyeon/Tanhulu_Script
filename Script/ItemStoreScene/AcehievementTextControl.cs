using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcehievementTextControl : MonoBehaviour
{
    public int index;
    private int achievementStatus;

    public Sprite[] starSprite;
    private Image achievementImage;

    private TextMeshProUGUI titleName;
    private TextMeshProUGUI description;

    private string titleNamefromConMgr;
    private string descriptionfromConMgr;

    private void Start()
    {
        // AchievementControlManager에서 index더 추가하면 여기도 늘어나도록 설정하기
        if (index >= 7)
        {
            Debug.Log("잘못된 index번호 입력");
        }
        else
        {
            titleName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            description = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            achievementImage = transform.GetChild(2).GetComponent<Image>();

            titleNamefromConMgr = AchievementControlManager.Instance.GetAchievementTitleText(index);
            descriptionfromConMgr = AchievementControlManager.Instance.GetAchievementDescription(index);

            ChangeStatusByIndex(index);
            titleName.text = titleNamefromConMgr;
            description.text = descriptionfromConMgr;
        }

    }

    public void ChangeImage(int _starNumber)
    {
        achievementImage.sprite = starSprite[_starNumber];
    }

    public void ChangeStatusByIndex(int _index)
    {
        achievementStatus = AchievementControlManager.Instance.GetAchievementStatus(_index);

        switch(achievementStatus)
        {
            case 0:
                ChangeImage(0);
                break;
            case 1:
                ChangeImage(1);
                break;
            case 2:
                ChangeImage(2);
                break;
            case 3:
                ChangeImage(3);
                break;
            default:
                break;
        }

    }

}
