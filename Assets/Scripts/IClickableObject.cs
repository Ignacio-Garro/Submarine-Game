using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickableObject
{
    void OnClick(MonoBehaviour playerThatClicked);
}

