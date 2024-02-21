using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToppingScript : MonoBehaviour
{
    public bool isPlacedOnFruit = false;

    private Rigidbody2D rb;
    private Camera mainCamera; // 카메라 참조를 위한 변수
    private GameObject lackOfMoney; // 돈부족 안내 오브젝트

    private string alarmText;

    private int toppingTypeNumber = 0;
    private int toppingLevel = 0;
    private bool toppingDone = false;
    private bool isFollowingMouse = false;

    public void SetToppingType(int _type) { toppingTypeNumber = _type; }
    public int GetToppingType() { return toppingTypeNumber; }

    public void SetToppingLevel(int _level) { toppingLevel = _level; }
    public int GetToppingLevel() { return toppingLevel; }

    public bool IsPlacedOnFruit => isPlacedOnFruit;
    public bool IsFollowingMouse => isFollowingMouse;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main; 

        lackOfMoney = GameObject.Find("Canvas").transform.Find("LackOfMoney").gameObject;
        alarmText = "돈이 부족합니다";
    }

    private void OnEnable()
    {
        toppingDone = false;
        isPlacedOnFruit = false;
        
    }
    private void OnDisable() 
    {
        isFollowingMouse = false;
    }

    void Update()
    {
        if (!toppingDone)
        {
            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseRelease();
            }

            if (Input.GetMouseButton(0))
            {
                FollowingTopping();
            }
        }
    }

    void FollowingTopping()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        rb.MovePosition(new Vector2( mousePosition.x, mousePosition.y));
        isFollowingMouse = true;
    }
    void HandleMouseRelease()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, transform.forward,15.0f);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider.gameObject.CompareTag("Fruit"))
            {
                this.transform.SetParent(hit.transform);
                toppingDone = true;
                isPlacedOnFruit = true;

                FruitScript parentFruit = hit.transform.GetComponentInParent<FruitScript>();
                if (parentFruit != null)
                {
                    parentFruit.CalcToppingCount(this.toppingTypeNumber);
                    SkewerScript parentSkewer = parentFruit.transform.GetComponentInParent<SkewerScript>();
                    parentSkewer.CheckToppingChild();
                }

                string spriteName = GetComponent<SpriteRenderer>().sprite.name;

                List<IngredientsData> list = UserDataControlManager.Instance.GetPlayerDatas().allListIngredients;
                foreach (IngredientsData data in list)
                {
                    if(data.korName == spriteName)
                    {
                        int money = data.saleMoney / 100;
                        if(money > UserDataControlManager.Instance.GetPlayerMoney())
                        {
                            this.transform.SetParent(null);
                            this.gameObject.SetActive(false);

                            lackOfMoney.SetActive(true);
                            lackOfMoney.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = alarmText;
                        }
                        else
                            UserDataControlManager.Instance.DontSaveUpdateMoney(-money);
                    }
                }

                continue;
            }
        }

        if(this.transform.parent == null)
        {
            this.gameObject.SetActive(false);
        }
        isFollowingMouse = false;  
    }
}
