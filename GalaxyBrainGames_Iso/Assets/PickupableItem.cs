using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private string itemID = "Item_0";

    private bool pickedUp = false;

    void PickUpItem()
    {
        if(!pickedUp && creatureData != null)
        {
            pickedUp = true;
            creatureData.AddItemToInventory(itemID);
            gameObject.SetActive(false);
        }
    }
}
