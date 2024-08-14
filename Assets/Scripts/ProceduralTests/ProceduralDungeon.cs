
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

class MapRoom
{
    public Vector3Int OriginalPosition;
    public float Rotation;
    public RoomObject Room;
    public MapRoom(RoomObject Room, Vector3Int OriginalPosition, float Rotation)
    {
        this.Room = Room;
        this.OriginalPosition = OriginalPosition;
        this.Rotation = Rotation;
    }

}

public class ProceduralDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject tilePrefab;
    [SerializeField] int numberOfRooms = 4;
    [SerializeField] int tileSize = 20;
    [SerializeField] bool generate = true;

    List<MapRoom> placedTiles = new List<MapRoom>();
    List<Vector3Int> usedPositions = new List<Vector3Int>();
    List<Vector3Int> posibleExitsNoConnection = new List<Vector3Int>();
    List<Vector3Int> posibleExitsConnection = new List<Vector3Int>();
    List<Vector3Int> posibleConnections = new List<Vector3Int>();
  
    List<RoomObject> posibleTiles = new List<RoomObject>();
    List<RoomObject> randomSelectedTiles = new List<RoomObject>();
    

    private void Start()
    {
        if (!generate) return;
        //placedTiles.Clear();
        ObtainTiles();
        FillRandomList();
        BuildMap();
        InstanciateMap();
    }


    private void InstanciateMap()
    {
        foreach(MapRoom tile in placedTiles)
        {
            RoomObject roomObject = Instantiate(tile.Room.gameObject, transform.position + new Vector3(tile.OriginalPosition.x, tile.OriginalPosition.y, tile.OriginalPosition.z) * tileSize , tile.Room.transform.rotation).GetComponent<RoomObject>();
            if (roomObject == null) continue;
            foreach(Vector3Int doorPosition in roomObject.RoomTilesWithDoor)
            {
                if (IsPositionAConnection(doorPosition + tile.OriginalPosition + Vector3Int.forward))
                {
                    roomObject.OpenDoor(1, doorPosition);
                }
                if (IsPositionAConnection(doorPosition + tile.OriginalPosition + Vector3Int.back))
                {
                    roomObject.OpenDoor(3, doorPosition);
                }
                if (IsPositionAConnection(doorPosition + tile.OriginalPosition + Vector3Int.right))
                {
                    roomObject.OpenDoor(0, doorPosition);
                }
                if (IsPositionAConnection(doorPosition + tile.OriginalPosition + Vector3Int.left))
                {
                    roomObject.OpenDoor(2, doorPosition);
                }
                if (IsPositionAConnection(doorPosition + tile.OriginalPosition + Vector3Int.up))
                {
                    roomObject.OpenDoor(4, doorPosition);
                }
                if (IsPositionAConnection(doorPosition + tile.OriginalPosition + Vector3Int.down))
                {
                    roomObject.OpenDoor(5, doorPosition);
                }
            }

        }
    }

    private void BuildMap()
    {
        RoomObject FirstRoom = randomSelectedTiles.First()?.GetComponent<RoomObject>();
        if (FirstRoom == null) return;
        AddRoom(new MapRoom(FirstRoom, Vector3Int.zero, 0));
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector3Int nextRoomPosition = GetFirstPosibleExit();
            if (nextRoomPosition == Vector3Int.up) break;
            List<Vector3Int> posibleStartPositions = new List<Vector3Int>();
            if(!IsPositionInUse(nextRoomPosition + Vector3Int.forward))
            {
                posibleStartPositions.Add(nextRoomPosition + Vector3Int.forward);
            }
            if (!IsPositionInUse(nextRoomPosition + Vector3Int.back))
            {
                posibleStartPositions.Add(nextRoomPosition + Vector3Int.back);
            }
            if (!IsPositionInUse(nextRoomPosition + Vector3Int.right))
            {
                posibleStartPositions.Add(nextRoomPosition + Vector3Int.right);
            }
            if (!IsPositionInUse(nextRoomPosition + Vector3Int.left))
            {
                posibleStartPositions.Add(nextRoomPosition + Vector3Int.left);
            }
            if (!IsPositionInUse(nextRoomPosition + Vector3Int.up))
            {
                posibleStartPositions.Add(nextRoomPosition + Vector3Int.up);
            }
            if (!IsPositionInUse(nextRoomPosition + Vector3Int.down))
            {
                posibleStartPositions.Add(nextRoomPosition + Vector3Int.down);
            }
            posibleStartPositions = posibleStartPositions.OrderBy(item => Random.value).ToList();
            if (!posibleStartPositions.Any())
            {
                posibleExitsConnection.Remove(nextRoomPosition);
                posibleExitsNoConnection.Remove(nextRoomPosition);
            } 
            if (!randomSelectedTiles.Any()) FillRandomList();
            MapRoom room = SearchRoomFromList(randomSelectedTiles, posibleStartPositions);
            if (room == null) room = SearchRoomFromList(posibleTiles, posibleStartPositions);
            else randomSelectedTiles.Remove(room.Room);
            if (room != null) AddRoom(room);
            else{
                Debug.Log("No se ha encontrado sala");
            }
            
        }
    }


    private MapRoom SearchRoomFromList(List<RoomObject> roomList, List<Vector3Int> posibleStartPositions)
    {
        MapRoom room = null;
        for (int j = 0; j < roomList.Count; j++)
        {
            RoomObject tile = roomList[j];
            foreach (Vector3Int position in posibleStartPositions)
            {
                foreach (Vector3Int doorPos in tile.RoomTilesWithDoor)
                {
                    Vector3Int origin = position - doorPos;
                    bool isValid = true;
                    foreach (Vector3Int tilePos in tile.RoomTilesWithoutDoor)
                    {
                        if (IsPositionInUse(origin + tilePos))
                        {
                            isValid = false;
                            break;
                        }
                    }
                    foreach (Vector3Int tilePos in tile.RoomTilesWithDoor)
                    {
                        if (IsPositionInUse(origin + tilePos))
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        room = new MapRoom(tile, origin, 0);
                        break;
                    }
                }
                if (room != null) break;
            }
            if (room != null) break;
        }
        return room;
    }

    private Vector3Int GetFirstPosibleExit()
    {
        if(posibleExitsNoConnection.Any()) return posibleExitsNoConnection[0];
        if (posibleExitsConnection.Any())
        {
            Vector3Int exit = posibleExitsConnection[0];
            posibleExitsConnection.Remove(exit);
            posibleExitsConnection.Add(exit);
            return exit;
        }
        return Vector3Int.up;
    }

    private void AddRoom(MapRoom Room)
    {
        placedTiles.Add(Room);
        foreach (Vector3Int position in Room.Room.RoomTilesWithoutDoor)
        {
           usedPositions.Add(position + Room.OriginalPosition);
        }
        foreach (Vector3Int position in Room.Room.RoomTilesWithDoor)
        {
            usedPositions.Add(position + Room.OriginalPosition);
            AddConnectionExit(position + Room.OriginalPosition, Room.Room.RoomTilesWithDoor.Count <= 1);
            posibleConnections.Add(position + Room.OriginalPosition);
        }
    }

    private void AddConnectionExit(Vector3Int position, bool isOnlyExit)
    {
        int connections = 0;
        void CheckConnection(Vector3Int offset)
        {
            if (IsPositionAConnection(offset))
            {
                connections++;
                if (posibleExitsNoConnection.Contains(offset))
                {
                    posibleExitsNoConnection.Remove(offset);
                    posibleExitsConnection.Add(offset);
                }
                if(IsConnectionFull(offset)) posibleExitsConnection.Remove(offset);
            }
        }
        CheckConnection(position + Vector3Int.forward);
        CheckConnection(position + Vector3Int.back);
        CheckConnection(position + Vector3Int.right);
        CheckConnection(position + Vector3Int.left);
        CheckConnection(position + Vector3Int.up);
        CheckConnection(position + Vector3Int.down);
        if (!IsConnectionFull(position))
        {
            if(connections <= 0 || isOnlyExit) posibleExitsNoConnection.Add(position);
            else if (connections < 6) posibleExitsConnection.Add(position);
        }
        
    }

    private bool IsConnectionFull(Vector3Int position)
    {
        int used = 0; ;
        void CheckConnection(Vector3Int offset)
        {
            if (IsPositionInUse(offset)) used++;
        }
        CheckConnection(position + Vector3Int.forward);
        CheckConnection(position + Vector3Int.back);
        CheckConnection(position + Vector3Int.right);
        CheckConnection(position + Vector3Int.left);
        CheckConnection(position + Vector3Int.up);
        CheckConnection(position + Vector3Int.down);
        if (used < 6) return false;
        return true;
    }

    private bool IsPositionInUse(Vector3Int position)
    {
        return usedPositions.Contains(position) || position.y > 0;
    }

    private bool IsPositionAConnection(Vector3Int position)
    {
        return posibleConnections.Contains(position);
    }

    private void ObtainTiles()
    {
        posibleTiles.Clear();
        foreach (Transform tileObject in tilePrefab.transform)
        {
            if(tileObject.gameObject.GetComponent<RoomObject>() != null)
            {
                posibleTiles.Add(tileObject.gameObject.GetComponent<RoomObject>());
            }
        }
    }

    private void FillRandomList()
    {
        List<RoomObject> copy = new List<RoomObject>(posibleTiles);
        while(copy.Any())
        {
            int indx = Random.Range(0, copy.Count);
            randomSelectedTiles.Add(copy[indx]);
            copy.RemoveAt(indx);
        }
    }

    


  
  
}
