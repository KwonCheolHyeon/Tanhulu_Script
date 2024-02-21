using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    float time = 0;

    private void Update()
    {
        time += Time.deltaTime;

        if (time > 1.0f) // ±ôºý
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); //²¨Á³´Ù°¡

            if(time > 2.0f)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                time = 0.0f;
            }
        }       
    }

}
