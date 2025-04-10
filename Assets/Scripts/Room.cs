using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

    public GameObject character;
    public CharacterScript characterScript;
    public newLevelSystem levelGenerator;
    public newLevelSystem otherLevelGenerator;
    public Tilemap wallsTilemap, groundTilemap, Tilemap;
    public GameObject corridor;
    public Tile leftWallTile, rightWallTile, upWallTile, downWallTile,openDoorTile,blackTile;
    public int corridorDistance,enemyWaveCount;
    Vector2Int neighborX, neighborY;
    public Vector2Int gridPosition;
    private Tilemap closestTilemap=null;        
    private float closestDistance = Mathf.Infinity;
    private Tilemap corridorTilemap;
    List<Vector2Int> neighborDirections = new List<Vector2Int>();
    Vector2Int currentPos;
    int roomWidth;
    int roomHeight;
    public Collider2D[] enemies;
    LayerMask enemyLayer;


    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        character = GameObject.FindGameObjectWithTag("Player");
        characterScript=character.GetComponent<CharacterScript>();
        otherLevelGenerator = GameObject.Find("LevelGenerator").GetComponent<newLevelSystem>();
        neighborX = new Vector2Int(otherLevelGenerator.neighorDistance, 1);
        neighborY = new Vector2Int(1, otherLevelGenerator.neighorDistance);
        Transform tilemapTransform = corridor.transform.Find("ground");
        if(tilemapTransform != null )
        {
            corridorTilemap= tilemapTransform.GetComponent<Tilemap>();
        }


        //Vector2Int kontrolEdilenPos = new Vector2Int(34, 0);
        //foreach (var key in levelGenerator.roomPositions)
        //{
        //    if (key == kontrolEdilenPos)
        //    {
        //        Debug.Log("Pozisyon eþleþti!");
        //    }
        //}
        //Debug.Log("roomPositions baþta: " + otherLevelGenerator.roomPositions.Count);
        StartCoroutine(Wait());

    }
    void PlaceWalls()
    {
        
        currentPos = gridPosition;

        roomWidth = groundTilemap.cellBounds.size.x;
        roomHeight = groundTilemap.cellBounds.size.y;
        //print("ROOM " + gameObject.name + " roomWidth = " + roomWidth);
        //print("ROOM " + gameObject.name + " roomHeight = " + roomHeight);

        //Debug.Log("Current Position (gridPosition): " + gridPosition);
        foreach (var roomPos in otherLevelGenerator.roomPositions)
        {
            //Debug.Log("Room Position: " + roomPos);
        }
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborCheck;

            neighborCheck = currentPos + (dir * otherLevelGenerator.neighorDistance);
            
            //Debug.Log($"Direction: {dir} => Calculated neighborCheck: {neighborCheck}");
            bool shouldCreate = true;
            
            foreach (var pos in otherLevelGenerator.roomPositions)
            {
                //Debug.Log($"Comparing roomPos: {pos} with neighborCheck: {neighborCheck}");
                if (pos == neighborCheck)
                {
                    shouldCreate = false;
                    neighborDirections.Add (dir);
                    break;
                }
            }
            if (shouldCreate)
            {
                //Debug.Log("No matching neighbor found for direction " + dir + ", duvar oluþturulacak.");
                //print("komþu bulunamadý");
                Vector3Int startWallPos = CalculateStartWallPosition(dir, currentPos, roomWidth, roomHeight);
                Tile whichTile = GetWallTile(dir,rightWallTile,leftWallTile,upWallTile,downWallTile);
                for (int j = -2; j < 2; j++)
                {
                    Vector3Int wallPos = startWallPos;
                    if (dir.y != 0)
                    {
                        wallPos.x += j;
                    }
                    if (dir.x != 0)
                    {
                        wallPos.y += j;
                    }
                    wallsTilemap.SetTile(wallPos, whichTile);
                }
            }
            else
            {
                //Debug.Log("Neighbor exists for direction " + dir + ", duvar oluþturulmayacak.");
            }
        }
    }

    Vector3Int CalculateStartWallPosition(Vector2Int dir, Vector2Int currentPos, int roomWidth, int roomHeight)
    {
        Vector3Int startWallPos = Vector3Int.zero;
        //print("ROOM " + gameObject.name + " CurrentPos = " + currentPos);
        if (dir == Vector2Int.right)
            startWallPos = new Vector3Int(roomWidth / 2-4, -1, 0);
        else if (dir == Vector2Int.left)
            startWallPos = new Vector3Int(-roomWidth / 2-3, -1, 0);
        else if (dir == Vector2Int.up)
            startWallPos = new Vector3Int(-3, roomHeight / 2-2, 0);
        else if (dir == Vector2Int.down)
            startWallPos = new Vector3Int(-3, -roomHeight / 2-1, 0);
        //print("ROOM " + gameObject.name + " WorldToCellPOSITION = " + groundTilemap.WorldToCell(transform.position));
        //print("ROOM " + gameObject.name + " return = " + startWallPos);
        return startWallPos;
    }
    
    Tile GetWallTile(Vector2Int dir,Tile right, Tile left, Tile up, Tile down)
    {
        switch (dir)
        {
            case Vector2Int v when v == Vector2Int.right: return right;
            case Vector2Int v when v == Vector2Int.left: return left;
            case Vector2Int v when v == Vector2Int.up: return up;
            case Vector2Int v when v == Vector2Int.down: return down;
            default: return null;
        }
    }
    IEnumerator Wait()
    {
        //Debug.Log("Waiting for roomPositions to update...");
        while (otherLevelGenerator.roomPositions.Count == 0)
        {
            yield return null;
        }

        //Debug.Log("roomPositions güncellendi ve içeriyor: " + otherLevelGenerator.roomPositions.Count + " oda");
        foreach (var pos in otherLevelGenerator.roomPositions)
        {
            //Debug.Log("Room position: " + pos);
        }

        yield return new WaitForSeconds(0.5f);
        PlaceWalls();
    }
    public void openDoors()
    {
        print("open doors çaðýrýldý");
        foreach(var direction in directions)
        {
            if(neighborDirections.Contains(direction))
            {
                Vector3Int startDoorPos=CalculateStartWallPosition(direction, currentPos, roomWidth, roomHeight);
                Tile whichTile = openDoorTile;
                for (int j = -1; j < 1; j++)
                {
                    Vector3Int wallPos = startDoorPos;
                    if (direction.y != 0)
                    {
                        wallPos.x += j;
                    }
                    if (direction.x != 0)
                    {
                        wallPos.y += j;
                    }
                    wallsTilemap.SetTile(wallPos, null);
                }
            }
        }
    }
    public void closeDoors()
    {
        print("close doors çaðýrýldý");
        foreach (var direction in directions)
        {
            if (neighborDirections.Contains(direction))
            {
                Vector3Int startDoorPos = CalculateStartWallPosition(direction, currentPos, roomWidth, roomHeight);
                Tile whichTile = GetWallTile(direction, rightWallTile, leftWallTile, upWallTile, downWallTile);
                for (int j = -1; j < 1; j++)
                {
                    Vector3Int wallPos = startDoorPos;
                    if (direction.y != 0)
                    {
                        wallPos.x += j;
                    }
                    if (direction.x != 0)
                    {
                        wallPos.y += j;
                    }
                    wallsTilemap.SetTile(wallPos, blackTile);
                }
            }
        }
    }
}



