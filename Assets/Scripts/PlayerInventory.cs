using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using static UnityEditor.Progress;

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

    public ItemPickable currentHoldingItem => inventoryList.Any() ? inventoryList[selectedItem] : null;

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
        if(currentHoldingItem != null) InputManager.Instance.StopUsingItem(currentHoldingItem);
        inventoryList.Add(item);
        selectedItem = inventoryList.Count - 1;
        item.IsBeingHold = true;
        item.ChangeItemProperty(this);
        DeactivateColliders(item.gameObject);
        NetworkCommunicationManager.Instance.DeactivateRigidBodyServerRpc(item.gameObject);
        NetworkCommunicationManager.Instance.DeactivatePhysicCollisionsServerRpc(item.gameObject);
        item.transform.position = Vector3.zero;
        item.transform.rotation = Quaternion.identity;
        newItemSelected();
        
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

    public void DeactivateVisibility(GameObject item)
    {

        if (item == null) return;
        Renderer[] allChildren = item.GetComponentsInChildren<Renderer>(true);
        foreach (var item1 in allChildren)
        {
            item1.enabled = false;
        }
    }

    public void ActivateVisibility(GameObject item)
    {

        if (item == null) return;
        Renderer[] allChildren = item.GetComponentsInChildren<Renderer>(true);
        foreach (var item1 in allChildren)
        {
            item1.enabled = true;
        }
    }

    public void TryToDropCurrentObject(GameObject player, Camera camera)
    {
        if (!inventoryList.Any()) return;
        NetworkCommunicationManager.Instance.ActivateRigidBodyServerRpc(inventoryList[selectedItem].gameObject);
        ActivateColliders(inventoryList[selectedItem].gameObject);
        NetworkCommunicationManager.Instance.ActivatePhysicCollisionsServerRpc(inventoryList[selectedItem].gameObject);
        inventoryList[selectedItem].IsBeingHold = false;
        InputManager.Instance.ReleaseItemUsage(currentHoldingItem);
        inventoryList.RemoveAt(selectedItem);
        if (selectedItem > 0)
        {
            selectedItem -= 1;
        }
        
        newItemSelected();
    }

    public void ExtractItemForcefully(ItemPickable item)
    {
        if (!inventoryList.Any()) return;
        if (item != null && inventoryList[selectedItem] != item) return;
        inventoryList[selectedItem].IsBeingHold = false;
        InputManager.Instance.ReleaseItemUsage(currentHoldingItem);
        inventoryList.RemoveAt(selectedItem);
        if (selectedItem > 0)
        {
            selectedItem -= 1;
        }
        newItemSelected();
    }

    public void ChangeSelectedInventoryObject(int index)
    {
        if (inventoryList.Count <= index) return;
        InputManager.Instance.StopUsingItem(currentHoldingItem);
        selectedItem = index;
        newItemSelected();
    }

    void Update(){  //THIS NEEDS TO BE OPTIMIZED

        // Change selected item with mouse scroll wheel
        /*float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            if (scroll > 0f)
            {
                InputManager.Instance.StopUsingItem(currentHoldingItem);
                selectedItem++;
                if (selectedItem >= inventoryList.Count)
                {
                    selectedItem = 0;
                }
            }
            else if (scroll < 0f)
            {
                InputManager.Instance.StopUsingItem(currentHoldingItem);
                selectedItem--;
                if (selectedItem < 0)
                {
                    selectedItem = inventoryList.Count - 1;
                }
            }
            newItemSelected();
        }*/

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
            NetworkCommunicationManager.Instance.DeactivateVisibilityServerRpc(item.gameObject);
            NetworkCommunicationManager.Instance.DeactivateCollisionsServerRpc(item.gameObject);
            DeactivateVisibility(item.gameObject);
        });
        if(inventoryList.Count > 0){
            NetworkCommunicationManager.Instance.ActivateVisibilityServerRpc(inventoryList[selectedItem].gameObject);
            NetworkCommunicationManager.Instance.ActivateCollisionsServerRpc(inventoryList[selectedItem].gameObject);
            ActivateVisibility(inventoryList[selectedItem].gameObject);
            
            //GameObject selectedItemGameObject = itemSetActive[inventoryList[selectedItem]];
            //selectedItemGameObject.SetActive(true);
        }
    }

    

}


