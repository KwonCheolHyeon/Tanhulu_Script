using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    // 상수 : 이동 관련
    private const float directionForceReduceRate = 0.935f; // 감속비율
    private const float directionForceMin = 0.001f; // 설정치 이하일 경우 움직임을 멈춤

    // 변수 : 이동 관련
    private bool userMoveInput; // 현재 조작을 하고있는지 확인을 위한 변수
    private Vector3 startPosition;  // 입력 시작 위치를 기억
    private Vector3 directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기 위한 변수

    // 컴포넌트
    private new Camera camera;

    private bool isDebug = false;
    public bool IsDebugging() {  return isDebug; }

    private void Start()
    {
        camera = Camera.main;

        // 게임 시작시 카메라의 첫 위치를 주문 받는 씬의 위치로 지정
        Vector3 startPos = new Vector3 (-20, 0, -10);
        this.transform.position = startPos;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && isDebug == false) 
        {
            isDebug = true;
        }
        else if(Input.GetKeyDown(KeyCode.P) && isDebug == true)
        {
            isDebug = false;
        }

        if(isDebug == true)
        {        
            // 카메라 포지션 이동
            ControlCameraPosition();

            // 조작을 멈췄을때 감속
            ReduceDirectionForce();

            // 카메라 위치 업데이트
            UpdateCameraPosition();
        }

    }

    void ControlCameraPosition()
    {
        var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            CameraPositionMoveStart(mouseWorldPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            CameraPositionMoveProgress(mouseWorldPosition);
        }
        else
        {
            CameraPositionMoveEnd();
        }
    }
    private void CameraPositionMoveStart(Vector3 _startPosition)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider == null || hit.collider.CompareTag("Untagged"))
        {
            userMoveInput = true;
            this.startPosition = _startPosition;
            directionForce = Vector2.zero;
        }
    }
    private void CameraPositionMoveProgress(Vector3 _targetPosition)
    {
        if (!userMoveInput)
        {
            return;
        }

        directionForce = new Vector3(startPosition.x - _targetPosition.x, 0f, 0f);
    }
    private void CameraPositionMoveEnd()
    {
        userMoveInput = false;
    }
    private void ReduceDirectionForce()
    {
        // 조작 중일때는 아무것도 안함
        if (userMoveInput)
        {
            return;
        }

        // 감속 수치 적용
        directionForce *= directionForceReduceRate;

        // 작은 수치가 되면 강제로 멈춤
        if (directionForce.magnitude < directionForceMin)
        {
            directionForce = Vector3.zero;
        }
    }
    private void UpdateCameraPosition()
    {
        // 이동 수치가 없으면 아무것도 안함
        if (directionForce == Vector3.zero)
        {
            return;
        }

        var currentPosition = transform.position;
        var targetPosition = currentPosition + directionForce;
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.5f);
    }
}
