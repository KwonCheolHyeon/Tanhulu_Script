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
                    Debug.Log("잘못된 스타스코어");
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


    // 스코어에 따른 리뷰 스크립트
    void Star1Case()
    {
        switch(randomNumber)
        {
            case 0:
                reviewText.text = "이게 뭐죠? 내가 주문한 탕후루가 아닌데요?";
                break;
            case 1:
                reviewText.text = "흠… 제 생각과는 다르네요. 주문서를 제대로 확인하고 만들어주세요.";
                break;
            case 2:
                reviewText.text = "조금 성의 없는 거 같은데… 탕후루를 좀 더 신경 써서 만들어주세요";
                break;
            default:
                Debug.Log("잘못된 랜덤값");
                break;
        }
    }

    void Star2Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "요청한 것과 조금 다른 거 같은데요? 아쉽네요.";
                break;
            case 1:
                reviewText.text = "조금 더 열심히 만들었으면 더 좋았을 거 같아요.";
                break;
            case 2:
                reviewText.text = "부족한 부분이 많은 것 같지만.. 나쁘지 않아요.";
                break;
            default:
                Debug.Log("잘못된 랜덤값");
                break;
        }
    }

    void Star3Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "제가 생각한 탕후루와 조금 다르지만 맛있네요.";
                break;
            case 1:
                reviewText.text = "전체적으로 괜찮았지만  조금 아쉬운 느낌 ?!";
                break;
            case 2:
                reviewText.text = "그냥 어디서나 먹을 수 있는 무난한 탕후루예요.";
                break;
            default:
                Debug.Log("잘못된 랜덤값");
                break;
        }
    }

    void Star4Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "무난하게 맛있고 생각보다 빨리 나와서 좋아요!";
                break;
            case 1:
                reviewText.text = "거의 완벽한 탕후루, 2% 아쉽지만 맛있게 먹었어요.";
                break;
            case 2:
                reviewText.text = "맛있네요. 다음에 또 올게요.";
                break;
            default:
                Debug.Log("잘못된 랜덤값");
                break;
        }
    }

    void Star5Case()
    {
        switch (randomNumber)
        {
            case 0:
                reviewText.text = "인생 탕후루! 이런 탕후루 먹을 수 있어서 오늘도 행복합니다.";
                break;
            case 1:
                reviewText.text = "너무 맛있어서 맨날 여기서만 사 먹어요. 오늘도 혈관에 탕후루가 흐른다…*";
                break;
            case 2:
                reviewText.text = "주문대로 완벽하고 빨리 나와서 너무 좋았어요!";
                break;
            default:
                Debug.Log("잘못된 랜덤값");
                break;
        }
    }

    void Feedback(string _type)
    {
        switch (_type)
        {
            // 스틱 꽂는
            case "Stick":
                reviewText.text += "\n이쁘게 꽂아주세요";
                break;

            // 시간
            case "Time":
                reviewText.text += "\n시간이 너무 오래걸려요";
                break;

            // 탕후루 조건
            case "Tanghulu":
                reviewText.text += "\n주문한 과일이 아니에요!!!!";
                break;

            // 토핑양
            case "Topping":
                reviewText.text += "\n토핑을 잘 올려주세욧!";
                break;

            // 소스 점수
            case "Source":
                reviewText.text += "\n주문한 소스로 잘 발라주면 좋겠어요! ";
                break;

            // 차가운지
            case "Cold":
                reviewText.text += "\n좀 더 차가웠으면 좋겠어요.. ㅠ ";
                break;

            default:
                Debug.Log("잘못된 랜덤값");
                break;
        }
    }
}
