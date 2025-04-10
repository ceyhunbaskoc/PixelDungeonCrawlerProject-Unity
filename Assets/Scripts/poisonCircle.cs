using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonCircle : MonoBehaviour
{
    public float poisonDamage;
    private List<FirstEnemyScript> enemies=new List<FirstEnemyScript>();


    private void Update()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].hp -= poisonDamage * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            print("düþma girdi");
            FirstEnemyScript es = collision.GetComponent<FirstEnemyScript>();
            if (es != null)
            {
                enemies.Add(es);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            print("düþma çýktý");
            FirstEnemyScript es = collision.GetComponent<FirstEnemyScript>();
            if (es != null)
            {
                enemies.Remove(es);
            }
        }
    }
}
