using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarControlScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> StarObjects;
    [SerializeField]
    private GameObject moneyText;
    [SerializeField]
    private Sprite starSprite;
    [SerializeField]
    private Sprite redStarSprite;

    private void OnDisable()
    {
        for (int i = 0; i < StarObjects.Count; i++)
        {
            StarObjects[i].GetComponent<Image>().color = Color.white;
            StarObjects[i].GetComponent<Image>().sprite = starSprite;
        }
    }

    public void SetStar(int _score, int _money)
    {
        for (int i = _score; i < StarObjects.Count; i++)
        {
            StarObjects[i].GetComponent<Image>().color = Color.black;
        }
        //if(_score >= 5)
        //{
        //    for (int i = 5; i < _score; i++)
        //    {
        //        StarObjects[i - 5].GetComponent<Image>().sprite = redStarSprite;
        //    }
        //}

        moneyText.GetComponent<TextMeshProUGUI>().text = "Money : " + _money;

        UserDataControlManager.Instance.MakeTanhuluCounts(1);
        UserDataControlManager.Instance.EarnCurrentMoney(_money);

        if (_score >= 5)
        {
            UserDataControlManager.Instance.MakeFiveTanhuluCounts(1);
        }
    }
}
