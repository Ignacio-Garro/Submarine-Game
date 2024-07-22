using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : NetworkBehaviour
{
    [Header("Gerneral")]
    public List<itemType> inventoryList;
    public int selectedItem;
    public float playerReach;
    public float throwForce;
    [SerializeField] Camera cam;
    [SerializeField] GameObject throwItem_gameobjectPostion;

     [Header("Keys")]

    [SerializeField] KeyCode throwItemKey;
    [SerializeField] KeyCode pickUpItemKey;

    [Header("Item gameobjects")]
    [SerializeField] GameObject RedCan_item;
    [SerializeField] GameObject BlueCan_item;

    

    [Header("UI")]
    [SerializeField] Image[] inventorySlotImage = new Image[3];
    [SerializeField] Image[] inventoryBackgroundImage = new Image[3];
    [SerializeField] Sprite emptySlotImage;
    //[SerializeField] GameObject PickUpTextHover;

    


    private Dictionary<itemType, GameObject> itemSetActive = new Dictionary<itemType, GameObject>(){};
    private Dictionary<itemType, GameObject> itemInstantiate => InventoryInfoManager.Instance.SpawnableObjects;
    void Start()
    {
        itemSetActive.Add(itemType.redCan, RedCan_item);
        itemSetActive.Add(itemType.blueCan, BlueCan_item);

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
        inventoryList.Add(item.ItemScriprableObject.item_type);
        selectedItem = inventoryList.Count - 1;
        newItemSelected();
    }

    public void TryToDropCurrentObject(GameObject player, Camera camera)
    {
        if (inventoryList.Count <= 0) return;
        NetworkCommunicationManager.Instance.SpawnNetworkWithForceObjectServerRpc(inventoryList[selectedItem], throwItem_gameobjectPostion.transform.position, throwItem_gameobjectPostion.transform.forward, throwForce);
        //GameObject thrownItem = Instantiate(itemInstantiate[inventoryList[selectedItem]], position: throwItem_gameobjectPostion.transform.position, new Quaternion());
        inventoryList.RemoveAt(selectedItem);

        if (selectedItem != 0)
        {
            selectedItem -= 1;
        }
        newItemSelected();
        /*Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(throwItem_gameobjectPostion.transform.forward * throwForce, ForceMode.VelocityChange);
        }*/
        
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
        itemSetActive.Keys.ToList().ForEach(type => {
            NetworkCommunicationManager.Instance.DeactivateItemServerRpc(GameManager.Instance.ActualPlayer, type);
            HideItem(type);
        });
        if(inventoryList.Count > 0){
            NetworkCommunicationManager.Instance.ActivateItemServerRpc(GameManager.Instance.ActualPlayer, inventoryList[selectedItem]);
            ShowItem(inventoryList[selectedItem]);
            //GameObject selectedItemGameObject = itemSetActive[inventoryList[selectedItem]];
            //selectedItemGameObject.SetActive(true);
        }
    }

    public void HideItem(itemType item)
    {
        itemSetActive.TryGetValue(item, out GameObject itemObject);
        if(itemObject != null)
        {
            itemObject.SetActive(false);
        }
    }

    public void ShowItem(itemType item)
    {
        itemSetActive.TryGetValue(item, out GameObject itemObject);
        if (itemObject != null)
        {
            itemObject.SetActive(true);
        }
    }

}


