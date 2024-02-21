using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSkewerContainerScript : MonoBehaviour
{
    public GameObject skewerPrefab; // 프리팹화된 꼬치
    public GameObject spawnPoint; // 생성 지점
    private int poolSize = 5; // 풀 크기
    private List<GameObject> pools = new List<GameObject>(); // 풀
    private Vector3 spawnPosition = new Vector3(70, -6.5f, 0); // 활성화될 위치
    private Camera mainCamera; // 메인 카메라\

    private GameObject nowSkewer;
    private bool isInputReleased = false;
    private int lastChildCount = 0;

    private bool countToppings = false;
    private bool countToppings2 = false;
    void Start()
    {
        mainCamera = Camera.main;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject skewer = Instantiate(skewerPrefab);
            skewer.SetActive(false);
            pools.Add(skewer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isInputReleased = true;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 광선을 생성합니다.
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            // 광선이 현재 오브젝트의 콜라이더와 충돌했는지 확인합니다.
            if (hit.collider != null && hit.collider.gameObject == gameObject && TutorialManager.Instance.GetMeaseaging() ==false)
            {
                // 스폰 위치가 비어 있는지 확인하고 꼬치를 스폰합니다.
                SpawnSkewer2();
            }
        }



        CheckForNewChild();
        CheckFirstToppingPlaced();
        //GameObject activeTopping = FindActiveGameObjectWithName("ToppingPrefab(Clone)");
        //if (activeTopping != null)
        //{
        //    ToppingScript toppingScript = activeTopping.GetComponent<ToppingScript>();
        //    StartCoroutine(CheckPlacementCoroutine(toppingScript));

           
        //}
    }
    IEnumerator CheckPlacementCoroutine(ToppingScript toppingScript)
    {
        yield return new WaitForSeconds(0.1f); // 짧은 지연 시간
        if (toppingScript != null && toppingScript.IsPlacedOnFruit)
        {
            CheckFirstToppingPlaced();
        }
    }
    private void SpawnSkewer2()
    {
        if (IsSpawnLocationClear())
        {
            // 풀에서 비활성화된 오브젝트를 찾아 활성화합니다.
            foreach (GameObject skewer in pools)
            {
                if (!skewer.activeInHierarchy && skewer.transform.childCount == 0)
                {
                    // 진동 발생 함수 (밀리초 단위)
                    VibrationManager.Instance.CreateOneShot(80);
                    skewer.transform.position = spawnPoint.transform.position;
                    skewer.SetActive(true);
                    nowSkewer = skewer;
                    TutorialManager.Instance.NextDetailTutorialStep();
                    break;
                }
            }
        }
    }

    private bool IsSpawnLocationClear()
    {
        // 스폰 포인트 위치 주변에서 충돌을 검사하기 위해 작은 반경을 사용합니다
        float checkRadius = 0.5f; // 반경은 충돌할 것으로 예상되는 오브젝트의 크기에 따라 조정할 수 있습니다
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.transform.position, checkRadius);

        return colliders.Length == 0;
    }

    private void CheckForNewChild()
    {
        if (nowSkewer != null && isInputReleased)
        {
            int currentChildCount = nowSkewer.transform.childCount;
            if (currentChildCount > lastChildCount)
            {
                OnNewChildAdded(); // Call the function when a new child is detected
                isInputReleased = false; // Reset the flag
            }
            lastChildCount = currentChildCount; // Update the lastChildCount for the next frame
        }

        
    }

    private void OnNewChildAdded()
    {
        if (nowSkewer.transform.childCount == 1)
        {
            TutorialManager.Instance.NextDetailTutorialStep();
        }

        if (nowSkewer.transform.childCount == 4)
        {
            TutorialManager.Instance.NextDetailTutorialStep();
        }
    }


    private void GetNextBtn() 
    {
        nowSkewer.GetComponent<SkewerScript>().GetNextTableButton();
    }

    private void CheckFirstToppingPlaced()
    {
        if (nowSkewer != null && countToppings == false && nowSkewer.GetComponent<SkewerScript>().GetToppingCount() >= 1 )
        {
            countToppings = true;
            TutorialManager.Instance.NextDetailTutorialStep();
        }

        if (nowSkewer != null && countToppings2 == false && nowSkewer.GetComponent<SkewerScript>().GetToppingCount() >= 8)
        {
            countToppings2 = true;
            TutorialManager.Instance.NextDetailTutorialStep();
        }
    }

    GameObject FindActiveGameObjectWithName(string name)
    {
       
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name && obj.activeInHierarchy && obj.GetComponent<ToppingScript>().IsFollowingMouse)
            {
                return obj;
            }
        }
        return null; // Return null if no active object with the name is found
    }
}
