using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyShooting_SO : ScriptableObject
{
    public GameObject prefabShoot;
    public float gunDmg;
    public float gunBulletSpeed;
    public float gunCooldown = 0.5f;
}
