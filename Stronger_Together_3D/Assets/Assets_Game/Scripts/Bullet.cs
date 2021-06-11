using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if(this.gameObject.tag == "BulletEnemy" && collision.collider.CompareTag("Enemy"))
        {
            return;
        }
        Destroy(this.gameObject);
    }


   
}
