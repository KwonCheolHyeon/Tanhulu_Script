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
        for (int i = 0; i < decoration.transform.childCount; i++) //decoration.transform.childCount �ش� ������Ʈ �� Content�� �ڽ��� ����
        {
            decoration.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = wallSprite[i].name;//Content�� �ڽ��� ù��° (0) �ڽ� == NameText������Ʈ
           
            decoration.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = wallSprite[i];//Content�� �ڽ��� �ι�° (1) �ڽ� == ItemImage ������Ʈ

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
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "��� ����";
                    decoration.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                }

            }

            for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UsingTrueWallList.Count; k++)
            {
                if (BackGroundManager.Instance.GetBGImageData().UsingTrueWallList[k].name == wallSprite[i].name)
                {
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "������";
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
                        Debug.Log("���ſϷ�");

                        SoundManager.Instance.PlaySFXSound("Buy");

                        GameObject.Find("CalcMoneyManger").GetComponent<SettlementScript>()
                            .StarUpdate(-int.Parse(BackGroundManager.Instance.GetBGImageData().UnlockFalseWallList[k].unlockPrice));

                        BackGroundManager.Instance.UpdateObjectUnlock(_name);

                        _textObj.transform.parent.parent.GetChild(3).gameObject.SetActive(false);
                        _textObj.GetComponent<TextMeshProUGUI>().text = "��� ����";
                    }
                    else 
                    {
                        // ��ȭ ����
                        this.GetComponent<ItemStoreControlManager>().BlurOption.LackOfMoneyPage.SetActive(true);
                        this.GetComponent<ItemStoreControlManager>().BlurOption.LackOfMoneyPage
                            .transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "���� �����ؿ�.";
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < decoration.transform.childCount; i++) // decoration.transform.childCount �ش� ������Ʈ �� Content�� �ڽ��� ����
        {

            BackGroundManager.Instance.UpdateObjectUsing(_name);

            for (int k = 0; k < BackGroundManager.Instance.GetBGImageData().UsingTrueWallList.Count; k++)
            {

                // UsingTrueWallList�� 1���� ���� �������� ������Ʈ�� ������
                if (BackGroundManager.Instance.GetBGImageData().UsingTrueWallList[k].name == wallSprite[i].name)
                {
                    decoration.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "������";

                    // ������ "������"�� ������Ʈ�� ���� "������" ������ ������Ʈ�� �ٸ��� ������ ������Ʈ�� "��� ����"���� ����
                    if(currObj != decoration.transform.GetChild(i).gameObject)
                    {
                        currObj.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "��� ����";
                        currObj = decoration.transform.GetChild(i).gameObject;
                    }
                    return;
                }
            }
        }
    }
}
