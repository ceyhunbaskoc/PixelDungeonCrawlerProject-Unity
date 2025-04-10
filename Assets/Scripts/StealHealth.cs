using System.Collections;
using UnityEngine;

public class StealHealth : MonoBehaviour
{
    public float fireRange;
    public float stealCoolDown, stealAmount;
    private CharacterScript characterScript;
    private FirstEnemyScript firstEnemyScript;
    public bool canSteal=true;

    private void Start()
    {
        characterScript = GetComponent<CharacterScript>();
        fireRange = characterScript.fireRange;
    }
    private void Update()
    {
        if(canSteal&&Input.GetKeyUp(KeyCode.H))
        {
            StartCoroutine(Steal());
        }
    }

    IEnumerator Steal()
    {
        canSteal = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fireRange);

        Collider2D nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        float distance;

        for (int i = 0; i < colliders.Length; i++)
        {

            if (colliders[i].CompareTag("Enemy"))
            {
                Vector2 rayDirection = (colliders[i].transform.position - transform.position).normalized;
                distance = Vector2.Distance(transform.position, colliders[i].transform.position);
                int layerMask = ~LayerMask.GetMask("Player", "rifle");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, distance, layerMask);
                Debug.DrawRay(transform.position, rayDirection * distance, Color.green, 1f); // Ray'i görselleþtir


                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {

                    if (distance < nearestDistance)
                    {
                        nearestEnemy = colliders[i];
                        nearestDistance = distance;
                    }
                }

            }
        }
        firstEnemyScript = nearestEnemy.GetComponent<FirstEnemyScript>();
        firstEnemyScript.hp -= stealAmount;
        characterScript.hp += stealAmount;
        yield return new WaitForSeconds(stealCoolDown);
        canSteal = true;
    }

}
