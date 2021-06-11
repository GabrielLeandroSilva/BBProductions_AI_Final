using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager manager;
   
    private void Awake()
    {
        if (!manager)
        {
            manager = this;
        }
    }

    public float cooldown = 8f;
    public GameObject[] enemyPrefab;
    
    
    public void StartSpawn(Vector3 location)
    {
        Vector3 loc = location;
        int rd = Random.Range(0, enemyPrefab.Length);
        StartCoroutine(SpawnEnemy(enemyPrefab[rd], loc));
    }

    
    public IEnumerator SpawnEnemy(GameObject enemy, Vector3 location)
    {
        yield return new WaitForSeconds(cooldown + Random.Range(2f, 10f));
        // GetComponent<PhotonView>().RPC("Spawn", RpcTarget.AllBufferedViaServer, enemy, location);
        if(PhotonNetwork.IsMasterClient)
        {
            Spawn(enemy, location);
        } else
        {
            Spawn(enemy, location);
        }
    } 
    
    public void Spawn(GameObject enemy, Vector3 location)
    {
        Instantiate(enemy, location, Quaternion.identity);
    }

    
}
