using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SwitchGun : MonoBehaviour
{
    [Header("Armas")]
    [SerializeField]GameObject firstGun;
    [SerializeField]GameObject secondGun;

    [SerializeField]int selectWeapon = 0;

    [Header("PLAYER")]
    [SerializeField] GameObject Player;

    public bool isChanging = false;
    public bool LockingShoot = false;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = this.GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            this.selectWeapon = 0;
            photonView.RPC("SelectGun", RpcTarget.All, this.selectWeapon);
            //SelectGun();
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            if (!Player.GetComponent<Player>().isDead)
            {

                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetAxis("Mouse ScrollWheel") > 0f)
                {

                    if (!isChanging)
                    {
                        this.isChanging = true;
                        Player.GetComponent<Animator>().SetBool("changeweapon", true);
                        this.selectWeapon = 0;
                        //photonView.RPC("SelectGun", RpcTarget.All, this.selectWeapon);
                        StartCoroutine(TimerWeapon());
                    }

                }

                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    if (!isChanging && GetComponentInParent<Player>().currentLevel >= 4)
                    {
                        this.isChanging = true;
                        Player.GetComponent<Animator>().SetBool("changeweapon", true);
                        this.selectWeapon = 1;
                        // photonView.RPC("SelectGun", RpcTarget.All, this.selectWeapon);
                        StartCoroutine(TimerWeapon());
                    }
                }

                else
                {
                    Player.GetComponent<Animator>().SetBool("changeweapon", false);
                }
            }

        }


    }

    IEnumerator TimerWeapon()
    {
        yield return new WaitForSeconds(.7f);
        photonView.RPC("SelectGun", RpcTarget.All, this.selectWeapon);
    }

    /**
     * Metodo para selecionar a arma para o jogador
     */
     [PunRPC]
    void SelectGun(int index)
    {
        this.isChanging = false;
        if (index == 0)
        {
            firstGun.SetActive(true);
            secondGun.SetActive(false);
        }
        else
        {
            firstGun.SetActive(false);
            secondGun.SetActive(true);
        }
    }
}
