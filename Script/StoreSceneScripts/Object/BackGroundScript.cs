using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScript : MonoBehaviour
{
    void Start()
    {
        BackGroundManager.Instance.SetBackGroundImage(transform.Find("BackGround_Object").gameObject);
        
    }

    void Update()
    {
        if(this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name 
            != BackGroundManager.Instance.GetBGImageData().UsingTrueWallList[0].name)
        {
            BackGroundManager.Instance.SetBackGroundImage(transform.Find("BackGround_Object").gameObject);
        }
    }
}
