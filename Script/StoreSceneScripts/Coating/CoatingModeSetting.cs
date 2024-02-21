using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public enum CoatingName
{
    NOTSELECTED,
    Sugar,
    Chocolate,
}

public class CoatingModeSetting : MonoBehaviour
{
    // 마우스 따라다니는 스푼 관련
    public GameObject SugarSpoon;
    public GameObject ChocoSpoon;

    [SerializeField]
    private GameObject toppingBoard;

    private GameObject mouseFollower;

    private bool isFollowing = false;
    public bool GetFollowing() { return isFollowing; }

    // 칠하기 미니게임 나가기 버튼
    public GameObject ExitButton;

    // 카메라 관련 정보값
    private Vector3 fixedCameraPosition = new Vector3(175.95f, -7.58f, -10.0f);
    private float fixedCameraSize = 12.6f;

    private Vector3 prevCameraPosition;
    private float prevCameraSize;

    public bool IsDone = false;
    public bool GetIsDone() { return IsDone; }
    private bool isCotingMode = false;
    public bool GetCotingMode() { return isCotingMode; }

    private CoatingName WhichCoating = CoatingName.NOTSELECTED;
    public CoatingName GetWhichCoating() { return WhichCoating; }

    void Update()
    {
        if (IsDone == true && ExitButton.activeSelf == true)
        {

            ExitButton.SetActive(false);
            
        }
        if (Input.GetMouseButtonDown(0))
        {
            // 터치한 부분이 UI Object 라면 True 반환
            if (EventSystem.current.IsPointerOverGameObject() == true)
                return;

            HandleMouseInput();
        }

        if (isFollowing && mouseFollower)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseFollower.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    public void ChangeCanUseSause(bool _canUseSause)
    {
        if(_canUseSause)
        {
            isFollowing = _canUseSause;
            prevCameraPosition = Camera.main.transform.position;
            prevCameraSize = Camera.main.orthographicSize;
            CameraSetUp(_canUseSause);

            ExitButton.SetActive(_canUseSause);
            

        }
        else
        {
            isFollowing = _canUseSause;
            CameraSetUp(_canUseSause);

            ExitButton.SetActive(_canUseSause);
            IsDone = true;

          


            if (mouseFollower)
            {
                Destroy(mouseFollower);
            }

            SkewerScript skewerScript = GameObject.Find("SkewerPrefab(Clone)").GetComponent<SkewerScript>();
            skewerScript.CheckToppingChild();
        }
    }

    private void CameraSetUp(bool _isFixed)
    {
        if (Camera.main)
        {
            if (_isFixed)
            {
                Camera.main.transform.position = fixedCameraPosition;
                Camera.main.orthographicSize = fixedCameraSize;

                isCotingMode = true;
            }
            else
            {
                Camera.main.transform.position = prevCameraPosition;
                Camera.main.orthographicSize = prevCameraSize;

                isCotingMode = false;
            }
        }
    }

    private void HandleMouseInput()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null)
            return;


        if (hit.collider.CompareTag("SauseContainer") && IsDone == false)
        {
            // 진동 발생 함수 (밀리초단위)
            VibrationManager.Instance.CreateOneShot(80);
            SoundManager.Instance.PlaySFXSound("ContainerSound");

            switch (hit.collider.gameObject.name)
            {
                case "Sause_Container1":

                    mouseFollower = Instantiate(SugarSpoon, mousePos, Quaternion.identity);
                    ChangeCanUseSause(true);
                    WhichCoating = CoatingName.Sugar;
                    break;

                case "Sause_Container2":

                    mouseFollower = Instantiate(ChocoSpoon, mousePos, Quaternion.identity);
                    ChangeCanUseSause(true);
                    WhichCoating = CoatingName.Chocolate;
                    break;
            }
        }
    }
}
