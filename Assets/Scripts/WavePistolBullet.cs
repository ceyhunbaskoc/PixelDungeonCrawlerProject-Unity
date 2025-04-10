using UnityEngine;

public class WavePistolBullet : MonoBehaviour
{
    public float growingSpeed,maxScale;
    

    
    private void Update()
    {
        if (transform.localScale.x < maxScale)
        {
            transform.localScale += Vector3.one * growingSpeed * Time.deltaTime;
        }

        
        transform.localScale = Vector3.Min(transform.localScale, Vector3.one * maxScale);
    }
}
