using AssetKits.ParticleImage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SettlementScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> starObjects;
    [SerializeField]
    private List<GameObject> starEffectObjects;
    [SerializeField]
    private GameObject allStarEffectObject;
    [SerializeField]
    private GameObject noStarEffectObject;
    [SerializeField]
    private GameObject[] explaneText;
    [SerializeField]
    private Sprite[] starImage;//0 꺼진거 1 켜진거
    [SerializeField]
    private GameObject particleObject;//0 꺼진거 1 켜진거
    [SerializeField]
    private GameObject calcCanvas;//정산씬
    [SerializeField]
    private GameObject tabCanvas;//상점씬
    [SerializeField]
    private GameObject storeButton;// 정산씬에 있는 상점으로 가는 버튼
    [SerializeField]
    private GameObject[] moneyAndStarObject;
    [SerializeField]
    private GameObject coinEffectObject;
    private TextMeshProUGUI[] cachedTextComponents;

    private int makeTanhulu = 0;
    private int fiveTanhulu = 0;
    private int failTanhulu = 0;
 
    private int money = 0;

    private bool doneCalc = true;
    private Coroutine moneyCountCoroutine = null;
    private Coroutine starCountCoroutine = null;
    public bool IsDoneCalc() { return doneCalc; }
    public void SetMoney(int _money) { money = _money; }
    public void SetMakeTanhulu(int _makeTanhulu) { makeTanhulu = _makeTanhulu; }
    public void SetFiveTanhulu(int _fiveTanhulu) { fiveTanhulu = _fiveTanhulu; }
    public void SetFailTanhulu(int _failTanhulu) { failTanhulu = _failTanhulu; }

    private void Start()
    {
        calcCanvas.SetActive(true);
        storeButton.SetActive(false);
        tabCanvas.SetActive(false);
        cachedTextComponents = new TextMeshProUGUI[explaneText.Length];
        for (int i = 0; i < explaneText.Length; i++)
        {
            cachedTextComponents[i] = explaneText[i].GetComponent<TextMeshProUGUI>();
            explaneText[i].GetComponent<TextMeshProUGUI>().text = "";
        }

        for (int i = 0; i < 3; i++)
        {
            starObjects[i].GetComponent<UnityEngine.UI.Image>().sprite = starImage[0];
            starEffectObjects[i].SetActive(false);
        }
        allStarEffectObject.SetActive(false);
        noStarEffectObject.SetActive(false);
        particleObject.SetActive(false);
        ///난이도에 따라 조절 예정
        int starCount = 0; 
        if (makeTanhulu >= 3) 
        {
            starCount += 1;
        }
        if (fiveTanhulu >= 1) 
        {
            starCount += 1;
        }
        if (failTanhulu <= 2)
        {
            starCount += 1;
        }
        SetStarEffect(starCount);
        doneCalc = true;

        
        moneyAndStarObject[0].GetComponent<TextMeshProUGUI>().text = (UserDataControlManager.Instance.GetPlayerMoney()).ToString();
        moneyAndStarObject[1].GetComponent<TextMeshProUGUI>().text = "Stars : " + (UserDataControlManager.Instance.GetPlayerStar()).ToString();
    }

    private void Update()
    {
      
    }

    public void SetStarEffect(int _star)
    {
        StartCoroutine(SetStarsCoroutine(_star));
    }

    private IEnumerator SetStarsCoroutine(int _star)
    {
        // 별이 없는 경우
        if (_star == 0)
        {
            noStarEffectObject.SetActive(true);
            for (int index = 0; index < 3; index++)
            {
                TextOn(index);
                yield return new WaitForSeconds(0.8f);
            }
        }
        else
        {
            // 별이 있는 경우
            for (int index = 0; index < 3; index++)
            {
                if (index < _star)
                {
                    starObjects[index].GetComponent<UnityEngine.UI.Image>().sprite = starImage[1];
                    starEffectObjects[index].SetActive(true);
                    switch (index)
                    {
                        case 0:
                            SoundManager.Instance.PlaySFXSound("Star1");
                            break;
                        case 1:
                            SoundManager.Instance.PlaySFXSound("Star2");
                            break;
                        case 2:
                            SoundManager.Instance.PlaySFXSound("Star3");
                            break;
                    }
                }
                else
                {
                    starObjects[index].GetComponent<UnityEngine.UI.Image>().sprite = starImage[0];
                    starEffectObjects[index].SetActive(false);
                }

                TextOn(index);
                // 3개의 별 모두 활성화된 경우
                if (index == 2 && _star == 3)
                {
                    allStarEffectObject.SetActive(true);
                    particleObject.SetActive(true);
                    SoundManager.Instance.PlaySFXSound("FinishStar");
                }
                yield return new WaitForSeconds(0.8f);
            }
        }
        TextOn(3);
        StartCoroutine(MoneyAndStarsEffects(money, _star));
        storeButton.SetActive(true);
    }

    IEnumerator MoneyAndStarsEffects(int _money, int _stars)//효과
    {
        yield return new WaitForSeconds(0.2f);
#if UNITY_EDITOR
        //_money = 10000;
#endif

        StartCoroutine(MoneyCount(_money, UserDataControlManager.Instance.GetPlayerMoney()));
        StartCoroutine(StarCount(_stars, UserDataControlManager.Instance.GetPlayerStar()));
    }

    // MoneyCount 코루틴 최적화
    IEnumerator MoneyCount(float target, float current) 
    {
        doneCalc = false;
        float duration = 0.3f;
        float finalTarget = current + target;
        finalTarget = Mathf.Max(finalTarget, 0);

        float offset = (finalTarget - current) / duration;

        while ((target >= 0 && current < finalTarget) || (target < 0 && current > finalTarget))
        {
            current += offset * Time.deltaTime;
            current = (target >= 0) ? Mathf.Min(current, finalTarget) : Mathf.Max(current, finalTarget);
            moneyAndStarObject[0].GetComponent<TextMeshProUGUI>().text = ((int)current).ToString();
            yield return null;
        }

        InstantiateCoinEffect();

        // 계산이 완료된 후 UserDataControlManager를 한 번만 업데이트
        UserDataControlManager.Instance.UpdateMoney((int)target);
        moneyAndStarObject[0].GetComponent<TextMeshProUGUI>().text = ((int)finalTarget).ToString();
        doneCalc = true;
    }

    private void InstantiateCoinEffect()
    {
        Transform moneyTextTr = GameObject.Find("Canvas").transform.Find("Money_Text");
        GameObject effectObj = GameObject.Instantiate(coinEffectObject);
        effectObj.transform.SetParent(calcCanvas.transform);
        effectObj.transform.localPosition = new Vector2(209, -398);
        effectObj.transform.localScale = new Vector3(1, 1, 1);
        effectObj.transform.GetChild(0).GetComponent<ParticleImage>().attractorTarget = moneyTextTr;
    }

    // target: 플레이어가 얻는 별, current: 현재 별
    IEnumerator StarCount(float target, float current) 
    {
        doneCalc = false;
        float duration = 0.5f; // 카운트 기간
        float finalTarget = current + target; // 최종 목표 계산
        finalTarget = Mathf.Max(finalTarget, 0); // 최종 목표가 0보다 작지 않은지 확인

        // 목표값에 따라 증가할지 감소할지 결정
        float offset = (finalTarget - current) / duration;
        if (target >= 0)
        {
            // Increasing stars
            while (current < finalTarget)
            {
                current += offset * Time.deltaTime;
                current = Mathf.Min(current, finalTarget); // Clamp current to avoid overshooting
                moneyAndStarObject[1].GetComponent<TextMeshProUGUI>().text = "Stars : " + ((int)current).ToString();
                yield return null;
            }
        }
        else
        {
            // Decreasing stars
            while (current > finalTarget)
            {
                current += offset * Time.deltaTime; // offset is negative here
                current = Mathf.Max(current, finalTarget); // Clamp current to avoid undershooting
                moneyAndStarObject[1].GetComponent<TextMeshProUGUI>().text = "Stars : " + ((int)current).ToString();
                yield return null;
            }
        }

        UserDataControlManager.Instance.UpdateStar((int)target);
        moneyAndStarObject[1].GetComponent<TextMeshProUGUI>().text = "Stars : " + ((int)finalTarget).ToString();
        doneCalc = true;
    }

    public void MoneyUpdate(float target) 
    {
        if (moneyCountCoroutine != null)
        {
            StopCoroutine(moneyCountCoroutine);
        }
        float currentMoney = UserDataControlManager.Instance.GetPlayerMoney();
        moneyCountCoroutine = StartCoroutine(MoneyCount(target, currentMoney));
    }

    public void StarUpdate(float target)
    {

        if (starCountCoroutine != null)
        {
            StopCoroutine(starCountCoroutine);
        }
        starCountCoroutine = StartCoroutine(StarCount(target, UserDataControlManager.Instance.GetPlayerStar()));
    }

    private void TextOn(int _a)
    {
        if (_a < cachedTextComponents.Length)
        {
            string text = _a switch
            {
                0 => "완성한 탕후루 갯수 : " + makeTanhulu,
                1 => "5성 이상 탕후루 갯수 : " + fiveTanhulu,
                2 => "버린 탕후루 갯수 : " + failTanhulu,
                3 => "오늘의 수익 : " + money,
                _ => ""
            };
            cachedTextComponents[_a].text = text;
        }
    }

    public void SetExit() 
    {
        calcCanvas.SetActive(false);
        tabCanvas.SetActive(true);
        SoundManager.Instance.PlayBGMSound("ItemStore");
    }
}
