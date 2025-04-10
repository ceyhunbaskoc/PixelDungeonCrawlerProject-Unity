using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class newLevelSystem : MonoBehaviour
{
    public GameObject[] startRoomPrefabs;
    public GameObject[] fightRoomPrefabs;
    public GameObject portalRoomPrefab;

    public GameObject leftCorridorPrefab;
    public GameObject rightCorridorPrefab;
    public GameObject middleCorridorPrefab;

    public int wallsDistance;
    public int secondCornerPrefabDistance,cornerPrefabDistance;
    public int minIntermediateRoomCount, maxIntermediateRoomCount;
    public int neighorDistance;

    public Dictionary<Vector2Int, GameObject> rooms = new Dictionary<Vector2Int, GameObject>();
    public List<Vector2Int> roomPositions = new List<Vector2Int>();
    List<Vector2Int> corridorPositions = new List<Vector2Int>();

    private int randomIndermediateRoomCount;
    private Vector2Int lastIndermediateRoomPos;
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
    private Vector2Int startPos = Vector2Int.zero;
    private int roomNo = 0;
    private int sayac = 0;
    void Start()
    {
        randomIndermediateRoomCount = Random.Range(minIntermediateRoomCount, maxIntermediateRoomCount);
        print(randomIndermediateRoomCount);
        GenerateLevel();
        
    }

    void GenerateLevel()
    {
        GenerateRoom(startPos, startRoomPrefabs[Random.Range(0, startRoomPrefabs.Length)]);   //Baþlangýç odasý

        GenerateIntermediateRooms(startPos);  //Ara odalar(FightRooms)

        Vector2Int portalRoomPos = RandomNeighbor(lastIndermediateRoomPos);   //Portal odasýnýn konumu

        GenerateRoom(portalRoomPos, portalRoomPrefab);    //Portal odasý

        for (int i = 0; i < randomIndermediateRoomCount+2; i++)
        {
            GameObject room = GameObject.Find(i.ToString());
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(room.transform.position.x), Mathf.RoundToInt(room.transform.position.y));
            PlaceCorridors(pos);
        }

        


    }

    void GenerateRoom(Vector2Int pos, GameObject prefab)
    {
        if (!rooms.ContainsKey(pos))
        {
            GameObject newRoomObj = Instantiate(prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            
            Room newRoom = newRoomObj.GetComponent<Room>();
            newRoom.gridPosition= pos;
            rooms.Add(pos, newRoomObj);
            roomPositions.Add(pos);
            newRoomObj.name = roomNo.ToString();
            roomNo++;
        }
    }

    Vector2Int RandomNeighbor(Vector2Int pos)
    {
        Vector2Int distanceX = new Vector2Int(neighorDistance, 1);
        Vector2Int distanceY = new Vector2Int(1, neighorDistance);
        Vector2Int neighborPos;
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int randomDirection = directions[Random.Range(0, directions.Length)];
            if (randomDirection == Vector2.right || randomDirection == Vector2.left)
            {
                neighborPos = pos + randomDirection * distanceX;
            }
            else { neighborPos = pos + randomDirection * distanceY; }

            if (!rooms.ContainsKey(neighborPos))
            {
                return neighborPos;
            }
        }
        //her yön doluysa
        Vector2Int Default = pos + Vector2Int.up * distanceX;
        return Default;
    }

    void GenerateIntermediateRooms(Vector2Int startPos)    //BFS (Breadth-First Search) algoritmasý//
    {
        Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
        roomQueue.Enqueue(startPos);


        while (rooms.Count < randomIndermediateRoomCount + 1 && roomQueue.Count > 0)
        {
            Vector2Int current = roomQueue.Dequeue();
            Vector2Int newRoom = RandomNeighbor(current);


            if (!rooms.ContainsKey(newRoom))
            {
                int whichRoom = Random.Range(0, fightRoomPrefabs.Length);
                GenerateRoom(newRoom, fightRoomPrefabs[whichRoom]);
                lastIndermediateRoomPos = newRoom;
                roomQueue.Enqueue(newRoom);
                
            }
        }
    }

    void PlaceCorridors(Vector2Int position)
    {

        


        for (int i = 0; i < directions.Length; i++)
        {

            Vector2Int neighborPosition = position + directions[i] * neighorDistance;

            if (rooms.ContainsKey(neighborPosition))
            {
                GameObject positionRoom = rooms[position];
                Tilemap[] posTilemaps = positionRoom.GetComponentsInChildren<Tilemap>();
                Tilemap targetposTilemap = null;
                foreach (Tilemap tilemap in posTilemaps)
                {
                    if (tilemap.name == "walls")
                    {
                        targetposTilemap = tilemap;
                        break;
                    }
                }
                Vector2Int posMiddlePos = Vector2Int.zero;


                GameObject neighborRoom = rooms[neighborPosition];
                Tilemap[] neighTilemaps = neighborRoom.GetComponentsInChildren<Tilemap>();
                Tilemap targetneiTilemap = null;
                foreach (Tilemap tilemap in neighTilemaps)
                {
                    if (tilemap.name == "walls")
                    {
                        targetneiTilemap = tilemap;
                        break;
                    }
                }
                Vector2Int neiMiddlePos=Vector2Int.zero;

                Vector2Int oppositeDirection = -directions[i];
                Vector2Int middlePoint;

                if (targetposTilemap != null)
                {
                    posMiddlePos=FoundMiddlePos(targetposTilemap, directions[i]);
                }
                if (targetneiTilemap != null)
                {
                    neiMiddlePos=FoundMiddlePos(targetneiTilemap, oppositeDirection);
                }
                middlePoint = (posMiddlePos + neiMiddlePos) / 2;
                while (sayac < 1)
                {
                    //print("posMiddlePos " + posMiddlePos);
                    //print("neiMiddlePos " + neiMiddlePos);
                    //print("middlePoint " + middlePoint);
                    //print("Distance = " + Vector2Int.Distance(posMiddlePos, neiMiddlePos));
                    sayac++;
                }
                
                if (!corridorPositions.Contains(middlePoint))
                {

                    corridorPositions.Add(middlePoint);
                    if (Vector2Int.Distance(posMiddlePos, neiMiddlePos) == 16)  //////////////// Kýsa koridor oluþturuluyor...
                    {
                        Quaternion radius;
                        if (posMiddlePos.y != neiMiddlePos.y)
                        {
                            radius = Quaternion.Euler(new Vector3(0, 0, 90));
                            Instantiate(middleCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y, 0), radius);
                        }
                        else
                        {
                            radius = Quaternion.Euler(new Vector3(0, 0, 0));
                            Instantiate(middleCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y, 0), radius);
                        }
                    }
                    else if (Vector2Int.Distance(posMiddlePos, neiMiddlePos) == 20)//////////////// Uzun koridor oluþturuluyor...
                    {
                        Quaternion radius;
                        if (posMiddlePos.y != neiMiddlePos.y)
                        {
                            radius = Quaternion.Euler(new Vector3(0, 0, 90));
                            Vector2Int cornerDistanceY = new Vector2Int(0, cornerPrefabDistance);
                            Instantiate(middleCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y, 0), radius);
                            Instantiate(rightCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y + cornerDistanceY.y, 0), radius);
                            Instantiate(leftCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y - cornerDistanceY.y, 0), radius);
                        }
                        else
                        {
                            radius = Quaternion.Euler(new Vector3(0, 0, 0));
                            Vector2Int cornerDistanceX = new Vector2Int(cornerPrefabDistance, 0);
                            Instantiate(middleCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y, 0), radius);
                            Instantiate(rightCorridorPrefab, new Vector3(middlePoint.x + cornerDistanceX.x, middlePoint.y, 0), radius);
                            Instantiate(leftCorridorPrefab, new Vector3(middlePoint.x -cornerDistanceX.x, middlePoint.y, 0), radius);
                        }
                    }
                    else if(Vector2Int.Distance(posMiddlePos,neiMiddlePos)==24)
                    {
                        
                        Quaternion radius;
                        if (posMiddlePos.y != neiMiddlePos.y)
                        {
                            radius = Quaternion.Euler(new Vector3(0, 0, 90));
                            Vector2Int cornerDistanceY = new Vector2Int(0, cornerPrefabDistance);
                            Vector2Int distanceY = new Vector2Int(0, secondCornerPrefabDistance);
                            Instantiate(middleCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y, 0), radius);
                            Instantiate(rightCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y + cornerDistanceY.y, 0), radius);
                            Instantiate(rightCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y + cornerDistanceY.y+distanceY.y, 0), radius);
                            Instantiate(leftCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y - cornerDistanceY.y, 0), radius);
                            Instantiate(leftCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y - cornerDistanceY.y-distanceY.y, 0), radius);
                        }
                        else
                        {
                            radius = Quaternion.Euler(new Vector3(0, 0, 0));
                            Vector2Int cornerDistanceX = new Vector2Int(cornerPrefabDistance, 0);
                            Vector2Int distanceX = new Vector2Int(secondCornerPrefabDistance, 0);
                            Instantiate(middleCorridorPrefab, new Vector3(middlePoint.x, middlePoint.y, 0), radius);
                            Instantiate(rightCorridorPrefab, new Vector3(middlePoint.x + cornerDistanceX.x, middlePoint.y, 0), radius);
                            Instantiate(rightCorridorPrefab, new Vector3(middlePoint.x + cornerDistanceX.x+distanceX.x, middlePoint.y, 0), radius);
                            Instantiate(leftCorridorPrefab, new Vector3(middlePoint.x - cornerDistanceX.x, middlePoint.y, 0), radius);
                            Instantiate(leftCorridorPrefab, new Vector3(middlePoint.x - cornerDistanceX.x-distanceX.x, middlePoint.y, 0), radius);
                        }
                    }
                }




            }


        }
    }

    Vector2Int FoundMiddlePos(Tilemap tilemap, Vector2Int direction)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is null!");
            return Vector2Int.zero;
        }
        BoundsInt bounds = tilemap.cellBounds;
        int x, y;

        if (direction == Vector2Int.right)
        {
            x = bounds.xMax - 1;
            y = bounds.yMin + (bounds.size.y / 2);
        }
        else if (direction == Vector2Int.left)
        {
            x = bounds.xMin + 1;
            y = bounds.yMin + (bounds.size.y / 2);
        }
        else if (direction == Vector2Int.up)
        {
            x = bounds.xMin + (bounds.size.x / 2);
            y = bounds.yMax - 1;
        }
        else if (direction == Vector2Int.down)
        {
            x = bounds.xMin + (bounds.size.x / 2);
            y = bounds.yMin + 1;
        }
        else { return Vector2Int.zero; }
        Vector3Int middleCell = new Vector3Int(x, y, 0);
        Vector3 middleWorld = tilemap.CellToWorld(middleCell);
        return new Vector2Int(Mathf.RoundToInt(middleWorld.x), Mathf.RoundToInt(middleWorld.y));





    }
}
