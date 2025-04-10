using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject portaltRoomPrefab;
    public GameObject[] roomPrefabs;
    public GameObject corridorPrefab;
    public GameObject rightDoor, leftDoor,upDoor,downDoor,leftWall,rightWall,upWall,downWall;
    List<Vector2Int> corridors = new List<Vector2Int>();
    List<Vector2Int> addWall = new List<Vector2Int>();
    public int minRoomCount,maxRoomCount;
    public int getRandomNeighborSize,doorDistance;
    private int roomNo=0;
    int randomRoomCount;

    private Dictionary<Vector2Int,Room> grid = new Dictionary<Vector2Int,Room>();
    private Vector2Int startPosition = Vector2Int.zero;
    Tilemap corridorTileMap;
    BoundsInt corridorBounds;
    int corridorWidth;
    int corridorHeight;


    private void Start()
    {
        corridorTileMap= corridorPrefab.GetComponent<Tilemap>();
        corridorBounds = corridorTileMap.cellBounds;
        corridorWidth = corridorBounds.size.x;
        corridorHeight = corridorBounds.size.y;
        GenerateLevel();
    }
    void GenerateLevel()
    {
        //baslangýc odasý
        PlaceRoom(startPosition, startRoomPrefab);

        //random diger odalar
        randomRoomCount = Random.Range(minRoomCount, maxRoomCount);
        print(randomRoomCount);
        //Vector2Int rightNeighbor = startPosition + Vector2Int.right * getRandomNeighborSize;
        GenerateRooms(startPosition);

        //portal odasý(en uzak noktaya koyuyor)
        PlacePortalRoom();

        for (int i = 0;i<randomRoomCount+1;i++)
        {
            GameObject room=GameObject.Find(i.ToString());
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(room.transform.position.x), Mathf.RoundToInt(room.transform.position.y));
            GetCorridorNeighbour(pos);

        }
        for (int i = 0; i < randomRoomCount + 1; i++)
        {
            GameObject room = GameObject.Find(i.ToString());
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(room.transform.position.x), Mathf.RoundToInt(room.transform.position.y));
            PlaceDoors(pos);

        }
        


    }

    void PlaceRoom(Vector2Int position, GameObject roomPrefab)
    {
        
        if (!grid.ContainsKey(position))
        {
            GameObject newRoomObj = Instantiate(roomPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            Room newRoom = newRoomObj.GetComponent<Room>();

            grid.Add(position, newRoom);
            newRoomObj.name = roomNo.ToString();
            roomNo++;
            


        }
    }

    

    void GenerateRooms(Vector2Int startPos)    //BFS (Breadth-First Search) algoritmasý//
    {
        Queue<Vector2Int> roomQueue= new Queue<Vector2Int>();
        roomQueue.Enqueue(startPos);
        

        while (grid.Count < randomRoomCount && roomQueue.Count > 0)
        {
            Vector2Int current = roomQueue.Dequeue();
            Vector2Int newRoom = GetRandomNeighbour(current);

            if (!grid.ContainsKey(newRoom))
            {
                int whichRoom = Random.Range(0, roomPrefabs.Length);
                PlaceRoom(newRoom, roomPrefabs[whichRoom]);
                roomQueue.Enqueue(newRoom);
            }
        }
    }

    void PlacePortalRoom()
    {
        Vector2Int farthestRoom = FindFarthestRoom();
        PlaceRoom(GetRandomNeighbour(farthestRoom), portaltRoomPrefab);
    }

    Vector2Int GetRandomNeighbour(Vector2Int position)
    {
        Vector2Int[] direction = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };

        Vector2Int randomDirection = direction[Random.Range(0, direction.Length)];
        
        for(int i = 0; i < direction.Length; i++)
        {

            Vector2Int neighborPosition = position + randomDirection * getRandomNeighborSize;

            if(!grid.ContainsKey(neighborPosition))
            {
                return neighborPosition;
            }

            randomDirection = direction[Random.Range(0, direction.Length)];
        }
        print("her yön dolu.");
        Vector2Int randomNeighbor = position + direction[Random.Range(0, direction.Length)] * getRandomNeighborSize;
        return randomNeighbor;
    }

    Vector2Int FindFarthestRoom()
    {
        Vector2Int farthestRoom = startPosition;
        int maxDistance = 0;
        foreach(var room in grid.Keys)
        {
            int distance = Mathf.Abs(room.x - startPosition.x) + Mathf.Abs(room.y - startPosition.y);
            if(distance > maxDistance )
            {
                farthestRoom = room;
                maxDistance = distance;
            }
        }
        return farthestRoom;
    }

    void GetCorridorNeighbour(Vector2Int position)
    {
        Vector2Int[] direction = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };

        //List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int i = 0; i < direction.Length; i++)
        {

            Vector2Int neighborPosition = position + direction[i] * getRandomNeighborSize;

            if (grid.ContainsKey(neighborPosition) && !AreCorridorsConnected(position, neighborPosition))
            {
                PlaceCorridor(position, neighborPosition);
            }

            
        }
        //int corridorCount= Random.Range(1,neighbours.Count+1);
        //for(int i = 0;i<corridorCount;i++)
        //{
        //    if(!AreCorridorsConnected(position, neighbours[i]))
        //    {
        //    PlaceCorridor(position, neighbours[i]);

        //    }
            
        //}
        
    }

    void PlaceCorridor(Vector2Int from, Vector2Int to)
    {
        Quaternion radius;
        if(to.y != from.y)
        {
            radius = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            radius = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        Vector2 position = (Vector2)from  + ((Vector2)(to - from) /2); // Ýki oda arasýnda konumlandýr
        Instantiate(corridorPrefab, new Vector3(position.x, position.y, 0), radius);
        corridors.Add(from+((to-from)/2));
    }

    bool AreCorridorsConnected(Vector2Int from, Vector2Int to)
    {
        Vector2Int position = from + ((to - from) / 2);
        if(corridors.Contains(position))
        {
            return true;
        }
        else { return false; }



    }

    void PlaceDoors(Vector2Int position)
    {
        Vector2Int[] direction = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        List<Vector2Int> addWall = new List<Vector2Int>();
        for(int i = 0; i < direction.Length;i++)
        {
            addWall.Add(direction[i]);
        }

        for (int i = 0; i < direction.Length; i++)
        {
            Vector2Int currentDireciton = direction[i];
            Vector2Int neighborPosition = position + direction[i] * getRandomNeighborSize;
            Vector2Int corridorPosition = position + ((neighborPosition-position)/2);

            if (corridors.Contains(corridorPosition))
            {
                print("corridor konumu var");
                Vector2Int doorPosition = position + direction[i]*doorDistance;
                if (currentDireciton==Vector2Int.up)
                {
                    Instantiate(upDoor, new Vector3(doorPosition.x,doorPosition.y,0), Quaternion.identity);
                    addWall.Remove(direction[i]);
                }
                if (currentDireciton == Vector2Int.down)
                {
                    Instantiate(downDoor, new Vector3(doorPosition.x, doorPosition.y, 0), Quaternion.identity);
                    addWall.Remove(direction[i]);
                }
                if (currentDireciton == Vector2Int.right)
                {
                    Instantiate(rightDoor, new Vector3(doorPosition.x, doorPosition.y, 0), Quaternion.identity);
                    addWall.Remove(direction[i]);
                }
                if (currentDireciton == Vector2Int.left)
                {
                    Instantiate(leftDoor, new Vector3(doorPosition.x, doorPosition.y, 0), Quaternion.identity);
                    addWall.Remove(direction[i]);
                }
            }

        }
        for (int i = 0; i < addWall.Count; i++)
        {
            print("duvarolustu");
            Vector2Int directionWall;
            directionWall = position + addWall[i] * doorDistance;
            if (addWall[i] == Vector2Int.up)
            {
                Instantiate(upWall, new Vector3(directionWall.x, directionWall.y, 0), Quaternion.identity);
            }
            if (addWall[i] == Vector2Int.down)
            {
                Instantiate(downWall, new Vector3(directionWall.x, directionWall.y, 0), Quaternion.identity);
            }
            if (addWall[i] == Vector2Int.right)
            {
                Instantiate(rightWall, new Vector3(directionWall.x, directionWall.y, 0), Quaternion.identity);
            }
            if (addWall[i] == Vector2Int.left)
            {
                Instantiate(leftWall, new Vector3(directionWall.x, directionWall.y, 0), Quaternion.identity);
            }

        }


    }

    void GetBorderTile(Vector2Int position)
    {

    }








    void newGetCorridorNeighbor(Vector2Int position)
    {
        Vector2Int[] direction = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };

        //List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int i = 0; i < direction.Length; i++)
        {

            Vector2Int neighborPosition = position + direction[i] * getRandomNeighborSize;

            if (grid.ContainsKey(neighborPosition) && !AreCorridorsConnected(position, neighborPosition))
            {
                PlaceCorridor(position, neighborPosition);
            }


        }
    }
    Vector2Int newGetRandomNeighbour(Vector2Int position)
    {
        Vector2Int[] direction = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, };

        Vector2Int randomDirection = direction[Random.Range(0, direction.Length)];

        for (int i = 0; i < direction.Length; i++)
        {

            Vector2Int neighborPosition = position + randomDirection * getRandomNeighborSize;

            if (!grid.ContainsKey(neighborPosition))
            {
                return neighborPosition;
            }

            randomDirection = direction[Random.Range(0, direction.Length)];
        }
        print("her yön dolu.");
        Vector2Int randomNeighbor = position + direction[Random.Range(0, direction.Length)] * getRandomNeighborSize;
        return randomNeighbor;
    }







}
