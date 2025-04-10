using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab,enemyBullet1Prefab,arrowPrefab;
    public int poolSize, enemyBulletPoolSize,bowArrowPoolSize;
    private List<GameObject> bulletPool = new List<GameObject>();
    private List<GameObject> enemyBulletPool = new List<GameObject>();
    private List<GameObject> bowArrowPool = new List<GameObject>();

    private void Start()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
        for (int i = 0; i < enemyBulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(enemyBullet1Prefab);
            bullet.SetActive(false);
            enemyBulletPool.Add(bullet);
        }
        for (int i = 0; i < bowArrowPoolSize; i++)
        {
            GameObject bullet = Instantiate(arrowPrefab);
            bullet.SetActive(false);
            bowArrowPool.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        foreach(GameObject bullet in bulletPool)
        {
            if(!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        ///Eðer tüm mermiler kullanýlýyorsa yeni mermi oluþtur.
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.SetActive(true);
        bulletPool.Add(newBullet);
        return newBullet;
    }

    public GameObject GetEnemyBullet()
    {
        foreach (GameObject bullet in enemyBulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        ///Eðer tüm mermiler kullanýlýyorsa yeni mermi oluþtur.
        GameObject newBullet = Instantiate(enemyBullet1Prefab);
        newBullet.SetActive(true);
        enemyBulletPool.Add(newBullet);
        return newBullet;
    }

    public GameObject GetBowArrow()
    {
        foreach (GameObject bullet in bowArrowPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        ///Eðer tüm mermiler kullanýlýyorsa yeni mermi oluþtur.
        GameObject newBullet = Instantiate(arrowPrefab);
        newBullet.SetActive(true);
        bowArrowPool.Add(newBullet);
        return newBullet;
    }
}

