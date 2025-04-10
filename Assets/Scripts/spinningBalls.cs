using System.Collections;
using UnityEngine;

public class spinningBalls : MonoBehaviour
{
    public GameObject spinningBallsPrefab;
    public bool canCreate = true;
    public float coolDown,spinSpeed,spinningBallTime;
    private GameObject balls;
    private void Update()
    {
        if(canCreate&&Input.GetKeyUp(KeyCode.B))
        {
            StartCoroutine(CreateBalls());
        }
        if(GameObject.FindGameObjectWithTag("spinningBalls")==true)
        {
            balls.transform.position=transform.position;
            balls.transform.eulerAngles += new Vector3(0,0,spinSpeed);
        }
    }
    IEnumerator CreateBalls()
    {
        canCreate = false;
        balls=Instantiate(spinningBallsPrefab,transform.position,Quaternion.identity);

        yield return new WaitForSeconds(spinningBallTime);
        Destroy(balls);
        StartCoroutine(CoolDown());
    }
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDown);
        canCreate = true;
    }
}
