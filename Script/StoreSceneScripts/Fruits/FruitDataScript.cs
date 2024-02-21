using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitDataScript : MonoBehaviour
{
    private int fruitLevel = 1;
    private int fruitTypeNumber = 0;
    public void SetFruitType(int _type) { fruitTypeNumber = _type; }
    public void SetFruitLevel(int _Level) { fruitLevel = _Level; }
    public int GetFruitType() { return fruitTypeNumber; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetFruitPrice()// 과일가격 회수
    {
        int fruitBasePrice = 500;

        if (fruitTypeNumber == 0)
        {
            fruitBasePrice = 500;
        }
        else if (fruitTypeNumber == 1)
        {
            fruitBasePrice = 750;
        }
        else if (fruitTypeNumber == 2)
        {
            fruitBasePrice = 1000;
        }
        else if (fruitTypeNumber == 3)
        {
            fruitBasePrice = 2000;
        }
        else if (fruitTypeNumber == 4)
        {
            fruitBasePrice = 4000;
        }
        else if (fruitTypeNumber == 5)
        {
            fruitBasePrice = 6000;
        }
        else if (fruitTypeNumber == 6)
        {
            fruitBasePrice = 8000;
        }
        else if (fruitTypeNumber == 7)
        {
            fruitBasePrice = 10000;
        }
        else if (fruitTypeNumber == 8)
        {
            fruitBasePrice = 12000;
        }
        else if (fruitTypeNumber == 9)
        {
            fruitBasePrice = 14000;
        }
        else if (fruitTypeNumber == 10)
        {
            fruitBasePrice = 16000;
        }

        var prices = CalculateSalesPrice(fruitBasePrice, fruitLevel);

        // Check if the prices dictionary contains the fruit level
        if (prices.TryGetValue(fruitLevel, out int price))
        {
            //Debug.Log("과일 가격 출력 FruitScript.cs GetFruitPrice()");
            return price;
        }
        else
        {
            //Debug.Log("과일 가격 오류 FruitScript.cs GetFruitPrice()");
            return 0;
        }
    }

    public Dictionary<int, int> CalculateSalesPrice(int basePrice, int levels)//판매 가격을 정해주는 함수
    {
        var prices = new Dictionary<int, int>();

        for (int level = 1; level <= levels; level++)
        {
            // Each level's price increases by 5% of the base price from the previous level
            float priceIncrease = basePrice * 0.05f * (level - 1);
            int roundedPrice = Mathf.RoundToInt(basePrice + priceIncrease);
            prices.Add(level, roundedPrice);
        }

        return prices;
    }
}
