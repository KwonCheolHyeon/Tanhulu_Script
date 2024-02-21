using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeEffect : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 10.0f)]
    private float fadeTime;

    // 페이드 효과에 사용되는 검은 바탕 이미지
    private Image image;
    public Image GetEffectImage() { return image; } 

    private bool isFadeIn = false;
    public void SetFadeIn(bool _isFadeIn) { isFadeIn = _isFadeIn; }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator Fade(float start, float end)
    {
        float startTime = Time.time;
        float currentTime = 0.0f;
        float percent = 0.0f;

        while(percent < 1)
        {
            currentTime = Time.time - startTime;
            percent = currentTime / fadeTime;

            Color color = image.color;
            color.a = Mathf.Lerp(start, end, percent); 
            image.color = color;

            yield return null;
        }
    }
}
