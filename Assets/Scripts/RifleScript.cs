using UnityEngine;

public class RifleScript : MonoBehaviour
{
    public GameObject handPosition;
    private bool isPickable = false;

    private void Start()
    {
        handPosition = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if(FindChildWithTag()==null)
        {

        if (isPickable && Input.GetKeyDown(KeyCode.E))
        {
            PickUpGun();
        }
        }
        else if(FindChildWithTag()!=null)
        {
            
            if (isPickable && Input.GetKeyDown(KeyCode.E))
            {
                deleteRifle();
                PickUpGun();
            }
        }
    }

    private void PickUpGun()
    {
        transform.SetParent(handPosition.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPickable = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPickable = false;

        }
    }
    Transform FindChildWithTag()
    {
        Transform[] child =handPosition.GetComponentsInChildren<Transform>();

        for(int i = 0; i < child.Length; i++)
        {
            if (child[i].tag =="pistol"|| child[i].tag == "bow"|| child[i].tag == "asa")
            {
                return child[i]; //silah var
            }
        }
        return null; // silah yok
    }
    void deleteRifle()
    {
        Transform child = FindChildWithTag();
        child.SetParent(null);
    }
}
