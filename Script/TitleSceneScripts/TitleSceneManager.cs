using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject tabPage;
    public GameObject BlurPage;

    private bool IsStart = false;
    private bool IsEnd = false;

    public void SetStart(bool _isStart) { IsStart = _isStart; }
    public void SetEnd(bool _isEnd) { IsEnd = _isEnd; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        // �ɼ� �������� Ÿ��Ʋ���� �ִ� UI ��ġ���� �ʵ��� BlurPage����
        if(tabPage.activeSelf)
        {
            BlurPage.SetActive(true);
        }
        else
        {
            BlurPage.SetActive(false);
        }

        StartOrEnd();
    }

    void StartOrEnd()
    {
        // Start�� End ��ư�� ������ �� True�� ��ȯ
        // True�Ͻ� ���̵��� �������� ��� ����
        if (IsStart)
        {
            GameStart();
        }

        if (IsEnd)
        {
            GameEnd();
        }
    }

    void GameStart()
    {
        //// Ʃ�丮�� ���� �ٷ� �����ϴ� �ڵ�
        //SoundManager.Instance.SoundDataSave();
        //VibrationManager.Instance.VibrationDataSave();

        //SceneManager.LoadScene("StoreScene");

        //SoundManager.Instance.PlayBGMSound("Store");

        bool isCompleteTutorial = SoundManager.Instance.GetSoundDatas().isCompleteTutorial;
        if (isCompleteTutorial == true)
        {
            // ����� ���� ������ ����
            SoundManager.Instance.SoundDataSave();
            VibrationManager.Instance.VibrationDataSave();

            SceneManager.LoadScene("StoreScene");

            SoundManager.Instance.PlayBGMSound("Store");
        }
        else if (isCompleteTutorial == false)
        {
            // ����� ���� ������ ����
            SoundManager.Instance.SoundDataSave();
            VibrationManager.Instance.VibrationDataSave();

            SceneManager.LoadScene("TutorialStoreScene");

            SoundManager.Instance.PlayBGMSound("Store");
        }

    }
    void GameEnd()
    {
        // ���� ����.
        // ��ó���⸦ �̿��� �����͸� ����� ���� �������α׷�, ����Ͽ��� ������ �� �����Ͽ� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void TutorialPage() 
    {
        SceneManager.LoadScene("TutorialStoreScene");

    }
}
