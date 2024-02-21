using ScratchCardAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PrefabScratchCardScript : MonoBehaviour
{
    public ScratchCard Card;
    public EraseProgress Progress;

    private bool IsDone;
    public bool GetIsDone() { return IsDone; }

    public float CoatingProgress;
    public int score = 0;
    public int GetScore() { return score; }

    private int whichCoating = 2;
    public int GetCoatingNumber() { return whichCoating; }

    // ToppingTable 
    public GameObject ToppingTable;

    CoatingName WhichCoating;

    SpriteRenderer spriterenderer;

    // �ҽ� ĥ������ ��������Ʈ
    public Sprite SugarSprite;
    public Sprite ChocolateSprite;


    void Start()
    {
        Card = transform.GetChild(0).transform.GetChild(0).GetComponent<ScratchCard>();
        Progress = transform.GetChild(0).transform.GetChild(0).GetComponent<EraseProgress>();
        spriterenderer = transform.GetChild(0).transform.GetChild(2).GetComponent<SpriteRenderer>();

        Card.enabled = false;
        CoatingProgress = 0.0f;
        score = 0;
        whichCoating = 2;
        ToppingTable = GameObject.Find("Topping_Table");
    }

    private void OnEnable()
    {
        IsDone = false;
        Card = transform.GetChild(0).transform.GetChild(0).GetComponent<ScratchCard>();
        Progress = transform.GetChild(0).transform.GetChild(0).GetComponent<EraseProgress>();
        spriterenderer = transform.GetChild(0).transform.GetChild(2).GetComponent<SpriteRenderer>();

        Card.enabled = false;
        CoatingProgress = 0.0f;
        score = 0;
        whichCoating = 2;
        ToppingTable = GameObject.Find("Topping_Table");
    }

    void Update()
    {
        //CalculateProgress();

        if (ToppingTable.GetComponent<CoatingModeSetting>().GetFollowing() == true)
        {
            ChangeCanUseSause(true);
        }
        else
        {
            ChangeCanUseSause(false);
        }

        if (WhichCoating == CoatingName.Sugar)
        {
            whichCoating = 0;
            ChangeCoatingSprite(SugarSprite);
        }
        else if (WhichCoating == CoatingName.Chocolate)
        {
            whichCoating = 1;
            ChangeCoatingSprite(ChocolateSprite);
        }
    }

    public void ChangeCanUseSause(bool _canUseSause)
    {
        if (_canUseSause)
        {
            Card.enabled = _canUseSause;
            IsDone = _canUseSause;
            WhichCoating = (CoatingName)ToppingTable.GetComponent<CoatingModeSetting>().GetWhichCoating();
        }
        else
        {
            Card.enabled = _canUseSause;
        }
    }

    public void CalculateProgress(int _index)
    {
        // ���� ���൵ ���� 
        CoatingProgress = Progress.GetProgress();
        if (_index == 0 || _index == 3)
        {
            // ���� ���൵�� ���� ���� �߰�
            if (CoatingProgress >= 0 && CoatingProgress <= 1.0f)
            {
                if (CoatingProgress <= 0.55f)//�߰� ��
                {
                    score += 6;
                }
                else if (CoatingProgress <= 0.6f) //20�� //80%�̻�
                {
                    score += 5;
                }
                else if (CoatingProgress <= 0.65f)// 16�� // 60% �̻�
                {
                    score += 4;
                }
                else if (CoatingProgress <= 0.75f)// 5�� // 20% �̻�
                {
                    score += 2;
                }
                else
                {
                    score = 0;
                }
            }
            else 
            {
                Debug.Log("��ũ��ġ ���� ����");
            }
        }
        else 
        {
            if (CoatingProgress >= 0 && CoatingProgress <= 1.0f)
            {
                if (CoatingProgress <= 0.45f)//�߰� ��
                {
                    score += 6;
                }
                else if (CoatingProgress <= 0.5f) //20�� //80%�̻�
                {
                    score += 5;
                }
                else if (CoatingProgress <= 0.55f)// 16�� // 60% �̻�
                {
                    score += 4;
                }
                else if (CoatingProgress <= 0.65f)// 5�� // 20% �̻�
                {
                    score += 2;
                }
                else
                {
                    score = 0;
                }
            }
            else
            {
                Debug.Log("��ũ��ġ ���� ����");
            }
        }
      
    }

    private void ChangeCoatingSprite(Sprite _newSprite)
    {
        if (spriterenderer)
        {
            spriterenderer.sprite = _newSprite;
        }
    }
}

