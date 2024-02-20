using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickableObject
{
    void OnClick(MonoBehaviour playerThatClicked);
    void OnClickRelease();
}

public interface IInteractuableObject
{
    void OnInteract(MonoBehaviour playerThatInteracted);

}

