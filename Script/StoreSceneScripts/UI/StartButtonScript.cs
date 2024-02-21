using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonScript : MonoBehaviour
{
    Exit canvasExitScript;

    private void Start()
    {
        canvasExitScript = FindObjectOfType<Exit>();
        if (canvasExitScript == null)
        {
            Debug.LogError("Exit script not found in the scene!");
            return;
        }
    }

    private void Update()
    {
        if (canvasExitScript != null && canvasExitScript.GetIsGameStart() == false)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

}
