using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TitleButtonManager : MonoBehaviour
{
    public GameObject startButton;
    public GameObject endButton;

    public GameObject optionButton;

    private RectTransform rectTr;
    private Button btn;
    
    // 버튼 클릭시 변경전 버튼 Size 
    private Vector2 prevSize = Vector2.zero;

    private void Start()
    {
        if (startButton == null)
        {
            startButton = GameObject.Find("Start_Button");
        }
        if (endButton == null)
        {
            endButton = GameObject.Find("End_Button");
        }
        if (optionButton == null)
        {
            optionButton = GameObject.Find("Option_Button");
        }
    }

    public void SetClickSize()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        switch (obj.name)
        {
            case "Start_Button":
                rectTr = startButton.GetComponent<RectTransform>();
                btn = startButton.GetComponent<Button>();
                break;

            default:
                break;
        }

        prevSize = rectTr.sizeDelta;
        Vector2 size = prevSize * 1.2f;
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x );
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    public void SetReSize()
    {
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, prevSize.x);
        rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, prevSize.y);
    }

    // 마우스가 설정한 버튼위에 있는지 확인, 있으면 true 
    public bool IsMousePosInButton()
    {
        Vector2 localMousePos = rectTr.InverseTransformPoint(Input.mousePosition);

        if(rectTr.rect.Contains(localMousePos))
        {
            return true;
        }

        SetReSize();
        return false;
    }
}
