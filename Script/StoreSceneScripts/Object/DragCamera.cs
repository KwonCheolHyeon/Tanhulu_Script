using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    // ��� : �̵� ����
    private const float directionForceReduceRate = 0.935f; // ���Ӻ���
    private const float directionForceMin = 0.001f; // ����ġ ������ ��� �������� ����

    // ���� : �̵� ����
    private bool userMoveInput; // ���� ������ �ϰ��ִ��� Ȯ���� ���� ����
    private Vector3 startPosition;  // �Է� ���� ��ġ�� ���
    private Vector3 directionForce; // ������ �������� ������ �����ϸ鼭 �̵� ��Ű�� ���� ����

    // ������Ʈ
    private new Camera camera;

    private bool isDebug = false;
    public bool IsDebugging() {  return isDebug; }

    private void Start()
    {
        camera = Camera.main;

        // ���� ���۽� ī�޶��� ù ��ġ�� �ֹ� �޴� ���� ��ġ�� ����
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
            // ī�޶� ������ �̵�
            ControlCameraPosition();

            // ������ �������� ����
            ReduceDirectionForce();

            // ī�޶� ��ġ ������Ʈ
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
        // ���� ���϶��� �ƹ��͵� ����
        if (userMoveInput)
        {
            return;
        }

        // ���� ��ġ ����
        directionForce *= directionForceReduceRate;

        // ���� ��ġ�� �Ǹ� ������ ����
        if (directionForce.magnitude < directionForceMin)
        {
            directionForce = Vector3.zero;
        }
    }
    private void UpdateCameraPosition()
    {
        // �̵� ��ġ�� ������ �ƹ��͵� ����
        if (directionForce == Vector3.zero)
        {
            return;
        }

        var currentPosition = transform.position;
        var targetPosition = currentPosition + directionForce;
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.5f);
    }
}
