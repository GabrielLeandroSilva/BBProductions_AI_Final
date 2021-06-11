using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRadius : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().EventDetect.Invoke(GetComponentInParent<Player>().gameObject);
        }
    }
}
