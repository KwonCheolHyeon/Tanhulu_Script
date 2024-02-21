using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReviewScript : MonoBehaviour
{
    public TextMeshProUGUI reviewText;
    private int randomNumber = 0;

    private List<Tuple<int, string>> jumsuList = new List<Tuple<int, string>>();
    public void SetJumsuList(int _jumsu, string _index) { jumsuList.Add(new Tuple<int, string>( _jumsu,_index)); }


    private void OnDisable()
    {
        jumsuList.Clear(); 
    }

    public void SetSpeechBubble(int _score)
    {
        randomNumber = UnityEngine.Random.Range(0, 3);

        switch(_score)
        {
            case 0:
            case 1:
                Star1Case();
                break;
            case 2:
                Star2Case();   
                break;
            case 3:
                Star3Case();
                break;
            case 4:
                Star4Case();
                break;
            case 5:
                Star5Case();
                break;
            default:
                if(_score > 5)
                {
                    Star5Case();
                }
                else
                {
                    Debug.Log("�߸��� ��Ÿ���ھ�");
                }
                break;
   
        }

        if(_score <= 4)
        {
            jumsuList.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            if(jumsuList[0].Item2 == "Cold" && jumsuList[0].Item1 == 10)
            {
                Feedback(jumsuList[1].Item2);
            }
            else
            {
                Feedback(jumsuList[0].Item2);
            }
        }
    }


    // ���ھ ���� ���� ��ũ��Ʈ
    void Star1Case()
    {
        switch(randomNumber)
        {
            case 0:
                reviewText.text = "�̰� ����? ���� �ֹ��� ���ķ簡 �ƴѵ���?";
                break;
            case 1:
                reviewText.text = "�졦 �� �������� �ٸ��׿�. �ֹ����� ����� Ȯ���ϰ� ������ּ���.";
                break;
            case 2:
                reviewText.text = "���� ���� ���� �� �������� ���ķ縦 �� �� �Ű� �Ἥ ������ּ���";
                break;
            default:
                Debug.Log("�߸��� ������");
                break;
        }
    }

    void Star2Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "��û�� �Ͱ� ���� �ٸ� �� ��������? �ƽ��׿�.";
                break;
            case 1:
                reviewText.text = "���� �� ������ ��������� �� ������ �� ���ƿ�.";
                break;
            case 2:
                reviewText.text = "������ �κ��� ���� �� ������.. ������ �ʾƿ�.";
                break;
            default:
                Debug.Log("�߸��� ������");
                break;
        }
    }

    void Star3Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "���� ������ ���ķ�� ���� �ٸ����� ���ֳ׿�.";
                break;
            case 1:
                reviewText.text = "��ü������ ����������  ���� �ƽ��� ���� ?!";
                break;
            case 2:
                reviewText.text = "�׳� ��𼭳� ���� �� �ִ� ������ ���ķ翹��.";
                break;
            default:
                Debug.Log("�߸��� ������");
                break;
        }
    }

    void Star4Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "�����ϰ� ���ְ� �������� ���� ���ͼ� ���ƿ�!";
                break;
            case 1:
                reviewText.text = "���� �Ϻ��� ���ķ�, 2% �ƽ����� ���ְ� �Ծ����.";
                break;
            case 2:
                reviewText.text = "���ֳ׿�. ������ �� �ðԿ�.";
                break;
            default:
                Debug.Log("�߸��� ������");
                break;
        }
    }

    void Star5Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "�λ� ���ķ�! �̷� ���ķ� ���� �� �־ ���õ� �ູ�մϴ�.";
                break;
            case 1:
                reviewText.text = "�ʹ� ���־ �ǳ� ���⼭�� �� �Ծ��. ���õ� ������ ���ķ簡 �帥�١�*";
                break;
            case 2:
                reviewText.text = "�ֹ���� �Ϻ��ϰ� ���� ���ͼ� �ʹ� ���Ҿ��!";
                break;
            default:
                Debug.Log("�߸��� ������");
                break;
        }
    }

    void Feedback(string _type)
    {
        switch (_type)
        {
            // ��ƽ �ȴ�
            case "Stick":
                reviewText.text += "\n�̻ڰ� �Ⱦ��ּ���";
                break;

            // �ð�
            case "Time":
                reviewText.text += "\n�ð��� �ʹ� �����ɷ���";
                break;

            // ���ķ� ����
            case "Tanghulu":
                reviewText.text += "\n�ֹ��� ������ �ƴϿ���!!!!";
                break;

            // ���ξ�
            case "Topping":
                reviewText.text += "\n������ �� �÷��ּ���!";
                break;

            // �ҽ� ����
            case "Source":
                reviewText.text += "\n�ֹ��� �ҽ��� �� �߶��ָ� ���ھ��! ";
                break;

            // ��������
            case "Cold":
                reviewText.text += "\n�� �� ���������� ���ھ��.. �� ";
                break;

            default:
                Debug.Log("�߸��� ������");
                break;
        }
    }
}
