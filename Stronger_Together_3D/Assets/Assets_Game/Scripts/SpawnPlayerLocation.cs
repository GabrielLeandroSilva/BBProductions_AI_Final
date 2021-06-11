using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(1, 5), transform.position.y, transform.position.z);
    }

   
}
