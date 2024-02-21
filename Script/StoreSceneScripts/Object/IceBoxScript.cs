using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UIElements;


public class IceBoxScript : MonoBehaviour
{
    public GameObject starPos;
    private GameObject initStarPos;
    public GameObject canvas;

    private SpriteRenderer sprite;

    [SerializeField]
    private Sprite closeBox;
    [SerializeField]
    private Sprite openBox;

    [SerializeField]
    private GameObject effectCanvas;

    private List<GameObject> objects = new List<GameObject>();

    float scaleSpeed = 0.1f;
    public float moveSpeed = 1.0f;

    private bool isCollisionInProgress = false;
    private Queue<GameObject> objectsToActivate = new Queue<GameObject>();

    // ��Ȱ��ȭ �� �ٽ� Ȱ��ȭ�� ������ �ð�
    private float activationDelay = 2.0f;

    private bool success = false;
    public bool IsSuccess() { return success; }
    private int failCount = 0;

    private bool startTutorialIceBoxTutorial = false;
    public bool IsStartTutorialIceBoxTutorial() { return startTutorialIceBoxTutorial; }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // �浹�� ���� ���� �ƴ϶��, ���� ������Ʈ�� Ȱ��ȭ�ϰ� �浹�� �����մϴ�.
        if (!isCollisionInProgress && objectsToActivate.Count > 0)
        {
            if(initStarPos != null)
            {
                success = initStarPos.transform.GetChild(3).GetComponent<StarPosScript>().GetStop();
                failCount = initStarPos.transform.GetChild(3).GetComponent<StarPosScript>().GetFailCount();
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            switch (currentSceneName)
            {
                case "StoreScene":
                    if (success || failCount >= 5)
                    {
                        // �̴ϰ��� ���� ����Ʈ �ִϸ��̼��� ������ �ȵǼ� �̴ϰ����� ���� ���� ������Ѵ�.
                        if (failCount >= 5)
                        {
                            Destroy(initStarPos);
                            MissionManager.Instance.ClearMission(6);
                        }
                        GameObject nextObject = objectsToActivate.Dequeue();
                        StartCoroutine(ActivateObjectWithDelay(nextObject, activationDelay));
                        isCollisionInProgress = true;
                    }
                    break;
                case "TutorialStoreScene":
                    if (success )
                    {
                        // �̴ϰ��� ���� ����Ʈ �ִϸ��̼��� ������ �ȵǼ� �̴ϰ����� ���� ���� ������Ѵ�.
                        failCount = 5;
                        if (failCount >= 5)
                        {
                            Destroy(initStarPos);
                        }
                        GameObject nextObject = objectsToActivate.Dequeue();
                        StartCoroutine(ActivateObjectWithDelay(nextObject, activationDelay));
                        isCollisionInProgress = true;
                    }
                    break;
                // �ٸ� �� �̸��� ���� ���̽����� �߰��� �� �ֽ��ϴ�.
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Skewer") && collision.GetComponent<SkewerScript>().IsFrozenState() == false)
        {
            sprite.sprite = openBox;
            
            SoundManager.Instance.PlaySFXSound("Freezing");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Skewer") && collision.GetComponent<SkewerScript>().IsFrozenState() == false)
        {
            OtherChangeScale(collision.gameObject);
        }
    }

    void OtherChangeScale(GameObject _other)
    {
        Vector2 otherPos = _other.transform.position;

        // ���� �������� ������
        Vector2 currentScale = _other.transform.localScale;

        // �������� ����
        currentScale -= new Vector2(0.4f * Time.deltaTime, scaleSpeed * Time.deltaTime);

        // �������� ������� Ȯ���Ͽ� ������ ���� �ʵ��� ��ȣ
        currentScale = new Vector2(
            Mathf.Max(currentScale.x, 0f),
            Mathf.Max(currentScale.y, 0f)
        );

        Vector2 pos = this.transform.position;
        pos.y += 3.0f;
        // ���ο� �������� ����
        _other.transform.localScale = currentScale;
        if (currentScale.x < 1.5f)
        {
            sprite.sprite = closeBox;

            objectsToActivate.Enqueue(_other.gameObject);
            StartCoroutine(DeactivateObjectWithDelay(_other.gameObject, activationDelay));
        }
        else
        {
            // ������Ʈ �̵�
            _other.transform.position = Vector3.Lerp(otherPos, pos, moveSpeed * Time.deltaTime);
        }
    }

    private void ResetCollision()
    {
        isCollisionInProgress = false;
    }

    // �����ð� �Ŀ� ������Ʈ�� Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator ActivateObjectWithDelay(GameObject _obj, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        SkewerScript skewer = _obj.GetComponent<SkewerScript>();

        _obj.transform.localScale = new Vector2(2.5f,1);

        canvas.GetComponent<Exit>().SetCurrentTable(CurrentTable.CompleteTable);

        // �ϼ� ����Ʈ�� ���ķ� �ڿ� ���̵��� ĵ������ ����
        // EffectCanvas�� Ȱ��ȭ
        ChangeCanvas();

        float x = effectCanvas.GetComponent<RectTransform>().transform.position.x;
        
        _obj.transform.position = new Vector2(x - 6.0f, -5.0f);
        _obj.transform.localEulerAngles = new Vector3(0, 0, 20);

        skewer.SetPrevPos(_obj.transform.position);

        if (failCount == 0)
            skewer.SetFrozenState(true);

        success = false;
        failCount = 0;

        ResetCollision();
    }

    // �����ð� �Ŀ� ������Ʈ�� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator DeactivateObjectWithDelay(GameObject _obj, float _delay)
    {
        _obj.transform.position = new Vector2(-100.0f, -100.0f);

        // �̴ϰ��� ���۽� ���������� ��Ȱ��ȭ �� ���ù�ư ������Ʈ ��Ȱ��ȭ
        bool active = canvas.transform.Find("Setting_Tab").GetChild(2).gameObject.activeSelf;
        if (active == true)
        {
            canvas.transform.Find("Setting_Tab").GetChild(2).gameObject.SetActive(false);
        }
        canvas.transform.Find("Setting_Tab").GetChild(0).GetComponent<TabButton>().enabled = false;

        // �̴ϰ��� ����
        initStarPos = GameObject.Instantiate(starPos);
        startTutorialIceBoxTutorial = true;
        initStarPos.transform.SetParent(canvas.transform);
        initStarPos.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 350.0f);
        initStarPos.GetComponent<RectTransform>().localScale = new Vector2(2, 2);
        yield return new WaitForSeconds(_delay);
        ResetCollision();
    }

    private void ChangeCanvas()
    {
        effectCanvas.SetActive(true);
        effectCanvas.GetComponent<Canvas>().sortingLayerName = "Container";

        SoundManager.Instance.PlaySFXSound("FinishMaking");
    }
}
