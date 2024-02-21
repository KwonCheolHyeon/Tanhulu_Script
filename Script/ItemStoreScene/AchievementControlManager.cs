using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class AchievementData
{
    public AchievementData(int _index, string _titleName, string _description, int _level, bool _isSuccess)
    {
        Index = _index;
        TitleName = _titleName;
        Description = _description;
        Level = _level;
        IsSuccess = _isSuccess;
    }
    public int Index;
    public string TitleName;
    public string Description;
    public int Level;
    public bool IsSuccess;

    public int GetIndex() { return Index; }
    public void SetIndex(int _indexl) { Index = _indexl; }
    public string GetTitleName() { return TitleName; }
    public void SetTitleName(string _titleName) { TitleName = _titleName; }
    public string GetDescription() { return Description; }
    public void SetDescription(string _description) { Description = _description; }
    public int GetLevel() { return Level; }
    public void SetLevel(int _level) { Level = _level; }
    public bool GetSuccess() { return IsSuccess; }
    public void SetSuccess(bool _isSuccess) { IsSuccess = _isSuccess; }
}

public class TotalAchievementData
{
    public List<AchievementData> allAchievementData = new List<AchievementData>();
    public int totalScore;
}

public class AchievementControlManager : MonoBehaviour
{
    private static AchievementControlManager instance = null;

    private TotalAchievementData totalAchievementData;
    private string datakey = "AchievementData";
    private string saveFileName = "SaveFileAch.txt";

    public int GetTotalScore() { return totalAchievementData.totalScore; }
    public void SetTotalScore(int _totalScore) { totalAchievementData.totalScore = _totalScore; }
    public void PlusTotalScore(int _totalScore) { totalAchievementData.totalScore += _totalScore; }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        DataLoad();

    }

    void Start()
    {
        DataLoad();
    }

    private void Update()
    {
    }

    public static AchievementControlManager Instance
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

    public void UpdateSuccessLevel(int _level, int _index)
    {
        AchievementData achievementToUpdate = totalAchievementData.allAchievementData.Find
            (a => a.GetLevel() == _level && a.GetIndex() == _index);

        if (achievementToUpdate != null && !achievementToUpdate.GetSuccess())
        {
            switch (_level)
            {
                case 0:
                    PlusTotalScore(1);
                    UserDataControlManager.Instance.UpdateStar(1);
                    achievementToUpdate.SetSuccess(true);
                    DataSave();
                    break;
                case 1:
                    PlusTotalScore(1);
                    UserDataControlManager.Instance.UpdateStar(1);
                    achievementToUpdate.SetSuccess(true);
                    DataSave();
                    break;
                case 2:
                    PlusTotalScore(1);
                    UserDataControlManager.Instance.UpdateStar(1);
                    achievementToUpdate.SetSuccess(true);
                    DataSave();
                    break;
                default:
                    break;
            }
        }
    }

    public int GetAchievementStatus(int _index)
    {
        /*
        ��ȯ���� ����
        0�̸� ��� ������ false,
        1�̸� ���� 0�� true,
        2�̸� ���� 0�� 1�� true,
        3�̸� ��� ������ true
         */

        var indexGroup = totalAchievementData.allAchievementData.Where(a => a.GetIndex() == _index);

        bool[] levelSuccessStates = new bool[3];

        for (int level = 0; level <= 2; level++)
        {
            levelSuccessStates[level] = indexGroup.Any(a => a.GetLevel() == level && a.IsSuccess);
        }

        // ��ȯ�� ���
        if (!levelSuccessStates[0] && !levelSuccessStates[1] && !levelSuccessStates[2])
        {
            return 0;
        }
        else if (levelSuccessStates[0] && !levelSuccessStates[1] && !levelSuccessStates[2])
        {
            return 1;
        }
        else if (levelSuccessStates[0] && levelSuccessStates[1] && !levelSuccessStates[2])
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public string GetAchievementTitleText(int _index)
    {
        string text = ".";
        int levelStatus = GetAchievementStatus(_index);
        if (levelStatus >=2)
        {
            levelStatus = 2;
        }

        AchievementData achievementToUpdate = totalAchievementData.allAchievementData.Find
            (a => a.GetIndex() == _index && a.GetLevel() == levelStatus);

        text = achievementToUpdate.GetTitleName();

        return text;
    }

    public string GetAchievementDescription(int _index)
    {
        string text = ".";
        int levelStatus = GetAchievementStatus(_index);
        if (levelStatus >= 2)
        {
            levelStatus = 2;
        }

        AchievementData achievementToUpdate = totalAchievementData.allAchievementData.Find
            (a => a.GetIndex() == _index && a.GetLevel() == levelStatus);

        text = achievementToUpdate.GetDescription();

        return text;
    }


    #region DBAndSaveLoad
    private void FirstAchievementSetting()
    {
        //int _index, string _titleName, string _description, int _level, bool _isSuccess
        totalAchievementData.allAchievementData.Add
    (new AchievementData(0, "���ķ� ������� ����", "�Ϸ絿�� ���ķ� 3�������", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(0, "���ķ� ������� ����", "�Ϸ絿�� ���ķ� 5�������", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(0, "���ķ� ������� ����", "�Ϸ絿�� ���ķ� 6�������", 2, false));

        totalAchievementData.allAchievementData.Add
    (new AchievementData(1, "���д� ������ ��Ӵ�", "�Ϸ絿�� ���ķ� 1��������", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(1, "���д� ������ ��Ӵ�", "�Ϸ絿�� ���ķ� 3��������", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(1, "���д� ������ ��Ӵ�", "�Ϸ絿�� ���ķ� 5��������", 2, false));

        totalAchievementData.allAchievementData.Add
    (new AchievementData(2, "������ ���ķ� ����", "�Ϸ絿�� 10,000��� ����", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(2, "������ ���ķ� ����", "�Ϸ絿�� 50,000��� ����", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(2, "������ ���ķ� ����", "�Ϸ絿�� 200,000��� ����", 2, false));

        totalAchievementData.allAchievementData.Add
    (new AchievementData(3, "��� ����", "���� ��� 100,000��� �̻�", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(3, "��� ����", "���� ��� 1,000,000��� �̻�", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(3, "��� ����", "���� ��� 5,000,000��� �̻�", 2, false));

        totalAchievementData.allAchievementData.Add
    (new AchievementData(4, "���� �ݷ���", "���� 4�� �������", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(4, "���� �ݷ���", "���� 8�� �������", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(4, "���� �ݷ���", "���� 10�� �������", 2, false));

        totalAchievementData.allAchievementData.Add
    (new AchievementData(5, "���� �ݷ���", "���� 3�� �������", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(5, "���� �ݷ���", "���� 6�� �������", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(5, "���� �ݷ���", "���� 9�� �������", 2, false));

        totalAchievementData.allAchievementData.Add
    (new AchievementData(6, "������ ����", "�÷��� �ϼ� 10���� �̻�", 0, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(6, "������ ����", "�÷��� �ϼ� 30���� �̻�", 1, false));
        totalAchievementData.allAchievementData.Add
    (new AchievementData(6, "������ ����", "�÷��� �ϼ� 50���� �̻�", 2, false));

    }

    private void DataSave()
    {
        ES3.Save(datakey, totalAchievementData, saveFileName);
    }

    private void DataLoad()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {
            totalAchievementData = ES3.Load<TotalAchievementData>(datakey, saveFileName);
            //SortMaterialData();
            Debug.Log("������ �ε� �Ϸ�");
        }
        else
        {
            totalAchievementData = new TotalAchievementData();
            FirstAchievementSetting();
            SetTotalScore(0);
            DataSave();
            totalAchievementData = ES3.Load<TotalAchievementData>(datakey, saveFileName);
            //SortMaterialData();
            Debug.Log("������ ���� �Ϸ�");
        }
    }
    #endregion

    //���� ���� �����Լ�
    #region AchievementCheck
    public void CheckMakeTanghuluCount(int _count)
    {
        if (_count >= 3)
        {
            UpdateSuccessLevel(0, 0);
        }
        if (_count >= 5)
        {
            UpdateSuccessLevel(1, 0);
        }
        if (_count >= 6)
        {
            UpdateSuccessLevel(2, 0);
        }
    }

    public void CheckFailTanghuluCount(int _count)
    {
        if (_count >= 1)
        {
            UpdateSuccessLevel(0, 1);
        }
        if (_count >= 3)
        {
            UpdateSuccessLevel(1, 1);
        }
        if (_count >= 5)
        {
            UpdateSuccessLevel(2, 1);
        }

    }

    public void CheckEarnedMoneyOnToday(int _money)
    {
        if (_money >= 10000)
        {
            UpdateSuccessLevel(0, 2);
        }
        if (_money >= 50000)
        {
            UpdateSuccessLevel(1, 2);
        }
        if (_money >= 200000)
        {
            UpdateSuccessLevel(2, 2);
        }
    }

    public void CheckPlayerMoney(int _money)
    {
        if (_money >= 100000)
        {
            UpdateSuccessLevel(0, 3);
        }
        if (_money >= 1000000)
        {
            UpdateSuccessLevel(1, 3);
        }
        if (_money >= 5000000)
        {
            UpdateSuccessLevel(2, 3);
        }
    }

    public void CheckUnlockFruitCount(int _unlockFruit)
    {
        if (_unlockFruit >= 4)
        {
            UpdateSuccessLevel(0, 4);
        }
        if (_unlockFruit >= 8)
        {
            UpdateSuccessLevel(1, 4);
        }
        if (_unlockFruit >= 10)
        {
            UpdateSuccessLevel(2, 4);
        }
    }

    public void CheckUnlockToppingCount(int _unlockTopping)
    {
        if (_unlockTopping >= 3)
        {
            UpdateSuccessLevel(0, 5);
        }
        if (_unlockTopping >= 6)
        {
            UpdateSuccessLevel(1, 5);
        }
        if (_unlockTopping >= 9)
        {
            UpdateSuccessLevel(2, 5);
        }
    }

    public void CheckDayCount(int _day)
    {
        if (_day >= 10)
        {
            UpdateSuccessLevel(0, 6);
        }
        if (_day >= 30)
        {
            UpdateSuccessLevel(1, 6);
        }
        if (_day >= 50)
        {
            UpdateSuccessLevel(2, 6);
        }

    }
    #endregion

    //Text �� �����̹��� ����
    #region TextAndImage

    #endregion
}