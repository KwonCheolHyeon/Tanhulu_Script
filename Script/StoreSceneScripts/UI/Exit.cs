using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// 현재 사용중인 테이블
public enum CurrentTable
{
    StoreTable,
    FruitTable,
    ToppingTable,
    IceBoxTable,
    CompleteTable
}

public class Exit : MonoBehaviour
{

    [SerializeField]
    private GameObject orderSheetPanel;
    [SerializeField]
    private GameObject fruitTable;
    [SerializeField]
    private GameObject toppingTable;
    [SerializeField]
    private GameObject iceBoxTable;
    [SerializeField]
    private GameObject resultPanel;
    [SerializeField]
    private GameObject settingTabPage;
    [SerializeField]
    private GameObject blurPage;
    [SerializeField]
    private GameObject orderSheetButton;
    [SerializeField]
    private GameObject missionSheetButton;
    [SerializeField]
    private GameObject missionSheetPanel;
    [SerializeField]
    private GameObject lackOfMondy;

    [SerializeField]
    private GameObject customerA;

    private GameObject SkewerObj;

    public void SetSkewer(GameObject _skewer) { SkewerObj = _skewer; }

    private CurrentTable eCurrTable = CurrentTable.FruitTable;
    public CurrentTable GeteCurrentTable() { return eCurrTable; }
    public void SetCurrentTable(CurrentTable _table) { eCurrTable = _table; }

    private bool isGameStart = false;
    public bool GetIsGameStart() { return isGameStart; }
    public void SetIsGameStart(bool _isGameStart) { isGameStart = _isGameStart; }

    Scene currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        // 옵션 설정동안 타이틀씬에 있는 UI 터치되지 않도록 BlurPage생성
        if (settingTabPage.activeSelf || orderSheetPanel.activeSelf || missionSheetPanel.activeSelf || lackOfMondy.activeSelf)
        {
            blurPage.SetActive(true);
        }
        else
        {
            blurPage.SetActive(false);
        }

        // 탕후루 완성 이펙트가 출력중에는 설정버튼 비활성화
        if (eCurrTable == CurrentTable.CompleteTable)
        {
            settingTabPage.transform.parent.gameObject.SetActive(false);

            
            string sceneName = currentScene.name;
            if (sceneName == "StoreScene")
            {
                missionSheetButton.SetActive(false);

                if (missionSheetPanel.activeSelf == true)
                    missionSheetPanel.SetActive(false);
            }
          
        }
        else if(eCurrTable != CurrentTable.CompleteTable 
            && settingTabPage.transform.parent.gameObject.activeSelf == false)
        {
            settingTabPage.transform.parent.gameObject.SetActive(true);
           
            string sceneName = currentScene.name;
            if (sceneName == "StoreScene")
            {
                missionSheetButton.SetActive(true);
            }
        }
    }

    public void ExitButton()
    {
        orderSheetPanel.SetActive(false);
    }

    public void ResultPanelExit()
    {
        resultPanel.SetActive(false);
        customerA.GetComponent<CustomerScript>().SetIsOrderCompleted(true);
    }

    public void CookingStart()
    {
        // 카메라 이동 전 주문서 Panel은 닫기
        orderSheetPanel.SetActive(false);

        orderSheetButton.SetActive(true);

        Vector2 pos = fruitTable.transform.position;
        Camera.main.transform.position = new Vector3(pos.x - 1.7f, pos.y, Camera.main.transform.position.z);

        eCurrTable = CurrentTable.FruitTable;

        isGameStart = true;
    }

    public void TableChange()
    {
        switch (eCurrTable)
        {
            case CurrentTable.FruitTable:
                TransformToppingPos();
                break;
            case CurrentTable.ToppingTable:
                TransformIceBoxPos();
                break;
            case CurrentTable.IceBoxTable:
                break;
            default:
                break;
        }
    }

    public void TransformToppingPos()
    {
        SkewerObj.GetComponent<SkewerScript>().AddFruitsToSkewer();
        Vector2 pos = toppingTable.transform.position;
        Camera.main.transform.position = new Vector3(pos.x - 1.7f, pos.y, Camera.main.transform.position.z);

        toppingTable.GetComponent<CoatingModeSetting>().IsDone = false;

        foreach (Transform child in this.transform)
        {
           if(child.CompareTag("Exit") && eCurrTable == CurrentTable.FruitTable)
            {
                eCurrTable = CurrentTable.ToppingTable;

                Vector3 skewerPos = new Vector3(169.0f, SkewerObj.transform.position.y - 1.0f, SkewerObj.transform.position.z);

                // 테이블을 변경하면서 탕후루 오브젝트의 위치도 변경
                SkewerObj.GetComponent<SkewerScript>().SetPrevPos(skewerPos);
                SkewerObj.transform.position = skewerPos;
                child.gameObject.SetActive(false);
            }
        }
    }

    public void TransformIceBoxPos()
    {
        Vector2 pos = iceBoxTable.transform.position;
        Camera.main.transform.position = new Vector3(pos.x - 1.7f, pos.y, Camera.main.transform.position.z);

        orderSheetButton.SetActive(false);
        if(orderSheetPanel.activeSelf == true)
        {
            orderSheetPanel.SetActive(false);
            TimeManager.Instance.TimeStart();
        }


        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("Exit") && eCurrTable == CurrentTable.ToppingTable)
            {
                eCurrTable = CurrentTable.IceBoxTable;
                
                if(lackOfMondy.activeSelf == true)
                {
                    lackOfMondy.SetActive(false);
                }
                
                Vector3 skewerPos = new Vector3(iceBoxTable.transform.position.x - 15.0f
                    , SkewerObj.transform.position.y + 4.5f
                    , SkewerObj.transform.position.z);

                // 테이블을 변경하면서 탕후루 오브젝트의 위치도 변경
                SkewerObj.GetComponent<SkewerScript>().SetPrevPos(skewerPos);
                SkewerObj.transform.position = skewerPos;
                child.gameObject.SetActive(false);

                Scene currentScene = SceneManager.GetActiveScene();
                string sceneName = currentScene.name;
                if (sceneName == "StoreScene")
                {
                    MissionManager.Instance.ClearMission(5, SkewerObj.GetComponent<SkewerScript>().GetToppingCounts());
                }  
            }
        }
    }

    public void TitleSceneChange()
    {
        // 변경된 사운드 데이터 저장
        SoundManager.Instance.SoundDataSave();
        VibrationManager.Instance.VibrationDataSave();
        SceneManager.LoadScene("TitleScene");
    }
}
