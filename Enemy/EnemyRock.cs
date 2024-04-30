using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRock : MonoBehaviour
{
    private void Update()
    {
        DestroyObjectDelayed();
    }

    void DestroyObjectDelayed()
    {
        Destroy(gameObject, 3);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }

        Destroy(gameObject, 0);
    }
}
