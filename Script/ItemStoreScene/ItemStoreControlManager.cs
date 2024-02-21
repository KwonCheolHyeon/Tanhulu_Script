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
    [SerializeField]//과일그림 세팅
    private Sprite[] fruitSprite = new Sprite[12];
    [SerializeField]//토핑그림 세팅
    private Sprite[] toppingSprite = new Sprite[11];
    [SerializeField]//토핑그림 세팅
    private Sprite[] saurceSprite = new Sprite[2];
    [SerializeField]
    private GameObject[] IngredientChangePageScrollViewsContents;//스크롤 뷰
    [SerializeField]
    private GameObject[] IngredientPagePageScrollViewsContents;//재료 업그레이드나 해금

    [SerializeField]
    private GameObject[] IngredientChangePanel;//바꾸는 패널
    [SerializeField]
    private GameObject IngredientChangePanelObjectList;//바꾸는 패널의 오브젝트 리스트
    [SerializeField]
    private GameObject[] IngredientChangeButton;//바꾸는 버튼
    [SerializeField]
    private GameObject[] IngredientPageButton;//바꾸는 버튼
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
                IngredientChangePanel[0].SetActive(true);//과일
                ChangePanelControlSort(0);
                IngredientChangePanel[1].SetActive(false);
                break;
           case 2:
                IngredientChangePanel[0].SetActive(false);
                IngredientChangePanel[1].SetActive(true);//토핑
                ChangePanelControlSort(1); //토핑버전 추가
                break;
           default:
                Debug.Log("ChangePanelControl(), panelState 에러");
                break;
        }
    }

    private void ChangePanelControlSort(int _index)//바꾸는 패널안에 목록들을 정렬
    {
        List<IngredientsData> allList = null;
        HashSet<int> currentIngredients;

        // _index에 따라 MainIngredient 또는 Topping 리스트를 선택
        if (_index == 0)//과일
        {
            var pData = UserDataControlManager.Instance.GetPlayerDatas();
            allList = pData.allListIngredients.FindAll(x => x.GetMaterialType() == 0 && x.GetIsUnlock() == true);
            currentIngredients = new HashSet<int>(fruitIngredientList);
        }
        else // _index ==  1  //토핑
        {
            var pData = UserDataControlManager.Instance.GetPlayerDatas();
            allList = pData.allListIngredients.FindAll(x => x.GetMaterialType() == 2 && x.GetIsUnlock() == true);
            currentIngredients = new HashSet<int>(toppingIngredientList);
        }

        Transform parentTransform = IngredientChangePanel[_index].transform.GetChild(0).transform;
        int childCount = parentTransform.childCount;

        // 각 IngredientChangePanel의 자식들에 대한 처리
        for (int i = 0; i < childCount; i++)
        {
            if (i < allList.Count)
            {
                IngredientsData ingredient = allList[i];
                // 해당 인덱스의 Ingredient 유형이 _index에 따라 선택된 유형과 일치하는지 확인
                if (ingredient.GetMaterialType() == 0 || ingredient.GetMaterialType() == 2)
                {
                    //bool isUnlock = ingredient.isUnlock;
                    bool isAlreadySelected = currentIngredients.Contains(ingredient.GetIndex());

                    // 해당 아이템의 활성 상태 설정
                    parentTransform.GetChild(i).gameObject.SetActive(!isAlreadySelected);
                }
                else
                {
                    parentTransform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                // allList에 해당 인덱스가 없을 경우, 해당 자식을 비활성화
                parentTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ChangePanelControlConnectDB(int _object)//재료 바꾸는 패널 안에서 과일과 토핑에 관한 패널
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
    public void ChangePanelControlToppingConnectDB(int _objectindex)//재료 바꾸는 패널 안에서 과일과 토핑에 관한 패널
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
    public void ChangePanelControlOffButton()//재료 바꾸는 패널 안에서 과일과 토핑에 관한 패널
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
        // 재료 유형에 따라 적절한 스프라이트 배열에서 스프라이트를 선택
        switch (ingredient.type)
        {
            case 0: // 과일
                return fruitSprite[ingredient.index];
            case 2: // 토핑
                return toppingSprite[ingredient.index];

            default:
                return null;
        }
    }

    public void SettingIngredientPage()//수정
    {
        var allList = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients;

        var fruitList = allList.FindAll(x => x.type == 0);
        var sourceList = allList.FindAll(x => x.type == 1);
        var toppingList = allList.FindAll(x => x.type == 2);
        for (int i = 0; i < fruitList.Count; i++) // 과일은 0부터 10까지
        {
            SetIngredientPageMainItem(fruitList, 0, i);
        }

        // 토핑 리스트 아이템 설정
        for (int i = 0; i < toppingList.Count; i++) // 과일은 0부터 10까지
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
        string sauceName = _index == 0 ? "설탕 소스" : "초콜릿 소스";
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
                Debug.LogError("잘못된 인덱스입니다 - 과일");
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
                Debug.LogError("잘못된 인덱스입니다 - 토핑");
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
            Debug.Log("구매완료");

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
            BlurOption.LackOfMoneyPage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "돈이 부족해요.";
            Debug.Log("돈이 부족합니다");
        }
    }

    private void UnLockAndFruitListUpdate(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            if (fruitIngredientList[i] == -1) // '빈' 상태 확인
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
            if (toppingIngredientList[i] == -1) // '빈' 상태 확인
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
            if (sauceIngredientList[i] == -1) // '빈' 상태 확인
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
                Debug.LogError("잘못된 인덱스입니다 - 과일");
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
                Debug.LogError("잘못된 인덱스입니다 - 토핑");
            }
        }
    }
    private void ProcessUpgrade(IngredientsData ingredient, int _index, int _type)
    {
        int itemPrice = SettingPrice(ingredient.shopMoney, ingredient.currentLevel);
        int myMoney = UserDataControlManager.Instance.GetPlayerMoney();

        if (itemPrice <= myMoney)
        {
            Debug.Log("구매완료");
            SoundManager.Instance.PlaySFXSound("Buy");
            UserDataControlManager.Instance.UpdateObjectLevelUp(_type, _index);
            SettlementScript.GetComponent<SettlementScript>().MoneyUpdate(-itemPrice);
            SettingIngredientPage();
        }
        else
        {
            BlurOption.LackOfMoneyPage.SetActive(true);
            BlurOption.LackOfMoneyPage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "돈이 부족해요.";
            Debug.Log("돈이 부족합니다");
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
                    Debug.LogError("잘못된 인덱스입니다 - 소스");
                }
            }
            else
            {
                Debug.LogError("잘못된 인덱스입니다 - 소스");
            }
        }
        SettingIngredientPage();
    }
}
