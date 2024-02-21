using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using static ItemStoreControlManager;

public class ItemStoreControlManager : MonoBehaviour
{
    [SerializeField]//���ϱ׸� ����
    private Sprite[] fruitSprite = new Sprite[12];
    [SerializeField]//���α׸� ����
    private Sprite[] toppingSprite = new Sprite[11];
    [SerializeField]//���α׸� ����
    private Sprite[] saurceSprite = new Sprite[2];
    [SerializeField]
    private GameObject[] IngredientChangePageScrollViewsContents;//��ũ�� ��
    [SerializeField]
    private GameObject[] IngredientPagePageScrollViewsContents;//��� ���׷��̵峪 �ر�

    [SerializeField]
    private GameObject[] IngredientChangePanel;//�ٲٴ� �г�
    [SerializeField]
    private GameObject IngredientChangePanelObjectList;//�ٲٴ� �г��� ������Ʈ ����Ʈ
    [SerializeField]
    private GameObject[] IngredientChangeButton;//�ٲٴ� ��ư
    [SerializeField]
    private GameObject[] IngredientPageButton;//�ٲٴ� ��ư
    [SerializeField]
    private GameObject SettlementScript;
    private int panelState;
    private int nowItemIndex;

    private List<int> fruitIngredientList = new List<int>();
    private List<int> toppingIngredientList = new List<int>();
    private List<int> sauceIngredientList = new List<int>();

    [SerializeField]
    public BLUROPTION BlurOption;

    [Serializable]
    public class BLUROPTION
    {
        public GameObject NextDayPage;
        public GameObject SettingPage;
        public GameObject GuidePage;
        public GameObject BlurPage;

        public GameObject LackOfMoneyPage;
        public GameObject BlurPage2;

        public GameObject IngredientChangePanel;
        public GameObject ToppingChangePanel;
        public GameObject BlurPage3;

        public GameObject SettingCanvas;
        public GameObject CalcCanvas;
    }

    private void Start()
    {
        var fruitList = UserDataControlManager.Instance.GetFruitIngredients();
        var toppingList = UserDataControlManager.Instance.GetToppingListIngredients();
        var sauceList = UserDataControlManager.Instance.GetSourceIngredients();
        for (int i = 0; i < 5; i++)
        {
            // Check if 'i' is within the bounds of 'fruitList'
            if (i < fruitList.Count && fruitList[i] != null)
            {
                fruitIngredientList.Add(fruitList[i].GetIndex());
            }
            else
            {
                fruitIngredientList.Add(-1);
            }

            // Do the same check for 'toppingList'
            if (i < toppingList.Count && toppingList[i] != null)
            {
                toppingIngredientList.Add(toppingList[i].GetIndex());
            }
            else
            {
                toppingIngredientList.Add(-1);
            }

           
        }

        for (int i = 0; i < 2; i++) 
        {
            if (i < sauceList.Count && sauceList[i] != null)
            {
                sauceIngredientList.Add(sauceList[i].GetIndex());
            }
            else
            {
                sauceIngredientList.Add(-1);
            }
        }

        panelState = 0;
        SettingScrollViews();
    }

    private void Update()
    {
       
        if (BlurOption.NextDayPage.activeSelf ||
            BlurOption.SettingPage.activeSelf ||
            BlurOption.GuidePage.activeSelf)
        {
            BlurOption.BlurPage.SetActive(true);
        }
        else
        {
            BlurOption.BlurPage.SetActive(false);
        }

        if (BlurOption.LackOfMoneyPage.activeSelf)
        {
            BlurOption.BlurPage2.SetActive(true);
        }
        else
        {
            BlurOption.BlurPage2.SetActive(false);
        }

        if (BlurOption.IngredientChangePanel.activeSelf || BlurOption.ToppingChangePanel.activeSelf)
        {
            BlurOption.BlurPage3.SetActive(true);
            BlurOption.SettingCanvas.GetComponent<Canvas>().sortingLayerName = "Default";
        }
        else if (BlurOption.CalcCanvas.activeSelf)
        {
            BlurOption.BlurPage3.SetActive(false);
            BlurOption.SettingCanvas.GetComponent<Canvas>().sortingLayerName = "Default";
        }
        else
        {
            BlurOption.BlurPage3.SetActive(false);
            BlurOption.SettingCanvas.GetComponent<Canvas>().sortingLayerName = "Topping";
        }

    }

    public void ChangeTitleScene()
    {
        VibrationManager.Instance.VibrationDataSave();
        SoundManager.Instance.SoundDataSave();
        SoundManager.Instance.PlayBGMSound("Title");

        BackGroundManager.Instance.Save();

        UserDataControlManager.Instance.UpdateContainer(fruitIngredientList, toppingIngredientList, sauceIngredientList);
        UserDataControlManager.Instance.UpdateTitleSceneChage();

    }

    public void NextDay() 
    {
        VibrationManager.Instance.VibrationDataSave();
        SoundManager.Instance.SoundDataSave();
        SoundManager.Instance.PlayBGMSound("Store");

        BackGroundManager.Instance.Save();

        UserDataControlManager.Instance.UpdateContainer(fruitIngredientList, toppingIngredientList, sauceIngredientList);
        UserDataControlManager.Instance.UpdateNextDays();
    }


    public void ChangePanelControl(int _itemIndex) 
    {

        if (_itemIndex >= 0 && _itemIndex < 5) 
        {
            panelState = 1;
        }
        else if (_itemIndex >= 5 && _itemIndex < 10)
        {
            panelState = 2;
        }
        else 
        {
            panelState = 0;
        }

        nowItemIndex = _itemIndex;

        switch (panelState) 
        {
           case 0:
                IngredientChangePanel[0].SetActive(false);
                IngredientChangePanel[1].SetActive(false);
                break;
           case 1:
                IngredientChangePanel[0].SetActive(true);//����
                ChangePanelControlSort(0);
                IngredientChangePanel[1].SetActive(false);
                break;
           case 2:
                IngredientChangePanel[0].SetActive(false);
                IngredientChangePanel[1].SetActive(true);//����
                ChangePanelControlSort(1); //���ι��� �߰�
                break;
           default:
                Debug.Log("ChangePanelControl(), panelState ����");
                break;
        }
    }

    private void ChangePanelControlSort(int _index)//�ٲٴ� �гξȿ� ��ϵ��� ����
    {
        List<IngredientsData> allList = null;
        HashSet<int> currentIngredients;

        // _index�� ���� MainIngredient �Ǵ� Topping ����Ʈ�� ����
        if (_index == 0)//����
        {
            var pData = UserDataControlManager.Instance.GetPlayerDatas();
            allList = pData.allListIngredients.FindAll(x => x.GetMaterialType() == 0 && x.GetIsUnlock() == true);
            currentIngredients = new HashSet<int>(fruitIngredientList);
        }
        else // _index ==  1  //����
        {
            var pData = UserDataControlManager.Instance.GetPlayerDatas();
            allList = pData.allListIngredients.FindAll(x => x.GetMaterialType() == 2 && x.GetIsUnlock() == true);
            currentIngredients = new HashSet<int>(toppingIngredientList);
        }

        Transform parentTransform = IngredientChangePanel[_index].transform.GetChild(0).transform;
        int childCount = parentTransform.childCount;

        // �� IngredientChangePanel�� �ڽĵ鿡 ���� ó��
        for (int i = 0; i < childCount; i++)
        {
            if (i < allList.Count)
            {
                IngredientsData ingredient = allList[i];
                // �ش� �ε����� Ingredient ������ _index�� ���� ���õ� ������ ��ġ�ϴ��� Ȯ��
                if (ingredient.GetMaterialType() == 0 || ingredient.GetMaterialType() == 2)
                {
                    //bool isUnlock = ingredient.isUnlock;
                    bool isAlreadySelected = currentIngredients.Contains(ingredient.GetIndex());

                    // �ش� �������� Ȱ�� ���� ����
                    parentTransform.GetChild(i).gameObject.SetActive(!isAlreadySelected);
                }
                else
                {
                    parentTransform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                // allList�� �ش� �ε����� ���� ���, �ش� �ڽ��� ��Ȱ��ȭ
                parentTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ChangePanelControlConnectDB(int _object)//��� �ٲٴ� �г� �ȿ��� ���ϰ� ���ο� ���� �г�
    {
        bool noSameObject = true;
        for(int index = 0; index < 5; index++) 
        {
            if (fruitIngredientList[index] == _object) 
            {
                noSameObject = false;
            }
        }
        if (noSameObject == true) 
        {
            fruitIngredientList[nowItemIndex] = _object;
            //DataManager.Instance.UpdateObjectsUsageByNames(_object, nowItemIndex);
        }
                
        ChangePanelControlOffButton();

    }
    public void ChangePanelControlToppingConnectDB(int _objectindex)//��� �ٲٴ� �г� �ȿ��� ���ϰ� ���ο� ���� �г�
    {
        bool noSameObject = true;
        for (int index = 0; index < 5; index++)
        {
            if (toppingIngredientList[index] == _objectindex)
            {
                noSameObject = false;
            }
        }
        if (noSameObject == true)
        {
            toppingIngredientList[nowItemIndex - 5] = _objectindex;

        }

        ChangePanelControlOffButton();

    }
    public void ChangePanelControlOffButton()//��� �ٲٴ� �г� �ȿ��� ���ϰ� ���ο� ���� �г�
    {
        SettingScrollViews();
        ChangePanelControl(10);
    }


    private void SettingScrollViews()
    {
        var pData = UserDataControlManager.Instance.GetPlayerDatas();

        var mainList = pData.allListIngredients.FindAll(x => x.type == 0);
        var toppingList = pData.allListIngredients.FindAll(x => x.type == 2);

        for (int index = 0; index < 5; index++)
        {
            UpdateIngredientUI(IngredientChangePageScrollViewsContents[0], index, mainList, fruitIngredientList);
            UpdateIngredientUI(IngredientChangePageScrollViewsContents[1], index, toppingList, toppingIngredientList);
        }
    }
    private void UpdateIngredientUI(GameObject scrollViewContent, int index, List<IngredientsData> ingredients, List<int> ingredientIndices)
    {
        if (index >= ingredientIndices.Count) return;

        var ingredientIndex = ingredientIndices[index];
        var ingredient = ingredients.FirstOrDefault(item => item.index == ingredientIndex);

        Transform child = scrollViewContent.transform.GetChild(index);

        if (ingredientIndices[index] != -1)
        {
            child.GetComponent<Button>().interactable = true;
            child.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Lv " + ingredient.currentLevel;
            child.GetChild(0).GetComponent<TextMeshProUGUI>().text = ingredient.korName;
            child.GetChild(1).GetComponent<Image>().sprite = GetSpriteForIngredient(ingredient);
        }
        else
        {
            child.GetComponent<Button>().interactable = false;
            child.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Lv ?";
        }
    }

    private Sprite GetSpriteForIngredient(IngredientsData ingredient)
    {
        // ��� ������ ���� ������ ��������Ʈ �迭���� ��������Ʈ�� ����
        switch (ingredient.type)
        {
            case 0: // ����
                return fruitSprite[ingredient.index];
            case 2: // ����
                return toppingSprite[ingredient.index];

            default:
                return null;
        }
    }

    public void SettingIngredientPage()//����
    {
        var allList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients;

        var fruitList = allList.FindAll(x => x.type == 0);
        var sourceList = allList.FindAll(x => x.type == 1);
        var toppingList = allList.FindAll(x => x.type == 2);
        for (int i = 0; i < fruitList.Count; i++) // ������ 0���� 10����
        {
            SetIngredientPageMainItem(fruitList, 0, i);
        }

        // ���� ����Ʈ ������ ����
        for (int i = 0; i < toppingList.Count; i++) // ������ 0���� 10����
        {
            SetIngredientPageMainItem(toppingList, 1, i);
        }

        SetSaucePageItem(sourceList, 2, 0);
        SetSaucePageItem(sourceList, 2, 1);
    }

    private void SetIngredientPageMainItem(List<IngredientsData> _allList, int _listIndex, int _index)
    {
        IngredientsData ingredient = _allList[_index];
        GameObject parentObject = IngredientPagePageScrollViewsContents[_listIndex]; // 0 for fruit, 1 for topping
        Transform childTransform = parentObject.transform.GetChild(_index).transform; // Use _index directly

       
        if (!ingredient.GetIsUnlock())
        {
            childTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ingredient.GetKorName(); // NameText
            childTransform.GetChild(1).GetComponent<Image>().sprite = (_listIndex == 0) ? fruitSprite[_index] : toppingSprite[_index]; // Image
            childTransform.GetChild(2).gameObject.SetActive(false);
            childTransform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ingredient.GetShopMoney().ToString();

            if (!childTransform.GetChild(4))
            {
                childTransform.GetChild(4).gameObject.SetActive(true);
            }
            childTransform.GetChild(5).gameObject.SetActive(false);                                                                                         
        }
        else
        {
            childTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV " + ingredient.currentLevel + " " + ingredient.korName;
            childTransform.GetChild(1).GetComponent<Image>().sprite = (_listIndex == 0) ? fruitSprite[_index] : toppingSprite[_index];
            childTransform.GetChild(2).gameObject.SetActive(true);
            childTransform.GetChild(4).gameObject.SetActive(false);
            childTransform.GetChild(5).gameObject.SetActive(false);
            if (ingredient.currentLevel == 10)
            {
                childTransform.GetChild(2).GetComponent<Button>().interactable = false;
                childTransform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Master";
                childTransform.GetChild(2).gameObject.SetActive(false);
                childTransform.GetChild(5).gameObject.SetActive(true);
                childTransform.GetChild(5).transform.GetComponent<TextMeshProUGUI>().text = "Master";
            }
            else
            {
                childTransform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SettingPrice(ingredient.shopMoney, ingredient.currentLevel).ToString();
            }

            childTransform.GetChild(3).gameObject.SetActive(false);
        }

        SettingScrollViews();
    }


    private void SetSaucePageItem(List<IngredientsData> _allList, int _listIndex, int _index)
    {
        IngredientsData ingredient = _allList[_index];
        GameObject parentObject = IngredientPagePageScrollViewsContents[_listIndex]; // 2 for sauce
        Transform childTransform = parentObject.transform.GetChild(_index).transform;

        // Sauce name and sprite setup
        string sauceName = _index == 0 ? "���� �ҽ�" : "���ݸ� �ҽ�";
        Sprite sauceSprite = _index == 0 ? saurceSprite[0] : saurceSprite[1];

        childTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sauceName; // NameText
        childTransform.GetChild(1).GetComponent<Image>().sprite = sauceSprite;

        // If the sauce is unlocked (or it is the default unlocked sauce)
        if (ingredient.isUnlock || _index == 0) // Assuming sugar sauce at index 0 is always unlocked
        {
            // Set to Master status directly upon unlockin
            childTransform.GetChild(2).gameObject.SetActive(true); // Enable the 'Master' text
            childTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Master";

            childTransform.GetChild(3).gameObject.SetActive(false);
            childTransform.GetChild(4).gameObject.SetActive(false);

        }
        else
        {
            // If the sauce is not unlocked
            childTransform.GetChild(2).gameObject.SetActive(false); // Disable the 'Master' text
            childTransform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ingredient.GetShopMoney().ToString();
            childTransform.GetChild(4).gameObject.SetActive(true);
        }
    }

    public int SettingPrice(int firstPrice, int level)
    {
        int finalPrice = 0;
        int previousPrice = firstPrice; // Holds the price of the previous level

        for (int i = 1; i <= level; i++)
        {
            switch (i)
            {
                case 1:
                    finalPrice = firstPrice + firstPrice / 2;
                    break;
                case 2:
                    finalPrice = previousPrice * 2;
                    break;
                case 3:
                    finalPrice = previousPrice + (previousPrice / 2);
                    break;
                case 4:
                    finalPrice = previousPrice * 2;
                    break;
                case 5:
                    finalPrice = previousPrice * 2;
                    break;
                case 6:
                    finalPrice = previousPrice * 2;
                    break;
                case 7:
                    finalPrice = previousPrice * 2;
                    break;
                case 8:
                    finalPrice = previousPrice * 2;
                    break;
                case 9:
                    finalPrice = previousPrice * 2;
                    break;
                default:
                    break;
            }

            previousPrice = finalPrice; // Update the previousPrice for the next iteration
        }

        return finalPrice;
    }
    public void UnLockFruit(int _index)
    {
        if (SettlementScript.GetComponent<SettlementScript>().IsDoneCalc())
        {
            var fruitList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients.FindAll(x => x.type == 0);

            if (_index >= 0 && _index < fruitList.Count)
            {
                IngredientsData fruit = fruitList[_index];
                ProcessUnlocking(fruit, _index, 0); // 0 for fruits
            }
            else
            {
                Debug.LogError("�߸��� �ε����Դϴ� - ����");
            }
        }
    }
    public void UnLockTopping(int _index)
    {
        if (SettlementScript.GetComponent<SettlementScript>().IsDoneCalc())
        {
            var toppingList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients.FindAll(x => x.type == 2);

            if (_index >= 0 && _index < toppingList.Count)
            {
                IngredientsData topping = toppingList[_index];
                ProcessUnlocking(topping, _index, 2); // 2 for toppings
            }
            else
            {
                Debug.LogError("�߸��� �ε����Դϴ� - ����");
            }
        }
    }
    private void ProcessUnlocking(IngredientsData ingredient, int _index, int _type)
    {
        int itemPrice = ingredient.shopMoney;
        int myMoney = UserDataControlManager.Instance.GetPlayerMoney();

        if (itemPrice <= myMoney)
        {
           
            UserDataControlManager.Instance.UpdateObjectUnLock(_type, _index);
            SettlementScript.GetComponent<SettlementScript>().MoneyUpdate(-itemPrice);
            Debug.Log("���ſϷ�");

            if (_type == 0)
            {
                UnLockAndFruitListUpdate(_index);
            }
            else if (_type == 2)
            {
                UnLockAndToppingListUpdate(_index);
            }
            else if (_type == 1) 
            {
                UnLockAndSourceListUpdate(_index);
            }
            SettingIngredientPage();
        }
        else
        {
            BlurOption.LackOfMoneyPage.SetActive(true);
            BlurOption.LackOfMoneyPage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "���� �����ؿ�.";
            Debug.Log("���� �����մϴ�");
        }
    }

    private void UnLockAndFruitListUpdate(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            if (fruitIngredientList[i] == -1) // '��' ���� Ȯ��
            {
                fruitIngredientList[i] = _index;
                SettingScrollViews();
                break;
            }
        }
    }

    private void UnLockAndToppingListUpdate(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            if (toppingIngredientList[i] == -1) // '��' ���� Ȯ��
            {
                toppingIngredientList[i] = _index;
                SettingScrollViews();
                break;
            }
        }
    }
    private void UnLockAndSourceListUpdate(int _index)
    {
        for (int i = 0; i < 2; i++)
        {
            if (sauceIngredientList[i] == -1) // '��' ���� Ȯ��
            {
                sauceIngredientList[i] = _index;
                SettingScrollViews();
                break;
            }
        }
    }

    public void UpgradeFruit(int _index)
    {
        if (SettlementScript.GetComponent<SettlementScript>().IsDoneCalc())
        {
            var fruitList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients.FindAll(x => x.type == 0);

            if (_index >= 0 && _index < fruitList.Count)
            {
                ProcessUpgrade(fruitList[_index], _index, 0); // 0 for fruits
            }
            else
            {
                Debug.LogError("�߸��� �ε����Դϴ� - ����");
            }
        }
    }
    public void UpgradeTopping(int _index)
    {
        if (SettlementScript.GetComponent<SettlementScript>().IsDoneCalc())
        {
            var toppingList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients.FindAll(x => x.type == 2);

            if (_index >= 0 && _index < toppingList.Count)
            {
                ProcessUpgrade(toppingList[_index], _index, 2); // 2 for toppings
            }
            else
            {
                Debug.LogError("�߸��� �ε����Դϴ� - ����");
            }
        }
    }
    private void ProcessUpgrade(IngredientsData ingredient, int _index, int _type)
    {
        int itemPrice = SettingPrice(ingredient.shopMoney, ingredient.currentLevel);
        int myMoney = UserDataControlManager.Instance.GetPlayerMoney();

        if (itemPrice <= myMoney)
        {
            Debug.Log("���ſϷ�");
            SoundManager.Instance.PlaySFXSound("Buy");
            UserDataControlManager.Instance.UpdateObjectLevelUp(_type, _index);
            SettlementScript.GetComponent<SettlementScript>().MoneyUpdate(-itemPrice);
            SettingIngredientPage();
        }
        else
        {
            BlurOption.LackOfMoneyPage.SetActive(true);
            BlurOption.LackOfMoneyPage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "���� �����ؿ�.";
            Debug.Log("���� �����մϴ�");
        }
    }

    public void UnLockSauce(int _index)
    {
        if (SettlementScript.GetComponent<SettlementScript>().IsDoneCalc())
        {
            var sauceList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients.FindAll(x => x.type == 1);

            // Since we're only concerned with the chocolate sauce (index 1)
            if (_index == 1 && _index < sauceList.Count)
            {
                IngredientsData sauce = sauceList[_index];
                if (sauce.GetMaterialType() == 1) // Check if it's a sauce
                {
                    ProcessUnlocking(sauce, _index, 1); // 1 for sauce
                }
                else
                {
                    Debug.LogError("�߸��� �ε����Դϴ� - �ҽ�");
                }
            }
            else
            {
                Debug.LogError("�߸��� �ε����Դϴ� - �ҽ�");
            }
        }
        SettingIngredientPage();
    }
}
