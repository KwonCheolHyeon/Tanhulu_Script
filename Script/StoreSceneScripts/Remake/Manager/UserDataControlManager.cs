using ES3Types;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class IngredientsData
{
    public IngredientsData(int _type, string _name, string _korName, int _index, int _shopMoney, int _shopStar, int _saleMoney, int _currentLevel, bool _isUsing, bool _isUnlock)
    {
        type = _type;
        name = _name;
        korName = _korName;
        index = _index;
        shopMoney = _shopMoney;
        shopStar = _shopStar;
        saleMoney = _saleMoney;
        currentLevel = _currentLevel;
        isUsing = _isUsing;
        isUnlock = _isUnlock;
    }

    public int type        ;// 0 for fruit, 1 for sauce, 2 for topping
    public string name     ;
    public string korName  ;
    public int index       ;
    public int shopMoney   ;
    public int shopStar    ;
    public int saleMoney   ;
    public int currentLevel;
    public bool isUsing ;
    public bool isUnlock;

    public int GetMaterialType() { return type; }
    public void SetMaterialType(int _type) { type = _type; }
    public string GetName() { return name; }
    public void SetName(string _name) { name = _name; }
    public string GetKorName() { return korName; }
    public void SetKorName(string _korName) { korName = _korName; }
    public int GetIndex() { return index; }
    public void SetIndex(int _index) { index = _index; }
    public int GetShopMoney() { return shopMoney; }
    public void SetShopMoney(int _shopMoney) { shopMoney = _shopMoney; }
    public int GetShopStar() { return shopStar; }
    public void SetShopStar(int _shopStar) { shopStar = _shopStar; }
    public int GetSaleMoney() { return saleMoney; }
    public void SetSaleMoney(int _saleMoney) { saleMoney = _saleMoney; }
    public int GetCurrentLevel() { return currentLevel; }
    public void SetCurrentLevel(int _currentLevel) { currentLevel = _currentLevel; }
    public bool GetIsUsing() { return isUsing; }
    public void SetIsUsing(bool _isUsing) { isUsing = _isUsing; }
    public bool GetIsUnlock() { return isUnlock; }
    public void SetIsUnlock(bool _isUnlock) { isUnlock = _isUnlock; }

}

public class PlayerData 
{
    public int Dataversion;
    public int playerMoney;
    public int playerStar;
    public int playerDate;
    public List<IngredientsData> allListIngredients = new List<IngredientsData>();
   
}
public class UserDataControlManager : MonoBehaviour
{
    private PlayerData playerData;

    public PlayerData GetPlayerDatas() { return playerData; }
    public int GetDataVersion() { return playerData.Dataversion; }
    public void SetDataVersion(int _dataversion) { playerData.Dataversion = _dataversion; }
    public int GetPlayerMoney() { return playerData.playerMoney; }
    public void SetPlayerMoney(int _playerMoney) { playerData.playerMoney = _playerMoney; }
    public int GetPlayerStar() { return playerData.playerStar; }
    public void SetPlayerStar(int _playerStar) { playerData.playerStar = _playerStar; }
    public int GetPlayerDate() { return playerData.playerDate; }
    public void SetPlayerDate(int _playerDate) { playerData.playerDate = _playerDate; }



    private static UserDataControlManager instance = null;
    private string datakey = "PlayerDatas";
    private string saveFileName = "SaveFileTan.txt";
    private string currentSceneName = "";
    private string previousSceneName = "";
    private string alarmText = "";
    private int currentMoney = 0;
    private int makeTanhuluCount = 0;
    private int makeFiveTanhuluCount = 0;
    private int throwAwayTanhuluCount = 0;

    private List<IngredientsData> fruitIngredients = new List<IngredientsData>();
    private List<IngredientsData> sourceIngredients = new List<IngredientsData>();
    private List<IngredientsData> toppingListIngredients = new List<IngredientsData>();

    public List<IngredientsData> GetFruitIngredients() { return fruitIngredients; }
    public List<IngredientsData> GetSourceIngredients() { return sourceIngredients; }
    public List<IngredientsData> GetToppingListIngredients() { return toppingListIngredients; }

    public void EarnCurrentMoney(int _money) { currentMoney += _money; }
    public void SpendCurrentMoney(int _money) { currentMoney -= _money; }
    public void MakeTanhuluCounts(int _tanghulu) { makeTanhuluCount += _tanghulu; }
    public void MakeFiveTanhuluCounts(int _tanghulu) { makeFiveTanhuluCount += _tanghulu; }
    public void ThrowAwayTanhuluCounts(int _tanghulu) { throwAwayTanhuluCount += _tanghulu; }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        alarmText = "���ķ� ��ȸ���� Ư�� �������� ���Ծ��!\n���� �� �������� �ʵ��� �������ּ���!";

        DataLoad();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public static UserDataControlManager Instance
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
        DataLoad();

        if (ContainerManager.Instance != null)
        {
            ContainerManager.Instance.SettingContainer();
        }
        currentMoney = 0;
        makeTanhuluCount = 0;
        makeFiveTanhuluCount = 0;
        throwAwayTanhuluCount = 0;

        SubsidyPayment("StoreScene", playerData.playerMoney);
    }

    // ������ ���
    private void SubsidyPayment(string _sceneName, int _money)
    {
        if (currentSceneName == _sceneName)
        {
            if (playerData.playerDate > 10 && _money < 4000)
            {
                GameObject.Find("Canvas").transform.Find("LackOfMoney").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("LackOfMoney")
                    .transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = alarmText;

                playerData.playerMoney += 4000;
            }
            else if ((playerData.playerDate >=2 && playerData.playerDate <= 10) && _money < 2000)
            {
                GameObject.Find("Canvas").transform.Find("LackOfMoney").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("LackOfMoney")
                    .transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = alarmText;

                playerData.playerMoney += 2000;
            } 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // ����� ���� ������ ����
            SoundManager.Instance.SoundDataSave();
            VibrationManager.Instance.VibrationDataSave();
            SceneManager.LoadScene("ItemStoreScene");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void DataSave() 
    {
        ES3.Save(datakey, playerData, saveFileName);
    }
    private void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {
            playerData = ES3.Load<PlayerData>(datakey, saveFileName);
            SortMaterialData();
            Debug.Log("������ �ε� �Ϸ�");
        }
        else
        {
            playerData = new PlayerData();
            FirstSetting();
            DataSave();
            playerData = ES3.Load<PlayerData>(datakey, saveFileName);
            SortMaterialData();
            Debug.Log("������ ���� �Ϸ�");
        }
    }

    private void FirstSetting()
    {
        SetDataVersion(1);
        SetPlayerMoney(1000);
        SetPlayerStar(0);
        SetPlayerDate(1);
        FristMaterialSetting();
    }
    private void FristMaterialSetting()
    {
        playerData.allListIngredients.Add(new IngredientsData(0, "Grape", "����", 0, 500, 0, 500, 1, true, true));
        playerData.allListIngredients.Add(new IngredientsData(0, "Apple", "���", 1, 4000, 1, 750, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Banana", "�ٳ���", 2, 9000, 2, 1000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "CherryTomato", "����丶��", 3, 12000, 3, 2000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Strawberry", "����", 4, 30000, 4, 4000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Pineapple", "���ξ���", 5, 80000, 15, 6000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Orange", "������", 6, 720000, 20, 8000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Cherry", "ü��", 7, 960000, 25, 10000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Mango", "����", 8, 1200000, 30, 12000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "Kiwi", "Ű��", 9, 2400000, 35, 14000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "SweetSapphireGrapes", "���θӽ�Ĺ", 10, 2800000, 40, 16000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(0, "ShineMuscat", "���θӽ�Ĺ", 11, 3840000, 45, 18000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(1, "Sugar", "����", 0, 0, 0, 0, 10, true, true));
        playerData.allListIngredients.Add(new IngredientsData(1, "Choco", "����", 1, 10000, 20, 0, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "Cookie", "��Ű", 0, 500, 0, 500, 1, true, true));
        playerData.allListIngredients.Add(new IngredientsData(2, "ChocolateRing", "���ڸ�", 1, 4000, 1, 4000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "Almond", "�Ƹ��", 2, 9000, 2, 6000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "GummyBear", "���̺���", 3, 12000, 3, 8000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "Granola", "�׷����", 4, 30000, 4, 10000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "RainbowSprinkle", "���κ��� ������Ŭ", 5, 80000, 15, 12000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "BaconSprinkle", "������ ������Ŭ", 6, 720000, 20, 14000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "HeartSprinkle", "��Ʈ��� ������Ŭ", 7, 960000, 25, 16000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "PoppingCandy", "���� ĵ��", 8, 1200000, 30, 18000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "ChocoBall", "���ں�", 9, 2400000, 35, 20000, 0, false, false));
        playerData.allListIngredients.Add(new IngredientsData(2, "Marshmallow", "���ø��", 10, 2800000, 40, 22000, 0, false, false));
    }
    private void AddMaterial()
    {
        IngredientsData newMaterial = new IngredientsData(2, "NewMaterialName", "���ο����", 12, 50000, 5, 25000, 0, false, true);
        // �� ��� �߰�
        AddNewMaterial(newMaterial);
    }
    private void AddNewMaterial(IngredientsData newMaterial)//�߰��Ҷ� ���
    {
        var existingMaterial = playerData.allListIngredients.Find(m =>
        m.GetIndex() == newMaterial.GetIndex() && m.GetMaterialType() == newMaterial.GetMaterialType());

        // �ش� ��ᰡ ������ ����Ʈ�� �߰�
        if (existingMaterial == null)
        {
            playerData.allListIngredients.Add(newMaterial);
            //SaveMaterials(); // ������Ʈ�� ����Ʈ ����
        }
        else
        {
            // �ʿ��� ��� ���� ��� ������ ������Ʈ
            //UpdateMaterial(existingMaterial, newMaterial);
        }
    }
    private void SortMaterialData() 
    {
        fruitIngredients = playerData.allListIngredients.FindAll(x => x.GetMaterialType() == 0 && x.GetIsUsing() == true);
        sourceIngredients = playerData.allListIngredients.FindAll(x => x.GetMaterialType() == 1 && x.GetIsUsing() == true);
        toppingListIngredients = playerData.allListIngredients.FindAll(x => x.GetMaterialType() == 2 && x.GetIsUsing() == true);
        if (fruitIngredients.Count >= 6)
        {
            Debug.LogError("fruitIngredients�� ����" + fruitIngredients.Count);
        }
        else 
        {
            Debug.Log("fruitIngredients�� ����" + fruitIngredients.Count);
        }

        if (toppingListIngredients.Count >= 6)
        {
            Debug.LogError("toppingListIngredients ����" + toppingListIngredients.Count);
        }
        else
        {
            Debug.Log("toppingListIngredients ����" + toppingListIngredients.Count);
        }

        if (sourceIngredients.Count >= 3)
        {
            Debug.LogError("sourceIngredients ����" + sourceIngredients.Count);
        }
        else
        {
            Debug.Log("sourceIngredients ����" + sourceIngredients.Count);
        }

    }
    public void UpdateObjectLevelUp(int _type, int _index) //������ �Լ��� ���
    {
        IngredientsData materialToUpdate = playerData.allListIngredients.Find(m =>
        m.GetMaterialType() == _type && m.GetIndex() == _index);

        // �ش� ��ü�� �����ϸ�
        if (materialToUpdate != null)
        {
            // ���� ������ �����ͼ� 1 ����
            int newLevel = materialToUpdate.GetCurrentLevel() + 1;

            // �ִ� ������ 10�̹Ƿ�, 10�� �ʰ����� �ʰ� ����
            newLevel = Mathf.Min(newLevel, 10);

            // ���ο� ������ ����
            materialToUpdate.SetCurrentLevel(newLevel);

            // ������ ���� (�ɼ�)
            DataSave();
            SortMaterialData();

        }
        else 
        {
            Debug.LogError("UpdateObjectLevelUp ����!");
        }
    }
    public void UpdateObjectUnLock(int _type, int _index) //�رݽ� ���
    {
        IngredientsData materialToUpdate = playerData.allListIngredients.Find(m =>
           m.GetMaterialType() == _type && m.GetIndex() == _index);
        // �ش� ��ü�� �����ϸ�
        if (materialToUpdate != null && materialToUpdate.GetIsUnlock() == false)
        {

            materialToUpdate.SetIsUnlock(true);
            materialToUpdate.SetCurrentLevel(1);
            // ������ ���� (�ɼ�)
            DataSave();
            SortMaterialData();
        }
        else
        {
            Debug.LogError("UpdateObjectUnLock ����!");
        }
    }

    public void UpdateMoney(int _getMoney)
    {
        int currentMoney = GetPlayerMoney();
        int newMoney = currentMoney + _getMoney;

        if (newMoney < 0)
        {
            newMoney = 0;
        }

        SetPlayerMoney(newMoney);
        DataSave();
    }

    public void DontSaveUpdateMoney(int _getMoney)
    {
        int currentMoney = GetPlayerMoney();
        int newMoney = currentMoney + _getMoney;

        if (newMoney < 0)
        {
            newMoney = 0;
        }

        SetPlayerMoney(newMoney);
    }

    public void UpdateStar(int _getStar) 
    {
        int currentStar = GetPlayerStar();
        int newStar = currentStar + _getStar;
        if (newStar < 0)
        {
            newStar = 0;
        }
       SetPlayerStar(newStar);
        DataSave();
    }
    public void UpdateTitleSceneChage()//�������� Ÿ��Ʋ �� ����
    {
        int currentDay = GetPlayerDate();
        currentDay += 1;
        SetPlayerDate(currentDay);
        DataSave();
        SceneManager.LoadScene("TitleScene");
    }
    public void UpdateNextDays()//������ ������ ���� ���� ���� �� �������� 
    {
        int currentDay = GetPlayerDate();
        currentDay += 1;
        SetPlayerDate(currentDay);
        DataSave();
        SceneManager.LoadScene("StoreScene");
    }
    public void UpdateContainer(List<int> _fruitIndices, List<int> _toppingIndices, List<int> _sauceIndices)
    {
        // Limit the number of elements in each list to their maximum
        _fruitIndices = _fruitIndices.Take(5).ToList();
        _toppingIndices = _toppingIndices.Take(5).ToList();
        _sauceIndices = _sauceIndices.Take(2).ToList();

        // Reset 'isUsing' status of all ingredients
        foreach (var ingredient in playerData.allListIngredients)
        {
            ingredient.SetIsUsing(false);
        }

        // Update 'isUsing' status based on provided indices
        UpdateIngredientsUsage(_fruitIndices, 0); // Type 0 for fruits
        UpdateIngredientsUsage(_toppingIndices, 2); // Type 2 for toppings
        UpdateIngredientsUsage(_sauceIndices, 1); // Type 1 for sauces

        DataSave();
        SortMaterialData();
    }

    private void UpdateIngredientsUsage(List<int> indices, int type)
    {
        foreach (int index in indices)
        {
            foreach (var ingredient in playerData.allListIngredients)
            {
                if (ingredient.GetMaterialType() == type && indices.Contains(ingredient.GetIndex()))
                {
                    ingredient.SetIsUsing(true);
                }
            }
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name + " " + mode);
        previousSceneName = currentSceneName;
        currentSceneName = scene.name;
        CheckSceneTransition();
    }
    private void CheckSceneTransition()//�� ���涧 �߻�
    {
        if (previousSceneName == "StoreScene" && currentSceneName == "ItemStoreScene")
        {
            RunFunctionForStoreToItemStore();
            UpdateAchievementInfo();
        }
        else if ((previousSceneName == "ItemStoreScene" && currentSceneName == "StoreScene")
            || (previousSceneName == "TutorialStoreScene" && currentSceneName == "StoreScene")
            || (previousSceneName == "TitleScene" && currentSceneName == "StoreScene"))
        {
            Start();
        }
    }
    private void RunFunctionForStoreToItemStore()
    {
        var game = GameObject.Find("CalcMoneyManger");
        game.GetComponent<SettlementScript>().SetMoney(currentMoney);
        game.GetComponent<SettlementScript>().SetMakeTanhulu(makeTanhuluCount);
        game.GetComponent<SettlementScript>().SetFiveTanhulu(makeFiveTanhuluCount);
        game.GetComponent<SettlementScript>().SetFailTanhulu(throwAwayTanhuluCount);
    }

    private void UpdateAchievementInfo()
    {
        //0 = ���� / 2 = ����
        var unLockFruit = playerData.allListIngredients.FindAll(x => x.isUnlock == true && x.type == 0);
        var unLockTopping = playerData.allListIngredients.FindAll(x => x.isUnlock == true && x.type == 2);

        var game = GameObject.Find("AchievementManager");

        game.GetComponent<AchievementControlManager>().CheckMakeTanghuluCount(makeTanhuluCount);
        game.GetComponent<AchievementControlManager>().CheckFailTanghuluCount(throwAwayTanhuluCount);
        game.GetComponent<AchievementControlManager>().CheckEarnedMoneyOnToday(currentMoney);
        game.GetComponent<AchievementControlManager>().CheckPlayerMoney(GetPlayerMoney());
        game.GetComponent<AchievementControlManager>().CheckUnlockFruitCount(unLockFruit.Count());
        game.GetComponent<AchievementControlManager>().CheckUnlockToppingCount(unLockTopping.Count());
        game.GetComponent<AchievementControlManager>().CheckDayCount(GetPlayerDate());

        
    }


}
