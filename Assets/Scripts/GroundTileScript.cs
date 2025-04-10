using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileScript : MonoBehaviour
{
    
    public Room roomScript;
    public float waitCount,enterVelocity;
    public int boxSize;
    public Collider2D[] enemies;
    Vector2 direction;
    Vector2Int boxSizeOverlap;
    LayerMask enemyLayer;
    bool canCheckEnemies=false;


    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        boxSizeOverlap = new Vector2Int(boxSize,boxSize);
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb= collision.gameObject.GetComponent<Rigidbody2D>();
            //print("center = " + gameObject.transform.position);
            direction = (new Vector2(gameObject.transform.position.x-3,gameObject.transform.position.y-1) - new Vector2(collision.gameObject.transform.position.x,collision.gameObject.transform.position.y)).normalized;
            //print("direction = " + direction);
            rb.linearVelocity += direction * enterVelocity;
            roomScript.characterScript.inPushtoRoom = true;
            canCheckEnemies = true;
            StartCoroutine(Wait());
            StartCoroutine(RepeatCheck());
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        roomScript.openDoors();
    //    }
    //}

    void CheckEnemies()
    {
        //print("checkEnemies çaðrýldý");
        enemies = Physics2D.OverlapBoxAll(transform.position, boxSizeOverlap, 0, enemyLayer);
        //print("Bulunan düþman sayýsý: " + enemies.Length);
        if (enemies.Length == 0)
        {
            //print("enemies boþ");
            roomScript.openDoors();
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitCount);
        roomScript.characterScript.inPushtoRoom = false;
        roomScript.closeDoors();
    }
    IEnumerator RepeatCheck()
    {
        CheckEnemies();
        yield return new WaitForSeconds(1f);
        StartCoroutine(RepeatCheck());
    }
    


}
