using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchEvent : MonoBehaviour
{
    private bool orderOn = false;
    public bool GetOrederOn() { return orderOn; }

    private void Start()
    {
        orderOn = false;
    }
    void OnMouseDown()
    {
#if UNITY_EDITOR
        // 터치한 부분이 UI Object 라면 True 반환
        if (EventSystem.current.IsPointerOverGameObject() == false)
            Touch();
#elif UNITY_ANDROID
        if(GameObject.FindWithTag("Canvas").transform.Find("BlurPage").gameObject.activeSelf == false)
            Touch();
#endif
    }
    public void Touch()
    {

        GameObject obj = GameObject.FindWithTag("Canvas");
        if (obj != null)
        {
            Transform orderSheetPanel = obj.transform.Find("Order_Sheet_Panel");
            if (orderSheetPanel != null)
            {
                orderSheetPanel.gameObject.SetActive(true);
                SoundManager.Instance.PlaySFXSound("Click");
            }
        }
    }
}
