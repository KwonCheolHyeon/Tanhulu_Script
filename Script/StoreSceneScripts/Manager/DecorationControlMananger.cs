using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorationControlMananger : MonoBehaviour
{
    [SerializeField]
    private GameObject decoration;

    [SerializeField]
    private Sprite[] wallSprite;

    [SerializeField]
    private GameObject chectSure;

    private GameObject currObj;

    private void Start()
    {
        SettingDecoration();

    }

    public void SettingDecoration() 
    {
        for (int i = 0; i < decoration.transform.childCount; i++) //decoration.transform.childCount 해당 오브젝트 즉 Content의 자식의 갯수
        {
            decoration.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = wallSprite[i].name;//Content의 자식의 첫번째 (0) 자식 == NameText오브젝트
           
            decoration.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = wallSprite[i];//Content의 자식의 두번째 (1) 자식 == ItemImage 오브젝트

            Button button = decoration.transform.GetChild(i).GetChild(2).GetComponent<Button>();
            button.onClick.AddListener(
                () => SetBackgroundSprite(button.transform.parent.transform.GetChild(1).GetComponent<Image>().sprite.name
                , button.transform.GetChild(0).gameObject)
                );

            for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList.Count; k++)
            {
                if (BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].name == wallSprite[i].name)
                {
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text 
                        = BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].unlockPrice;
                    decoration.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
                }
            }

            for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UnlockTrueWallList.Count; k++)
            {
                if (BackGroundManager.Instance.GetBGImageData().UnlockTrueWallList[k].name == wallSprite[i].name)
                {
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "사용 가능";
                    decoration.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                }

            }

            for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UsingTrueWallList.Count; k++)
            {
                if (BackGroundManager.Instance.GetBGImageData().UsingTrueWallList[k].name == wallSprite[i].name)
                {
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "적용중";
                    currObj = decoration.transform.GetChild(i).gameObject;
                    decoration.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                }
            }
        }
    }

    public  void SetBackgroundSprite(string _name, GameObject _textObj)
    {
        for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList.Count; k++)
        {
            if (BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].name == _textObj.transform.parent.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                if (BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].isUnlock == false)
                {
                    if (int.Parse(BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].unlockPrice) 
                        <= UserDataControlManager.Instance.GetPlayerStar())
                    {
                        Debug.Log("구매완료");

                        SoundManager.Instance.PlaySFXSound("Buy");

                        GameObject.Find("CalcMoneyManger").GetComponent<SettlementScript>()
                            .StarUpdate(-int.Parse(BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].unlockPrice));

                        BackGroundManager.Instance.UpdateObjectUnlock(_name);

                        _textObj.transform.parent.parent.GetChild(3).gameObject.SetActive(false);
                        _textObj.GetComponent<TextMeshProUGUI>().text = "사용 가능";
                    }
                    else 
                    {
                        // 재화 부족
                        this.GetComponent<ItemStoreControlManager>().BlurOption.LackOfMoneyPage.SetActive(true);
                        this.GetComponent<ItemStoreControlManager>().BlurOption.LackOfMoneyPage
                            .transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "별이 부족해요.";
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < decoration.transform.childCount; i++) // decoration.transform.childCount 해당 오브젝트 즉 Content의 자식의 갯수
        {

            BackGroundManager.Instance.UpdateObjectUsing(_name);

            for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UsingTrueWallList.Count; k++)
            {

                // UsingTrueWallList는 1개로 현재 적용중인 오브젝트가 들어가있음
                if (BackGroundManager.Instance.GetBGImageData().UsingTrueWallList[k].name == wallSprite[i].name)
                {
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "적용중";

                    // 기존에 "적용중"인 오브젝트와 새로 "적용중" 설정된 오브젝트가 다르면 기존의 오브젝트를 "사용 가능"으로 변경
                    if(currObj != decoration.transform.GetChild(i).gameObject)
                    {
                        currObj.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "사용 가능";
                        currObj = decoration.transform.GetChild(i).gameObject;
                    }
                    return;
                }
            }
        }
    }
}
