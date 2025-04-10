using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RageSkill : MonoBehaviour
{
    public GameObject character;
    private CharacterScript characterScript;
    private float exFireRate;
    public float rageTime,fireRateIncrease,coolDown;
    public bool canRage=true;
    void Start()
    {
        
        characterScript=character.GetComponent<CharacterScript>();
        
    }


    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Q)&&canRage) 
        {
        StartCoroutine(Rage());
        }
    }

    IEnumerator Rage()
    {
        canRage = false;
        exFireRate = characterScript.fireRate;
        fireRateIncrease = (FindChildWithTag() * 3.5f) / 10f;
        characterScript.fireRate -= fireRateIncrease;
        yield return new WaitForSeconds(rageTime);
        characterScript.fireRate = exFireRate;
        StartCoroutine(CoolDown());
    }
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        canRage=true;
    }
    float FindChildWithTag()
    {
        Transform[] child = character.GetComponentsInChildren<Transform>();

        for (int i = 0; i < child.Length; i++)
        {
            if (child[i].tag == "pistol")
            {
                return characterScript.fireRate; 
            }
            if (child[i].tag == "asa")
            {
                return characterScript.asaRate;
            }
        }
        return 0; 
    }

}
