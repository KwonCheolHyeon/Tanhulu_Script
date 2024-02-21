using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialTuchEvent : MonoBehaviour
{
    private bool orderOn = false;
    public bool GetOrederOn() { return orderOn; }

    [SerializeField]
    private GameObject order_Sheet_Panel;
   
    private void Start()
    {
        orderOn = false;
    }
    void OnMouseDown()
    {
        if (this.transform.parent.gameObject.GetComponent<TutorialOrderManager>().GetTutorialEnd() && TutorialManager.Instance.GetMeaseaging() == false)
        {
            Touch();
        }
    }

    public void Touch()
    {
        order_Sheet_Panel.SetActive(true);
        SoundManager.Instance.PlaySFXSound("Click");
        TutorialManager.Instance.NextDetailTutorialStep();
        orderOn = true;
    }
}
