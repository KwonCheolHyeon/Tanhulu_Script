using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOrderManager : MonoBehaviour
{

    bool tutorialEnd;
    public bool GetTutorialEnd() { return tutorialEnd; }
    void Start()
    {
        tutorialEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<OrderManager>().GetOrder() && tutorialEnd == false) 
        {
            tutorialEnd =true ;
            TutorialManager.Instance.NextTutorialStep();
            this.enabled = false;   
        }
    }
}
