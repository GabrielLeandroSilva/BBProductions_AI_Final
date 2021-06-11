using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveMenu : MonoBehaviour
{

    public float speedCamera = 100;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(99999, transform.position.y, transform.position.z), speedCamera * Time.deltaTime);
    }
}
