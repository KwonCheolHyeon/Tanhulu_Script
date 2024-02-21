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

    // �� ��밡���� ���� �� �������� ���� ���
    private List<List<IngredientsData>> orderMainIngredientList;
    private List<List<IngredientsData>> orderSauceList;
    private List<List<IngredientsData>> orderToppingList;
    public List<List<IngredientsData>> GetOrderMainIngredientList() { return orderMainIngredientList; }
    public List<List<IngredientsData>> GetOrderSauceList() { return orderSauceList; }
    public List<List<IngredientsData>> GetOrderToppingList() { return orderToppingList; }

    // �ֹ� ���� ���� �������� 4���� ������ ���ϵ� �ֹ� ������ ����
    // ���� ���� : 0.0f ~ 1.0f
    private float randomRate = 0.8f;

    private bool isOrder = false;
    public void SetOrder(bool _isOrder) { isOrder = _isOrder; }
    public bool GetOrder() { return isOrder; }

    // �ؽ�Ʈ ����
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

        // �ֹ������Ҷ� ��밡���� ���
        List<IngredientsData> currMainIngredientList = UserDataControlManager.Instance.GetFruitIngredients();
        List<IngredientsData> currSauceList = UserDataControlManager.Instance.GetSourceIngredients();
        List<IngredientsData> currToppingList = UserDataControlManager.Instance.GetToppingListIngredients();

        if (orderMainIngredientList.Count == 0)
        {
            // ���� ���
            RandomOrder(orderMainIngredientList, currMainIngredientList
                , Convert.ToInt32(currMainIngredientList[0].index)
                , Convert.ToInt32(currMainIngredientList[currMainIngredientList.Count - 1].index)
                , 4);

            // �ҽ�
            CreateSauceOrder(orderSauceList, currSauceList);
            

            // ����
            CreateToppingOrder(orderToppingList, currToppingList
                , Convert.ToInt32(currToppingList[0].index)
                , Convert.ToInt32(currToppingList[currToppingList.Count - 1].index));
        }

        OrderSheetPrint();

        isOrder = true;
    }

    

    // ���� �ֹ�(�ߺ� ����)
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

    // ���� �ֹ�(�ߺ� �Ұ�)
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
            
        slot[0].GetComponent<TextMeshProUGUI>().text = "���� ��� : ";
        // ���� ���
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
        // �ҽ��� 1���� 
        slot[1].GetComponent<TextMeshProUGUI>().text = "��  �� : " + orderSauceList[0][0].korName;

        // ����
        slot[2].GetComponent<TextMeshProUGUI>().text = "��  �� : " + orderToppingList[0][0].korName;

        StartButton.SetActive(true);
    }
    void SlotTextClear()
    {
        // �ؽ�Ʈ �ʱ�ȭ
        slot[0].GetComponent<TextMeshProUGUI>().SetText("");
        slot[1].GetComponent<TextMeshProUGUI>().SetText("");
        slot[2].GetComponent<TextMeshProUGUI>().SetText("");
    }
}
