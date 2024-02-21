using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    int prevMoney = - 10;

    void FixedUpdate()
    {
       if(prevMoney != UserDataControlManager.Instance.GetPlayerMoney())
        {
            GetComponent<TextMeshProUGUI>().text = "";
            GetComponent<TextMeshProUGUI>().text += UserDataControlManager.Instance.GetPlayerMoney();
            prevMoney = UserDataControlManager.Instance.GetPlayerMoney();
        }
    }
}
