using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayScript : MonoBehaviour
{

    // Update is called once per frame
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text += UserDataControlManager.Instance.GetPlayerDate();
    }
}
