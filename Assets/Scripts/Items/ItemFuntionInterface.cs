using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface ItemFunctionInterface 
{
    public void OnItemUse(GameObject interactingObject);
    public void OnItemUnuse(GameObject interactingObject);
    public void OnItemRemove(GameObject interactingObject);
    public void OnItemOutOfView(GameObject interactingObject);
    public void OnItemInView(GameObject interactingObject);

}


