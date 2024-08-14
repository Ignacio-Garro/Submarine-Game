
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField] List<Vector3Int> roomTilesWithoutDoor;
    [SerializeField] List<Vector3Int> roomTilesWithDoor;
    [SerializeField] int weigth = 1;
    public List<Vector3Int> RoomTilesWithoutDoor => roomTilesWithoutDoor;
    public List<Vector3Int> RoomTilesWithDoor => roomTilesWithDoor;
    public int Weigth => weigth;

    public void OpenDoor(int orientation, Vector3 position)
    {

    }
}
