using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEditor.PlayerSettings;

public class ButtonBlinkingScript : MonoBehaviour
{
    public float resizeDuration = 2f; // Time duration for resizing in seconds
    public float initialScaleX = 0.9f;
    public float initialScaleY = 0.9f;
    public float finalScaleX = 1.5f;
    public float finalScaleY = 1.5f;
    public float lerpSpeed = 2f; // Adjust the speed of the interpolation

    private bool isGrowing = true;

    private bool isStartCoroutine = false;

    private void Start()
    {
        this.transform.localScale = new Vector3(initialScaleX, initialScaleY, 1f);
    }

    private void OnEnable()
    {
        isStartCoroutine = true;
        this.transform.localScale = Vector3.one;

        StartCoroutine(ResizeButtonContinuously());
    }

    private void Update()
    {

        if (this.transform.parent.gameObject.activeSelf == false && isStartCoroutine == true)
        {
            isStartCoroutine = false;
            StopAllCoroutines();
        }

    }
    IEnumerator ResizeButtonContinuously()
    {
        while (true)
        {
            float startTime = Time.time;

            while (Time.time - startTime < resizeDuration)
            {
                float t = (Time.time - startTime) / resizeDuration;

                if (isGrowing)
                {
                    // Grow from initial to final scale
                    this.transform.localScale = Vector3.Lerp(new Vector3(initialScaleX, initialScaleY, 1f), new Vector3(finalScaleX, finalScaleY, 1f), t);
                }
                else
                {
                    // Shrink from final to initial scale
                    this.transform.localScale = Vector3.Lerp(new Vector3(finalScaleX, finalScaleY, 1f), new Vector3(initialScaleX, initialScaleY, 1f), t);
                }

                yield return null;
            }

            // Toggle between growing and shrinking
            isGrowing = !isGrowing;
        }
    }
}
