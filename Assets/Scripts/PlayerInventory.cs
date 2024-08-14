using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerInventory : NetworkBehaviour
{

    [Header("Gerneral")]
    public List<ItemPickable> inventoryList;
    public int selectedItem;
    public float playerReach;
    public float throwForce;
    [SerializeField] Transform objectInventoryPosition;
    public Transform ObjectInventoryPosition => objectInventoryPosition;
    [SerializeField] int inventoryCapacity = 3;


    [Header("UI")]
    [SerializeField] Image[] inventorySlotImage = new Image[3];
    [SerializeField] Image[] inventoryBackgroundImage = new Image[3];
    [SerializeField] Sprite emptySlotImage;
    //[SerializeField] GameObject PickUpTextHover;

    
    void Start()
    {
       
        emptySlotImage = null;

        newItemSelected();

        if (IsOwner)
        {
            InputManager.Instance.onDropPressed += TryToDropCurrentObject;
            InputManager.Instance.onOnePressed += (_,_) => ChangeSelectedInventoryObject(0);
            InputManager.Instance.onTwoPressed += (_,_) => ChangeSelectedInventoryObject(1);
            InputManager.Instance.onThreePressed += (_,_) => ChangeSelectedInventoryObject(2);

        }
    }

    public void PickupObject(ItemPickable item)
    {
        if (inventoryList.Count >= inventoryCapacity) return;
        inventoryList.Add(item);
        selectedItem = inventoryList.Count - 1;
        item.IsBeingHold = true;
        NetworkCommunicationManager.Instance.ChangeOwnerShipServerRpc(item.gameObject, NetworkManager.Singleton.LocalClientId);
        DeactivateColliders(item.gameObject);
        NetworkCommunicationManager.Instance.DeactivateCollisionsServerRpc(item.gameObject);
        Rigidbody rb = item.gameObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        item.transform.position = Vector3.zero;
        item.transform.rotation = Quaternion.identity;
        newItemSelected();
        item.currentInventory = this;
    }

    public void DeactivateColliders(GameObject obj)
    {
        
        List<Transform> transformList = new List<Transform>();
        if (obj != null)
        {
            transformList.Add(obj.transform);
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null) collider.isTrigger = true;
        }
        while (transformList.Any())
        {
            foreach (Transform child in transformList[0])
            {
                Collider collider = child.gameObject.GetComponent<Collider>();
                if (collider != null) collider.isTrigger = true;
                transformList.Add(child);
            }
            transformList.RemoveAt(0);
        }
    }

    public void ActivateColliders(GameObject obj)
    {
        
        List<Transform> transformList = new List<Transform>();
        if (obj != null)
        {
            transformList.Add(obj.transform);
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null) collider.isTrigger = false;
        }
        while (transformList.Any())
        {
            foreach (Transform child in transformList[0])
            {
                Collider collider = child.gameObject.GetComponent<Collider>();
                if (collider != null) collider.isTrigger = false;
                transformList.Add(child);
            }
            transformList.RemoveAt(0);
        }
    }

    public void TryToDropCurrentObject(GameObject player, Camera camera)
    {
        if (!inventoryList.Any()) return;
        Rigidbody rb = inventoryList[selectedItem].GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        ActivateColliders(inventoryList[selectedItem].gameObject);
        NetworkCommunicationManager.Instance.ActivateCollisionsServerRpc(inventoryList[selectedItem].gameObject);
        inventoryList[selectedItem].IsBeingHold = false;
        inventoryList.RemoveAt(selectedItem);
        if (selectedItem != 0)
        {
            selectedItem -= 1;
        }
        
        newItemSelected();
    }

    public void ChangeSelectedInventoryObject(int index)
    {
        if (inventoryList.Count <= index) return;
        selectedItem = index;
        newItemSelected();
    }

    void Update(){  //THIS NEEDS TO BE OPTIMIZED

        // Change selected item with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            if (scroll > 0f)
            {
                selectedItem++;
                if (selectedItem >= inventoryList.Count)
                {
                    selectedItem = 0;
                }
            }
            else if (scroll < 0f)
            {
                selectedItem--;
                if (selectedItem < 0)
                {
                    selectedItem = inventoryList.Count - 1;
                }
            }
            newItemSelected();
        }

        //UI
        /*
        for (int i = 0; i < 2; i++)
        {
            if (i < inventoryList.Count)
            {
                inventorySlotImage[i].sprite = itemSetActive[inventoryList[i]].GetComponent<Item>().itemScriptableObject.item_sprite;
            }
            else
            {
                //inventorySlotImage[i].sprite = emptySlotImage;
            }
        }

        int a = 0;

        foreach(Image image in inventoryBackgroundImage)
        {
            if (a == selectedItem && inventoryList.Count > 0)
            {
                image.color = new Color32(145, 255, 126, 255);
            }
            else
            {
                image.color = new Color32(219, 219, 219, 255);
            }
            a++;
        }
        */

        
    }

    private void newItemSelected(){
        inventoryList.ForEach(item => {
            NetworkCommunicationManager.Instance.DeactivateItemServerRpc(GameManager.Instance.ActualPlayer, item.gameObject);
            item.gameObject.SetActive(false);
        });
        if(inventoryList.Count > 0){
            NetworkCommunicationManager.Instance.ActivateItemServerRpc(GameManager.Instance.ActualPlayer, inventoryList[selectedItem].gameObject);
            inventoryList[selectedItem].gameObject.SetActive(true);
            //GameObject selectedItemGameObject = itemSetActive[inventoryList[selectedItem]];
            //selectedItemGameObject.SetActive(true);
        }
    }

    

}


