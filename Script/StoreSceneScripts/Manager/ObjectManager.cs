using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CustomerA;
    [SerializeField]
    private GameObject Slicker;

    private Sprite[] npcSprites;
    private int currRandom = -1;

    float orderTime = 0.0f;
    public float GetOrderTime() { return orderTime; }

    private void Start()
    {
        npcSprites = Resources.LoadAll<Sprite>("Image/Npc/NPCList");
    }
    
    private void Update()
    {
        orderTime += Time.deltaTime;
        if (TimeManager.Instance.GetTimeOver())
        {
            HandleEndTime();
        }
        else
        {
            KioskOrder();
        }
    }


    void HandleEndTime()
    {
        // 마지막 주문이 완료되었는지, 주문이 시작되지 않았는지 확인
        if (CustomerA.GetComponent<CustomerScript>().GetIsOrderCompleted() || OrderManager.Instance.GetOrder() == false)
        {
            // 데이터를 저장하고 다음 장면으로 전환
            TransitionToEndScene();
        }
    }

    void TransitionToEndScene()
    {
        SoundManager.Instance.SoundDataSave();
        VibrationManager.Instance.VibrationDataSave();
        SceneManager.LoadScene("ItemStoreScene");
        SoundManager.Instance.StopBGMSound();
        Debug.Log("Transitioning to End Scene");
    }

    void KioskOrder()
    {
        if (OrderManager.Instance.GetOrder() == false && orderTime > 3.0f)
        {
            Slicker.SetActive(true);

            CustomerA.SetActive(true);
            CustomerA.GetComponent<CustomerScript>().SetIsOrderCompleted(false);
          
            int random = Random.Range(0, npcSprites.Length);
            if(random == currRandom) 
                return;
         
            CustomerA.GetComponent<SpriteRenderer>().sprite = npcSprites[random];
            OrderManager.Instance.CreateOrder();

            SoundManager.Instance.PlaySFXSound("Customer");
        }

        bool complete = CustomerA.GetComponent<CustomerScript>().GetIsOrderCompleted();
        if (complete == true)
        {
            CustomerA.SetActive (false);    

            Slicker.SetActive(false);

            OrderManager.Instance.OrderCrear();
            OrderManager.Instance.OrderSheetPrint();
            OrderManager.Instance.SetOrder(false);

            CustomerA.GetComponent<CustomerScript>().SetIsOrderCompleted(false);
            orderTime = 0.0f;
        }
    }
}
