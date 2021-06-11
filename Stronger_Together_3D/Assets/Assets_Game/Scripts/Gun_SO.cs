using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Gun_SO : ScriptableObject
{
    public GameObject prefabShoot;
    public Material MeshGun;
    public float gunDmg;
    public float gunBulletSpeed;
    public float gunCooldown = 0.5f;
}
