using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


struct LevelTile
{
    public TileMapObject Tile;
    public int Rotation;
    public LevelTile(TileMapObject tile, int rotation)
    {
        Tile = tile;
        Rotation = rotation;
    }
}

struct RoomGrid
{
    public Vector3Int OriginPosition;
    public Vector3Int RoomDimensions;
    public List<List<List<bool>>> Grid;
    
    public RoomGrid(Vector3Int originPosition, Vector3Int roomDimensions, List<List<List<bool>>> grid)
    {
        OriginPosition = originPosition;
        RoomDimensions = roomDimensions;
        Grid = grid;
    }
}

public class ProceduralDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject tilePrefab;
    [SerializeField] int numberOfRooms = 4;
    [SerializeField] int maxRoomSize = 6;
    [SerializeField] int minRoomSize = 3;
    [SerializeField] int xGridSize = 10;
    [SerializeField] int yGridSize = 8;
    [SerializeField] int zGridSize = 8;
    [SerializeField] float tileSize = 20;
    [SerializeField]int numberOfExits = 3;
    [SerializeField] bool generate = true;
    [SerializeField] private float roomWideHillProbability = 0.1f;
    [SerializeField] private int maxWideHillsCount = 3;
    [SerializeField] private float roomTallHillProbability = 0.1f;
    [SerializeField] private int maxTallHillsCount = 3;

    List<List<List<bool>>> Grid = new List<List<List<bool>>>();
    List<LevelTile> placedTiles = new List<LevelTile>();
    List<TileMapObject> posibleTiles = new List<TileMapObject>();
    List<TileMapObject> randomSelectedTiles = new List<TileMapObject>();
    

    private void Start()
    {
        if (!generate) return;
        //placedTiles.Clear();
        ObtainTiles();
        FillRandomList();
        InstanciateMap(BuildGrid());
    }


    private void InstanciateMap(Vector3Int GridDimensions)
    {
        for (int i = 0; i < GridDimensions.x; i++)
        {
            for (int j = 0; j < GridDimensions.y; j++)
            {
                for (int k = 0; k < GridDimensions.z; k++)
                {
                    if (!Grid[i][j][k])
                    {
                        LevelTile tile = SelectTail(new Vector3Int(i,j,k), GridDimensions);
                        Instantiate(tile.Tile, new Vector3(i,-j,k) * tileSize + transform.position, Quaternion.Euler(0, tile.Rotation, 0));
                    }
                }
            }
        }
    }

    private Vector3Int BuildGrid()
    {
        int xDim = xGridSize;
        int yDim = yGridSize;
        int zDim = zGridSize;
        FillEmptyGrid(xDim, yDim, zDim);
        int nRooms = numberOfRooms;
        List<RoomGrid> rooms = new List<RoomGrid>();
        for(int i = 0; i < nRooms; i++)
        {
            rooms.Add(BuildRoomGrid(new Vector3Int(xDim, yDim, zDim)));
        }
        bool first = true;
        for (int i = 0; i < rooms.Count; i++)
        {
            RoomGrid room = rooms[i];
            if (first)
            {
                int origXfirst = Random.Range(0, xDim - room.RoomDimensions.x);
                int origYfirst = -1;
                int origZfirst = Random.Range(0, zDim - room.RoomDimensions.z);
                room.OriginPosition = new Vector3Int(origXfirst, origYfirst, origZfirst);
                first = false;
            }
            else
            {
                int origX = Random.Range(0, xDim - room.RoomDimensions.x);
                int origY = Random.Range(-1, yDim - room.RoomDimensions.y);
                int origZ = Random.Range(0, zDim - room.RoomDimensions.z);
                room.OriginPosition = new Vector3Int(origX, origY, origZ);
            }
            rooms[i] = room;
        }
        

        SeparateRooms(rooms, new Vector3Int(xDim, yDim, zDim));
        TranscribeRoomsToGrid(rooms);
        return new Vector3Int(xDim, yDim, zDim);
    }

    private void TranscribeRoomsToGrid(List<RoomGrid> rooms)
    {
        List<Vector4> exits = new List<Vector4>();
        int roomNumber = 0;
        foreach (RoomGrid room in rooms)
        {
            roomNumber++;
            List<Vector4> posibleExits = new List<Vector4>();
            for (int i = 0; i < room.RoomDimensions.x; i++)
            {
                for (int j = 0; j < room.RoomDimensions.y; j++)
                {
                    for(int k = 0; k < room.RoomDimensions.z; k++)
                    {
                        if (room.Grid[i][j][k])
                        {
                            int openNext = 0;
                            
                            if (i != 0 && !room.Grid[i - 1][j][k]) openNext++;
                            if (k != 0 && !room.Grid[i][j][k - 1]) openNext++;
                            if (j != 0 && !room.Grid[i][j - 1][k]) openNext++;
                            if (i != room.RoomDimensions.x - 1 && !room.Grid[i + 1][j][k]) openNext++;
                            if (k != room.RoomDimensions.z - 1 && !room.Grid[i][j][k + 1]) openNext++;
                            if (j != room.RoomDimensions.y - 1 && !room.Grid[i][j + 1][k]) openNext++;
                            if(openNext == 1 && j + room.OriginPosition.y != -1)
                            {
                                posibleExits.Add(new Vector4(i + room.OriginPosition.x, j + room.OriginPosition.y, k + room.OriginPosition.z, roomNumber));
                            }
                        }
                        else
                        {
                            Grid[i + room.OriginPosition.x][j + room.OriginPosition.y][k + room.OriginPosition.z] = false;
                        }
                    }
                }
            }
            for(int i = 0; i < numberOfExits; i++)
            {
                if (!posibleExits.Any()) break;
                int indx = Random.Range(0, posibleExits.Count);
                exits.Add(posibleExits[indx]);
                posibleExits.RemoveAt(indx);
            }
        }
        Dictionary<int,List<int>> exitsTo = new Dictionary<int,List<int>>();
        for (int i = 0; i < exits.Count;  i++)
        {
            Vector4 exit = exits[i];
            if (exits.Count <= 1) break;
            List<Vector4> sortedExits = new List<Vector4> (exits);
            sortedExits.Sort((ele1, ele2) =>
            {
                return Vector3.Distance(new Vector3(ele1.x, ele1.y, ele1.z), new Vector3(exit.x, exit.y, exit.z)).CompareTo(Vector3.Distance(new Vector3(ele2.x, ele2.y, ele2.z), new Vector3(exit.x, exit.y, exit.z)));
            });
            Vector4 toConnect = sortedExits.Find((ele) => ele.w != exit.w && (!exitsTo.TryGetValue((int)exit.w, out List<int> conections) || !conections.Contains((int)ele.w)));
            if (exitsTo.ContainsKey((int)exit.w))
            {
                exitsTo.TryGetValue((int)exit.w, out List<int> conections);
                conections.Add((int)toConnect.w);
            }
            else
            {
                exitsTo.Add((int)exit.w, new List<int>
                {
                    (int)toConnect.w
                });
            }
            if (exitsTo.ContainsKey((int)toConnect.w))
            {
                exitsTo.TryGetValue((int)toConnect.w, out List<int> conections);
                conections.Add((int)exit.w);
            }
            else
            {
                exitsTo.Add((int)toConnect.w, new List<int>
                {
                    (int)exit.w
                });
            }

            exits.Remove(toConnect);

            int x = (int)exit.x;
            int y = (int) exit.y;
            int z = (int) exit.z;
            bool finished = false;
            Grid[x][y][z] = false;
            while (x != toConnect.x)
            {
                x = (int)Mathf.MoveTowards(x, toConnect.x, 1);
                if (!Grid[x][y][z] && IsGreaterVector(new Vector3(x,y,z), rooms[(int)toConnect.w - 1].OriginPosition) && IsGreaterVector(rooms[(int)toConnect.w - 1].OriginPosition + rooms[(int)toConnect.w - 1].RoomDimensions, new Vector3(x,y,z)))
                {
                    finished = true; 
                }
                if (finished)
                {
                    break;
                }
                Grid[x][y][z] = false;
            }
            while (y != toConnect.y)
            {
                y = (int)Mathf.MoveTowards(y, toConnect.y, 1);
                if (!Grid[x][y][z] && IsGreaterVector(new Vector3(x, y, z), rooms[(int)toConnect.w - 1].OriginPosition) && IsGreaterVector(rooms[(int)toConnect.w - 1].OriginPosition + rooms[(int)toConnect.w - 1].RoomDimensions, new Vector3(x, y, z)))
                {
                    finished = true;
                }
                if (finished)
                {
                    break;
                }
                Grid[x][y][z] = false;
            }
            while (z != toConnect.z)
            {
                z = (int)Mathf.MoveTowards(z, toConnect.z, 1);
                if (!Grid[x][y][z] && IsGreaterVector(new Vector3(x, y, z), rooms[(int)toConnect.w - 1].OriginPosition) && IsGreaterVector(rooms[(int)toConnect.w - 1].OriginPosition + rooms[(int)toConnect.w - 1].RoomDimensions, new Vector3(x, y, z)))
                {
                    finished = true;
                }
                if (finished)
                {
                    break;
                }
                Grid[x][y][z] = false;
            }

        }

    }

    private bool IsGreaterVector(Vector3 first, Vector3 second)
    {
        return (first.x > second.x) && (first.y > second.y) && (first.z > second.z);
    }


    private void SeparateRooms(List<RoomGrid> rooms, Vector3Int GridDimensions)
    {
        for(int k = 0; k < 10000; k++)
        {
            bool neededSeparation = false;
            
            for(int i = 0; i < rooms.Count; i++) 
            {
                
                for (int j = 0; j < rooms.Count; j++)
                {
                    if (i == j) continue;
                    neededSeparation = neededSeparation || Separate2Rooms(rooms, i, j, GridDimensions);
                }
            }
            if (!neededSeparation) break;
            if (k == 10000 - 1) Debug.LogError("No se han conseguido separar las salas");
        }
        
    }

    private bool Separate2Rooms(List<RoomGrid> rooms, int r1, int r2, Vector3Int GridDimensions)
    {
        bool areSeparated = Check2RoomsSeparated(rooms[r1], rooms[r2]);
        if (areSeparated) return false;
        RoomGrid room1 = rooms[r1];
        RoomGrid room2 = rooms[r2];
        Vector3Int room1OriginalOrigin = room1.OriginPosition;
        Vector3Int room2OriginalOrigin = room2.OriginPosition;
        Vector3 room1Dir = room1.OriginPosition - room2.OriginPosition;
        Vector3 room2Dir = room2.OriginPosition - room1.OriginPosition;
        room1Dir.Normalize();
        room2Dir.Normalize();
        Vector3 room1AcumulatedOffset = Vector3.zero;
        Vector3 room2AcumulatedOffset = Vector3.zero;
        int limit = 0;
        while (!areSeparated)
        {
            limit++;
            if(limit > 10000)
            {
                Debug.LogError("2 salas no se han separado correctamente");
                break;
            }
            room1AcumulatedOffset += room1Dir;
            room2AcumulatedOffset += room2Dir;
            Vector3Int intRoom1AcumulatedOffset = new Vector3Int(Mathf.RoundToInt(room1AcumulatedOffset.x), Mathf.RoundToInt(room1AcumulatedOffset.y), Mathf.RoundToInt(room1AcumulatedOffset.z));
            Vector3Int intRoom2AcumulatedOffset = new Vector3Int(Mathf.RoundToInt(room2AcumulatedOffset.x), Mathf.RoundToInt(room2AcumulatedOffset.y), Mathf.RoundToInt(room2AcumulatedOffset.z));
            room1.OriginPosition = new Vector3Int(Mathf.Clamp(room1OriginalOrigin.x + intRoom1AcumulatedOffset.x, 0, GridDimensions.x - room1.RoomDimensions.x), Mathf.Clamp(room1OriginalOrigin.y + intRoom1AcumulatedOffset.y, -1, GridDimensions.y - room1.RoomDimensions.y), Mathf.Clamp(room1OriginalOrigin.z + intRoom1AcumulatedOffset.z, 0, GridDimensions.z - room1.RoomDimensions.z));
            room2.OriginPosition = new Vector3Int(Mathf.Clamp(room2OriginalOrigin.x + intRoom2AcumulatedOffset.x, 0, GridDimensions.x - room2.RoomDimensions.x), Mathf.Clamp(room2OriginalOrigin.y + intRoom2AcumulatedOffset.y, -1, GridDimensions.y - room2.RoomDimensions.y), Mathf.Clamp(room2OriginalOrigin.z + intRoom2AcumulatedOffset.z, 0, GridDimensions.z - room2.RoomDimensions.z));
            areSeparated = Check2RoomsSeparated(room1, room2);
        }
        rooms[r1] = room1;
        rooms[r2] = room2;
        return true;
    }

    private bool Check2RoomsSeparated(RoomGrid room1, RoomGrid room2)
    {
        bool xNoSolapation = room1.OriginPosition.x + room1.RoomDimensions.x <= room2.OriginPosition.x + 1 || room2.OriginPosition.x + room2.RoomDimensions.x <= room1.OriginPosition.x + 1;
        bool yNoSeparation = room1.OriginPosition.y + room1.RoomDimensions.y <= room2.OriginPosition.y + 1|| room2.OriginPosition.y + room2.RoomDimensions.y <= room1.OriginPosition.y + 1;
        bool zNoSeparation = room1.OriginPosition.z + room1.RoomDimensions.z <= room2.OriginPosition.z + 1 || room2.OriginPosition.z + room2.RoomDimensions.z <= room1.OriginPosition.z + 1;
        return xNoSolapation || yNoSeparation || zNoSeparation;
    }
    private bool vectorIsGreater(Vector3Int vector1, Vector3Int vector2)
    {
        return (vector1.x >= vector2.x && vector1.y >= vector2.y && vector1.z >= vector2.z);
    }

    private void FillEmptyGrid(int xDim, int yDim, int zDim)
    {
        Grid.Clear();
        for (int i = 0; i < xDim; i++)
        {
            List<List<bool>> xList = new List<List<bool>>();
            Grid.Add(xList);
            for (int j = 0; j < yDim; j++)
            {
                List<bool> yList = new List<bool>();
                xList.Add(yList);
                for (int k = 0; k < zDim; k++)
                {
                    yList.Add(true);
                }
            }
        }
    }
    private RoomGrid BuildRoomGrid(Vector3Int gridDimensions)
    {
        List<List<List<bool>>> roomGrid = new List<List<List<bool>>>();
        int xDim = Random.Range(minRoomSize, Mathf.Min(maxRoomSize + 1, gridDimensions.x));
        int yDim = Random.Range(minRoomSize, Mathf.Min(maxRoomSize + 1, gridDimensions.y));
        int zDim = Random.Range(minRoomSize, Mathf.Min(maxRoomSize + 1, gridDimensions.z));

        for (int i = 0; i < xDim; i++)
        {
            List<List<bool>> xList = new List<List<bool>>();
            roomGrid.Add(xList);
            for (int j = 0; j < yDim; j++)
            {
                List<bool> yList = new List<bool>();
                xList.Add(yList);
                for(int k = 0; k < zDim; k++)
                {

                    if (i == 0 || i == xDim - 1)
                    {
                        yList.Add(true);
                        continue;
                    }
                    if (j == 0 || j == yDim - 1) {
                        yList.Add(true);
                        continue;
                    }
                    if (k == 0 || k == zDim - 1) {
                        yList.Add(true);
                        continue;
                    }
                    /*if (consecutiveHills > 0)
                    {
                        yList.Add(true);
                        consecutiveHills--;
                        continue;
                    }
                    else if (tallHill > 0)
                    {
                        if (axis == 0 && axisNumber == i) { yList.Add(true); continue; }
                        if (axis == 1 && axisNumber == j) { yList.Add(true); continue; }
                        if (axis == 2 && axisNumber == k) { yList.Add(true); continue; }
                    }                    
                    if(tallHill == 0)
                    {
                        if (Random.Range(0, 1) < roomTallHillProbability)
                        {
                            tallHill = Random.Range(1, maxTallHillsCount + 1);
                            axis = Random.Range(0, 3);
                            if(axis == 0) axisNumber = i;
                            else if(axis == 1) axisNumber = j;
                            else if (axis == 2) axisNumber = k;
                        }

                    }
                    if(consecutiveHills == 0)
                    {
                        if(Random.Range(0,1) < roomWideHillProbability)
                        {
                            consecutiveHills = Random.Range(1, maxWideHillsCount + 1);
                        }
                    }*/
                    yList.Add(false);
                }
            }
        }
        return new RoomGrid(new Vector3Int(0, 0, 0), new Vector3Int(xDim, yDim, zDim), roomGrid);
    }

    //In grid true means wall
    private LevelTile SelectTail(Vector3Int position, Vector3Int gridDimensions)
    {
        
        int x = position.x;
        int y = position.y;
        int z = position.z;
        int dimX = gridDimensions.x;
        int dimY = gridDimensions.y;
        int dimZ = gridDimensions.z;
        if (Grid[x][y][z]) return new LevelTile();
        bool upWall = y > 0 && Grid[x][y - 1][z];
        bool downWall = y >= dimY - 1 || Grid[x][y + 1][z];
        bool rightWall = x >= dimX - 1 || Grid[x + 1][y][z];
        bool leftWall = x <= 0 || Grid[x - 1][y][z];
        bool frontWall = z <= 0 || Grid[x][y][z - 1];
        bool backWall = z >= dimZ - 1 || Grid[x][y][z+1];
        int rotation = 0;

        if (!randomSelectedTiles.Any()) FillRandomList();
        TileMapObject selectedTile = randomSelectedTiles.Find((element) =>
        {
            for (int r = 0; r <= 270; r += 90)
            {
                if (element.UpWall == upWall
                && element.DownWall == downWall
                && element.GetHorizontalConnectionRotated(0, r) == rightWall
                && element.GetHorizontalConnectionRotated(2, r) == leftWall
                && element.GetHorizontalConnectionRotated(1, r) == frontWall
                && element.GetHorizontalConnectionRotated(3, r) == backWall)
                {
                    rotation = r;
                    return true;
                }
            }
            return false;
        });
        if(selectedTile != null)
        {
            randomSelectedTiles.Remove(selectedTile);
        }
        else
        {
            selectedTile = posibleTiles.Find((element) =>
            {
                for (int r = 0; r <= 270; r += 90)
                {
                    if (element.UpWall == upWall
                    && element.DownWall == downWall
                    && element.GetHorizontalConnectionRotated(0, r) == rightWall
                    && element.GetHorizontalConnectionRotated(2, r) == leftWall
                    && element.GetHorizontalConnectionRotated(1, r) == frontWall
                    && element.GetHorizontalConnectionRotated(3, r) == backWall)
                    {
                        rotation = r;
                        return true;
                    }
                }
                return false;
            });
        }
        if(selectedTile == null)
        {
            Debug.LogError("No se ha encontrado la tile: \nArriba: " + upWall + "\nAbajo " + downWall + "\nDerecha: " + rightWall + "\nIzquierda " + leftWall + "\nFront: " + frontWall + "\nBack: " + backWall);
        }
        return new LevelTile(selectedTile, rotation);
    }

    

    private void ObtainTiles()
    {
        posibleTiles.Clear();
        foreach (Transform tileObject in tilePrefab.transform)
        {
            if(tileObject.gameObject.GetComponent<TileMapObject>() != null)
            {
                posibleTiles.Add(tileObject.gameObject.GetComponent<TileMapObject>());
            }
        }
    }

    private void FillRandomList()
    {
        List<TileMapObject> copy = new List<TileMapObject>(posibleTiles);
        while(copy.Any())
        {
            int indx = Random.Range(0, copy.Count);
            randomSelectedTiles.Add(copy[indx]);
            copy.RemoveAt(indx);
        }
    }

    


  
  
}
