using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    private static OrderManager instance = null;

    // 위 사용가능한 재료들 중 랜덤으로 돌린 결과
    private List<List<IngredientsData>> orderMainIngredientList;
    private List<List<IngredientsData>> orderSauceList;
    private List<List<IngredientsData>> orderToppingList;
    public List<List<IngredientsData>> GetOrderMainIngredientList() { return orderMainIngredientList; }
    public List<List<IngredientsData>> GetOrderSauceList() { return orderSauceList; }
    public List<List<IngredientsData>> GetOrderToppingList() { return orderToppingList; }

    // 주문 랜덤 비율 높을수록 4가지 과일이 통일된 주문 비율이 높음
    // 설정 범위 : 0.0f ~ 1.0f
    private float randomRate = 0.8f;

    private bool isOrder = false;
    public void SetOrder(bool _isOrder) { isOrder = _isOrder; }
    public bool GetOrder() { return isOrder; }

    // 텍스트 슬롯
    public List<GameObject> slot;

    public GameObject StartButton;
   

    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static OrderManager Instance
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
    private void Start()
    {
        orderMainIngredientList = new List<List<IngredientsData>>();
        orderSauceList = new List<List<IngredientsData>>();
        orderToppingList = new List<List<IngredientsData>>();
    }
    private void Update()
    {

    }

    public void OrderCrear()
    {
        orderMainIngredientList.Clear();
        orderSauceList.Clear();
        orderToppingList.Clear();
    }

    public void CreateOrder()
    {
        orderMainIngredientList.Clear();

        // 주문생성할때 사용가능한 재료
        List<IngredientsData> currMainIngredientList = UserDataControlManager.Instance.GetFruitIngredients();
        List<IngredientsData> currSauceList = UserDataControlManager.Instance.GetSourceIngredients();
        List<IngredientsData> currToppingList = UserDataControlManager.Instance.GetToppingListIngredients();

        if (orderMainIngredientList.Count == 0)
        {
            // 메인 재료
            RandomOrder(orderMainIngredientList, currMainIngredientList
                , Convert.ToInt32(currMainIngredientList[0].index)
                , Convert.ToInt32(currMainIngredientList[currMainIngredientList.Count - 1].index)
                , 4);

            // 소스
            CreateSauceOrder(orderSauceList, currSauceList);
            

            // 토핑
            CreateToppingOrder(orderToppingList, currToppingList
                , Convert.ToInt32(currToppingList[0].index)
                , Convert.ToInt32(currToppingList[currToppingList.Count - 1].index));
        }

        OrderSheetPrint();

        isOrder = true;
    }

    

    // 랜덤 주문(중복 가능)
    void RandomOrder(List<List<IngredientsData>> _orderIngredients, List<IngredientsData> _currIngredients, int _min, int _max, int _index)
    {
        if (UserDataControlManager.Instance.GetPlayerDate() >= 20)
            randomRate = 0.2f;
        else if (UserDataControlManager.Instance.GetPlayerDate() >= 10)
            randomRate = 0.5f;
        else 
        {
            randomRate = 0.8f;
        }

        float hidden = UnityEngine.Random.Range(0.0f, 1.0f);
        if (hidden <= randomRate)
        {
            int random = UnityEngine.Random.Range(0, _currIngredients.Count);
            for (int i = 0; i < _index; i++)
            {

                List<IngredientsData> ingredients = new List<IngredientsData>();
                ingredients.Add(_currIngredients[random]);

                _orderIngredients.Add(ingredients);
            }
        }
        else
        {
            for (int i = 0; i < _index; i++)
            {
                int random = UnityEngine.Random.Range(0, _currIngredients.Count);

                List<IngredientsData> ingredients = new List<IngredientsData>();
                ingredients.Add(_currIngredients[random]);

                _orderIngredients.Add(ingredients);
            }
        }
    }

    void CreateSauceOrder(List<List<IngredientsData>> _orderIngredients, List<IngredientsData> _currIngredients)
    {

        int random = UnityEngine.Random.Range(0, _currIngredients.Count);

        List<IngredientsData> ingredients = new List<IngredientsData>();
        ingredients.Add(_currIngredients[random]);

        _orderIngredients.Add(ingredients);
       
    }

    // 랜덤 주문(중복 불가)
    void CreateToppingOrder(List<List<IngredientsData>> _orderIngredients, List<IngredientsData> _currIngredients, int _min, int _max)
    {

        int random = UnityEngine.Random.Range(0, _currIngredients.Count);

        List<IngredientsData> ingredients = new List<IngredientsData>();
        ingredients.Add(_currIngredients[random]);

        _orderIngredients.Add(ingredients);
    }
    public void OrderSheetPrint()
    {
        SlotTextClear();

        if (orderMainIngredientList.Count == 0)
        {
            StartButton.SetActive(false);
            return;
        }
            
        slot[0].GetComponent<TextMeshProUGUI>().text = "메인 재료 : ";
        // 메인 재료
        for (int i = 0; i < orderMainIngredientList.Count; i++)
        {
            if (i == orderMainIngredientList.Count - 1)
            {
                slot[0].GetComponent<TextMeshProUGUI>().text += orderMainIngredientList[i][0].korName;
            }
            else
            {
                slot[0].GetComponent<TextMeshProUGUI>().text += orderMainIngredientList[i][0].korName + ", ";
            }
        }
        // 소스는 1개만 
        slot[1].GetComponent<TextMeshProUGUI>().text = "소  스 : " + orderSauceList[0][0].korName;

        // 토핑
        slot[2].GetComponent<TextMeshProUGUI>().text = "토  핑 : " + orderToppingList[0][0].korName;

        StartButton.SetActive(true);
    }
    void SlotTextClear()
    {
        // 텍스트 초기화
        slot[0].GetComponent<TextMeshProUGUI>().SetText("");
        slot[1].GetComponent<TextMeshProUGUI>().SetText("");
        slot[2].GetComponent<TextMeshProUGUI>().SetText("");
    }
}
