using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStoreTutorial : MonoBehaviour
{
    private int playerDays;
    private bool isDone = false;
    public void SetIsDone(bool _isDone) { isDone = _isDone; }

    public GameObject BlurPage;
    public GameObject Text;

    private void Start()
    {
        playerDays = UserDataControlManager.Instance.GetPlayerDate();
    }

    private void Update()
    {
        if (isDone || playerDays >=2)
        {
            BlurPage.SetActive(false);
            Text.SetActive(false);
        }
    }

    public void SetTutorial()
    {
        if (playerDays == 1)
        {
            BlurPage.SetActive(true);
            Text.SetActive(true);
        }
        else
        {
            BlurPage.SetActive(false);
            Text.SetActive(false);
        }
    }


}
