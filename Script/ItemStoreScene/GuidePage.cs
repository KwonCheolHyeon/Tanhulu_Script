using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidePage : MonoBehaviour
{
    private Image guidePageImage;

    public Sprite[] guideSprites;

    private int guideNumber = 0;

    private void Start()
    {
        guidePageImage = transform.GetChild(2).GetComponent<Image>();
        if (guideSprites.Length > 0)
        {
            guidePageImage.sprite = guideSprites[0];
            UpdateGuideNumber();
        }
        else
        {
            Debug.LogError("GuideSprites가 할당되지 않았습니다.");
        }
    }

    public void ChangeImage(bool isRight)
    {
        guideNumber = Mathf.Clamp(guideNumber + (isRight ? 1 : -1), 0, guideSprites.Length - 1);
        guidePageImage.sprite = guideSprites[guideNumber];
    }

    private void UpdateGuideNumber()
    {
        for (int i = 0; i < guideSprites.Length; i++)
        {
            if (guidePageImage.sprite == guideSprites[i])
            {
                guideNumber = i;
                return;
            }
        }

        Debug.LogError("가이드 스프라이트 오류");
    }
}
