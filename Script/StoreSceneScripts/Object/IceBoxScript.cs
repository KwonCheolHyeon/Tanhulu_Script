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

    // 비활성화 후 다시 활성화할 딜레이 시간
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
        // 충돌이 진행 중이 아니라면, 다음 오브젝트를 활성화하고 충돌을 시작합니다.
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
                        // 미니게임 성공 이펙트 애니메이션이 실행이 안되서 미니게임을 따로 삭제 해줘야한다.
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
                        // 미니게임 성공 이펙트 애니메이션이 실행이 안되서 미니게임을 따로 삭제 해줘야한다.
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
                // 다른 씬 이름에 대한 케이스들을 추가할 수 있습니다.
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

        // 현재 스케일을 가져옴
        Vector2 currentScale = _other.transform.localScale;

        // 스케일을 줄임
        currentScale -= new Vector2(0.4f * Time.deltaTime, scaleSpeed * Time.deltaTime);

        // 스케일이 양수인지 확인하여 음수가 되지 않도록 보호
        currentScale = new Vector2(
            Mathf.Max(currentScale.x, 0f),
            Mathf.Max(currentScale.y, 0f)
        );

        Vector2 pos = this.transform.position;
        pos.y += 3.0f;
        // 새로운 스케일을 적용
        _other.transform.localScale = currentScale;
        if (currentScale.x < 1.5f)
        {
            sprite.sprite = closeBox;

            objectsToActivate.Enqueue(_other.gameObject);
            StartCoroutine(DeactivateObjectWithDelay(_other.gameObject, activationDelay));
        }
        else
        {
            // 오브젝트 이동
            _other.transform.position = Vector3.Lerp(otherPos, pos, moveSpeed * Time.deltaTime);
        }
    }

    private void ResetCollision()
    {
        isCollisionInProgress = false;
    }

    // 지연시간 후에 오브젝트를 활성화하는 코루틴
    private IEnumerator ActivateObjectWithDelay(GameObject _obj, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        SkewerScript skewer = _obj.GetComponent<SkewerScript>();

        _obj.transform.localScale = new Vector2(2.5f,1);

        canvas.GetComponent<Exit>().SetCurrentTable(CurrentTable.CompleteTable);

        // 완성 이펙트가 탕후루 뒤에 보이도록 캔버스를 나눔
        // EffectCanvas를 활성화
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

    // 지연시간 후에 오브젝트를 비활성화하는 코루틴
    private IEnumerator DeactivateObjectWithDelay(GameObject _obj, float _delay)
    {
        _obj.transform.position = new Vector2(-100.0f, -100.0f);

        // 미니게임 시작시 설정페이지 비활성화 및 세팅버튼 컴포넌트 비활성화
        bool active = canvas.transform.Find("Setting_Tab").GetChild(2).gameObject.activeSelf;
        if (active == true)
        {
            canvas.transform.Find("Setting_Tab").GetChild(2).gameObject.SetActive(false);
        }
        canvas.transform.Find("Setting_Tab").GetChild(0).GetComponent<TabButton>().enabled = false;

        // 미니게임 실행
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
