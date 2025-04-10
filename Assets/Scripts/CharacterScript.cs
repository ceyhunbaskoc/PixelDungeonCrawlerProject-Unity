using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScript : MonoBehaviour
{
    public BulletPool bulletPool;
    public float minHp=0f,maxHp,hp,maxArmor, armor,maxEnergy, energy,waveBulletSpeed ,bulletSpeed, arrowSpeed,asaSpeed,asaRate, fireRate,waveRate, fireRange, movementSpeed,pistolEnergy,wavePistolEnergy,bowEnergy,asaEnergy,swordEnergy;
    public GameObject bulletPrefab, waveBulletPrefab, arrowPrefab, asaBulletPrefab;
    private Rigidbody2D bulletRb;
    private Rigidbody2D rbCharacter;
    public bool isFiring=false;
    public bool inPushtoRoom = false;
    public GameObject bowBar;
    private Slider bowBarSlider;
    public float bowBarmax=5f, bowBarFillSpeed=1f, currentBowBarValue=0f;
    public float asaBulletAngle;
    private Animator animator;
    


    void Start()
    {
        animator = GetComponent<Animator>();
        rbCharacter = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        hp=Mathf.Min(hp,maxHp);
        hp = Mathf.Max(hp, minHp);
        if(!inPushtoRoom)
        {
        if (Input.GetKey(KeyCode.W))
        {
            rbCharacter.linearVelocity = new Vector2(rbCharacter.linearVelocityX, movementSpeed);
                animator.SetBool("isMove", true);
            }
        if (Input.GetKey(KeyCode.A))
        {
            rbCharacter.linearVelocity = new Vector2(-movementSpeed, rbCharacter.linearVelocityY);
                animator.SetBool("isMove", true);
            }
        if (Input.GetKey(KeyCode.S))
        {
            rbCharacter.linearVelocity = new Vector2(rbCharacter.linearVelocityX, -movementSpeed);
                animator.SetBool("isMove", true);
            }
        if (Input.GetKey(KeyCode.D))
        {
            rbCharacter.linearVelocity = new Vector2(movementSpeed, rbCharacter.linearVelocityY);
                animator.SetBool("isMove", true);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                animator.SetBool("isMove", false);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                animator.SetBool("isMove", false);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                animator.SetBool("isMove", false);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                animator.SetBool("isMove", false);
            }
        }
        if (FindChildWithTag("pistol") != null && energy>=pistolEnergy)
        {
            //print(isFiring);
            if (Input.GetKey(KeyCode.Space) &&!isFiring)
            {
                StartCoroutine(FireBullet());
            }


        }
        if(FindChildWithTag("bow") != null && energy >= bowEnergy)
        {
            
            if (Input.GetKey(KeyCode.R))
            {
                bowBar.SetActive(true);
                currentBowBarValue += bowBarFillSpeed * Time.deltaTime;
                currentBowBarValue = Mathf.Clamp(currentBowBarValue, 0, bowBarmax);
                bowBarSlider = bowBar.GetComponent<Slider>();
                bowBarSlider.value = Mathf.InverseLerp(0,bowBarmax,currentBowBarValue);

            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                if(currentBowBarValue<2.5f)
                {
                    currentBowBarValue = 2.5f;
                }
                FireStrongArrow();
                currentBowBarValue = 0f;
                bowBarSlider.value = currentBowBarValue;
                bowBar.SetActive(false);

            }
        }
        if(FindChildWithTag("asa") != null && energy >= asaEnergy)
        {
            if (Input.GetKey(KeyCode.T)&&!isFiring)
            {
                StartCoroutine(FireAsa());
            }
        }
        void FindNearestEnemyAndFire(float energySpend,GameObject prefab,float speed)
        {
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
                    int layerMask = ~LayerMask.GetMask("Player","rifle","ground","Circles","decorations","bullet","enemyBullet");
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, distance,layerMask);
                    Debug.DrawRay(transform.position, rayDirection * distance, Color.green, 1f); // Ray'i görselleþtir
                    
                    //print(hit.collider.gameObject.name);
                    if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                    {
                        //print("hesap yapýlýyor...");
                        
                        if (distance < nearestDistance)
                        {
                            nearestEnemy = colliders[i];
                            nearestDistance = distance;
                        }
                    }

                }
            }
            
            if (nearestEnemy != null)
            {
                //print("Nearest Enemy = " + nearestEnemy.name);
                GameObject bullet = bulletPool.GetBullet();
                bullet.transform.position = transform.position;
                //bullet.transform.rotation = transform.rotation;
                //print("bullet oluþtu");
                if (energy >= energySpend)
                {

                    energy -= energySpend;
                }
                bulletRb = bullet.GetComponent<Rigidbody2D>();

                Vector2 direction = (nearestEnemy.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0,0,angle);    
                bulletRb.linearVelocity = direction * speed;
                

            }
            //else
            //{
            //    print("Nearest Enemy is empty...");
            //}
        }
        IEnumerator FireBullet()
        {
            if (isFiring)
                yield break;
            //will change
            isFiring = true;

            while (Input.GetKey(KeyCode.Space))
            {
                //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                FindNearestEnemyAndFire(pistolEnergy,bulletPrefab,bulletSpeed);

                
                yield return new WaitForSeconds(fireRate*Time.fixedDeltaTime);
            }

            isFiring = false;


        }
        IEnumerator WavePistol()
        {
            isFiring= true;
            while(Input.GetKey(KeyCode.Space))
            {
                FindNearestEnemyAndFire(wavePistolEnergy, waveBulletPrefab, waveBulletSpeed);

                yield return new WaitForSeconds(waveRate * Time.fixedDeltaTime);
            }
            isFiring = false;
        }
        void FireArrow()
        {

            //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            FindNearestEnemyAndFire(bowEnergy,arrowPrefab,arrowSpeed);




        }
        void FireStrongArrow()
        {
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
                    int layerMask = ~LayerMask.GetMask("Player", "rifle", "ground", "Circles", "decorations", "bullet", "enemyBullet");
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
            if (nearestEnemy != null)
            {
                
                GameObject bullet = bulletPool.GetBowArrow();
                bullet.transform.position = transform.position;
                //bullet.transform.rotation = transform.rotation;
                //print("bullet oluþtu");
                if (energy >= bowEnergy)
                {

                    energy -= bowEnergy;
                }
                bulletRb = bullet.GetComponent<Rigidbody2D>();

                Vector2 direction = (nearestEnemy.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0,0,angle);    
                bulletRb.linearVelocity = direction * arrowSpeed*currentBowBarValue;
                

            }
        }

        IEnumerator FireAsa()
        {
            isFiring = true;
            while (Input.GetKey(KeyCode.T))
            {

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
                        int layerMask = ~LayerMask.GetMask("Player", "rifle", "ground", "Circles", "decorations", "bullet", "enemyBullet");
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
                if (nearestEnemy != null)
                {

                    GameObject bullet = bulletPool.GetBullet();
                    GameObject bullet2 = bulletPool.GetBullet();
                    GameObject bullet3 = bulletPool.GetBullet();

                    bullet.transform.position = transform.position;
                    bullet2.transform.position = transform.position;
                    bullet3.transform.position = transform.position;
                    if (energy >= asaEnergy)
                    {

                        energy -= asaEnergy;
                    }
                    Rigidbody2D bulletRbo = bullet.GetComponent<Rigidbody2D>();
                    Rigidbody2D bullet2Rbo = bullet2.GetComponent<Rigidbody2D>();
                    Rigidbody2D bullet3Rbo = bullet3.GetComponent<Rigidbody2D>();

                    Vector2 baseDirection = (nearestEnemy.transform.position - transform.position).normalized;
                    Vector2 leftDirection = Quaternion.Euler(0, 0, asaBulletAngle) * baseDirection;
                    Vector2 rightDirection = Quaternion.Euler(0, 0, -asaBulletAngle) * baseDirection;

                    float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
                    float leftAngle = Mathf.Atan2(leftDirection.y, leftDirection.x) * Mathf.Rad2Deg;
                    float rightAngle = Mathf.Atan2(rightDirection.y, rightDirection.x) * Mathf.Rad2Deg;

                    bullet.transform.rotation = Quaternion.Euler(0, 0, baseAngle);
                    bullet2.transform.rotation = Quaternion.Euler(0, 0, leftAngle);
                    bullet3.transform.rotation = Quaternion.Euler(0, 0, rightAngle);

                    bulletRbo.linearVelocity = baseDirection * asaSpeed;
                    bullet2Rbo.linearVelocity = leftDirection * asaSpeed;
                    bullet3Rbo.linearVelocity = rightDirection * asaSpeed;


                }
                yield return new WaitForSeconds(asaRate*Time.fixedDeltaTime);
            }
            isFiring = false;
        }

        Transform FindChildWithTag(string tag)
        {
            Transform[] child = GetComponentsInChildren<Transform>();

            for (int i = 0; i < child.Length; i++)
            {
                if (child[i].tag == tag)
                {
                    return child[i]; //silah var
                }
            }
            return null; // silah yok
        }


    }
}
