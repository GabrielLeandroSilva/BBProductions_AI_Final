using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{

    public float eixoX;
    public float eixoY;
    public float eixoZ;
    public float speed = 1;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(eixoX, eixoY, eixoZ) * Time.deltaTime * speed);
    }
}
