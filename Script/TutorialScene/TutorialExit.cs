using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialExit : MonoBehaviour
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
    private GameObject guideText;
    [SerializeField]
    private GameObject settingTabPage;

    [SerializeField]
    private GameObject orderSheetButton;

    [SerializeField]
    private GameObject customerA;

    private GameObject SkewerObj;

    private TextMeshProUGUI proUGUI;

    public void SetSkewer(GameObject _skewer) { SkewerObj = _skewer; }

    private CurrentTable eCurrTable = CurrentTable.FruitTable;
    public CurrentTable GeteCurrentTable() { return eCurrTable; }
    public void SetCurrentTable(CurrentTable _table) { eCurrTable = _table; }

    private bool isGameStart = false;
    public bool GetIsGameStart() { return isGameStart; }
    public void SetIsGameStart(bool _isGameStart) { isGameStart = _isGameStart; }



    private void Start()
    {
        proUGUI = guideText.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
       
        // ���ķ� �ϼ� ����Ʈ�� ����߿��� ������ư ��Ȱ��ȭ
        if (eCurrTable == CurrentTable.CompleteTable)
        {
            settingTabPage.transform.parent.gameObject.SetActive(false);
        }
        else if (eCurrTable != CurrentTable.CompleteTable
            && settingTabPage.transform.parent.gameObject.activeSelf == false)
        {
            settingTabPage.transform.parent.gameObject.SetActive(true);
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
        // ī�޶� �̵� �� �ֹ��� Panel�� �ݱ�
        orderSheetPanel.SetActive(false);

        orderSheetButton.SetActive(true);

        Vector2 pos = fruitTable.transform.position;
        Camera.main.transform.position = new Vector3(pos.x - 1.7f, pos.y, Camera.main.transform.position.z);

        eCurrTable = CurrentTable.FruitTable;

        proUGUI.ClearMesh();
        proUGUI.text = "������ ��ġ�� �Ⱦ��ּ���";
        proUGUI.ForceMeshUpdate();

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
            if (child.CompareTag("Exit") && eCurrTable == CurrentTable.FruitTable)
            {
                eCurrTable = CurrentTable.ToppingTable;

                Vector3 skewerPos = new Vector3(169.0f, SkewerObj.transform.position.y - 1.0f, SkewerObj.transform.position.z);

                // ���̺��� �����ϸ鼭 ���ķ� ������Ʈ�� ��ġ�� ����

                SkewerObj.GetComponent<SkewerScript>().SetPrevPos(skewerPos);
                SkewerObj.transform.position = skewerPos;
                child.gameObject.SetActive(false);

                proUGUI.ClearMesh();
                proUGUI.text = "�ҽ��� �ٸ��� ������ �÷��ּ���";
                proUGUI.ForceMeshUpdate();
            }
        }
    }

    public void TransformIceBoxPos()
    {
        Vector2 pos = iceBoxTable.transform.position;
        Camera.main.transform.position = new Vector3(pos.x - 1.7f, pos.y, Camera.main.transform.position.z);

        orderSheetButton.SetActive(false);
        if (orderSheetPanel.activeSelf == true)
        {
            orderSheetPanel.SetActive(false);
            TimeManager.Instance.TimeStart();
        }


        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("Exit") && eCurrTable == CurrentTable.ToppingTable)
            {
                eCurrTable = CurrentTable.IceBoxTable;
                Vector3 skewerPos = new Vector3(iceBoxTable.transform.position.x - 15.0f
                    , SkewerObj.transform.position.y + 4.5f
                    , SkewerObj.transform.position.z);

                // ���̺��� �����ϸ鼭 ���ķ� ������Ʈ�� ��ġ�� ����

                SkewerObj.GetComponent<SkewerScript>().SetPrevPos(skewerPos);
                SkewerObj.transform.position = skewerPos;
                child.gameObject.SetActive(false);

                proUGUI.ClearMesh();
                proUGUI.text = "���ķ縦 ����� ���� �� �̴ϰ����� �����ϼ���";
                proUGUI.ForceMeshUpdate();
            }
        }
    }

    public void TitleSceneChange()
    {
        // ����� ���� ������ ����
        SoundManager.Instance.SoundDataSave();
        VibrationManager.Instance.VibrationDataSave();
        SceneManager.LoadScene("TitleScene");
    }
}
