using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] private Flashlight_SO itemSO;
    [SerializeField] private GameObject prefabHit;

    [Header("Player")]
    [SerializeField] private GameObject player;

    float nextFire = 0;
    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + itemSO.flCooldown;
            Shoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            finishAnimate();
        }
    }
    void Shoot()
    {
        GameObject tmp = Instantiate(prefabHit, transform.position, Quaternion.identity);        

        Destroy(tmp, .2f);

        if (!player.GetComponent<Animator>().GetBool("shoot"))
        {
            player.GetComponent<Animator>().SetBool("shoot", true);

        }
    }

    void finishAnimate()
    {
        player.GetComponent<Animator>().SetBool("shoot", false);
    }
}
