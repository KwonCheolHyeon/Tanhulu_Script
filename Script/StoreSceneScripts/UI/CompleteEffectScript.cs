using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Gpm.Ui;
using Unity.VisualScripting;

public class CompleteEffectScript : MonoBehaviour
{
    // ��Ȱ��ȭ �� �ٽ� Ȱ��ȭ�� ������ �ð�
    public float activationDelay = 0.0f;
    public bool isCollisionInProgress = false;

    private Queue<GameObject> objectsToActivate = new Queue<GameObject>();
    private GameObject otherObj;

    [SerializeField]
    private GameObject effectCanvas;

    public void SetOtherObject(GameObject _other) { otherObj = _other; }

    private FadeEffect fadeEffect;
    private Exit exit;

    private bool isFadeIn = false;

    private int completeTanghulu = 0;
    
    private bool buttonClick = false;
    private Scene currentScene;
    private void Start()
    {
        fadeEffect = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<FadeEffect>();
        exit = GameObject.Find("Canvas").GetComponent<Exit>();
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if(fadeEffect.GetEffectImage().color.a >= 1 && isFadeIn == false)
        {
            isFadeIn = true;
            exit.SetCurrentTable(CurrentTable.StoreTable);
            objectsToActivate.Enqueue(otherObj);
            StartCoroutine(DeactivateObjectWithDelay(otherObj, activationDelay));
        }
    }

    public void Next()
    {
        if (buttonClick == false)
        {
            buttonClick = true;

            string currentSceneName = SceneManager.GetActiveScene().name;
            switch (currentSceneName)
            {
                case "StoreScene":
                    isFadeIn = false;
                    StartCoroutine(fadeEffect.Fade(0, 1));
                    break;
                case "TutorialStoreScene":
                    isFadeIn = false;
                    StartCoroutine(fadeEffect.Fade(0, 1));
                    TutorialManager.Instance.NextTutorialStep();
                    break;
                // �ٸ� �� �̸��� ���� ���̽����� �߰��� �� �ֽ��ϴ�.
                default:
                    Debug.Log("���̸��� �߸���");
                    break;
            }
        }
    }


    private void ResetCollision()
    {
        isCollisionInProgress = false;
    }

    private IEnumerator DeactivateObjectWithDelay(GameObject _obj, float _delay)
    {

        _obj = GameObject.FindWithTag("Skewer");

        effectCanvas.GetComponent<Canvas>().sortingLayerID = 0;
        yield return new WaitForSeconds(_delay);

        // ���ķ�
        _obj.transform.position = new Vector2(-21.5f, -10.0f);
        _obj.GetComponent<SkewerScript>().SetPrevPos(_obj.transform.position);

        string sceneName = currentScene.name;
        if (sceneName == "StoreScene") 
        {
            _obj.GetComponent<SkewerScript>().SetIsFrozenStart(false);
        }

        Vector2 scale = _obj.transform.localScale;
        _obj.transform.localScale = scale / 2.5f;
        _obj.transform.localEulerAngles = new Vector3(0, 0, 0);

        completeTanghulu++;

        if (completeTanghulu >= 4)
            MissionManager.Instance.ClearMission(1);

        // ���ķ縦 �մ��ִ°����� �̵��� ī�޶� ��ȯ �ڷ�ƾ ����
        GameObject nextObject = objectsToActivate.Dequeue();
        StartCoroutine(ActivateObjectWithDelay(nextObject, activationDelay));

        ResetCollision();
    }

    // �����ð� �Ŀ� ������Ʈ�� Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator ActivateObjectWithDelay(GameObject _obj, float _delay)
    {
        yield return new WaitForSeconds(_delay);

        effectCanvas.SetActive(false);

        // ���̵�ƿ� �ڷ�ƾ ����
        StartCoroutine(fadeEffect.Fade(1,0));

        // ���⼭ �մԹ޴� ������ ī�޶� �̵�
        Camera.main.transform.position = new Vector3(-20.0f, 0.0f, Camera.main.transform.position.z);

        GameObject.Find("Canvas").transform.Find("Setting_Tab").GetChild(0).GetComponent<TabButton>().enabled = true;
        ResetCollision();

        buttonClick = false;
    }
}
