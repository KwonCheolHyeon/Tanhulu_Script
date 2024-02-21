using ScratchCardAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TrashCanScript : MonoBehaviour
{
    private bool isTrigger = false;

    private Collider2D otherColl;

    private void Update()
    {
        if(Input.GetMouseButtonUp(0) && isTrigger == true)
        {
            SkewerScript skewerScript = otherColl.GetComponent<SkewerScript>();
            skewerScript.GetNextTableButton().SetActive(false);
            SoundManager.Instance.PlaySFXSound("Rubbish");
            ClearFruitChild(otherColl.transform);
            DeactivateChildren(otherColl.transform);
            otherColl.transform.DetachChildren();
            UserDataControlManager.Instance.ThrowAwayTanhuluCounts(1);

            Exit exit = GameObject.Find("Canvas").GetComponent<Exit>();
            if (exit.GeteCurrentTable() == CurrentTable.ToppingTable)
                exit.CookingStart();

            MissionManager.Instance.ClearMission(3);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Skewer") )
        {
            isTrigger = true;
            otherColl = collision.GetComponent<Collider2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Skewer"))
        {
            isTrigger = false;
        }
    }

    private void ClearFruitChild(Transform _parent)
    {
        Transform[] myChildren = _parent.GetComponentsInChildren<Transform>();

        foreach (Transform child in myChildren)
        {
            if (child.gameObject.CompareTag("Topping"))
            {
                child.gameObject.SetActive(false);
                child.SetParent(null);
            }

            if (child.gameObject.CompareTag("Fruit"))
            {
                ScratchCardManager scratch = child.transform.GetChild(0).GetComponent<ScratchCardManager>();
                scratch.FillScratchCard();
            }
        }
    }

    private void DeactivateChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
        }
        parent.gameObject.SetActive(false);
    }
}
