using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class Jumsu
{
    private SkewerScript skewer;
    private int tanghuluStep;
    private bool timePerfect;//주방에서 부터 손님에게 넘길때 까지

    private int insertStickScore; //한 과일당 개별 점수
    private int sourceProgressScore;//4개의 progress의 평균값
    private int toppingScore; // 한 과일당 개별 점수 합치기
    private int timeScrore; // 시간 점수
    private bool isCold; // 냉장고에 들어갔는지 여부
                         //추가 별 관련
    private bool isStuffSequence;//재료 순서 지켰는가? 
    private bool isAllStuffDifferent; //모든 재료가 다른가?
    private bool isToppingEqually;//골고루 뿌려졌는가?
    private bool perfectSource;//소스 95%이상
    //토핑 관련 
    private int toppingLevel;//토핑의 레벨
    private int toppingType;//토핑의 타입
    public Jumsu(SkewerScript _thisSkewer, int _step, bool _time, int _insertStick,
        int _sourceProgress, int _toppingScore, bool _cold,
        bool _stuffSequence, bool _allStuffDifferent, bool _toppingEqually, int _timeScrore, bool _perfectSource,int _toppingLevel,int _toppingType)
    {
        skewer = _thisSkewer;
        tanghuluStep = _step;
        timePerfect = _time;
        insertStickScore = _insertStick;
        sourceProgressScore = _sourceProgress;
        toppingScore = _toppingScore;
        isCold = _cold;
        isStuffSequence = _stuffSequence;
        isAllStuffDifferent = _allStuffDifferent;
        isToppingEqually = _toppingEqually;
        timeScrore = _timeScrore;
        perfectSource = _perfectSource;
        toppingLevel = _toppingLevel;
        toppingType = _toppingType;
    }
    public void Reset()
    {
        tanghuluStep = 0;
        timePerfect = false;
        insertStickScore = 0;
        sourceProgressScore = 0;
        toppingScore = 0;
        timeScrore = 0;
        isCold = false;
        isStuffSequence = false;
        isAllStuffDifferent = false;
        isToppingEqually = false;
        perfectSource = false;
        toppingLevel = 0;
        toppingType = 0;
        // Add any other fields that need to be reset
    }
    public void SetTanghuluStep(int _step) { tanghuluStep = _step; }
    public void SetTimePerfect(bool _time) { timePerfect = _time; }
    public void SetTimeScrore(int _timeScrore) { timeScrore = _timeScrore; }
    public void SetInsertStickScore(int score) { insertStickScore += score; }
    public void SetSourceProgressScore(int score) { sourceProgressScore = score; }
    public void SetToppingScore(int score) { toppingScore = score; }
    public void SetColdState(bool cold) { isCold = cold; }
    public void SetStuffSequence(bool sequence) { isStuffSequence = sequence; }
    public void SetAllStuffDifferent(bool different) { isAllStuffDifferent = different; }
    public void SetToppingEqually(bool equally) { isToppingEqually = equally; }
    public void SetPerfectSource(bool _perfectSource) { perfectSource = _perfectSource; }
    public void SetToppingLevel(int _toppingLv) { toppingLevel = _toppingLv; }
    public void SetToppingType(int _toppingType) { toppingType = _toppingType; }

    public int GetTanghuluStep()
    {
        return tanghuluStep;
    }

    // Getter for timePerfect
    public bool GetTimePerfect()
    {
        return timePerfect;
    }

    // Getter for insertStickScore
    public int GetInsertStickScore()
    {
        return insertStickScore;
    }

    // Getter for sourceProgressScore
    public int GetSourceProgressScore()
    {
        return sourceProgressScore;
    }

    // Getter for toppingScore
    public int GetToppingScore()
    {
        return toppingScore;
    }

    // Getter for isCold
    public bool GetIsCold()
    {
        return isCold;
    }

    // Getter for isStuffSequence
    public bool GetIsStuffSequence()
    {
        return isStuffSequence;
    }

    // Getter for isAllStuffDifferent
    public bool GetIsAllStuffDifferent()
    {
        return isAllStuffDifferent;
    }

    // Getter for isToppingEqually
    public bool GetIsToppingEqually()
    {
        return isToppingEqually;
    }

    // Getter for timeScore
    public int GetTimeScore()
    {
        return timeScrore;
    }

    // Getter for perfectSource
    public bool GetPerfectSource()
    {
        return perfectSource;
    }

    public int GetToppingLv()
    {
        return toppingLevel;
    }

    public int GetToppingType()
    {
        return toppingType;
    }
}
public class SkewerScript : MonoBehaviour
{
    private Collider2D coll;
    private DragCamera cam;

    [SerializeField]
    private GameObject nextButton;
    [SerializeField]
    private GameObject nextTableButton;
    public GameObject GetNextTableButton() { return nextTableButton; }

    private GameObject canvas;
    private Exit canvasExit;
    public Exit GetExit() { return canvasExit; }

    private List<FruitDataScript> fruitsOnSkewer = new List<FruitDataScript>();
    private List<FruitScript> fruitsToppingCheckOnSkewer = new List<FruitScript>();

    private Jumsu jumsu;

    //private Vector3 offset;
    private Vector3 prevPos;
    public void SetPrevPos(Vector3 _prevPos) { prevPos = _prevPos; }
    public Vector3 GetPrevPos() { return prevPos; }

    private float makingTime = 0.0f;
    private Vector3 posi= new Vector3(75, -8f, 0);
    public float GetMakingTime() { return makingTime; }

    public Jumsu GetJumsu() { return jumsu; }
    // 냉동처리 확인 변수 true 반환 시 냉동처리 완료
    private bool isFrozenStart = false;
    public bool GetIsFrozenStart() { return isFrozenStart; }
    public void SetIsFrozenStart(bool _isFrozenStart) { isFrozenStart = _isFrozenStart; }

    private bool isFrozen = false;
    public bool IsFrozenState() { return isFrozen; }
    public void SetFrozenState(bool _isForzen) { isFrozen = _isForzen; }

    // 탕후루에 토핑이 있으면 true
    private bool isChildTopping = false;

    private bool toppingLittle = false;
    private int toppingCount = 0;
    private bool sourceLittle = false;
    private bool moneyOn;
    private bool bonus;

    private int starScore;
    private bool allowbiggerThanFiveStar = true;
    private bool toppingFourStar = false;
    private int moneySnum;

    private int countCoincideFruit = 0;

   

    public int GetStarScore() { return starScore; }
    public int GetHowMuch() { return moneySnum; }

    public int GetToppingCount() 
    {
        ToppingScore2();
        return toppingCount; 
    }

    private void Start()
    {
        coll = this.GetComponent<Collider2D>();
        prevPos = transform.position;

        cam = Camera.main.GetComponent<DragCamera>();

        canvas = GameObject.Find("Canvas");
        canvasExit = canvas.GetComponent<Exit>();
        canvasExit.SetSkewer(this.gameObject);

        if (nextTableButton == null)
        {
            nextTableButton = Instantiate(nextButton);
            nextTableButton.transform.SetParent(canvas.transform);
            nextTableButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200.0f, 120.0f);
            nextTableButton.GetComponent<RectTransform>().localScale = Vector3.one;
            nextTableButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { canvasExit.TableChange(); });
        }
        nextTableButton.SetActive(false);

        jumsu = new Jumsu(this, 1, false, 0, 0, 0, false, false, false, false, 0, false, 1, 0);
        moneySnum = 0;
        moneyOn = false;
        toppingLittle = false;
    }

    private void OnEnable()
    {
        isFrozenStart = false;
        isFrozen = false;
        isChildTopping = false;
        toppingLittle = false;
        toppingCount = 0;
        sourceLittle = false;
        moneyOn = false;
        bonus = false;
        toppingFourStar = false;
        starScore = 0;
        moneySnum = 0;
        countCoincideFruit = 0;
        makingTime = 0.0f;

        // Clearing and resetting lists
        fruitsOnSkewer.Clear();
        fruitsToppingCheckOnSkewer.Clear();

        // Resetting Jumsu instance if it's not null
        if (jumsu != null)
        {
            jumsu.Reset();
        }
        //this.transform.position = posi;
        prevPos = posi;

    }

    private void OnDisable() 
    {
        if (nextTableButton != null)
        {
            nextTableButton.SetActive(false);
        }
    }


    private void Update()
    {
        

        if (UnityEngine.Input.GetMouseButtonUp(0) && transform.position != prevPos)
        {
            // 냉장고와 충돌시 원래 자리로 돌아가지 못하도록 return;
            if (isFrozenStart == true )
                return;
            transform.position = prevPos;
        }

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                child.transform.parent = null;
            }

            if (transform.childCount <= 3)
            {
                if (nextTableButton != null)
                {
                    nextTableButton.SetActive(false);
                    return;
                }
            }
        }
        makingTime += Time.deltaTime;

        if (UnityEngine.Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("fruitsOnSkewer의 갯수 == " + fruitsOnSkewer.Count);
        }

    }
  
    private void CalculateStar(Jumsu _jumsu)
    {
        Debug.Log("CalculateStar() == ");
        int hap = 0;
        starScore = 0;
        bonus = false;
        hap += _jumsu.GetInsertStickScore();// 스틱 꽂는 점수
        Debug.Log("CalculateStar GetInsertStickScore == " + hap);
        hap += _jumsu.GetTimeScore();// 시간 점수
        Debug.Log("CalculateStar GetTimeScore == " + _jumsu.GetTimeScore());
        hap += _jumsu.GetTanghuluStep();// 탕후루 조건에 맞는지 점수
        Debug.Log("CalculateStar GetTanghuluStep == " + _jumsu.GetTanghuluStep());
        hap += _jumsu.GetToppingScore(); //토핑양
        Debug.Log("CalculateStar GetToppingScore == " + _jumsu.GetToppingScore());
        hap += _jumsu.GetSourceProgressScore(); //소스 점수
        Debug.Log("CalculateStar GetSourceProgressScore == " + _jumsu.GetSourceProgressScore());
        if (_jumsu.GetIsCold()) //차가우면 10점
        {
            hap += 10;
        }
        Debug.Log("CalculateStar hap == " + hap );
        if (hap >= 100)
        {
            starScore = 5;
            bonus = true;
        }
        else if (hap >= 80)
        {
            starScore = 4;
        }
        else if (hap >= 60)
        {
            starScore = 3;
        }
        else if (hap >= 40)
        {
            starScore = 2;
        }
        else if (hap >= 20)
        {
            starScore = 1;
        }
        else
        {
            starScore = 0;
        }

        if (toppingCount == 0) //토핑이 적으면 별을 깐다
        {
            starScore -= 3;
            bonus = false;
        }
        else if (toppingCount <= 2)
        {
            starScore -= 2;
            bonus = false;
        }
        else if (toppingCount <= 4 || toppingFourStar)
        {
            starScore -= 1; 
            bonus = false;
        }


        if (bonus == true && allowbiggerThanFiveStar)
        {
            if (jumsu.GetIsStuffSequence())// 순서가 맞는가?
            {
                starScore += 1;
            }

            if (jumsu.GetIsAllStuffDifferent()) //모든 재료가 다른가?
            {
                starScore += 1;
            }

            if (jumsu.GetTimePerfect()) //시간이 30초 이내로 
            {
                starScore += 1;
            }

            if (jumsu.GetIsToppingEqually()) //토핑이 골고루 뿌려 졌는가?
            {
                starScore += 1;
            }

            if (jumsu.GetPerfectSource()) //소스가 95%이상 뿌려 졌는가?
            {
                starScore += 1;
            }
        }
        Debug.Log("CalculateStar starScore == " + starScore);
        Debug.Log("CalculateStar allowbiggerThanFiveStar == " + allowbiggerThanFiveStar);
        Debug.Log("CalculateStar bonus == " + bonus);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("IceBox"))
        {
            isFrozenStart = true;
        }
    }

    public bool IsAtRightEnd(Collider2D fruitCollider)
    {
        float skewerRightEnd = coll.bounds.max.x;
        float fruitLeftEnd = fruitCollider.bounds.min.x + 3.0f;
        return fruitLeftEnd > skewerRightEnd;
    }

    public void AddFruitsToSkewer()
    {
        int fruitsCount = transform.childCount;
        if (fruitsCount == 4)
        {
            // fruitsOnSkewer List를 초기화합니다.
            fruitsOnSkewer.Clear();

            // 각 자식에 대해 FruitDataScript를 가져와서 fruitsOnSkewer List에 추가합니다.
            for (int index = 0; index < fruitsCount; index++)
            {
                Transform child = transform.GetChild(index);
                FruitDataScript fruitData = child.GetComponent<FruitDataScript>();
                FruitScript FruitScriptData = child.GetComponent<FruitScript>();

                if (fruitData != null)
                {
                    fruitsOnSkewer.Add(fruitData);
                    fruitsToppingCheckOnSkewer.Add(FruitScriptData);
                }
                else
                {
                    Debug.LogError("자식 " + child.name + "에서 FruitDataScript 컴포넌트를 찾을 수 없습니다.");
                }
            }

            // fruitsOnSkewer List가 올바르게 채워졌습니다.
        }
        else
        {
            Debug.LogError("막대기 오브젝트의 과일 수가 4개가 아닙니다.");
        }
    }

    public void AddFruitToSkewer()
    {
        foreach (var fruitOnSkewer in fruitsOnSkewer)
        {
            float relativeY = fruitOnSkewer.transform.localPosition.y;

            // Check if the relative Y position is within the desired range
            if (relativeY >= -2.0f && relativeY <= 2.0f)
            {
                jumsu.SetInsertStickScore(5); // Add 5 points
            }
            else
            {
                jumsu.SetInsertStickScore(3); // Add 3 points
            }
        }
    }

    public void CheckFruitSequence()//순서와 해당 과일 체크하는 함수 //// 마지막에 체크
    {
        var nowOrder = OrderManager.Instance.GetOrderMainIngredientList();
        Debug.Log("CheckFruitSequence() nowOrder == " + nowOrder);

        Debug.Log("CheckFruitSequence() fruitsOnSkewer == " + fruitsOnSkewer.Count);
        if (DoIngredientsMatch(nowOrder, fruitsOnSkewer)) //요소들은 똑같다
        {
            Debug.Log("요소들 만족 CheckFruitSequence()함수");
            jumsu.SetTanghuluStep(20);
        }
        else 
        {
            if (countCoincideFruit == 3)
            {
                jumsu.SetTanghuluStep(15);
            }
            else if (countCoincideFruit == 2)
            {
                jumsu.SetTanghuluStep(10);
            }
            else if (countCoincideFruit == 1)
            {
                jumsu.SetTanghuluStep(-40);
            }
            else if (countCoincideFruit == 0) 
            {
                jumsu.SetTanghuluStep(-50);
            }
            allowbiggerThanFiveStar = false;
            Debug.Log("요소들 불 만족 CheckFruitSequence()함수");
        }

        if (DoIngredientsMatchInOrder(nowOrder, fruitsOnSkewer)) //순서까지 똑같다
        {
            jumsu.SetStuffSequence(true);
        }

        if (AreSpecificIndicesUnique(nowOrder)) //모든 재료가 다 다르다.
        {
            jumsu.SetAllStuffDifferent(true);
        }


    }
    public bool DoIngredientsMatch(List<List<IngredientsData>> ingredients, List<FruitDataScript> fruits)
    {
        // Check if there are exactly 4 fruits for comparison
        if (fruits.Count != 4 || ingredients.Count != 4)
        {
            Debug.Log($"DoIngredientsMatch 꼬치에 과일과 오더가 4개가 안됨: {ingredients.Count}, Fruits count: {fruits.Count}");
            return false;
        }

        int matchCount = 0;
        List<int> ingredientIndexNumbers = new List<int>();
        List<int> fruitTypes = new List<int>();

        // Populate lists with index numbers and fruit types
        for (int i = 0; i < 4; i++)
        {
            if (ingredients[i].Count > 0)
            {
                //int ingredientIndex;
                if (ingredients[i][0].index == -1)
                {
                    Debug.Log($"Failed to parse ingredient index number at position {i}: {ingredients[i][0].index}");
                    return false;
                }
                ingredientIndexNumbers.Add(ingredients[i][0].index);
            }
            else
            {
                Debug.Log($"No ingredients found at position {i}");
                return false;
            }

            fruitTypes.Add(fruits[i].GetFruitType());
        }

        // Check for matches irrespective of order
        foreach (var fruitType in fruitTypes)
        {
            if (ingredientIndexNumbers.Contains(fruitType))
            {
                matchCount++;
                // Optional: Remove the matched element to avoid counting duplicates
                ingredientIndexNumbers.Remove(fruitType);
            }
        }

        if (matchCount >= 2) 
        {
            moneyOn = true;
        }
        countCoincideFruit = matchCount;
        if (matchCount == 4)
        {
            return true;
        }
        else
        {
            Debug.Log($"Not all elements matched. Matches found: {matchCount}/4");
            return false;
        }
    }

    public bool DoIngredientsMatchInOrder(List<List<IngredientsData>> ingredients, List<FruitDataScript> fruits)
    {
        // Check if there are exactly 4 fruits for comparison
        if (fruits.Count != 4)
        {
            return false;
        }

        // Compare elements in the same order
        for (int i = 0; i < 4; i++)
        {
            if (ingredients.Count > i && ingredients[i].Count > 0)
            {
                int ingredientIndex = ingredients[i][0].index;
                int fruitType = fruits[i].GetFruitType();

                if (ingredientIndex != fruitType)
                {
                    // If any pair of elements don't match, return false
                    return false;
                }
            }
            else
            {
                // Ingredient list is not as expected, return false
                return false;
            }
        }
        // All elements matched in order
        return true;
    }//순서까지 똑같다

    public bool AreSpecificIndicesUnique(List<List<IngredientsData>> ingredients)
    {
        HashSet<int> encounteredIndices = new HashSet<int>();

        for (int i = 0; i <= 3; i++)
        {
            // Check if the sublist exists and has at least one element
            if (ingredients.Count > i && ingredients[i].Count > 0)
            {
                int indexNumber = ingredients[i][0].index;

                if (encounteredIndices.Contains(indexNumber))
                {
                    // Duplicate indexNumber found
                    return false;
                }
                else
                {
                    encounteredIndices.Add(indexNumber);
                }
            }
        }

        // No duplicates found
        return true;
    } //모든 재료가 다 다르다.


    public void ReMakeToppingScrore() 
    {
        int toppingCounts = 0;
        bool toppingManJum = false;
        foreach (FruitScript fruit in fruitsToppingCheckOnSkewer)
        {
            int toppings = fruit.GetToppingCounts();
            if (toppings >= 2) 
            {
                toppingCounts += 2;
            }
            else
            {
                toppingCounts += toppings;
            }
        }

        if (toppingCounts >= 8)
        {
            jumsu.SetToppingScore(20);

            toppingManJum = true;
        }
        else if (toppingCounts >= 6)
        {
            jumsu.SetToppingScore(10);
            allowbiggerThanFiveStar = false;
        }
        else if (toppingCounts >= 4)
        {
            jumsu.SetToppingScore(5);
            allowbiggerThanFiveStar = false;
        }
        else
        {
            jumsu.SetToppingScore(0);
            toppingLittle = true;
            allowbiggerThanFiveStar = false;
        }


        toppingCount = toppingCounts;

        if (toppingManJum) 
        {

            bool allToppingsDone = true;

            // Assuming fruitsOnSkewer is a List of FruitDataScript objects
            foreach (FruitScript fruit in fruitsToppingCheckOnSkewer)
            {
                // Check if the IsToppingDone property exists and is true for each fruit
                if (!fruit.IsToppingDone())
                {
                    allToppingsDone = false;

                    break;
                }
            }

            // If all fruits have their toppings done, set topping equally
            if (allToppingsDone)
            {
                jumsu.SetToppingEqually(true);
            }
            else 
            {
                toppingFourStar = true;
            }
        }
    }

    public void ToppingScore2()
    {
        toppingCount = 0; // 토핑의 총 개수를 저장할 변수 초기화

        for (int i = 0; i < fruitsOnSkewer.Count; i++) // 꼬치에 있는 모든 과일에 대해 반복
        {
            Transform fruitTransform = fruitsOnSkewer[i].gameObject.transform;
            int howmanyTopping = fruitTransform.childCount - 1; // 첫 번째 자식 (ScratchCardManager) 제외

            toppingCount += howmanyTopping; // 각 과일에 올려진 토핑의 수를 toppingCount에 더함
        }

        // toppingCount 값에 따라 토핑 점수 계산 로직은 제거됨
    }

    public void RefrigeratorOn()
    {
        if (isFrozen)
        {
            jumsu.SetColdState(true);
            Debug.Log("RefrigeratorOn() 함수 isFrozen == " + isFrozen);
        }
        else 
        {
            allowbiggerThanFiveStar = false;
            Debug.Log("RefrigeratorOn() 함수 isFrozen == " + isFrozen);
        }

    }
    public void TimeScore()
    {
        if (makingTime <= 30.0f)
        {
            jumsu.SetTimeScrore(20);
            jumsu.SetTimePerfect(true);
        }
        else if (makingTime <= 40.0f)
        {
            jumsu.SetTimeScrore(16);
        }
        else if (makingTime <= 50.0f)
        {
            allowbiggerThanFiveStar = false;
            jumsu.SetTimeScrore(12);
        }
        else if (makingTime <= 60.0f)
        {
            allowbiggerThanFiveStar = false;
            jumsu.SetTimeScrore(8);
        }
        else
        {
            allowbiggerThanFiveStar = false;
            jumsu.SetTimeScrore(4);
        }
        Debug.Log("TimeScore() 함수 makingTime == " + makingTime);
    }

    public void ScratchScore()
    {
        int ScoreHap = 0;
        var list = OrderManager.Instance.GetOrderSauceList();

        for (int i = 0; i < fruitsOnSkewer.Count; i++)
        {
            fruitsOnSkewer[i].GetComponent<PrefabScratchCardScript>().CalculateProgress(i);
            ScoreHap += fruitsOnSkewer[i].GetComponent<PrefabScratchCardScript>().GetScore();
        }

        for (int i = 0; i < fruitsOnSkewer.Count; i++)
        {
            if (list[0][0].index == fruitsOnSkewer[i].GetComponent<PrefabScratchCardScript>().GetCoatingNumber())
            {

            }
            else 
            {
                ScoreHap -= 10;
            }
        }
        Debug.Log("ScratchScore() 함수 ScratchScore == " + ScoreHap);
        if (ScoreHap >= 24)//추가 별
        {
            jumsu.SetSourceProgressScore(20);
            jumsu.SetPerfectSource(true);

        }
        else if (ScoreHap < 24 && ScoreHap >= 20) // 추가별은 없음
        {
            jumsu.SetSourceProgressScore(20);
        }
        else
        {
            allowbiggerThanFiveStar = false;
            jumsu.SetSourceProgressScore(ScoreHap);
        }

        if (ScoreHap <= 10)
        {
            allowbiggerThanFiveStar = false;
            sourceLittle = true;
        }
        else 
        {
            sourceLittle = false;
        }


    }
    public void CompleteSkewer()
    {
        // Assuming all fruits are added
        AddFruitToSkewer();

        // Check the sequence and type of fruits
        CheckFruitSequence();

        // Calculate the topping score
        ReMakeToppingScrore();

        // Call this when the skewer is put in the refrigerator
        RefrigeratorOn();

        // Calculate time score
        TimeScore();

        // Call this method when the scratch card task is completed
        ScratchScore();

        CalcMoney();

    }
    private void CalcMoney()
    {
        CalculateStar(GetJumsu());

        int stars  = starScore;
        if (moneyOn == true && starScore>=2)// 과일이 다르다 0점
        {
            int originalMoney = 0;
            for (int i = 0; i < fruitsOnSkewer.Count; i++)
            {
                moneySnum += fruitsOnSkewer[i].GetFruitPrice();//각 과일의 가격을 가져와서 더한다.
            }
            
            moneySnum = Mathf.RoundToInt((moneySnum / 3) * 2 );//기본 돈 줄여놓은 상태
            originalMoney = moneySnum;

            
            switch (starScore)
            {
                case 6:
                    moneySnum += Mathf.RoundToInt(moneySnum * 0.2f);
                    break;
                case 7:
                    moneySnum += Mathf.RoundToInt(moneySnum * 0.4f);
                    break;
                case 8:
                    moneySnum += Mathf.RoundToInt(moneySnum * 0.6f);
                    break;
                case 9:
                    moneySnum += Mathf.RoundToInt(moneySnum * 0.8f);    
                    break;
                case 10:
                    moneySnum *= 2; // If you're doubling the money, simply multiply by 2
                    break;
            }



            if (sourceLittle == true) //소스가 적다 /2
            {
                moneySnum = Mathf.RoundToInt(moneySnum / 2);
            }

            if (toppingLittle == true)// 토핑이 적다 /2
            {
                moneySnum = Mathf.RoundToInt(moneySnum / 2);
            }
            else
            {
                Debug.Log("토핑 돈 계산 완료");
                moneySnum += ToppingCalc(jumsu.GetToppingScore(), jumsu.GetToppingLv(), jumsu.GetToppingType(), originalMoney);
            }

            if (isFrozen == false) // 냉동이 아니다 /2
            {
                moneySnum = Mathf.RoundToInt(moneySnum / 2);
            }


        }
        else 
        {
            Debug.Log("moneyOn == false");


            int originalMoney = 0;
            for (int i = 0; i < fruitsOnSkewer.Count; i++)
            {
                moneySnum += fruitsOnSkewer[i].GetFruitPrice();//각 과일의 가격을 가져와서 더한다.
            }

            if (countCoincideFruit == 2)
            {
                moneySnum /= 2;
            }
            else if (countCoincideFruit == 3)
            {
               int money = moneySnum/4;
               moneySnum = money*3;
            }
            else 
            {
                starScore = 0;
            }

            originalMoney = moneySnum;

            if (sourceLittle == true) //소스가 적다 /2
            {
                moneySnum = Mathf.RoundToInt(moneySnum / 2);
            }

            if (toppingLittle == true)// 토핑이 적다 /2
            {
                moneySnum = Mathf.RoundToInt(moneySnum / 2);
            }
            else
            {
                Debug.Log("토핑 돈 계산 완료");
                moneySnum += ToppingCalc(jumsu.GetToppingScore(), jumsu.GetToppingLv(), jumsu.GetToppingType(), originalMoney);
            }

            if (isFrozen == false) // 냉동이 아니다 /2
            {
                moneySnum = Mathf.RoundToInt(moneySnum / 2);
            }

            if (starScore <= 0) 
            {
                Debug.Log("starScore <= 0  starScore == " + starScore);
                moneySnum = 0;
            }
            countCoincideFruit = 0;
        }


    }
    private int ToppingCalc(int _toppingJumsu,int _toppingLv,int _toppingType, int _originalMoney) 
    {
        float addMoney = ((_toppingType + 1) * 5) + (_toppingLv * 0.3f);
        float toppingCalc = 0;
        toppingCalc = Mathf.RoundToInt(_originalMoney * (addMoney/100));
        if (_toppingJumsu == 10)
        {
           
        }
        else if (_toppingJumsu == 6) 
        {
            toppingCalc /= 2;
        }
        else if (_toppingJumsu == 2)
        {
            toppingCalc /= 3;
        }
      

        return Mathf.RoundToInt(toppingCalc);
    }

    public void CreateNextButton()
    {
        // 과일이 4개 이상 있을때만 nextButton 활성화
        if (transform.childCount > 3 && canvasExit.GeteCurrentTable() == CurrentTable.FruitTable)
        {
            nextTableButton.SetActive(true);
            return;
        }
    }

    // 탕후루에 토핑이 있는지 없는지 확인
    public void CheckToppingChild()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform childchild in child)
            {
                if (childchild.CompareTag("Topping") && CheckCoatingIsDone() == true)
                {
                    if(cam.GetComponent<Camera>().orthographicSize == 20)
                        isChildTopping = true;
                }
            }
        }

        if (isChildTopping == true && canvasExit.GeteCurrentTable() == CurrentTable.ToppingTable)
        {
            nextTableButton.SetActive(true);
            return;
        }
    }

    private bool CheckCoatingIsDone()
    {
        foreach (Transform child in transform)
        {
            return child.GetComponent<PrefabScratchCardScript>().GetIsDone();
        }
        return false;
    }

    public int GetToppingCounts()
    {
        int toppingCounts = 0;
        foreach (FruitScript fruit in fruitsToppingCheckOnSkewer)
        {
            int toppings = fruit.GetToppingCounts();
            if (toppings >= 3)
            {
                toppingCounts += 3;
            }
            else if (toppings >= 2)
            {
                toppingCounts += 2;
            }
            else
            {
                toppingCounts += toppings;
            }
        }

        return toppingCounts;
    }
}
