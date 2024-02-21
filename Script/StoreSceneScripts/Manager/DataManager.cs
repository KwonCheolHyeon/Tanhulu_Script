//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Ingredient
//{
//    public Ingredient(string _type, string _name, string _explain, string _indexNumber
//        , bool _isUsing, bool _isUnlock
//        , string _unlockPrice, string _salePrice, string _currentLV, string _maxLV)
//    {
//        type = _type;
//        name = _name;
//        explain = _explain;
//        indexNumber = _indexNumber;
//        isUsing = _isUsing;
//        isUnlock = _isUnlock;
//        unlockPrice = _unlockPrice;
//        salePrice = _salePrice;
//        currentLV = _currentLV;
//        maxLV = _maxLV;
//    }
//    public string type, name, explain, indexNumber; // Ÿ��, �����̸�, �ѱ��̸�, ��ü ��ȣ
//    public bool isUsing, isUnlock; // ��� ����, �ر� ����
//    public string unlockPrice, salePrice; // �ر� ����, �Ǹ� ����
//    public string currentLV, maxLV; // ���� ����, �ִ� ����
//    public string GetCurrentLV() { return currentLV; }
//    public string GetIndexNumber() { return indexNumber; }
//}

//public class Datas
//{
//    public int playerDays = 0;
//    public int playerMoney = 0;
//    public int playerStar = 0;
//    public List<Ingredient> ALLIngredientList = new List<Ingredient>();//��ü ǥ ����Ʈ

//    public List<Ingredient> mainIngredientList = new List<Ingredient>();//����� ���� ���� ����Ʈ
//    public List<Ingredient> sauceList = new List<Ingredient>();// ����� �ҽ� ����Ʈ
//    public List<Ingredient> toppingList = new List<Ingredient>();//����� ���� ����Ʈ
//    public List<Ingredient> fruitAllList = new List<Ingredient>();
//    public List<Ingredient> sauceAllList = new List<Ingredient>();
//    public List<Ingredient> toppingAllList = new List<Ingredient>();

//}

//public class DataManager : MonoBehaviour
//{
//    private Datas datas;
//    public Datas GetDatas() { return datas; }

//    private static DataManager instance = null;
//    private string datakey = "Datas";
//    private string saveFileName = "SaveFile.es3";
//    private string currentSceneName = "";
//    private string previousSceneName = "";
//    private int currentMoney = 0;
//    private int makeTanhuluCount = 0;
//    private int makeFiveTanhuluCount = 0;
//    private int throwAwayTanhuluCount = 0;


//    public void EarnCurrentMoney(int _money) { currentMoney += _money; }
//    public void SpendCurrentMoney(int _money) { currentMoney -= _money; }
//    public void MakeTanhuluCounts(int _tanghulu) { makeTanhuluCount += _tanghulu; }
//    public void MakeFiveTanhuluCounts(int _tanghulu) { makeFiveTanhuluCount += _tanghulu; }
//    public void ThrowAwayTanhuluCounts(int _tanghulu) { throwAwayTanhuluCount += _tanghulu; }

//    public int GetPlayerStar() { return datas.playerStar; }
//    public int GetPlayerMoney() { return datas.playerMoney; }
//    public int GetPlayerDay() { return datas.playerDays; }

//    public int GetCurrentMoney() { return currentMoney; }
//    public List<Ingredient> GetMainIngredientList() { return datas.mainIngredientList; }
//    public List<Ingredient> GetSauceList() { return datas.sauceList; }
//    public List<Ingredient> GetToppingList() { return datas.toppingList; }
//    public List<Ingredient> GetFruitAllInformationList() { return datas.fruitAllList; }
//    public List<Ingredient> GetSauseAllInformationList() { return datas.sauceAllList; }
//    public List<Ingredient> GetTopppingAllInformationList() { return datas.toppingAllList; }
//    public List<Ingredient> GetAllInformationList() { return datas.ALLIngredientList; }

//    void Awake()
//    {
//        if (null == instance)
//        {
//            instance = this;

//            DontDestroyOnLoad(this.gameObject);
//        }
//        else
//        {
//            Destroy(this.gameObject);
//        }

//        DataLoad();

//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }
//    public static DataManager Instance
//    {
//        get
//        {
//            if (null == instance)
//            {
//                return null;
//            }
//            return instance;
//        }
//    }

//    void Start()
//    {
//        DataLoad();
//        ClassifyMyIngredientList();
//        if (ContainerManager.Instance != null)
//        {
//            ContainerManager.Instance.SettingContainer();
//        }
//        currentMoney = 0;
//        makeTanhuluCount = 0;
//        makeFiveTanhuluCount = 0;
//        throwAwayTanhuluCount = 0;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.N))
//        {
//            // ����� ���� ������ ����
//            SoundManager.Instance.SoundDataSave();
//            VibrationManager.Instance.VibrationDataSave();
//            SceneManager.LoadScene("ItemStoreScene");
//        }


//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            DataSave();
//        }

//        if (datas.playerMoney < 0)
//        {
//            datas.playerMoney = 0;
//        }

//    }
//    void DataSave()
//    {
//        ES3.Save(datakey, datas);
//    }
//    private void DataLoad()
//    {

//        //datas = new Datas();
//        //InitializeDefaultData();
//        //DataSave();

//        //datas = ES3.Load<Datas>(datakey, saveFileName);
//        //ClassifyMyIngredientList();
//        //Debug.Log("������ �ε� �Ϸ�");


//        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
//        {
//            datas = ES3.Load<Datas>(datakey, saveFileName);
//            ClassifyMyIngredientList();
//            Debug.Log("������ �ε� �Ϸ�");
//        }
//        else
//        {
//            datas = new Datas();
//            InitializeDefaultData();
//            DataSave();

//            datas = ES3.Load<Datas>(datakey, saveFileName);
//            ClassifyMyIngredientList();
//            Debug.Log("������ �ε� �Ϸ�");
//        }
//    }


//    private void InitializeDefaultData()
//    {
//        datas.playerDays = 1;
//        datas.playerMoney = 1000;
//        datas.playerStar = 0;

//        datas.ALLIngredientList = new List<Ingredient>
//        {
//        new Ingredient("MainIngredient", "Grape", "����", "0", true, true, "0", "500", "1", "10"),//1
//        new Ingredient("MainIngredient", "Apple", "���", "1", false, false, "4000", "750", "1", "10"),//2
//        new Ingredient("MainIngredient", "Banana", "�ٳ���", "2", false, false, "9000", "1000", "1", "10"),//3
//        new Ingredient("MainIngredient", "CherryTomato", "��� �丶��", "3", false, false, "12000", "2000", "1", "10"),//4
//        new Ingredient("MainIngredient", "Strawberry", "����", "4", false, false, "30000", "4000", "1", "10"),//5
//        new Ingredient("MainIngredient", "Pineapple", "���ξ���", "5", false, false, "80000", "6000", "1", "10"),//10����
//        new Ingredient("MainIngredient", "Orange", "��", "6", false, false, "720000", "8000", "1", "10"),//20����
//        new Ingredient("MainIngredient", "Cherry", "ü��", "7", false, false, "960000", "10000", "1", "10"),//30����
//        new Ingredient("MainIngredient", "Mango", "����", "8", false, false, "1200000", "12000", "1", "10"),//40����
//        new Ingredient("MainIngredient", "Kiwi", "Ű��", "9", false, false, "2400000", "14000", "1", "10"),//60����
//        new Ingredient("MainIngredient", "SweetSapphireGrapes", "�����̾� ����", "10", false, false, "2800000", "16000", "1", "10"),//80����
//        new Ingredient("MainIngredient", "ShineMuscat", "���θӽ�Ĺ", "11", false, false, "3840000", "20000", "1", "10"),//100����
//        new Ingredient("MainIngredient", "NULL", "�����", "12", false, false, "0", "0", "1", "0"),
//        new Ingredient("Sauce", "Sugar", "����", "0", true, true, "0", "0", "1", "1"),
//        new Ingredient("Sauce", "Choco", "����", "1", true, true, "0", "0", "1", "1"),//14
//        new Ingredient("Sauce", "Caramel", "ī���", "2", false, false, "0", "0", "1", "1"),
//        new Ingredient("Topping", "Cookie", "��Ű", "0", true, true, "0", "2000", "1", "10"),//16
//        new Ingredient("Topping", "ChocolateRing", "���ڸ�", "1", false, false, "4000", "4000", "1", "10"),//17
//        new Ingredient("Topping", "Almond", "�Ƹ��", "2", false, false, "9000", "6000", "1", "10"),//18
//        new Ingredient("Topping", "GummyBear", "���̺���", "3", false, false, "12000", "8000", "1", "10"),//19
//        new Ingredient("Topping", "Granola", "�׶���", "4", false, false, "30000", "10000", "1", "10"),//20
//        new Ingredient("Topping", "RainbowSprinkle", "���κ��콺����Ŭ", "5", false, false, "80000", "12000", "1", "10"),//21
//        new Ingredient("Topping", "BaconSprinkle", "������������Ŭ", "6", false, false, "720000", "14000", "1", "10"),//22
//        new Ingredient("Topping", "HeartSprinkle", "��Ʈ��罺����Ŭ", "7", false, false, "960000", "16000", "1", "10"),//23
//        new Ingredient("Topping", "PoppingCandy", "���ɻ���", "8", false, false, "1200000", "18000", "1", "10"),//24
//        new Ingredient("Topping", "ChocoBall", "���ں�", "9", false, false, "2400000", "20000", "1", "10"),//25
//        new Ingredient("Topping", "Marshmallow", "���ø��", "10", false, false, "2800000", "40000", "1", "10"),//26
//        new Ingredient("Topping", "NULL", "�����", "11", false, false, "0", "0", "1", "0")
//        };
//    }

//    // ���� ������Ʈ�� ����ؼ� ������ �����͸� �����ϸ鼭 ���Ӱ� �߰��� �� �ֵ��� 
//    private void UpdateIngredientData()
//    {
//        List<Ingredient> updateList = new List<Ingredient>
//        {
//            new Ingredient(datas.ALLIngredientList[0].type, datas.ALLIngredientList[0].name, datas.ALLIngredientList[0].explain
//            , datas.ALLIngredientList[0].indexNumber, datas.ALLIngredientList[0].isUsing, datas.ALLIngredientList[0].isUnlock
//            , datas.ALLIngredientList[0].unlockPrice, datas.ALLIngredientList[0].salePrice
//            , datas.ALLIngredientList[0].currentLV, datas.ALLIngredientList[0].maxLV),//1

//        };
//    }

//    private void ClassifyMyIngredientList()//����
//    {//��� ������ �͵鸸 �з�

//        datas.mainIngredientList = datas.ALLIngredientList.FindAll(x => x.type == new string("MainIngredient") && x.isUsing);
//        datas.sauceList = datas.ALLIngredientList.FindAll(x => x.type == new string("Sauce") && x.isUsing);
//        datas.toppingList = datas.ALLIngredientList.FindAll(x => x.type == new string("Topping") && x.isUsing);

//        datas.fruitAllList = datas.ALLIngredientList.FindAll(x => x.type == new string("MainIngredient"));
//        datas.sauceAllList = datas.ALLIngredientList.FindAll(x => x.type == new string("Sauce"));
//        datas.toppingAllList = datas.ALLIngredientList.FindAll(x => x.type == new string("Topping"));
//    }
//    private void ClassifyMaintList()//����
//    {//��� ������ �͵鸸 �з�
//        datas.mainIngredientList = datas.ALLIngredientList.FindAll(x => x.type == new string("MainIngredient") && x.isUsing);
//    }
//    private void ClassifyToppingList()//����
//    {//��� ������ �͵鸸 �з�
//        datas.toppingList.Clear();
//        datas.toppingList = datas.ALLIngredientList.FindAll(x => x.type == new string("Topping") && x.isUsing);
//    }

//    public void UpdateObjectUnlock(string objectName)//�ر� ���� �Լ�
//    {
//        // ���� ������Ʈ���� �ش� �̸��� ���� ������Ʈ ã�� �� ����
//        for (int i = 0; i < datas.ALLIngredientList.Count; i++)
//        {
//            if (datas.ALLIngredientList[i].name == objectName)
//            {
//                Ingredient obj = datas.ALLIngredientList[i];
//                obj.isUnlock = true;

//                datas.ALLIngredientList[i] = obj; // ������ ������Ʈ�� �ٽ� ����Ʈ�� �Ҵ�
//                return;
//            }
//        }
//    }

//    public void UpdateObjectLevelUp(string objectName)//������ ����
//    {
//        // ���� ������Ʈ���� �ش� �̸��� ���� ������Ʈ ã�� �� ����
//        for (int i = 0; i < datas.ALLIngredientList.Count; i++)
//        {
//            if (datas.ALLIngredientList[i].name == objectName)
//            {
//                Ingredient obj = datas.ALLIngredientList[i];
//                int level = int.Parse(obj.currentLV);
//                if (level == 10)
//                {
//                    Debug.Log("10LV");
//                    break;
//                }
//                level += 1;
//                obj.currentLV = level.ToString();
//                datas.ALLIngredientList[i] = obj; // ������ ������Ʈ�� �ٽ� ����Ʈ�� �Ҵ�
//                return;
//            }
//        }
//        // ���� ������Ʈ���� �ش� �̸��� ���� ������Ʈ ã�� �� ����
//        for (int i = 0; i < datas.ALLIngredientList.Count; i++)
//        {
//            if (datas.ALLIngredientList[i].name == objectName)
//            {
//                Ingredient obj = datas.ALLIngredientList[i];
//                int level = int.Parse(obj.currentLV);
//                if (level == 10)
//                {
//                    Debug.Log("10LV");
//                    break;
//                }
//                level += 1;
//                obj.currentLV = level.ToString();
//                datas.ALLIngredientList[i] = obj; // ������ ������Ʈ�� �ٽ� ����Ʈ�� �Ҵ�
//                return;
//            }
//        }
//    }
//    public void UpdateObjectsUsageByNames(string objectNames, int _Number)//�ش� ������Ʈ �̸�, �ٲٴ� �ڽ� ��ȣ
//    {

//        for (int i = 0; i < datas.ALLIngredientList.Count; i++)
//        {
//            var ingredient = datas.ALLIngredientList[i];

//            // �ҽ��� ���� ����ϱ� ������ �ǳʶڴ�.
//            if (datas.ALLIngredientList[i].type == "Sauce")
//                continue;


//            if (_Number >= 0 && _Number < 5 && datas.ALLIngredientList[i].name == datas.mainIngredientList[_Number].name)
//            {
//                datas.ALLIngredientList[i].isUsing = false;
//            }


//            if (_Number >= 5 && _Number < 10 || datas.ALLIngredientList[i].name == datas.toppingList[_Number].name)
//            {
//                datas.ALLIngredientList[i].isUsing = false;
//            }


//            if (objectNames == ingredient.name)
//            {
//                datas.ALLIngredientList[i].isUsing = true;
//            }

//        }


//        if (_Number >= 0 && _Number < 5)
//        {
//            ClassifyMaintList();
//        }
//        else if (_Number >= 5 && _Number < 10)
//        {
//            ClassifyToppingList();
//        }
//    }

//    public void UpdateContainer(List<string> _fruit, List<string> _topping, List<string> _sauce)
//    {
//        for (int i = 0; i < datas.ALLIngredientList.Count; i++)
//        {
//            // �ҽ��� ���� ����ϱ� ������ �ǳʶڴ�.
//            if (datas.ALLIngredientList[i].type == "Sauce")
//                continue;

//            datas.ALLIngredientList[i].isUsing = false;
//        }

//        // ���� ����Ʈ ������Ʈ
//        foreach (string fruitName in _fruit)
//        {
//            if (fruitName != "NULL")
//            {
//                for (int i = 0; i < 27; i++)
//                {
//                    var fruit = datas.ALLIngredientList[i];
//                    if (fruitName == fruit.name)
//                    {
//                        datas.ALLIngredientList[i].isUsing = true;
//                    }
//                }
//            }
//        }

//        // ���� ����Ʈ ������Ʈ
//        foreach (string toppingName in _topping)
//        {
//            if (toppingName != "NULL")
//            {
//                for (int i = 0; i < 27; i++)
//                {
//                    var topping = datas.ALLIngredientList[i];
//                    if (toppingName == topping.name)
//                    {
//                        datas.ALLIngredientList[i].isUsing = true;
//                    }
//                }
//            }
//        }
//        DataSave();
//        ClassifyMyIngredientList();
//    }
//    public void UpdateMoney(int _getMoney)
//    {
//        datas.playerMoney += _getMoney;
//    }
//    public void UpdateStar(int _getStar)//�� ��ų� ������ ���
//    {
//        datas.playerStar += _getStar;
//    }
//    public void UpdateTitleSceneChage()
//    {
//        datas.playerDays += 1;
//        DataSave();
//        SceneManager.LoadScene("TitleScene");
//    }

//    public void UpdateNextDays()//������ ������ ���� ���� ���� �� �������� 
//    {
//        datas.playerDays += 1;
//        DataSave();
//        SceneManager.LoadScene("StoreScene");
//    }

//    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        Debug.Log("Scene Loaded: " + scene.name + " " + mode);
//        previousSceneName = currentSceneName;
//        currentSceneName = scene.name;
//        CheckSceneTransition();
//    }

//    private void CheckSceneTransition()//�� ���涧 �߻�
//    {
//        if (previousSceneName == "StoreScene" && currentSceneName == "ItemStoreScene")
//        {
//            RunFunctionForStoreToItemStore();
//        }
//        else if ((previousSceneName == "ItemStoreScene" && currentSceneName == "StoreScene")
//            || (previousSceneName == "TutorialStoreScene" && currentSceneName == "StoreScene")
//            || (previousSceneName == "TitleScene" && currentSceneName == "StoreScene"))
//        {
//            Start();
//        }
//    }

//    private void RunFunctionForStoreToItemStore()
//    {
//        var game = GameObject.Find("CalcMoneyManger");
//        game.GetComponent<SettlementScript>().SetMoney(currentMoney);
//        game.GetComponent<SettlementScript>().SetMakeTanhulu(makeTanhuluCount);
//        game.GetComponent<SettlementScript>().SetFiveTanhulu(makeFiveTanhuluCount);
//        game.GetComponent<SettlementScript>().SetFailTanhulu(throwAwayTanhuluCount);
//    }

//    void OnDestroy()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }
//}
