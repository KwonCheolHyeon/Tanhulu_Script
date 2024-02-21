using ScratchCardAsset.Core.ScratchData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;


public class BackGroundImage
{
    public BackGroundImage(string _type, string _name, bool _isUsing, bool _isUnlock, string _unlockPrice)
    {
        type = _type;
        name = _name;
        isUsing = _isUsing;
        isUnlock = _isUnlock;
        unlockPrice = _unlockPrice;
    }

    public string type, name;
    public bool isUsing , isUnlock;
    public string unlockPrice;
}

public class BackGroundImageData
{
    public List<BackGroundImage> wallImageData = new List<BackGroundImage>();

    public List<BackGroundImage> AllWallList = new List<BackGroundImage>(); // ��� Wall List

    // ����� �����Ǿ����� �ʴ� ������Ʈ List
    public List<BackGroundImage> UnlockFalseWallList = new List<BackGroundImage>();

    // ����� ������ ������Ʈ�� List
    public List<BackGroundImage> UnlockTrueWallList = new List<BackGroundImage>(); 

    // ���� ������� ������Ʈ�� List, �׻� 1���� ������Ʈ�� ������
    public List<BackGroundImage> UsingTrueWallList = new List<BackGroundImage>(); 


    public string SelectedSprite = "";
}

public class BackGroundManager : MonoBehaviour
{
    private static BackGroundManager instance;
    BackGroundImageData BGImageData;
    public BackGroundImageData GetBGImageData() { return BGImageData; }


    // easy save
    private string datakey = "BGImageData";
    private string saveFileName = "SaveBackGroundImageFile.es3";

    [SerializeField]
    private GameObject backGroundObject;

    void Awake()
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

        Load();
    }
    public static BackGroundManager Instance
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

    private void Start()
    {

    }

    public void Save()
    {

        ES3.Save(datakey, BGImageData, saveFileName);

    }

    public void Load()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists(datakey, saveFileName))
        {
            BGImageData = ES3.Load<BackGroundImageData>(datakey, saveFileName);
            ClassifyWallList();
            Debug.Log("BackGround ������ �ε� �Ϸ�");
        }
        else
        {
            BGImageData = new BackGroundImageData();
            InitializeDefaultData();
            Save();

            BGImageData = ES3.Load<BackGroundImageData>(datakey, saveFileName);
            ClassifyWallList();
            Debug.Log("BackGround ������ �ε� �Ϸ�");
        }
    }

    public void InitializeDefaultData() 
    {
        BGImageData.wallImageData = new List<BackGroundImage>
        {
            new BackGroundImage("Wall", "�⺻ ����", true, true, "0" ),
            new BackGroundImage("Wall", "�����ƻ� ����", false, false, "1"),
            new BackGroundImage("Wall", "��ȫ�� ����", false, false, "30"),
            new BackGroundImage("Wall", "�챸�� ����", false, false, "30"),
            new BackGroundImage("Wall", "����� ����", false, false, "30"),
            new BackGroundImage("Wall", "���λ� ����", false, false, "30"),
            new BackGroundImage("Wall", "��Ʈ�� ����", false, false, "30"),
            new BackGroundImage("Wall", "�ϴû� ����", false, false, "30"),
            new BackGroundImage("Wall", "û����� ����", false, false, "30"),
            new BackGroundImage("Wall", "����� ����", false, false, "30"),
            new BackGroundImage("Wall", "���� ����", false, false, "30"),
            new BackGroundImage("Wall", "ȸ�� ����", false, false, "30"),
            new BackGroundImage("Wall", "���� ����", false, false, "30"),
            new BackGroundImage("Wall", "����� ����", false, false, "50"),
            new BackGroundImage("Wall", "������ ����", false, false, "50"),
            new BackGroundImage("Wall", "��Ʈ ����", false, false, "50"),
            new BackGroundImage("Wall", "�� ����", false, false, "50"),
            new BackGroundImage("Wall", "�˷ϴ޷ϵ�Ʈ ����", false, false, "50"),
            new BackGroundImage("Wall", "��Ʈ ����", false, false, "50"),
        };
    }

    private void ClassifyWallList()
    {
        BGImageData.AllWallList = BGImageData.wallImageData.FindAll(x => x.type == new string("Wall"));
        BGImageData.UnlockTrueWallList = BGImageData.wallImageData.FindAll(x => x.isUnlock);
        BGImageData.UsingTrueWallList = BGImageData.wallImageData.FindAll(x => x.isUsing);

        BGImageData.UnlockFalseWallList = BGImageData.wallImageData.FindAll(x => x.isUnlock == false);
    }

    // �ر� ���� �Լ�
    public void UpdateObjectUnlock(string _objectName)
    {
        // ���� ������Ʈ���� �ش� �̸��� ���� ������Ʈ ã�� �� ����
        for (int i = 0; i < BGImageData.AllWallList.Count; i++)
        {
            if (BGImageData.AllWallList[i].name == _objectName)
            {
                // ����� ���� �Ǿ��ִ� ������Ʈ��� ��� ���θ� ���� �ϴ� �Լ��� �̵�
                if (BGImageData.AllWallList[i].isUnlock == true)
                {
                    //UpdateObjectUsing(_objectName);
                    return;
                }
                
                // ������Ʈ�� ��� ����
                BGImageData.AllWallList[i].isUnlock = true;
                ClassifyWallList();
                return;
            }
        }
    }

    // ��� ����
    public void UpdateObjectUsing(string _objectName)
    {
      
        // ���� ������Ʈ���� �ش� �̸��� ���� ������Ʈ ã�� �� ����
        for (int i = 0; i < BGImageData.UnlockTrueWallList.Count; i++)
        {
            if (BGImageData.UnlockTrueWallList[i].name == _objectName)
            {
                BGImageData.UnlockTrueWallList[i].isUsing = true;

                if (BGImageData.UsingTrueWallList == null)
                    BGImageData.UsingTrueWallList = new List<BackGroundImage>();

                BGImageData.UsingTrueWallList[0] = BGImageData.UnlockTrueWallList[i];
            }
            else
            {
                BGImageData.UnlockTrueWallList[i].isUsing = false;
            }
        }
    }

    public void SetBackGroundImage(GameObject _obj)
    {
        _obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/BackGround/Wall/" + BGImageData.UsingTrueWallList[0].name);

        if(_obj.GetComponent<SpriteRenderer>().sprite == null)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Image/BackGround/Wall/����");
            
            for(int i =0; i < sprites.Length; i++)
            {
                if(sprites[i].name == BGImageData.UsingTrueWallList[0].name)
                {
                    _obj.GetComponent<SpriteRenderer>().sprite = sprites[i];
                }
            }
        }
    }
}
