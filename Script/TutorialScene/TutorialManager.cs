using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private int tutorialStep = 0;
    public void NextTutorialStep() 
    {
        if (tochControl == false)
        {
            Debug.Log("���� �Ѿ �� �����ϴ�!!");
            return;
        }
        tutorialDetailStep = 0;
        tutorialStep += 1;
        Tutorial();
    }
    private int tutorialDetailStep = 0;
    public void NextDetailTutorialStep() 
    {
        if (tochControl == false )
        {
            Debug.Log("���� �Ѿ �� �����ϴ�!!");
            return;
        }
        tutorialDetailStep += 1;
        Tutorial();
    }

    private int prevLayer = 0;
    //private int prevLayer2 = 0;
    private int uiLayer = 10;
    //private int uiLayer2 = 10;

    [SerializeField]
    private GameObject blurPageObject;//� ȭ�� ������Ʈ
    [SerializeField]
    private GameObject blurObject;//���� ȭ�� ������Ʈ
    [SerializeField]
    private GameObject textMessageBarLeft;//�ڸ� ���� ������Ʈ
    [SerializeField]
    private GameObject textMessageBarRight;//�ڸ� ���� ������Ʈ
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject resultPanel;
    [SerializeField]
    private GameObject settingButton;

    private string m_text;
    private bool measeaging = true;
    private bool tochControl = true;
    private bool firstTouchBlock = false;
    public bool GetMeaseaging() { return measeaging; }
    public bool GetfirstTochBolck() { return firstTouchBlock; }
    #region
    //Tutorial 0
    [SerializeField]
    private GameObject kiosk;
    string prevSortinLayerName = "";
    //string prevSortinLayerName2 = "";
    [SerializeField]
    private GameObject order_Sheet_Panel;

    #endregion

    #region
    //Tutorial 1
    [SerializeField]
    private GameObject containerSkewer;
    [SerializeField]
    private GameObject containerFruit;
    #endregion

    #region
    //Tutorial 2
    [SerializeField]
    private GameObject sauceContainer1;
    [SerializeField]
    private GameObject sauceContainer2;
    [SerializeField]
    private GameObject Topping_Container1;

    #endregion

    SkewerScript _Skewerscr;
   

    #region
    ////tutorial 3
    //[serializefield]
    //private gameobject containerfruit;

    #endregion

    private static TutorialManager instance;

    public static TutorialManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("TutorialManager instance is not found!");
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

    void Start()
    {
        blurPageObject.SetActive(false);
        blurObject.SetActive(true); 
        textMessageBarLeft.SetActive(false);
        RectTransform rectTransform = textMessageBarRight.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(rectTransform.localScale.x * -1, rectTransform.localScale.y, rectTransform.localScale.z);
        RectTransform rectTransform2 = textMessageBarRight.transform.GetChild(0).GetComponent<RectTransform>();
        rectTransform2.localScale = new Vector3(rectTransform2.localScale.x * -1, rectTransform2.localScale.y, rectTransform2.localScale.z);
        textMessageBarRight.SetActive(false);

        tutorialStep = 0; 
        tutorialDetailStep = 0;
        settingButton.GetComponent<TabButton>().enabled = false;
        _Skewerscr = null;
    }
    
    void Update()
    {
       
    }
    private void OnScreenTouched()
    {
        Debug.Log("Screen was touched!");
    }
    void Tutorial()
    {
        switch (tutorialStep)
        {
            case 0:
               
                break;
            case 1:
                Step0Tutorial();
                break;
            case 2:
                Step1Tutorial();
                break;
            case 3:
                Step2Tutorial();
                break;
            case 4:
                Step3Tutorial();
                break;
            case 5:
                Step4Tutorial();
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;
            case 15:
                break;
            default:
                break;
        }


    }
    IEnumerator _typing(GameObject _textBar) 
    {
        _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        measeaging = true;
        tochControl = false;
        if (_textBar == null)
        {
            Debug.LogError("TextBar is null");
            yield break;
        }

        var textMesh = _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is not found on the child of TextBar");
            yield break;
        }

        yield return new WaitForSeconds(0.1f) ;
        for(int i = 0; i < m_text.Length; i++) 
        {
            _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_text.Substring(0,i);

            yield return new WaitForSeconds(0.06f);
           
        }
        measeaging = false;
        tochControl = true;
    }

    IEnumerator _typing2(GameObject _textBar)
    {
        
        _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        measeaging = true;
        tochControl = false;
        if (_textBar == null)
        {
            Debug.LogError("TextBar is null");
            yield break;
        }
       
        var textMesh = _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is not found on the child of TextBar");
            yield break;
        }

        yield return new WaitForSeconds(5.0f);
        textMessageBarLeft.SetActive(true);
        for (int i = 0; i < m_text.Length; i++)
        {
            _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_text.Substring(0, i);

            yield return new WaitForSeconds(0.05f);

        }
        measeaging = false;
        tochControl = true;
    }

    IEnumerator _typing3(GameObject _textBar)
    {

        _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        measeaging = true;
        tochControl = false;
        if (_textBar == null)
        {
            Debug.LogError("TextBar is null");
            yield break;
        }

        var textMesh = _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is not found on the child of TextBar");
            yield break;
        }

        yield return new WaitForSeconds(2.0f);
        textMessageBarRight.SetActive(true);
        for (int i = 0; i < m_text.Length; i++)
        {
            _textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_text.Substring(0, i);

            yield return new WaitForSeconds(0.05f);

        }
        measeaging = false;
        tochControl = true;
    }


    private void Step0Tutorial() 
    {
        
        switch(tutorialDetailStep) 
        {
            case 0:
                measeaging = true;
                blurPageObject.SetActive(true);
                blurObject.SetActive(false);

                Vector3 currentPosition = Camera.main.transform.position;

                blurPageObject.transform.position = new Vector3(currentPosition.x, currentPosition.y, 3.0f);
                Vector3 kioskPosition = kiosk.transform.GetChild(1).position;
                textMessageBarRight.SetActive(true);
                textMessageBarRight.transform.position = new Vector3(kioskPosition.x, kioskPosition.y, 3.0f);

                m_text = "�մ��� ���̳׿�!\nŰ����ũ�� ��ġ�� ������ ";
                StartCoroutine(_typing(textMessageBarRight));

                prevSortinLayerName = kiosk.GetComponent<SpriteRenderer>().sortingLayerName;
                kiosk.GetComponent<SpriteRenderer>().sortingLayerName = "Tutorial";
                prevLayer = kiosk.layer;
                kiosk.layer = uiLayer;
              
                    
                break; 
            case 1:
                firstTouchBlock= true;
                measeaging = true;
                kiosk.GetComponent<SpriteRenderer>().sortingLayerName = prevSortinLayerName;
                kiosk.layer = prevLayer;
                order_Sheet_Panel.transform.GetChild(4).gameObject.SetActive(false);
                order_Sheet_Panel.transform.GetChild(5).gameObject.SetActive(false);
                Vector3 currentPosition3 = order_Sheet_Panel.transform.GetChild(1).transform.GetChild(0).transform.position;
                textMessageBarRight.transform.position = new Vector3(currentPosition3.x, currentPosition3.y, 3.0f);

                m_text = "���� ����� ������\n �� ����� �ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                
                break;
            case 2:
                measeaging = true;
                kiosk.GetComponent<BoxCollider2D>().enabled = false;
                m_text = "���� ���� 4����\n���� �� �ֽ��ϴ�! ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 3:
                measeaging = true;
                Vector3 currentPosition4 = order_Sheet_Panel.transform.GetChild(2).transform.GetChild(0).transform.position;
                textMessageBarRight.transform.position = new Vector3(currentPosition4.x, currentPosition4.y, 3.0f);
                m_text = "� �ҽ�����\n �� ����� �ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 4:
                measeaging = true;
                m_text = "�ҽ��� ������ ���ݸ�\n�� ���� �߿� ���ɴϴ�! ";
                StartCoroutine(_typing(textMessageBarRight));

                break;
            case 5:
                measeaging = true;
                Vector3 currentPosition5 = order_Sheet_Panel.transform.GetChild(3).transform.GetChild(0).transform.position;
                textMessageBarRight.transform.position = new Vector3(currentPosition5.x, currentPosition5.y, 3.0f);
                m_text = "� ��������\n�� ����� �ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 6:
                measeaging = true;
                m_text = "������ ���Ͽ� 2����\n�÷��ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 7:
                textMessageBarRight.SetActive(false);
                order_Sheet_Panel.transform.GetChild(4).gameObject.SetActive(true);
                containerFruit.GetComponent<Collider2D>().enabled = false;
                containerSkewer.GetComponent<Collider2D>().enabled = false;
                break;
            default : 
                
                break;
        }
    }
    private void Step1Tutorial()
    {
        switch (tutorialDetailStep)
        {
            case 0:
                measeaging = true;
                textMessageBarLeft.SetActive(true);
                textMessageBarLeft.transform.localPosition = new Vector3(-270.0f, 14.0f, 3.0f);
                m_text = "���� ����� ��\n�ð��Դϴ�! ";
                StartCoroutine(_typing(textMessageBarLeft));
                containerSkewer.GetComponent<SkewerContainerScript>().enabled = false;
                break;
            case 1:
                measeaging = true;
                m_text = "���� ��ġ ���� ������\n��ġ�� �����ּ���! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 2:
                containerSkewer.GetComponent<Collider2D>().enabled = true;
                blurPageObject.SetActive(false);
                prevSortinLayerName = containerSkewer.GetComponent<SpriteRenderer>().sortingLayerName;
                containerSkewer.GetComponent<SpriteRenderer>().sortingLayerName = "Tutorial";
                break;
            case 3:
                containerSkewer.GetComponent<Collider2D>().enabled = false;
                measeaging = true;
                blurPageObject.SetActive(true);
                textMessageBarLeft.transform.localPosition = new Vector3(5.0f, 285.0f, 3.0f);
                containerSkewer.GetComponent<SpriteRenderer>().sortingLayerName = prevSortinLayerName;
                
                m_text = "���� ������ �巡���ؼ� ��ġ�� �Ⱦƺ�����! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 4:
                containerFruit.GetComponent<Collider2D>().enabled = true;
                blurPageObject.SetActive(false);

                break;
            case 5:
                blurPageObject.SetActive(true);
                measeaging = true;
                m_text = "���ƿ�! ��������\nä���ּ���! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 6:
                blurPageObject.SetActive(false);
                textMessageBarLeft.SetActive(false);
                break;
            case 7:
                blurPageObject.SetActive(true);
                measeaging = true;
                textMessageBarLeft.SetActive(true);
                m_text = "���߾��! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 8:
                measeaging = true;
              
                m_text = "������ �����߿� ��������\n�ֹ��� ���´�ϴ�. ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 9:
                blurPageObject.SetActive(false);
                break;
            default:

                break;
        }
    }

    private void Step2Tutorial()
    {
        switch (tutorialDetailStep)
        {
            case 0:

                blurPageObject.SetActive(true);
                measeaging = true;
                textMessageBarLeft.transform.localPosition = new Vector3(-208.0f, 80.0f, 3.0f);
                m_text = "������ �ҽ� �Դϴ�! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 1:
                measeaging = true;
                m_text = "�ֹ����� �´� �ҽ���\n�����ּ���! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 2:
                measeaging = true;
                m_text = "�ҽ��� ����\n�� �߶��ּ���! ";
                StartCoroutine(_typing(textMessageBarLeft));
                Collider2D Topping_Container1collider2D = Topping_Container1.GetComponent<Collider2D>();
                Topping_Container1collider2D.enabled = false;
                break;
            case 3:
                textMessageBarLeft.SetActive(false);
                var sauce = OrderManager.Instance.GetOrderSauceList();
                if (sauce[0][0].korName == "����")
                {
                    Collider2D sauceContainer2collider2D = sauceContainer2.GetComponent<Collider2D>();
                    if (sauceContainer2collider2D != null)
                    {
                        sauceContainer2collider2D.enabled = false;
                    }
                }
                else if (sauce[0][0].korName == "����")
                {
                    Collider2D sauceContainer1collider2D = sauceContainer1.GetComponent<Collider2D>();
                    if (sauceContainer1collider2D != null)
                    {
                        sauceContainer1collider2D.enabled = false;
                    }
                }
                else
                {
                    Debug.LogWarning("�ҽ� ����! Ʃ�丮�� �Ŵ��� class�� 3��° ����");
                }

                blurPageObject.SetActive(false);

                break;
            case 4:
                textMessageBarLeft.SetActive(true);
                blurPageObject.SetActive(true);
                measeaging = true;
                m_text = "��������! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 5:
                measeaging = true;
                textMessageBarLeft.transform.localPosition = new Vector3(120.0f, 285.0f, 3.0f);
                m_text = "������ �����Դϴ�! ";
                StartCoroutine(_typing(textMessageBarLeft));
                Collider2D colliderToppping = Topping_Container1.GetComponent<Collider2D>();
                colliderToppping.enabled = true;
                break;
            case 6:
                measeaging = true;
                m_text = "������ ���ϴ�\n2���� �÷� �ּ���! ";
                StartCoroutine(_typing(textMessageBarLeft));
                break;
            case 7:
                blurPageObject.SetActive(false);

                break;
            case 8:
                
                canvas.transform.Find("Next_Button(Clone)").gameObject.GetComponent<Button>().enabled = false;
                break;
            case 9:
                measeaging = true;
                blurPageObject.SetActive(true);
                m_text = "���� ��������! ";
                StartCoroutine(_typing(textMessageBarLeft));
                canvas.transform.Find("Next_Button(Clone)").gameObject.GetComponent<Button>().enabled = true;
                break;
            case 10:
                
                blurPageObject.SetActive(false);
                textMessageBarLeft.SetActive(false);
                break;
            default:

                break;
        }
    }

    private void Step3Tutorial()
    {
        switch (tutorialDetailStep)
        {
            case 0:
                _Skewerscr = GameObject.FindWithTag("Skewer").GetComponent<SkewerScript>();
                _Skewerscr.SetFrozenState(true);
                measeaging = true;
                blurPageObject.SetActive(true);
                textMessageBarRight.SetActive(true);
                textMessageBarRight.transform.localPosition = new Vector3(163.0f, 218.0f, 3.0f);
                m_text = "������ ����� �Դϴ� ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 1:
                measeaging = true;
                m_text = "���ķ縦 �巡���ؼ�\n����� �־��ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 2:
                measeaging = true;
                m_text = "����� ������\n�̴ϰ����� ����˴ϴ� ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 3:
                _Skewerscr.SetFrozenState(false);
                textMessageBarRight.SetActive(false);
                blurPageObject.SetActive(false);
                break;
            case 4:
                measeaging = true;
                blurPageObject.SetActive(true);
                textMessageBarRight.SetActive(true);
                m_text = "���� ����� ����\n��ư�� �����ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                break;
            case 5:
                blurPageObject.SetActive(false);
                textMessageBarRight.SetActive(false);
                break;
            case 6:
                measeaging = true;
                textMessageBarRight.SetActive(false);
                blurPageObject.SetActive(true);
                textMessageBarRight.transform.localPosition = new Vector3(-575.0f, 162.0f, 3.0f);
                m_text = "���� ���ϼ̾��! ";
                StartCoroutine(_typing3(textMessageBarRight));

                break;
            case 7:
                measeaging = true;
                m_text = "���� Ȯ�� ��ư��\n�����ּ���! ";
                StartCoroutine(_typing(textMessageBarRight));
                textMessageBarLeft.SetActive(false);
                break;
            case 8:
                blurPageObject.SetActive(false);
                textMessageBarRight.SetActive(false);
                textMessageBarLeft.SetActive(false);
                break;
            case 9:
                break;
            default:

                break;
        }
    }

    private void Step4Tutorial()
    {
        switch (tutorialDetailStep)
        {
            case 0:
                measeaging = true;
                blurPageObject.SetActive(true);
                textMessageBarLeft.transform.localPosition = new Vector3(120.0f, 30.0f, 3.0f);
                m_text = "�ϼ��� ���ķ縦\n�մԿ��� �ǳ��ָ�\n���̿���! ";
                StartCoroutine(_typing2(textMessageBarLeft));
                break;
            case 1:
               
                _Skewerscr.SetIsFrozenStart(false);
                blurPageObject.SetActive(false);
                textMessageBarLeft.SetActive(false);
                break;
            case 2:
                measeaging = true;
              
                blurPageObject.SetActive(true);
                textMessageBarLeft.SetActive(true);
                m_text = "���� �ڽŸ��� ���Ը�\n��غ�����! ";//���� �߻�
                StartCoroutine(_typing(textMessageBarLeft));
                
                resultPanel.SetActive(false);

                break;
            case 3:
                measeaging = true;
                blurObject.SetActive(true);
                blurPageObject.GetComponent<Button>().enabled = false;
                m_text = "����� �����~! ";
                StartCoroutine(_typing(textMessageBarLeft));
                StartCoroutine(nextDayDelay());
                break;
            case 4:
                
                break;
            case 5:
                
                break;
            case 6:
                
                break;
            case 7:
              
                break;
            case 8:

                break;
            case 9:

                break;
            default:

                break;
        }
    }

    IEnumerator nextDayDelay()
    {
        yield return new WaitForSeconds(2.0f);
        SoundManager.Instance.GetSoundDatas().isCompleteTutorial = true;
        SoundManager.Instance.SoundDataSave();
        UserDataControlManager.Instance.SetPlayerDate(1);
        SceneManager.LoadScene("StoreScene");
    }
}
