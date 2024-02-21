using ScratchCardAsset;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FruitScript : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private new Collider2D collider;
    private SpriteRenderer sprite;
    private SpriteRenderer scratchSprite;

    private GameObject guideArrow;
    private GameObject toppingTable;

    [SerializeField]
    private GameObject limitObj1;
    [SerializeField]
    private GameObject limitObj2;
    [SerializeField]
    private GameObject limitObj3;
    [SerializeField]
    private GameObject limitObj4;

    private Vector2 initialPosition;
    private Vector3 offset;

    private Vector2 initialScale;
    public Vector2 GetInitialScale()
    {
        return initialScale;
    }

    private int fruitLevel = 1;
    private int fruitTypeNumber = 0;
    public void SetFruitType(int _type) { fruitTypeNumber = _type; }
    public void SetFruitLevel(int _Level) { fruitLevel = _Level; }
    public int GetFruitType() { return fruitTypeNumber; }

    // Fruit�� Skewer�� �浹������ �ƴ��� Ȯ��
    private bool isSticking = false;

    private bool isGuideTrigger = false;

    private float vibrationTime = 0.0f;

    private int activeIndex = 0;

    private int soundRandomNumber;

    private int toppingCounts = 0;

    private bool toppingDone = false;

    public int GetToppingCounts()
    {
        return toppingCounts;
    }
    public bool IsToppingDone()
    {
        return toppingDone;
    }
    public void SetActiveIndex(int _activeIndex) { activeIndex = _activeIndex; }
    public int GetActiveIndex() { return activeIndex; }

    void OnMouseDown()
    {
        // ��ġ�� �κ��� UI Object ��� True ��ȯ
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

         if (this.transform.parent == null)
            return;

        offset = this.transform.parent.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void OnMouseDrag()
    {
        // ��ġ�� �κ��� UI Object ��� True ��ȯ
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

        if (this.transform.parent == null)
            return;

        if (toppingTable.GetComponent<CoatingModeSetting>().GetCotingMode() == false)
        {
            if(this.transform.parent.GetComponent<SkewerScript>().GetIsFrozenStart() == false)
                this.transform.parent.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }
     
    private void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        scratchSprite = transform.GetChild(0).transform.Find("Scratch Surface Sprite").GetComponent<SpriteRenderer>();  
        guideArrow = GameObject.Find("Fruit_Table").transform.Find("Arrow").gameObject;

        toppingTable = GameObject.Find("Topping_Table");
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(this.transform.parent == null || isSticking == false)
            {
                this.transform.parent = null;    
                this.gameObject.SetActive(false);
            }
            else 
            {
                SetToLimitX();
            }
            guideArrow.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            guideArrow.SetActive(true);
            DragFruit();
        }
    }

    private void OnDisable()
    {
        isSticking = false;
        isGuideTrigger = false;
        toppingCounts = 0;
        vibrationTime = 0.0f;
        toppingDone = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("GuideArrow") && this.transform.parent == null)
        {
            isGuideTrigger = true;
            VibrationManager.Instance.CreateOneShot(40);
        }
        

        if (collision.CompareTag("Skewer") && this.transform.parent == null 
            && collision.transform.childCount < 4 && isGuideTrigger)
        {
            bool rightEnd = collision.GetComponent<SkewerScript>().IsAtRightEnd(collider);
            if (rightEnd)
            {
                // ���� �߻� �Լ� (�и��ʴ���)
                VibrationManager.Instance.CreateOneShot(50);
                soundRandomNumber = Random.Range(0, 3);
                switch (soundRandomNumber)
                {
                    case 0:
                        SoundManager.Instance.PlaySFXSound("Skewer1");
                        break;
                    case 1:
                        SoundManager.Instance.PlaySFXSound("Skewer2");
                        break;
                    case 2:
                        SoundManager.Instance.PlaySFXSound("Skewer3");
                        break;
                    default:
                        Debug.Log("��ȿ���� ���� ��");
                        break;
                }

                this.transform.SetParent(collision.transform);
                isSticking = true;
                SkewerScript skewer = this.transform.parent.GetComponent<SkewerScript>();
                if (skewer != null)
                {
                    Debug.Log("������ ����⿡ ����!");
                    skewer.CreateNextButton();
                }
                else 
                {
                    Debug.Log("������ ����⿡ �� ����!");
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Skewer") && isGuideTrigger)
        {
            vibrationTime += Time.deltaTime;
            if (vibrationTime > 0.2f)
            {
                VibrationManager.Instance.CreateOneShot(15);
                vibrationTime = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Skewer"))
        {
            bool rightEnd = collision.GetComponent<SkewerScript>().IsAtRightEnd(this.collider);
            if (rightEnd)
            {
                isSticking = false;
                collision.GetComponent<SkewerScript>().CreateNextButton();
            }
        }
    }

    private void DragFruit()
    {
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newPos = isSticking ? new Vector2(mousePosition.x, transform.position.y) : new Vector2( mousePosition.x, mousePosition.y + 3.0f);
        rigidBody.MovePosition(newPos);

        CheckXAxisLimits();
    }

    // Sticking ���¿� ���� X�� �̵� ������ Ȯ���մϴ�.
    private void CheckXAxisLimits()
    {
        // ���⼭ ������ sticking ���¿� ���� X�� ������ �����մϴ�.
        if (this.transform.parent != null)
        {
            float limitX = GetLimitX();
        }
    }

    private float GetLimitX()
    {
        int count = transform.parent.childCount;
        switch (count)
        {
            case 1: return limitObj1.transform.position.x;
            case 2: return limitObj2.transform.position.x;
            case 3: return limitObj3.transform.position.x;
            case 4: return limitObj4.transform.position.x;
            default: return initialPosition.x;
        }
    }

    private void SetToLimitX()
    {
        float limitX = GetLimitX();
        transform.position = new Vector3(limitX, transform.position.y, transform.position.z);
        // �ڿ� ������ ���ķ簡 ���� ���̵��� sortingOrder ����
        int index = this.transform.parent.childCount;
        sprite.sortingOrder = index;
        scratchSprite.sortingOrder = index;

        this.gameObject.GetComponent<FruitDataScript>().SetFruitLevel(fruitLevel);
        this.gameObject.GetComponent<FruitDataScript>().SetFruitType(fruitTypeNumber);

        this.enabled = false;
    }

    // ������ ���� ����
    public int GetFruitPrice()// ���ϰ��� ȸ��
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
            //Debug.Log("���� ���� ��� FruitScript.cs GetFruitPrice()");
            return price;
        }
        else
        {
            //Debug.Log("���� ���� ���� FruitScript.cs GetFruitPrice()");
            return 0;
        }
    }

    public Dictionary<int, int> CalculateSalesPrice(int basePrice, int levels)//�Ǹ� ������ �����ִ� �Լ�
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

    public void CalcToppingCount(int _toppingType)
    {
        var type = OrderManager.Instance.GetOrderToppingList();
        if (type[0][0].index == _toppingType) //�ֹ��� Ÿ�԰� ���� ���� ���
        {
            toppingCounts += 1;
        }
        else
        {
            toppingCounts -= 1;
            if (toppingCounts < 0)
            {
                toppingCounts = 0;
            }
        }

        if (toppingCounts >= 2)
        {
            toppingDone = true;
        }
    }
}
