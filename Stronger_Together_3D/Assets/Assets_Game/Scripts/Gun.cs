using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class Gun : MonoBehaviour
{
    [SerializeField] public Gun_SO itemSO;

    public GameObject player;
    public GameObject SpotBullet;
    
    float nextFire = 0;
    float buffDamage = 0;
    float buffFireRating = 0;

    PhotonView photonView;

    private void Start()
    {
        photonView = this.GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            if(!player.GetComponentInChildren<SwitchGun>().isChanging && !player.GetComponent<Player>().isDead)
            {
                if (Input.GetMouseButton(0) && Time.time > nextFire)
                {
                    nextFire = Time.time + (itemSO.gunCooldown - buffFireRating);
                    photonView.RPC("Shoot", RpcTarget.All);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    finishAnimate();
                }
            }
        }

    }

    [PunRPC]
    void Shoot()
    {
        GameObject spotTmp = this.gameObject.GetComponentInChildren<SpotGun>().gameObject;
        
        GameObject tmp = Instantiate(itemSO.prefabShoot, spotTmp.transform.position, spotTmp.transform.rotation);
        tmp.GetComponent<Rigidbody>().AddForce(transform.forward * itemSO.gunBulletSpeed);
        tmp.GetComponent<Bullet>().damage = Mathf.RoundToInt(itemSO.gunDmg + buffDamage + player.GetComponent<Player>().currentLevel);
        tmp.GetComponent<MeshRenderer>().materials[0] = itemSO.MeshGun;

        Destroy(tmp, 2f);

        if(!player.GetComponent<Animator>().GetBool("shoot"))
        {
            player.GetComponent<Animator>().SetBool("shoot", true);
            
        }
    }    

    void finishAnimate()
    {
        player.GetComponent<Animator>().SetBool("shoot", false);
    }

    public void BuffDamageGun()
    {
        StartCoroutine(NormalizedDamage());
    }
    

    IEnumerator NormalizedDamage()
    {
        buffDamage = itemSO.gunDmg * 0.3f;
        yield return new WaitForSeconds(10f);
        buffDamage = 0;
        
    }

    public void BuffAttackSpeedGun()
    {
        StartCoroutine(NormalizedFireRating());
    }


    IEnumerator NormalizedFireRating()
    {
        buffFireRating = itemSO.gunCooldown * 0.3f;
        yield return new WaitForSeconds(10f);
        buffFireRating = 0;

    }

}
