using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickableObject
{
    void OnClick(GameObject playerThatInteracted);
    void OnClickRelease();
}

public interface IInteractuableObject
{
    void OnInteract(GameObject playerThatInteracted);
    void OnEnterInRange();
    void OnExitInRange();
}

public interface IGrabbableObject
{
    void OnGrab(GameObject playerThatInteracted);
    void OnDrop(GameObject playerThatInteracted);

}



