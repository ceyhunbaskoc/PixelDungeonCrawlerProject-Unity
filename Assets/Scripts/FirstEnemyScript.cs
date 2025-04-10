using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FirstEnemyScript : MonoBehaviour
{
    public enum AttackModes {Far,Near,Following };
    public AttackModes attackMode;
    private Transform target;
    public float hp, attackDamage,farAttackMin,farAttackMax,nearAttackDistance,bulletSpeed,bulletRate,farAttackTime;
    public GameObject character,bulletPrefab;
    private Rigidbody2D bulletRb;
    public bool isFiring=false,isRetreating=false;
    private float maxHp=100f,minHp=0f;
    public float detectRange;
    public BulletPool bulletPool;
    

    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        attackMode = AttackModes.Following;
    }
    private void Update()
    {
        hp = Mathf.Min(hp, maxHp);
        hp = Mathf.Max(hp, minHp);
        if(FindNearestPlayer()!=null)
        {
        target = FindNearestPlayer().transform;

        }
        float distance = Vector2.Distance(target.position, transform.position);

        if(isRetreating )
        {
            agent.SetDestination(transform.position+(transform.position-target.position).normalized*farAttackMin);
            if(distance>=farAttackMin )
            {
                attackMode = AttackModes.Far;
                isRetreating = false;
                agent.isStopped = true;
            }
            return;
        }
        
        agent.SetDestination(target.position);
        if (distance>=farAttackMin&&distance<=farAttackMax)
        {
            attackMode= AttackModes.Far;
            
            agent.isStopped = true;
            StartCoroutine(Wait());
            
        }
        if (!isFiring && attackMode == AttackModes.Far&&shouldFire(target))
        {
            StartCoroutine(FarAttack());
            
        }


        if (distance>farAttackMax)
        {
            attackMode = AttackModes.Following;
            
        }
        if(attackMode==AttackModes.Following)
        {
            agent.isStopped = false;
        }


        if(distance<=nearAttackDistance) 
        {
            attackMode = AttackModes.Near;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            StartCoroutine(animationWait());
        }
        
            
        
        
    }
    IEnumerator FarAttack()
    {
        if(attackMode!=AttackModes.Far)
        {
            yield break;
        }
        isFiring = true;
        Vector2 direction = (target.position-transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject EnemyBullet = bulletPool.GetEnemyBullet();
        EnemyBullet.transform.position = transform.position;
        EnemyBullet.transform.rotation=Quaternion.Euler(0,0,angle);
        bulletRb = EnemyBullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction*bulletSpeed;
        yield return new WaitForSeconds(bulletRate*Time.fixedDeltaTime);
        isFiring =false;
    }
    void NearAttack()
    {
        //Animasyon ile yapýlacak...
    }
    public void ReturnFarAttack()         //////ANIMASYONUN SONUNA EKLENECEK///////////
    {
        agent.isStopped = false;
        
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(farAttackTime);
        agent.isStopped = false;
        
    }
    IEnumerator animationWait()//////ReturnFarAttack yerine geçecek...//////////
    {
        yield return new WaitForSeconds(1.5f);
        isRetreating = true;
        agent.isStopped = false;
        attackMode = AttackModes.Following;
    }

    bool shouldFire(Transform nearestEnemy)
    {
        if(attackMode== AttackModes.Near)
        {
            return false;
        }
        float distance = Vector2.Distance(nearestEnemy.position, transform.position);
        Vector2 rayDirection = (nearestEnemy.position - transform.position).normalized;
        int layerMask = ~LayerMask.GetMask("Enemy", "rifle","ground","Circles","decorations","bullet","enemyBullet");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, distance, layerMask);
        Debug.DrawRay(transform.position, rayDirection * distance, Color.red, 1f);
        
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            //print("should fire = true");
            return true;
        }
        else
        {
            //print("should fire = false");
            return false;
        }


    }
    Collider2D FindNearestPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectRange);

        Collider2D nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;
        float distance;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                distance = Vector2.Distance(transform.position, collider.transform.position);
                Vector2 rayDirection = (collider.transform.position - transform.position).normalized;
                int layerMask = LayerMask.GetMask("Player");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, distance, layerMask);

                if (hit.collider != null)
                {
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlayer = collider;
                    }
                }

            }
        }
        
        return nearestPlayer;
    }
    

}
