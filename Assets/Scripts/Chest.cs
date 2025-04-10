using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool isOpenable,open=false;
    public GameObject asaPrefab, bowPrefab, pistolPrefab;
    SpriteRenderer sr;

    private void Start()
    {
        sr= GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if(isOpenable&&Input.GetKeyDown(KeyCode.E)&&!open)
        {
            int random = Random.Range(0, 2);
            if(random==0)
            {
                Instantiate(asaPrefab,transform.position, Quaternion.identity);
            }
            else if (random == 1)
            {
                Instantiate(bowPrefab, transform.position, Quaternion.identity);
            }
            else if (random == 2)
            {
                Instantiate(pistolPrefab, transform.position, Quaternion.identity);
            }
            sr.color = Color.blue;
            open = true;
            
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isOpenable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isOpenable = false;
        }
    }
}
