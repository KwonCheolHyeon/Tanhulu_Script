using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStepManager : MonoBehaviour
{
    private bool toppingTableEnd = false;
    private bool IceBoxTableEnd = false;
    private bool coatingPageEnd = false;
    private bool coatingPageEnd2 = false;

    [SerializeField]
    private GameObject iceboxObject;
    private bool iceboxStep1Done = false;
    private bool iceboxStep2Done = false;

    [SerializeField]
    private GameObject custumObject;
    private bool customStepDone = false;

    void Start()
    {

    }
    void OnMouseDown()
    {
        

    }
    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.name == "Canvas" && IceBoxTableEnd == false) 
        {
            checkToppingTable();
        }

        if (this.gameObject.name == "Topping_Table" && coatingPageEnd2 ==false)
        {
            ToppingTableControl();
        }

        if (iceboxObject != null && iceboxStep2Done == false) 
        {
            if (iceboxObject.GetComponent<IceBoxScript>().IsStartTutorialIceBoxTutorial()) 
            {

                if (iceboxStep1Done == false)
                {
                    iceboxStep1Done = true;
                    TutorialManager.Instance.NextDetailTutorialStep();
                }
                if(iceboxStep2Done == false && iceboxStep1Done == true && iceboxObject.GetComponent<IceBoxScript>().IsSuccess())
                {
                    iceboxStep2Done = true;
                    TutorialManager.Instance.NextDetailTutorialStep();
                }
            }
        }

        if (custumObject != null) 
        {
            if (customStepDone == false && custumObject.GetComponent<CustomerScript>().GetIsCompleted2())
            {

                TutorialManager.Instance.NextDetailTutorialStep();
                customStepDone = true;
                this.enabled = false;
            }
        }


    }

    public void touching(int wichType)
    {
        switch (wichType)
        {
            case 0:
                if (TutorialManager.Instance.GetMeaseaging() == false && TutorialManager.Instance.GetfirstTochBolck())
                {
                    TutorialManager.Instance.NextDetailTutorialStep();
                }
                break;
            case 1:


                break;
            case 2:
                break;
            default:
                break;

        }

    }

    public void next() 
    {
        TutorialManager.Instance.NextTutorialStep();
        //this.enabled = false;
    }

    public void checkToppingTable()
    {
        if (this.GetComponent<Exit>().GeteCurrentTable() == CurrentTable.ToppingTable && toppingTableEnd == false) 
        {
            toppingTableEnd = true;
            TutorialManager.Instance.NextTutorialStep();
        }

        if (this.GetComponent<Exit>().GeteCurrentTable() == CurrentTable.IceBoxTable && IceBoxTableEnd == false)
        {
            TutorialManager.Instance.NextTutorialStep();
            IceBoxTableEnd = true;
           
        }
    }

    public void ToppingTableControl() 
    {
        if (this.GetComponent<CoatingModeSetting>().GetCotingMode() && coatingPageEnd == false)
        {
            coatingPageEnd = true;
        }

        if(coatingPageEnd == true && this.GetComponent<CoatingModeSetting>().GetIsDone() == true && coatingPageEnd2 == false) 
        {
            coatingPageEnd2 = true;
            TutorialManager.Instance.NextDetailTutorialStep();
        }


    }
}
