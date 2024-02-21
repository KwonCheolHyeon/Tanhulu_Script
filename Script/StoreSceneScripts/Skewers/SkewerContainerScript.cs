using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SkewerContainerScript : MonoBehaviour
{
    public GameObject skewerPrefab; // ������ȭ�� ��ġ
    public GameObject spawnPoint; // ���� ����
    private int poolSize = 1 ; // Ǯ ũ��
    private List<GameObject> pools = new List<GameObject>(); // Ǯ
    private Vector3 spawnPosition = new Vector3(70, -8f, 0); // Ȱ��ȭ�� ��ġ
    private Camera mainCamera; // ���� ī�޶�
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
        mainCamera = Camera.main; // ���� ī�޶� �����ɴϴ�.
        // Ǯ �ʱ�ȭ
        for (int i = 0; i < poolSize; i++)
        {
            GameObject skewer = Instantiate(skewerPrefab);
            skewer.SetActive(false);
            pools.Add(skewer);
        }
    }

    void Update()
    {
        // ���콺 Ŭ���̳� ��ġ �Է��� Ȯ���մϴ�.
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ���� ������ �����մϴ�.
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            // ������ ���� ������Ʈ�� �ݶ��̴��� �浹�ߴ��� Ȯ���մϴ�.
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // ���� ��ġ�� ��� �ִ��� Ȯ���ϰ� ��ġ�� �����մϴ�.
                SpawnSkewer();
                SoundManager.Instance.PlaySFXSound("ContainerSound");
                Debug.Log("��ġ ����");
            }
        }
    }

    //private void SpawnSkewer()
    //{
    //    // ��ġ�� �κ��� UI Object ��� True ��ȯ
    //    if (EventSystem.current.IsPointerOverGameObject() == true)
    //        return;



    //    // Ǯ���� ��Ȱ��ȭ�� ������Ʈ�� ã�� Ȱ��ȭ�մϴ�.
    //    foreach (GameObject skewer in pools)
    //    {
    //        if (!skewer.activeInHierarchy && skewer.transform.childCount == 0)
    //        {
    //            // ���� �߻� �Լ� (�и��� ����)
    //            VibrationManager.Instance.CreateOneShot(80);

    //            skewer.transform.position = spawnPoint.transform.position;
    //            skewer.SetActive(true);
    //            break; // �ϳ��� ��ġ�� Ȱ��ȭ�� �� ������ Ż���մϴ�.
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
        // ���� ����Ʈ ��ġ �ֺ����� �浹�� �˻��ϱ� ���� ���� �ݰ��� ����մϴ�
        float checkRadius = 0.5f; // �ݰ��� �浹�� ������ ����Ǵ� ������Ʈ�� ũ�⿡ ���� ������ �� �ֽ��ϴ�
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.transform.position, checkRadius);

        // �浹ü�� �߰ߵ��� ������ ���� ��ȯ�մϴ� (��ġ�� ��� ����)
        return colliders.Length == 0;
    }
}

