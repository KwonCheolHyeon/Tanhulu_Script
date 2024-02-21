using ScratchCardAsset;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CustomerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject resultPanel;
    private StarControlScript controlScript;
    private ReviewScript reviewScript;

    private Transform otherTr;
    private SkewerScript skewerScript;

    private bool completed = false;
  
    private bool orderCompleted = false;

    private bool completed2 = false;
    public bool GetIsCompleted2() { return completed2; }
    public bool GetIsOrderCompleted() { return orderCompleted; }
    public void SetIsOrderCompleted(bool _complete) { orderCompleted = _complete; }

    bool isFirstColl = false;

    private float moveSpeed = 10f;  // Adjust the speed as needed
   
    private Vector2 startPos;
    private Vector2 pos;

    private float dir = 1;

    private void Awake()
    {
        startPos = this.transform.position;
     }

    private void Start()
    {
        controlScript = resultPanel.GetComponent<StarControlScript>();
        reviewScript = resultPanel.GetComponent<ReviewScript>();

        pos = this.transform.position;
    }

    private void OnEnable()
    {
        this.transform.position = startPos;
        pos = startPos;
        completed2 = false;
    }

    private void OnDisable()
    {
        isFirstColl = false;
    }

    private void Update()
    {

        if (completed == true)
            PingPong(dir);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit") && isFirstColl == false)
        {
            completed2 = true;
            isFirstColl = true;

            otherTr = collision.transform.parent;
            if (otherTr == null)
                otherTr = GameObject.FindWithTag("Skewer").transform;

            skewerScript = otherTr.GetComponent<SkewerScript>();
            skewerScript.CompleteSkewer();

            GameObject.Find("Canvas").GetComponent<Exit>().SetIsGameStart(false);

            resultPanel.SetActive(true);

            int score = skewerScript.GetStarScore();
            int money = skewerScript.GetHowMuch();
            float makingTime = skewerScript.GetMakingTime();

            reviewScript.SetJumsuList(skewerScript.GetJumsu().GetInsertStickScore(), "Stick");
            reviewScript.SetJumsuList(skewerScript.GetJumsu().GetTimeScore(), "Time");
            reviewScript.SetJumsuList(skewerScript.GetJumsu().GetTanghuluStep(), "Tanghulu");
            reviewScript.SetJumsuList(skewerScript.GetJumsu().GetToppingScore(), "Topping");
            reviewScript.SetJumsuList(skewerScript.GetJumsu().GetSourceProgressScore(), "Source");
            reviewScript.SetJumsuList(skewerScript.GetJumsu().GetIsCold() == true ? 10 : -10, "Cold");

            reviewScript.SetSpeechBubble(score);

            controlScript.SetStar(score, money);

            if(score >=5)
            {
                MissionManager.Instance.ClearMission(2);
            }

            if(makingTime <= 30.0f)
            {
                MissionManager.Instance.ClearMission(0);
            }

            // 진동 발생 함수 (밀리초 단위)
            VibrationManager.Instance.CreateOneShot(200);

            SoundManager.Instance.PlaySFXSound("Eating");
           
            SkewerReset();
        }
    }

    private void SkewerReset()
    {
        // 탕후루를 Store Table로 넘어오기 전의 크기로 설정
        Vector2 scale = otherTr.localScale;
        otherTr.localScale = scale * 2.5f;

        // 과일 오브젝트의 자식들중 필요없는 자식 오브젝트 Clear
        ClearFruitChild(otherTr);

        // 꼬치 오브젝트의 자식 오브젝트들(과일) Active = false
        DeActivateChildren(otherTr);

        // 꼬치 오브젝트의 자식 오브젝트들(과일).SetParent(null)
        otherTr.DetachChildren();

        SkewerContainerScript.Instance.ReturnSkewerToPool(skewerScript.gameObject);
    }

    private void ClearFruitChild(Transform _parent)
    {
        Transform[] myChildren = _parent.GetComponentsInChildren<Transform>();

        foreach (Transform child in myChildren)
        {
            if(child.gameObject.CompareTag("Topping"))
            {
                child.gameObject.SetActive(false);
                child.SetParent(null);
            }

            if(child.gameObject.CompareTag("Fruit"))
            {
                ScratchCardManager scratch = child.transform.GetChild(0).GetComponent<ScratchCardManager>();
                scratch.FillScratchCard();
            }
        }
    }

    private void DeActivateChildren(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            child.gameObject.SetActive(false);
        }
        _parent.gameObject.SetActive(false);
    }

    private void PingPong(float _move)
    {
        pos.y += (moveSpeed * _move) * Time.deltaTime;
        this.transform.position = pos;

        if (pos.y > startPos.y + 1f)
            dir = -1;

        if (pos.y < startPos.y - 8f)
        {
            orderCompleted = true;
            completed = false;
            dir = 1;
        }
    }
}
