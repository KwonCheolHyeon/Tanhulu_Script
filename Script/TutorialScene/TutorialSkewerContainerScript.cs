using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSkewerContainerScript : MonoBehaviour
{
    public GameObject skewerPrefab; // ������ȭ�� ��ġ
    public GameObject spawnPoint; // ���� ����
    private int poolSize = 5; // Ǯ ũ��
    private List<GameObject> pools = new List<GameObject>(); // Ǯ
    private Vector3 spawnPosition = new Vector3(70, -6.5f, 0); // Ȱ��ȭ�� ��ġ
    private Camera mainCamera; // ���� ī�޶�\

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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ���� ������ �����մϴ�.
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            // ������ ���� ������Ʈ�� �ݶ��̴��� �浹�ߴ��� Ȯ���մϴ�.
            if (hit.collider != null && hit.collider.gameObject == gameObject && TutorialManager.Instance.GetMeaseaging() ==false)
            {
                // ���� ��ġ�� ��� �ִ��� Ȯ���ϰ� ��ġ�� �����մϴ�.
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
        yield return new WaitForSeconds(0.1f); // ª�� ���� �ð�
        if (toppingScript != null && toppingScript.IsPlacedOnFruit)
        {
            CheckFirstToppingPlaced();
        }
    }
    private void SpawnSkewer2()
    {
        if (IsSpawnLocationClear())
        {
            // Ǯ���� ��Ȱ��ȭ�� ������Ʈ�� ã�� Ȱ��ȭ�մϴ�.
            foreach (GameObject skewer in pools)
            {
                if (!skewer.activeInHierarchy && skewer.transform.childCount == 0)
                {
                    // ���� �߻� �Լ� (�и��� ����)
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
        // ���� ����Ʈ ��ġ �ֺ����� �浹�� �˻��ϱ� ���� ���� �ݰ��� ����մϴ�
        float checkRadius = 0.5f; // �ݰ��� �浹�� ������ ����Ǵ� ������Ʈ�� ũ�⿡ ���� ������ �� �ֽ��ϴ�
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
