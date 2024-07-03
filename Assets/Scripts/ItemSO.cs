using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Properties")]
    public itemType item_type;
    public Sprite item_sprite;
}

public enum itemType { karambit, redCan, blueCan};

