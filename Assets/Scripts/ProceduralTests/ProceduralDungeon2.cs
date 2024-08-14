
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class ProceduralDungeon2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject tilePrefab;
    [SerializeField] int numberOfRooms = 4;
    [SerializeField] int distribution = 5;
    [SerializeField] int tileSize = 20;
    [SerializeField] bool generate = true;

    List<MapRoom> placedTiles = new List<MapRoom>();
    List<Vector3Int> usedPositions = new List<Vector3Int>();
    List<Vector3Int> posibleConnections = new List<Vector3Int>();
  
    List<RoomObject> posibleTiles = new List<RoomObject>();
    List<RoomObject> posibleHall = new List<RoomObject>();
    List<RoomObject> randomSelectedTiles = new List<RoomObject>();
    List<RoomObject> randomSelectedHall = new List<RoomObject>();
    

    private void Start()
    {
        if (!generate) return;
        //placedTiles.Clear();
        ObtainTiles();
        FillRandomTileList();
        FillRandomHallList();
        PlaceRooms();
        PlaceHalls();
        InstanciateMap();
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            if (randomSelectedTiles.Count == 0) FillRandomTileList();
            MapRoom newRoom = new MapRoom(randomSelectedTiles[0], new Vector3Int(UnityEngine.Random.Range(-distribution, distribution), UnityEngine.Random.Range(-2*distribution, -randomSelectedTiles[0].RoomTilesWithDoor.Select(v => v.y).Max()), UnityEngine.Random.Range(-distribution, distribution)), 0);
            placedTiles.Add(newRoom);
            randomSelectedTiles.RemoveAt(0);
        }
        SeparateRooms();
        foreach (MapRoom room in placedTiles)
        {
            AddRoom(room);
        }
    }

    private void PlaceHalls()
    {
        Dictionary<int, List<int>> roomConnections = new Dictionary<int, List<int>>();
        bool can = true;
        int indx = 0;
        while (can)
        {
            
            MapRoom room = placedTiles[indx];
            roomConnections.TryGetValue(indx, out List<int> con);
            int j = con == null ? 0 : con.Count;
            if(j > room.Room.RoomTilesWithDoor.Count - 1) can = false;
            for (; j < room.Room.RoomTilesWithDoor.Count; j++)
            {
                List<int> posibleConnections = new List<int>();
                if (j == room.Room.RoomTilesWithDoor.Count - 1)
                {
                    for (int i = 0; i < placedTiles.Count; i++)
                    {
                        if (!roomConnections.ContainsKey(i) && i != indx && placedTiles[i].Room.RoomTilesWithDoor.Count > 1)
                        {
                            posibleConnections.Add(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < placedTiles.Count; i++)
                    {
                        if (i != indx && !(roomConnections.TryGetValue(i, out List<int> c) && ((c.Count >= placedTiles[i].Room.RoomTilesWithDoor.Count) || c.Contains(indx))))
                        {
                            posibleConnections.Add(i);
                        }
                    }
                }
                if (posibleConnections.Count == 0)
                {
                    can = false;
                    break;
                }
                posibleConnections = posibleConnections.OrderBy(item => {
                    roomConnections.TryGetValue(item, out List<int> l);
                    return Vector3Int.Distance(placedTiles[indx].OriginalPosition + placedTiles[indx].Room.RoomTilesWithDoor[j], placedTiles[item].OriginalPosition + placedTiles[item].Room.RoomTilesWithDoor[l == null ? 0 : l.Count]);
                }).ToList();
                int selectedConnection = posibleConnections[0];
                if (!roomConnections.TryGetValue(indx, out List<int> connections)) roomConnections.Add(indx, new List<int> { selectedConnection });
                else connections.Add(selectedConnection);
                if (!roomConnections.TryGetValue(selectedConnection, out List<int> connections1)) roomConnections.Add(selectedConnection, new List<int> { indx });
                else connections1.Add(indx);
                indx = selectedConnection;
            }
        }
        for (int i = 0; i < placedTiles.Count; i++)
        {
            MapRoom room = placedTiles[i];
            roomConnections.TryGetValue(i, out List<int> con);
            int j = con == null ? 0 : con.Count;
            for (; j < room.Room.RoomTilesWithDoor.Count; j++)
            {
                List<int> posibleConnections = new List<int>();
                for (int k = 0; k < placedTiles.Count; k++)
                {
                    if (k != i && !(roomConnections.TryGetValue(k, out List<int> c) && ((c.Count >= placedTiles[k].Room.RoomTilesWithDoor.Count) || c.Contains(i))) && !(!roomConnections.ContainsKey(i) && !roomConnections.ContainsKey(k)))
                    {
                        posibleConnections.Add(k);
                    }
                }
                if (posibleConnections.Count == 0)
                {
                    continue;
                }
                posibleConnections = posibleConnections.OrderBy(item => {
                    roomConnections.TryGetValue(item, out List<int> l);
                    return Vector3.Distance(placedTiles[i].OriginalPosition + placedTiles[i].Room.RoomTilesWithDoor[j], placedTiles[item].OriginalPosition + placedTiles[item].Room.RoomTilesWithDoor[l == null ? 0 : l.Count]);
                }).ToList();
                int selectedConnection = posibleConnections[0];
                if (!roomConnections.TryGetValue(i, out List<int> connections)) roomConnections.Add(i, new List<int> { selectedConnection });
                else connections.Add(selectedConnection);
                if (!roomConnections.TryGetValue(selectedConnection, out List<int> connections1)) roomConnections.Add(selectedConnection, new List<int> { i });
                else connections1.Add(i);
            }
        }
        
        List<MapRoom> placedHalls = new List<MapRoom>();
        AStarPathfinding pathfinding = new AStarPathfinding();
        for (int i = 0; i < placedTiles.Count; i++)
        {
            roomConnections.TryGetValue(i, out List<int> connections);
            for(int j = 0; j < (connections == null ? 0 : connections.Count); j++)
            {
                int c = connections[j];
                if (c < i) continue;
                if (placedTiles[i].Room.RoomTilesWithDoor.Count <= j) Debug.Log("Indice j:" + j + "Tamaño: " + placedTiles[i].Room.RoomTilesWithDoor.Count);
                roomConnections.TryGetValue(c, out List<int> reverseConnection);
                Vector3Int startConnection = placedTiles[i].Room.RoomTilesWithDoor[j] + placedTiles[i].OriginalPosition;
                Vector3Int endConnection = placedTiles[c].Room.RoomTilesWithDoor[reverseConnection.FindIndex((ele) => ele == i)] + placedTiles[c].OriginalPosition;
                Vector3Int pathStart = GetClosestConnectionPosition(startConnection, endConnection);
                Vector3Int pathEnd = GetClosestConnectionPosition(endConnection, startConnection);
                List<Vector3Int> path =  pathfinding.CalculateShortestPath(pathStart, pathEnd, (position, exception) => IsPositionInUse(position, exception));
                if(path == null) path = new List<Vector3Int>();
                foreach (var item in path)
                {
                    if (IsPositionInUse(item)) continue;
                    if (randomSelectedHall.Count == 0) FillRandomHallList();
                    MapRoom hall = new MapRoom(randomSelectedHall[0], item, 0);
                    placedHalls.Add(hall);
                    AddRoom(hall);
                    randomSelectedHall.RemoveAt(0);
                }
            }
        }
        for (int i = placedTiles.Count - 1; i >= 0; i--)
        {
            if (!roomConnections.ContainsKey(i))
            {
                placedTiles.RemoveAt(i);
            }
        }
        foreach (var item in placedHalls)
        {
            placedTiles.Add(item);
        }
    }

    private Vector3Int GetClosestConnectionPosition(Vector3Int connection, Vector3Int destination)
    {
        List<Vector3Int> posibleStartPositions = new List<Vector3Int>();

        if (!IsPositionInUse(connection + Vector3Int.left) || IsPositionAConnection(connection + Vector3Int.left))
        {
            posibleStartPositions.Add(connection + Vector3Int.left);
        }

        if (!IsPositionInUse(connection + Vector3Int.right) || IsPositionAConnection(connection + Vector3Int.right))
        {
            posibleStartPositions.Add(connection + Vector3Int.right);
        }


        if (!IsPositionInUse(connection + Vector3Int.forward) || IsPositionAConnection(connection + Vector3Int.forward))
        {
            posibleStartPositions.Add(connection + Vector3Int.forward);
        }

        if (!IsPositionInUse(connection + Vector3Int.back) || IsPositionAConnection(connection + Vector3Int.back))
        {
            posibleStartPositions.Add(connection + Vector3Int.back);
        }
        if (!IsPositionInUse(connection + Vector3Int.down) || IsPositionAConnection(connection + Vector3Int.down))
        {
            posibleStartPositions.Add(connection + Vector3Int.down);
        }

        if (connection.y <= -1)
        {
            if(!IsPositionInUse(connection + Vector3Int.up) || IsPositionAConnection(connection + Vector3Int.up))
        {
                posibleStartPositions.Add(connection + Vector3Int.up);
            }
        }

        posibleStartPositions = posibleStartPositions.OrderBy(item => Vector3Int.Distance(item, destination)).ToList();
        return posibleStartPositions.First();
    }

    private void SeparateRooms()
    {
        bool hadToSeparate = true;
        while (hadToSeparate)
        {
            hadToSeparate = false;
            foreach(MapRoom room in placedTiles)
            {
                foreach (MapRoom room1 in placedTiles)
                {
                    if (room == room1) continue;
                    if (CheckRoomsAreOverlapping(room,room1))
                    {
                        hadToSeparate = true;
                        Separate2Rooms(room, room1);
                    }
                }
            }
        }
    }


    

    private void Separate2Rooms(MapRoom room1, MapRoom room2)
    {
        Vector3Int room1OriginalOrigin = room1.OriginalPosition;
        Vector3Int room2OriginalOrigin = room2.OriginalPosition;
        Vector3 room1Dir = room1.OriginalPosition - room2.OriginalPosition;
        Vector3 room2Dir = room2.OriginalPosition - room1.OriginalPosition;
        if(room1Dir == Vector3Int.zero && room2Dir == Vector3Int.zero)
        {
            room1Dir = Vector3Int.down;
            room2Dir = Vector3Int.up;
        }
        room1Dir.Normalize();
        room2Dir.Normalize();
        Vector3 room1AcumulatedOffset = Vector3.zero;
        Vector3 room2AcumulatedOffset = Vector3.zero;
        while (CheckRoomsAreOverlapping(room1, room2))
        {
            room1AcumulatedOffset += room1Dir;
            room2AcumulatedOffset += room2Dir;
            Vector3Int intRoom1AcumulatedOffset = new Vector3Int(Mathf.RoundToInt(room1AcumulatedOffset.x), Mathf.RoundToInt(room1AcumulatedOffset.y), Mathf.RoundToInt(room1AcumulatedOffset.z));
            Vector3Int intRoom2AcumulatedOffset = new Vector3Int(Mathf.RoundToInt(room2AcumulatedOffset.x), Mathf.RoundToInt(room2AcumulatedOffset.y), Mathf.RoundToInt(room2AcumulatedOffset.z));
            room1.OriginalPosition = new Vector3Int(room1OriginalOrigin.x + intRoom1AcumulatedOffset.x, Mathf.Min(room1OriginalOrigin.y + intRoom1AcumulatedOffset.y, 0 - room1.Room.RoomTilesWithDoor.Select(v=>v.y).Max()), room1OriginalOrigin.z + intRoom1AcumulatedOffset.z);
            room2.OriginalPosition = new Vector3Int(room2OriginalOrigin.x + intRoom2AcumulatedOffset.x, Mathf.Min(room2OriginalOrigin.y + intRoom2AcumulatedOffset.y, 0 - room2.Room.RoomTilesWithDoor.Select(v => v.y).Max()), room2OriginalOrigin.z + intRoom2AcumulatedOffset.z);
        }
    }
    
    private bool CheckRoomsAreOverlapping(MapRoom room1, MapRoom room2)
    {
        List<Vector3Int> usedPositions = new List<Vector3Int>();
        foreach (Vector3Int pos in room1.Room.RoomTilesWithDoor)
        {
            usedPositions.Add(pos + room1.OriginalPosition + Vector3Int.forward);
            usedPositions.Add(pos + room1.OriginalPosition + Vector3Int.back);
            usedPositions.Add(pos + room1.OriginalPosition + Vector3Int.up);
            usedPositions.Add(pos + room1.OriginalPosition + Vector3Int.down);
            usedPositions.Add(pos + room1.OriginalPosition + Vector3Int.left);
            usedPositions.Add(pos + room1.OriginalPosition + Vector3Int.right);
            usedPositions.Add(pos + room1.OriginalPosition);
        }
        foreach (Vector3Int pos in room1.Room.RoomTilesWithoutDoor)
        {
            usedPositions.Add(pos + room1.OriginalPosition);
        }

        foreach (Vector3Int pos in room2.Room.RoomTilesWithDoor)
        {
            if(usedPositions.Contains(pos + room2.OriginalPosition)) return true;
        }
        foreach (Vector3Int pos in room2.Room.RoomTilesWithoutDoor)
        {
            if(usedPositions.Contains(pos + room2.OriginalPosition)) return true;
        }
        return false;
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

    

    
    private void AddRoom(MapRoom Room)
    {
        foreach (Vector3Int position in Room.Room.RoomTilesWithoutDoor)
        {
           usedPositions.Add(position + Room.OriginalPosition);
        }
        foreach (Vector3Int position in Room.Room.RoomTilesWithDoor)
        {
            usedPositions.Add(position + Room.OriginalPosition);
            posibleConnections.Add(position + Room.OriginalPosition);
        }
    }

   

   

    private bool IsPositionInUse(Vector3Int position)
    {
        return usedPositions.Contains(position) || position.y > 0;
    }

    private bool IsPositionInUse(Vector3Int position, Vector3Int exception)
    {
        if (exception == position) return false;
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
            RoomObject room = tileObject.gameObject.GetComponent<RoomObject>();
            if (room != null)
            {
                for(int i = 0; i < room.Weigth; i++)
                {
                    if(room.RoomTilesWithDoor.Count == 1 && room.RoomTilesWithoutDoor.Count == 0)
                    {
                        posibleHall.Add(room);
                    }
                    else
                    {
                        posibleTiles.Add(room);
                    }
                }
            }
        }
        posibleTiles = posibleTiles.OrderBy(item => UnityEngine.Random.value).ToList();
        posibleHall = posibleHall.OrderBy(item => UnityEngine.Random.value).ToList();
    }

    private void FillRandomHallList()
    {
        List<RoomObject> copy = new List<RoomObject>(posibleHall);
        while (copy.Any())
        {
            int indx = UnityEngine.Random.Range(0, copy.Count);
            randomSelectedHall.Add(copy[indx]);
            copy.RemoveAt(indx);
        }
    }

    private void FillRandomTileList()
    {
        List<RoomObject> copy = new List<RoomObject>(posibleTiles);
        while(copy.Any())
        {
            int indx = UnityEngine.Random.Range(0, copy.Count);
            randomSelectedTiles.Add(copy[indx]);
            copy.RemoveAt(indx);
        }
    }

    


  
  
}
