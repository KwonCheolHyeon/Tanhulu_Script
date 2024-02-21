using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialSkewerSpawn : MonoBehaviour
{

    void Start()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
      
    }

    public void SpawnSkewer() 
    {
        TutorialManager.Instance.NextDetailTutorialStep();
    }


      
}
