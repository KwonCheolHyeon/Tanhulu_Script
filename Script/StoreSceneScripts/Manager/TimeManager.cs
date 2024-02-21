using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{

    [SerializeField] 
    private GameObject time_Slider;
    private float coolTime;

    public float currentCoolTime;
    private bool isTimeOver = false;
    public bool GetTimeOver() { return isTimeOver; }

    private bool isTutorialScene = false;

    private static TimeManager instance;

    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("TimeManager instance is not found!");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
#if UNITY_EDITOR
       string scneName = SceneManager.GetActiveScene().name;
       if (scneName == "TutorialStoreScene") 
           isTutorialScene = true;
       else
           isTutorialScene = false;

       coolTime = 60.0f;
#elif UNITY_ANDROID
        string scneName = SceneManager.GetActiveScene().name;
        if(scneName == "TutorialStoreScene")
            isTutorialScene = true;
        else
            isTutorialScene = false;

        coolTime = 180.0f;
#endif
        currentCoolTime = coolTime;
        isTimeOver = false;

        if(isTutorialScene == false)
            StartCoroutine(CooldownProcess());
    }

    private IEnumerator CooldownProcess()
    {
        while (currentCoolTime > 0.01)
        {
            // 부드러운 애니메이션을 위해 WaitForSeconds 대신 null 사용
            yield return null; 

            currentCoolTime -= Time.deltaTime;

            float fillAmount = currentCoolTime / coolTime;
            time_Slider.GetComponent<Slider>().value = fillAmount;
        }

        SoundManager.Instance.PlaySFXSound("TimeOver");
        isTimeOver = true;
    }
    public void TimeStop()
    {
        Time.timeScale = 0;
    }

    public void TimeStart()
    {
        Time.timeScale = 1;
    }
}
