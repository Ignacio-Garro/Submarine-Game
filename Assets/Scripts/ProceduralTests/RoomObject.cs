using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField] List<Vector3Int> roomTilesWithoutDoor;
    [SerializeField] List<Vector3Int> roomTilesWithDoor;
    public List<Vector3Int> RoomTilesWithoutDoor => roomTilesWithoutDoor;
    public List<Vector3Int> RoomTilesWithDoor => roomTilesWithDoor;

    public void OpenDoor(int orientation, Vector3 position)
    {

    }
}
