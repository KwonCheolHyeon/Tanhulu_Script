using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StarPosScript : MonoBehaviour
{
    [SerializeField]
    private GameObject effectObj;
    [SerializeField]
    private GameObject startObj;
    [SerializeField]
    private GameObject endObj;
    [SerializeField]
    private GameObject stopZoneObj;

    Vector2 pos = Vector2.zero;
    Vector2 prevPos = Vector2.zero;

    private float moveSpeed = 20.0f;
    private float maxMoveSpeed = 50.0f;
    private float minX = 0;
    private float maxX = 0;

    private bool isEnd = false;
    private bool isStop = false;
    public bool GetStop() {  return isStop; }

    private bool isStopZone = false;

    private int failCount = 0;
    public int GetFailCount() {  return failCount; }

    private void Start()
    {
        pos = this.transform.position;
        prevPos = this.transform.position;

        if(UserDataControlManager.Instance.GetPlayerDate() >= 10)
        {
            minX = stopZoneObj.transform.position.x - 3.0f;
            maxX = stopZoneObj.transform.position.x + 3.0f;

            stopZoneObj.transform.localScale = new Vector2(1, 1);
        }
        else
        {
            minX = stopZoneObj.transform.position.x - 6.0f;
            maxX = stopZoneObj.transform.position.x + 6.0f;
        }
    }

    private void Update()
    {
        if (isStop == true)
            return;

        if(pos.x > minX && pos.x < maxX)
        {
            isStopZone = true;
        }
        else
        {
            isStopZone = false;
        }

        if (pos.x < startObj.transform.position.x)
        {
            isEnd = false;
        }
        if (pos.x > endObj.transform.position.x && isEnd == false)
        {
            isEnd = true;
        }


        if (isEnd == false)
        {
            BackAndForth(1);
        }
        else
        {
            BackAndForth(-1);
        }
    }

    void BackAndForth(float _move)
    {
        pos.x += (moveSpeed * _move) * Time.deltaTime;
        this.transform.position = pos;
    }

    public void CreateEffect()
    {
        if(isStopZone == true)
        {
            GameObject go = GameObject.Instantiate(effectObj);
            go.transform.SetParent(this.transform);
            go.transform.position = this.transform.position;

            // 진동 발생 함수 (밀리초 단위)
            VibrationManager.Instance.CreateOneShot(150);

            SoundManager.Instance.PlaySFXSound("StarPos");

            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;
            if (sceneName == "StoreScene") 
            {
                MissionManager.Instance.ClearMission(4);
            }

            moveSpeed = 50.0f;

            isStop = true;
        }
        else
        {
            failCount++;
            pos = prevPos;
            if (moveSpeed >= maxMoveSpeed)
                moveSpeed = maxMoveSpeed;
            else
                moveSpeed += 5.0f;
        }
    }
}
