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
    private Sprite[] starImage;//0 ������ 1 ������
    [SerializeField]
    private GameObject particleObject;//0 ������ 1 ������
    [SerializeField]
    private GameObject calcCanvas;//�����
    [SerializeField]
    private GameObject tabCanvas;//������
    [SerializeField]
    private GameObject storeButton;// ������� �ִ� �������� ���� ��ư
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
        ///���̵��� ���� ���� ����
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
        // ���� ���� ���
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
            // ���� �ִ� ���
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
                // 3���� �� ��� Ȱ��ȭ�� ���
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

    IEnumerator MoneyAndStarsEffects(int _money, int _stars)//ȿ��
    {
        yield return new WaitForSeconds(0.2f);
#if UNITY_EDITOR
        //_money = 10000;
#endif

        StartCoroutine(MoneyCount(_money, UserDataControlManager.Instance.GetPlayerMoney()));
        StartCoroutine(StarCount(_stars, UserDataControlManager.Instance.GetPlayerStar()));
    }

    // MoneyCount �ڷ�ƾ ����ȭ
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

        // ����� �Ϸ�� �� UserDataControlManager�� �� ���� ������Ʈ
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

    // target: �÷��̾ ��� ��, current: ���� ��
    IEnumerator StarCount(float target, float current) 
    {
        doneCalc = false;
        float duration = 0.5f; // ī��Ʈ �Ⱓ
        float finalTarget = current + target; // ���� ��ǥ ���
        finalTarget = Mathf.Max(finalTarget, 0); // ���� ��ǥ�� 0���� ���� ������ Ȯ��

        // ��ǥ���� ���� �������� �������� ����
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
                0 => "�ϼ��� ���ķ� ���� : " + makeTanhulu,
                1 => "5�� �̻� ���ķ� ���� : " + fiveTanhulu,
                2 => "���� ���ķ� ���� : " + failTanhulu,
                3 => "������ ���� : " + money,
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
