using AssetKits.ParticleImage;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mission
{
    public Mission(string _name, int _index, bool _clear)
    {
        this.name = _name;
        this.index = _index;
        this.clear = _clear;
    }

    public string name;
    public int index;
    public bool clear;
}

public class MissionManager : MonoBehaviour
{
    // 모든 미션
    public List<Mission> allMissions = new List<Mission>();

    // 일일 미션
    public List<Mission> dailyMissions = new List<Mission>();


    [SerializeField]
    private GameObject missionSheetPanel;
    [SerializeField]
    private GameObject coinEffectObject;

    int objectsIndex = -1;

    private static MissionManager instance;
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    public static MissionManager Instance
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

    void Start()
    {
        Init();

        GenerateDailyMissions();
        DisplayDailyMissions();
    }

    void Init()
    {
        ToppingIndexShuffle();

        allMissions.Add(new Mission("30 초 안에 탕후루 만들기", 0, false));
        allMissions.Add(new Mission("탕후루 4개 이상 만들기", 1, false));
        allMissions.Add(new Mission("5성 이상 탕후루 만들기", 2, false));
        allMissions.Add(new Mission("탕후루를 쓰레기통에 버리기", 3, false));
        allMissions.Add(new Mission("냉장고 미니게임 성공하기", 4, false));
        allMissions.Add(new Mission("토핑 " + objectsIndex +"개 올리기", 5, false));
        allMissions.Add(new Mission("냉장고 미니게임 실패하기", 6, false));
    }

    // 일일 미션 생성
    private void GenerateDailyMissions()
    {
        ShuffleList(allMissions);

        dailyMissions = allMissions.GetRange(0, Mathf.Min(3, allMissions.Count));
    }

    // 랜덤
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void ToppingIndexShuffle()
    {
        objectsIndex = Random.Range(4, 13);
    }

    // 일일 미션 출력
    private void DisplayDailyMissions()
    {
        for (int i = 0; i < dailyMissions.Count; i++)
        {
            string mission = dailyMissions[i].name;
            missionSheetPanel.transform.GetChild(i + 1).GetComponent<TextMeshProUGUI>().text = mission;
        }
    }

    // _indext 값에 맞는 미션 클리어 함수 실행
    public void ClearMission(int _index, int _numberObjects = 0)
    {
        int num = 0;
        for (int i = 0; i < dailyMissions.Count; i++)
        {
            num++;
            // 이미 성공한 미션은 건너뛴다.
            if (dailyMissions[i].clear == true)
                continue;

            if(dailyMissions[i].index == _index)
            {
                // topping mission 일 경우
                if(_index == 5 && objectsIndex == _numberObjects)
                {
                    TextMeshProUGUI textMesh = missionSheetPanel.transform.GetChild(num).GetComponent<TextMeshProUGUI>();
                    textMesh.color = Color.red;
                    textMesh.fontStyle = FontStyles.Strikethrough;

                    dailyMissions[i].clear = true;
                }
                else if(_index != 5)
                {
                    TextMeshProUGUI textMesh = missionSheetPanel.transform.GetChild(num).GetComponent<TextMeshProUGUI>();
                    textMesh.color = Color.red;
                    textMesh.fontStyle = FontStyles.Strikethrough;

                    dailyMissions[i].clear = true;
                }
            }
        }
    }

    public void MissionButtonClick()
    {
        for (int i = 0; i < dailyMissions.Count; i++)
        {
            bool clear = dailyMissions[i].clear;
            int index = dailyMissions[i].index;
            if(clear == true && index != -1)
            {
                StartCoroutine(CreateCoinEffect(missionSheetPanel.transform.GetChild(i + 1).GetComponent<RectTransform>()));
                dailyMissions[i].index = -1;
            }
        }
    }

    private IEnumerator CreateCoinEffect(RectTransform _textPos)
    {
        yield return new WaitForSeconds(1f);

        Transform starTextTr = GameObject.Find("Canvas").transform.Find("Stars_Text");
        GameObject effectObj = GameObject.Instantiate(coinEffectObject);
        effectObj.transform.SetParent(starTextTr.parent.transform);
        effectObj.transform.localPosition = new Vector2(0, _textPos.localPosition.y);
        effectObj.transform.localScale = new Vector3(1, 1, 1);
        effectObj.transform.GetChild(0).GetComponent<ParticleImage>().attractorTarget = starTextTr;
    }
}
