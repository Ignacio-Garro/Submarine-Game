using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
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

    [Header("Item prefabs")]
    [SerializeField] GameObject RedCan_prefab;
    [SerializeField] GameObject BlueCan_prefab;

    [Header("UI")]
    [SerializeField] Image[] inventorySlotImage = new Image[3];
    [SerializeField] Image[] inventoryBackgroundImage = new Image[3];
    [SerializeField] Sprite emptySlotImage;
    //[SerializeField] GameObject PickUpTextHover;

    


    private Dictionary<itemType, GameObject> itemSetActive = new Dictionary<itemType, GameObject>(){};
    private Dictionary<itemType, GameObject> itemInstantiate = new Dictionary<itemType, GameObject>(){};
    void Start()
    {
        itemSetActive.Add(itemType.redCan, RedCan_item);
        itemSetActive.Add(itemType.blueCan, BlueCan_item);

        itemInstantiate.Add(itemType.redCan, RedCan_prefab);
        itemInstantiate.Add(itemType.blueCan, BlueCan_prefab);

        emptySlotImage = null;

        newItemSelected();
    }
    void Update(){  //THIS NEEDS TO BE OPTIMIZED
        //grab item
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo,playerReach)){
            IPickable item = hitInfo.collider.GetComponent<IPickable>();
            if(item != null){
                //PickUpTextHover.SetActive(true);
                if(Input.GetKey(pickUpItemKey)){
                    inventoryList.Add(hitInfo.collider.GetComponent<ItemPickable>().ItemScriprableObject.item_type);
                    item.PickItem();

                    selectedItem = inventoryList.Count -1;
                    newItemSelected();
                }
            }
            else{
                //PickUpTextHover.SetActive(false);
            }
        }
        else{
                //PickUpTextHover.SetActive(false);
            }
        //throw item
        if(Input.GetKeyDown(throwItemKey) && inventoryList.Count > 0){
            GameObject thrownItem = Instantiate(itemInstantiate[inventoryList[selectedItem]], position: throwItem_gameobjectPostion.transform.position, new Quaternion());
            inventoryList.RemoveAt(selectedItem);

            if(selectedItem != 0){
                selectedItem -=1;
            }
            newItemSelected();

            Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
            rb.AddForce(throwItem_gameobjectPostion.transform.forward * throwForce, ForceMode.VelocityChange);
        }


        //Change selected item with numpad
        if(Input.GetKeyDown(KeyCode.Alpha1) && inventoryList.Count > 0){
            selectedItem = 0;
            newItemSelected();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) && inventoryList.Count > 1){
            selectedItem = 1;
            newItemSelected();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3) && inventoryList.Count > 2){
            selectedItem = 2;
            newItemSelected();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4) && inventoryList.Count > 3){
            selectedItem = 3;
            newItemSelected();
        }

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
        RedCan_item.SetActive(false);
        BlueCan_item.SetActive(false);
        if(inventoryList.Count > 0){
            GameObject selectedItemGameObject = itemSetActive[inventoryList[selectedItem]];
            selectedItemGameObject.SetActive(true);
        }
    }
}

public interface IPickable{
    void PickItem();
}
