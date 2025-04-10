using System.Collections;
using UnityEngine;

public class ShieldSkill : MonoBehaviour
{
    public GameObject shieldPrefab;
    public bool canShield=true;
    public float shieldTime, coolDown;
    Transform shieldT;
    SpriteRenderer sr;
    Collider2D collider;
    GameObject shield;





    void Update()
    {
        if(canShield&&Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Shield());
        }
    }
    IEnumerator Shield()
    {
        canShield = false;
        shield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        sr=shield.GetComponent<SpriteRenderer>();
        collider = shield.GetComponent<Collider2D>();
        shieldT=shield.GetComponent<Transform>();
        shieldT.SetParent(gameObject.transform);
        yield return new WaitForSeconds(shieldTime);
        sr.enabled = false;
        collider.enabled = false;
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(coolDown);
        canShield = true;
        Destroy(shield);
    }
}
