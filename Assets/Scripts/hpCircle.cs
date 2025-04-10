using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class hpCircle : MonoBehaviour
{
    public float hpIncrease;
    private List<CharacterScript> charactersInZone=new List<CharacterScript>();
    void Start()
    {
        
    }

    private void Update()
    {
        for(int i = 0;i < charactersInZone.Count;i++)
        {
            charactersInZone[i].hp += hpIncrease * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterScript cs = collision.GetComponent<CharacterScript>();
            if (cs != null)
            {
                charactersInZone.Add(cs);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterScript cs = collision.GetComponent<CharacterScript>();
            if (cs != null)
            {
                charactersInZone.Remove(cs);
            }
        }
    }



}
