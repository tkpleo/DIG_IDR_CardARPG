using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Rigidbody rb;
    private Bullet bulletStats;
    public float bulletSpeed = 10f;
    public int bulletDamage = 1;
    public bool isStunBullet = false;
    public bool isAOEBullet = false;
    public bool isSlowBullet = false;

    // Update is called once per frame
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletStats = GetComponent<Bullet>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit an Enemy. Damage dealt: " + bulletDamage);

            ObjectPoolManager.ReturnObjectToPool(gameObject);
            collidedObject.GetComponent<EnemyBehavior>().TakeDamage(bulletStats);
            ResetStats();
        }
        else 
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void ResetStats()
    {
        bulletSpeed = 10f;
        bulletDamage = 1;
        isStunBullet = false;
        isAOEBullet = false;
    }

    private void OnEnable()
    {
        
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.fixedDeltaTime);
    }
}
