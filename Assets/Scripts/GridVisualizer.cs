using UnityEngine;

public class GridVisualizer : MonoBehaviour
{

    public int width = 10;  // Grid geni�li�i
    public int height = 10; // Grid y�ksekli�i
    public float cellSize = 1f; // H�cre boyutu

    void Update()
    {
        OnDrawGizmos();
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Dikey �izgileri �iz
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, height * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Yatay �izgileri �iz
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(width * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
