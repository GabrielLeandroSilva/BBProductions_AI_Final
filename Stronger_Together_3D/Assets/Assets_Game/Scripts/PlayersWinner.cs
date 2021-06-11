using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersWinner : MonoBehaviour
{

    public bool isReady;

    private void Start()
    {
        isReady = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(isReady)
            {

            other.tag = "Finish";
            other.GetComponent<Player>().showPopUp("FINALIZANDO A MISSÃO... RETORNANDO PARA CASA", Color.green, true, true);
            }
        }
    }
}
