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

    public List<BackGroundImage> AllWallList = new List<BackGroundImage>(); // 모든 Wall List

    // 잠금이 해제되어있지 않는 오브젝트 List
    public List<BackGroundImage> UnlockFalseWallList = new List<BackGroundImage>();

    // 잠금이 해제된 오브젝트의 List
    public List<BackGroundImage> UnlockTrueWallList = new List<BackGroundImage>(); 

    // 현재 사용중인 오브젝트의 List, 항상 1개의 오브젝트만 들어가있음
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
            Debug.Log("BackGround 데이터 로드 완료");
        }
        else
        {
            BGImageData = new BackGroundImageData();
            InitializeDefaultData();
            Save();

            BGImageData = ES3.Load<BackGroundImageData>(datakey, saveFileName);
            ClassifyWallList();
            Debug.Log("BackGround 데이터 로드 완료");
        }
    }

    public void InitializeDefaultData() 
    {
        BGImageData.wallImageData = new List<BackGroundImage>
        {
            new BackGroundImage("Wall", "기본 벽지", true, true, "0" ),
            new BackGroundImage("Wall", "복숭아색 벽지", false, false, "1"),
            new BackGroundImage("Wall", "분홍색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "살구색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "노란색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "연두색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "민트색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "하늘색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "청보라색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "보라색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "갈색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "회색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "허브색 벽지", false, false, "30"),
            new BackGroundImage("Wall", "고양이 벽지", false, false, "50"),
            new BackGroundImage("Wall", "곰돌이 벽지", false, false, "50"),
            new BackGroundImage("Wall", "도트 벽지", false, false, "50"),
            new BackGroundImage("Wall", "별 벽지", false, false, "50"),
            new BackGroundImage("Wall", "알록달록도트 벽지", false, false, "50"),
            new BackGroundImage("Wall", "하트 벽지", false, false, "50"),
        };
    }

    private void ClassifyWallList()
    {
        BGImageData.AllWallList = BGImageData.wallImageData.FindAll(x => x.type == new string("Wall"));
        BGImageData.UnlockTrueWallList = BGImageData.wallImageData.FindAll(x => x.isUnlock);
        BGImageData.UsingTrueWallList = BGImageData.wallImageData.FindAll(x => x.isUsing);

        BGImageData.UnlockFalseWallList = BGImageData.wallImageData.FindAll(x => x.isUnlock == false);
    }

    // 해금 관련 함수
    public void UpdateObjectUnlock(string _objectName)
    {
        // 과일 오브젝트에서 해당 이름을 가진 오브젝트 찾기 및 수정
        for (int i = 0; i < BGImageData.AllWallList.Count; i++)
        {
            if (BGImageData.AllWallList[i].name == _objectName)
            {
                // 잠금이 해제 되어있는 오브젝트라면 사용 여부를 설정 하는 함수로 이동
                if (BGImageData.AllWallList[i].isUnlock == true)
                {
                    //UpdateObjectUsing(_objectName);
                    return;
                }
                
                // 오브젝트의 잠금 해제
                BGImageData.AllWallList[i].isUnlock = true;
                ClassifyWallList();
                return;
            }
        }
    }

    // 사용 여부
    public void UpdateObjectUsing(string _objectName)
    {
      
        // 과일 오브젝트에서 해당 이름을 가진 오브젝트 찾기 및 수정
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
            Sprite[] sprites = Resources.LoadAll<Sprite>("Image/BackGround/Wall/벽지");
            
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
