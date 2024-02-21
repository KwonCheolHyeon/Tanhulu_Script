using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
