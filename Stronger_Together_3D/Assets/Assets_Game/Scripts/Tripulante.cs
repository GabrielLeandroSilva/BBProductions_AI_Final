using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class Tripulante : MonoBehaviour
{
    NavMeshAgent nav;
    public GameObject goal, mark, filed;

    public GameObject saveFx;
    public float speed = 1;
    bool isSaving = false;
    bool isVictory = false;
    public float goalZ;

    

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        StartSaving();
    }

    private void FixedUpdate()
    {
        if(isSaving)
        {
            CurrentGoal();
        }
        
        if(!isVictory)
        {
            if(Vector3.Distance(transform.position, GameManager.manager.ExitTripulante.transform.position) < 5)
            {
                isVictory = true;
                GetComponent<Animator>().SetTrigger("victory");
                Destroy(this.gameObject, 4f);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSaving)
        {
            if (other.CompareTag("Player"))
            {
                filed.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        
            if (other.CompareTag("Player"))
            {
                filed.SetActive(false);
            }
        
        
    }


    public void CurrentGoal()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Mathf.Clamp(mark.transform.localEulerAngles.z, goal.transform.localEulerAngles.z - 60, goal.transform.localEulerAngles.z ) == mark.transform.localEulerAngles.z)
            {
                iTween.PunchScale(GetComponentInChildren<PopupLook>().gameObject, Vector3.one * .06f, 1.2f);
                GameManager.manager.players[0].GetComponent<Player>().SendExp(150);
                this.gameObject.GetComponent<AudioSource>().Play();
                Instantiate(saveFx, transform.position, Quaternion.identity, this.gameObject.transform);
                if (GameManager.manager.players.Length > 1)
                {
                    GameManager.manager.players[1].GetComponent<Player>().SendExp(150);
                }
                GameManager.manager.GetComponent<PhotonView>().RPC("AddPontos", RpcTarget.AllBufferedViaServer);
                isSaving = false;
                GetComponent<Animator>().SetTrigger("run");
                GetComponentInChildren<Light>().color = Color.green;
                nav.SetDestination(GameManager.manager.ExitTripulante.transform.position);
            } else
            {
                StartSaving();
            }
        }
    }

    

    
    public void StartSaving()
    {
        goalZ = Random.Range(0, -361);
        isSaving = true;
        goal.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, goalZ));
        iTween.RotateAdd(mark, iTween.Hash("amount", Vector3.forward * -360, "time", 4f, "looptype", "loop", "easetype", "linear"));
    }

}
