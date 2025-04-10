using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    
    public LayerMask ignoreLayersPlayer,ignoreLayersEnemy;
    private LayerMask ignoreLayers;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        if(LayerMask.LayerToName(gameObject.layer)=="enemyBullet")
        {
            ignoreLayers = ignoreLayersEnemy;
            print("düþman mermisi");
        }
        else if (LayerMask.LayerToName(gameObject.layer)=="bullet")
        {
            ignoreLayers=ignoreLayersPlayer;
            print("karakter mermisi");
        }
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & ignoreLayers) == 0)
        {
            //print(collision.gameObject.name);
            rb.linearVelocity = new Vector2(0,0);
            animator.SetBool("Explosion", true);
        }
    }

    public void SetActiveFalse()
    {
        animator.SetBool("Explosion", false);
        gameObject.SetActive(false);
    }
}
