using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;
public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        //print("2 saniye baþladý");
        yield return new WaitForSeconds(2f);
        navMeshSurface.BuildNavMeshAsync();
        //print("NAVMESH OLUÞTU");

    }
}
