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
        // 옵션 설정동안 타이틀씬에 있는 UI 터치되지 않도록 BlurPage생성
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
        // Start와 End 버튼을 눌렀을 때 True를 반환
        // True일시 씬이동과 게임종료 기능 실행
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
        //// 튜토리얼 없이 바로 시작하는 코드
        //SoundManager.Instance.SoundDataSave();
        //VibrationManager.Instance.VibrationDataSave();

        //SceneManager.LoadScene("StoreScene");

        //SoundManager.Instance.PlayBGMSound("Store");

        bool isCompleteTutorial = SoundManager.Instance.GetSoundDatas().isCompleteTutorial;
        if (isCompleteTutorial == true)
        {
            // 변경된 사운드 데이터 저장
            SoundManager.Instance.SoundDataSave();
            VibrationManager.Instance.VibrationDataSave();

            SceneManager.LoadScene("StoreScene");

            SoundManager.Instance.PlayBGMSound("Store");
        }
        else if (isCompleteTutorial == false)
        {
            // 변경된 사운드 데이터 저장
            SoundManager.Instance.SoundDataSave();
            VibrationManager.Instance.VibrationDataSave();

            SceneManager.LoadScene("TutorialStoreScene");

            SoundManager.Instance.PlayBGMSound("Store");
        }

    }
    void GameEnd()
    {
        // 게임 종료.
        // 전처리기를 이용해 에디터를 사용할 때와 응용프로그램, 모바일에서 실행할 때 구분하여 실행
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
