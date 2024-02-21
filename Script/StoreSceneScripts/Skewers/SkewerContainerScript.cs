using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SkewerContainerScript : MonoBehaviour
{
    public GameObject skewerPrefab; // 프리팹화된 꼬치
    public GameObject spawnPoint; // 생성 지점
    private int poolSize = 1 ; // 풀 크기
    private List<GameObject> pools = new List<GameObject>(); // 풀
    private Vector3 spawnPosition = new Vector3(70, -8f, 0); // 활성화될 위치
    private Camera mainCamera; // 메인 카메라
    private GameObject activeSkewer = null;

    private static SkewerContainerScript instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

     
    }
    public static SkewerContainerScript Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    void Start()
    {
        mainCamera = Camera.main; // 메인 카메라를 가져옵니다.
        // 풀 초기화
        for (int i = 0; i < poolSize; i++)
        {
            GameObject skewer = Instantiate(skewerPrefab);
            skewer.SetActive(false);
            pools.Add(skewer);
        }
    }

    void Update()
    {
        // 마우스 클릭이나 터치 입력을 확인합니다.
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 광선을 생성합니다.
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            // 광선이 현재 오브젝트의 콜라이더와 충돌했는지 확인합니다.
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // 스폰 위치가 비어 있는지 확인하고 꼬치를 스폰합니다.
                SpawnSkewer();
                SoundManager.Instance.PlaySFXSound("ContainerSound");
                Debug.Log("꼬치 스폰");
            }
        }
    }

    //private void SpawnSkewer()
    //{
    //    // 터치한 부분이 UI Object 라면 True 반환
    //    if (EventSystem.current.IsPointerOverGameObject() == true)
    //        return;



    //    // 풀에서 비활성화된 오브젝트를 찾아 활성화합니다.
    //    foreach (GameObject skewer in pools)
    //    {
    //        if (!skewer.activeInHierarchy && skewer.transform.childCount == 0)
    //        {
    //            // 진동 발생 함수 (밀리초 단위)
    //            VibrationManager.Instance.CreateOneShot(80);

    //            skewer.transform.position = spawnPoint.transform.position;
    //            skewer.SetActive(true);
    //            break; // 하나의 꼬치를 활성화한 후 루프를 탈출합니다.
    //        }
    //    }

    //}

    private void SpawnSkewer()
    {
        // Return True if the touched part is a UI Object
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // Check if there is already an active skewer
        if (activeSkewer != null && activeSkewer.activeInHierarchy)
            return; // If there is an active skewer, do not spawn another one

        // Find a disabled object in the pool and activate it
        foreach (GameObject skewer in pools)
        {
            if (!skewer.activeInHierarchy)
            {
                // Vibration generation function (in milliseconds)
                VibrationManager.Instance.CreateOneShot(80);

                skewer.transform.position = spawnPoint.transform.position;
                skewer.SetActive(true);
                activeSkewer = skewer; // Set this skewer as the active one
                Debug.Log("SpawnSkewer: Skewer position set to " + skewer.transform.position);
                break; // Activate one skewer and then escape the loop
            }
        }
    }
    public void ReturnSkewerToPool(GameObject skewer)
    {
        skewer.SetActive(false);
        skewer.transform.position = spawnPosition; // Position where the skewer is stored in the pool
        Debug.Log("ReturnSkewerToPool: Skewer returned to pool at position " + spawnPosition);
        if (activeSkewer == skewer)
        {
            activeSkewer = null; // Reset the active skewer since it's now returned to the pool
        }

    }

    private bool IsSpawnLocationClear()
    {
        // 스폰 포인트 위치 주변에서 충돌을 검사하기 위해 작은 반경을 사용합니다
        float checkRadius = 0.5f; // 반경은 충돌할 것으로 예상되는 오브젝트의 크기에 따라 조정할 수 있습니다
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.transform.position, checkRadius);

        // 충돌체가 발견되지 않으면 참을 반환합니다 (위치가 비어 있음)
        return colliders.Length == 0;
    }
}

