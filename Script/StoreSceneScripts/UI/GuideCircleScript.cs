using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideCircleScript : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, 20.0f * Time.deltaTime);
    }
}
