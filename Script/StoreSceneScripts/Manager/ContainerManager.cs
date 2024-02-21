using Gpm.Common.ThirdParty.SharpCompress.Compressors.Xz;
using ScratchCardAsset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;


[Serializable]
public class Stuff
{
    public Stuff(GameObject _gObj,int _type, int _index)
    {
        gObj = _gObj;
        type = _type;
        index = _index;
    }
    public GameObject gObj;
    public int type;
    public int index;
}


public class ContainerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject fruitPrefab;
    [SerializeField]
    private GameObject toppingPrefab;
    private int poolSize = 30;

    Dictionary<string, int> fruitSpriteDict = new Dictionary<string, int> {
            { "Grape", 0 },
            { "Apple", 1 },
            { "Banana", 2 },
            { "CherryTomato", 3 },
            { "Strawberry", 4 },
            { "Pineapple", 5 },
            { "Orange", 6 },
            { "Cherry", 7 },
            { "Mango", 8 },
            { "Kiwi", 9 },
            { "SweetSapphireGrapes", 10 },
            { "ShineMuscat", 11 },
            { "null", 12 }
    };

    Dictionary<string, int> sauseSpriteDict = new Dictionary<string, int> {
            { "Sugar", 0 },
            { "Chocolate", 1 },
            { "null", 11 }
    };

    Dictionary<string, int> toppingSpriteDict = new Dictionary<string, int> {
            { "Cookie", 0 },
            { "ChocolateRing", 1 },
            { "Almond", 2 },
            { "GummyBear", 3 },
            { "Granola", 4 },
            { "RainbowSprinkle", 5 },
            { "BaconSprinkle", 6 },
            { "HeartSprinkle", 7 },
            { "PoppingCandy", 8 },
            { "ChocoBall", 9 },
            { "Marshmallow", 10 },
            { "null", 11 }
    };

    private int maxFruitSize = 11;

    //private int maxTopppingSize = 11;

    private Camera mainCamera;
    private bool canSpawn = true;
    private float spawnDelay = 0.5f; // 0.1초 간격으로 스폰 제한

    private List<Stuff>[] Containerpools = new List<Stuff>[22];//현재 토핑과 과일 최대 갯수 22개
    [SerializeField]//private지만 인스펙터에서 볼 수 있음
    private GameObject[] FruitContainers = new GameObject[5];//과일 모음
    [SerializeField]
    private GameObject[] ToppingContainers = new GameObject[5];//토핑 모음

    public List<Stuff> GetToppingContainerPool()
    {
        if (Containerpools.Length > 11) // Check if the index is within the bounds of the array
        {
            return Containerpools[11];
        }
        else
        {
            return null; // Or handle the out-of-bounds scenario appropriately
        }
    }
    //private GameObject[] SourceContainers = new GameObject[2];// 소스 모음


    // 과일 설정 메서드를 호출하는 델리게이트 정의
    private delegate void FruitSettingAction();
    private Dictionary<string, FruitSettingAction> fruitSettings;
    private int nowPickIndex;
    // 과일 설정 메서드를 호출하는 델리게이트 정의

    [SerializeField]//그림 세팅
    private Sprite[] fruitContainerSprite = new Sprite[12];
    [SerializeField]//그림 세팅
    private Sprite[] fruitSprite = new Sprite[11];
    [SerializeField]//그림 세팅
    private Sprite[] fruitSugarCoatingSprite = new Sprite[12];
    [SerializeField]//그림 세팅
    private Sprite[] fruitChocoCoatingSprite = new Sprite[12];

    [SerializeField]//그림 세팅
    private Sprite[] toppingContainerSprite = new Sprite[11];
    [SerializeField]
    private Sprite[] toppingSprite = new Sprite[11];


    BoxCollider2D coll = null;

    private static ContainerManager instance = null;
    public static ContainerManager Instance
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

    private void Start()
    {
        mainCamera = Camera.main;
        SettingContainer();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && canSpawn)
        {
            SpawnFromContainer();
            StartCoroutine(SpawnCooldown());
        }
    }

    public void SettingContainer()
    {
        List<IngredientsData> fruitIngredients = UserDataControlManager.Instance.GetFruitIngredients();
        List<IngredientsData> sauseIngredients = UserDataControlManager.Instance.GetSourceIngredients();
        List<IngredientsData> toppingIngredients = UserDataControlManager.Instance.GetToppingListIngredients();

        for (int index = 0; index < fruitIngredients.Count; index++)
        {
            int fruitIndex = fruitIngredients[index].GetIndex();
            FruitContainers[index].name = "FruitContainer" + index;
            FruitContainerSetting(index, fruitIndex);
        }

        for (int index = 0; index < toppingIngredients.Count; index++)
        {
            int toppingIndex = toppingIngredients[index].GetIndex();
            ToppingContainers[index].name = "ToppingContainer" + index;
            ToppingContainerSetting(index, toppingIndex);
        }
    }
    private void FruitContainerSetting(int _index, int _fruitIndex)
    {
        int fruitType = 0;
        List<IngredientsData> pData = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients;
        var allFruitIngredients = pData.FindAll(x => x.GetMaterialType() == 0);

        if (Containerpools[_index] == null)
        {
            Containerpools[_index] = new List<Stuff>();
        }
        if (_fruitIndex == -1)
        {
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            int fruitLv = 1;

            // 컨테이너 안에 있는 과일오브젝트 스프라이트 설정
            Stuff fruit = new Stuff(Instantiate(fruitPrefab), fruitType, _fruitIndex);
            fruit.gObj.GetComponent<SpriteRenderer>().sprite = fruitSprite[_fruitIndex];//이미지 순서변경해야함 임시로 10개 해야
            fruit.gObj.GetComponent<PrefabScratchCardScript>().SugarSprite = fruitSugarCoatingSprite[_fruitIndex];
            fruit.gObj.GetComponent<PrefabScratchCardScript>().ChocolateSprite = fruitChocoCoatingSprite[_fruitIndex];
            fruit.gObj.GetComponent<FruitScript>().SetFruitType(_fruitIndex);//과일안에 과일 타입을 지정

            // 컨테이너 스프라이트 및 위치 설정
            FruitContainers[_index].GetComponent<SpriteRenderer>().sprite = fruitContainerSprite[_fruitIndex];
            FruitContainers[_index].transform.localPosition = new Vector2(FruitContainers[_index].transform.localPosition.x, 2.98f);
            PrefabColliderSetting(_fruitIndex, fruit);

            for (int index = 0; index < allFruitIngredients.Count; index++) 
            {
                if (allFruitIngredients[index].GetIndex() == _fruitIndex) 
                {
                    fruitLv = allFruitIngredients[index].GetCurrentLevel();
                }
            }
            fruit.gObj.GetComponent<FruitScript>().SetFruitLevel(fruitLv);//과일안에 과일 타입을 지정
            fruit.gObj.SetActive(false);
            Containerpools[_index].Add(fruit);
        }
    }


    private void ToppingContainerSetting(int _index, int _toppingIndex)
    {
        int toppingType = 0;
        List<IngredientsData> pData = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients;
        var allToppingIngredients = pData.FindAll(x => x.GetMaterialType() == 2);

        if (Containerpools[_index + maxFruitSize] == null)
        {
            Containerpools[_index + maxFruitSize] = new List<Stuff>();
        }
        if (_toppingIndex == -1)
        {
            return;
        }
        for (int i = 0; i < poolSize; i++)
        {
            int toppingLv = 1;
            Stuff topping = new Stuff(Instantiate(toppingPrefab), toppingType, _toppingIndex);
            topping.gObj.GetComponent<SpriteRenderer>().sprite = toppingSprite[_toppingIndex];//이미지 순서변경해야함 임시로 10개 해야
            topping.gObj.GetComponent<ToppingScript>().SetToppingType(_toppingIndex);

            ToppingContainers[_index].GetComponent<SpriteRenderer>().sprite = toppingContainerSprite[_toppingIndex];
            ToppingContainers[_index].transform.localPosition = new Vector2(ToppingContainers[_index].transform.localPosition.x - 0.002f, 2.85f);

            for (int index = 0; index < allToppingIngredients.Count; index++)
            {
                if (allToppingIngredients[index].GetIndex() == _toppingIndex)
                {
                    toppingLv = allToppingIngredients[index].GetCurrentLevel();
                }
            }
            topping.gObj.GetComponent<ToppingScript>().SetToppingLevel(toppingLv);//topping
            topping.gObj.SetActive(false);
            Containerpools[_index + maxFruitSize].Add(topping);
        }
    }

    private void SpawnFromContainer()
    {
        if (Camera.main == null)
        {
            UnityEngine.Debug.LogError("Camera is null in SpawnFromContainer");
            return;
        }

        // 터치한 부분이 UI Object 라면 True 반환
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        // Check if the raycast hit a collider
        if (hit.collider != null)
        {
            string name = hit.collider.name;

            switch (name)
            {
                case "FruitContainer0":
                    nowPickIndex = 0;
                    break;
                case "FruitContainer1":
                    nowPickIndex = 1;
                    break;
                case "FruitContainer2":
                    nowPickIndex = 2;
                    break;
                case "FruitContainer3":
                    nowPickIndex = 3;
                    break;
                case "FruitContainer4":
                    nowPickIndex = 4;
                    break;
                case "FruitContainer5":
                    nowPickIndex = 5;
                    break;
                case "FruitContainer6":
                    nowPickIndex = 6;
                    break;
                case "FruitContainer7":
                    nowPickIndex = 7;
                    break;
                case "FruitContainer8":
                    nowPickIndex = 8;
                    break;
                case "FruitContainer9":
                    nowPickIndex = 9;
                    break;
                case "FruitContainer10":
                    nowPickIndex = 10;
                    break;
                case "ToppingContainer0":
                    nowPickIndex = 11;
                    break;
                case "ToppingContainer1":
                    nowPickIndex = 12;
                    break;
                case "ToppingContainer2":
                    nowPickIndex = 13;
                    break;
                case "ToppingContainer3":
                    nowPickIndex = 14;
                    break;
                case "ToppingContainer4":
                    nowPickIndex = 15;
                    break;
                case "ToppingContainer5":
                    nowPickIndex = 16;
                    break;
                case "ToppingContainer6":
                    nowPickIndex = 17;
                    break;
                case "ToppingContainer7":
                    nowPickIndex = 18;
                    break;
                case "ToppingContainer8":
                    nowPickIndex = 19;
                    break;
                case "ToppingContainer9":
                    nowPickIndex = 20;
                    break;
                case "ToppingContainer10":
                    nowPickIndex = 21;
                    break;
                case "ToppingContainer11":
                    nowPickIndex = 22;
                    break;
                case "SourceContainer0":
                    nowPickIndex = 23;
                    break;
                case "SourceContainer1":
                    nowPickIndex = 24;
                    break;
                case "nullContainer":
                    nowPickIndex = -1;
                    break;
                default:
                    nowPickIndex = -1;
                    break;
            };

            if (nowPickIndex == -1) { return; }

            foreach (Stuff @object in Containerpools[nowPickIndex])
            {
                if (!@object.gObj.activeInHierarchy)
                {
                    @object.gObj.transform.position = new Vector2(hit.point.x, hit.point.y + 3.0f); // Set the position to the clicked location
                    @object.gObj.SetActive(true);
                    
                    if(@object.gObj.CompareTag("Fruit"))
                    {
                        @object.gObj.GetComponent<FruitScript>().enabled = true;

                        Sprite sprite = @object.gObj.GetComponent<PrefabScratchCardScript>().SugarSprite;
                        @object.gObj.transform.GetChild(0).GetComponent<ScratchCardManager>().ScratchSurfaceSprite = sprite;

                        // 첫 오브젝트 활성화때 발생하는 버그로 인해 첫번째 이후 부터 실행하도록 FruitScript에 index 값 추가
                        if (@object.gObj.GetComponent<FruitScript>().GetActiveIndex() != 0)
                            @object.gObj.transform.GetChild(0).GetComponent<ScratchCardManager>().FillScratchCard();

                        @object.gObj.GetComponent<FruitScript>().SetActiveIndex(1);

                        // 진동 발생 함수 (밀리초단위)
                        VibrationManager.Instance.CreateOneShot(80);

                        SoundManager.Instance.PlaySFXSound("ContainerSound");
                    }

                    if(@object.gObj.CompareTag("Topping"))
                    {
                        // 토핑 생성시 랜덤으로 토핑 회전
                        float z = UnityEngine.Random.Range(-1.0f, 1.0f);
                        @object.gObj.transform.localRotation = new Quaternion(0, 0, z, @object.gObj.transform.localRotation.w);

                        // 진동 발생 함수 (밀리초단위)
                        VibrationManager.Instance.CreateOneShot(80);

                        SoundManager.Instance.PlaySFXSound("ContainerSound");
                    }
                    break;
                }
            }
        }
        else
        {
            nowPickIndex = -1;
        }
    }

    IEnumerator SpawnCooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnDelay);
        canSpawn = true;
    }

    void PrefabColliderSetting(int _index, Stuff _obj)
    {

        switch (_index)
        {
            case 0:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(-0.2f, -0.4f);
                coll.size = new Vector2(5.5f, 6);
                break;

            case 1:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.5f,0.35f);
                coll.size = new Vector2(5.5f, 7.8f);
                break;

            case 2:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.3f, 0.4f);
                coll.size = new Vector2(6.5f, 5.5f);
                break;

            case 3:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.1f, 0.0f);
                coll.size = new Vector2(5.3f, 6);
                break;

            case 4:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.size = new Vector2(5, 7.5f);
                break;

            case 5:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.2f, 0.2f);
                coll.size = new Vector2(4.5f, 6);
                break;

            case 6:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(-0.25f, 0.2f);
                coll.size = new Vector2(5.3f, 4.8f);
                break;

            case 7:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.1f, 0.0f);
                coll.size = new Vector2(4, 8);
                break;

            case 8:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.0f, 0.1f);
                coll.size = new Vector2(4.5f, 6);
                break;

            case 9:
                coll = _obj.gObj.GetComponent<BoxCollider2D>();
                coll.offset = new Vector2(0.0f, -0.5f);
                coll.size = new Vector2(5.5f, 5.5f);
                break;
        }
    }
}