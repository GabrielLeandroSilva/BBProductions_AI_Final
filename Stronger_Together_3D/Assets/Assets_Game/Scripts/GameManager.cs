using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    private void Awake()
    {
        if (!manager)
        {
            manager = this;
        }
    }

    public GameObject[] players;
    public GameObject[] Loots;
    public GameObject MasterPlayer;
    public GameObject ExitTripulante;
    public GameObject NetWorkSystem;

    public GameObject PainelReturnedHome;
    public GameObject PainelCentralActive;
    public GameObject LuzesAlarm;

    PhotonView photonView;

    public List<Transform> spawnTripulantes;
    public List<GameObject> lstPlayers;
    public GameObject parentTripulantes;    

    GameObject goSpeed, goDmg, goAtkSpeed;
    bool isSpeedUp, isDmgUp, isAtkSpeedUp;
    bool isReadyGame;
    bool isReset = false;

    float timeElapsedSpeed = 0, timeElapsedDMG = 0, timeElapsedATKFire = 0;
    public int indexDeathPlayers;
    public int indexTripulantes = 0;
    public int tripulantesTotal;

    private void Start()
    {
        tripulantesTotal = parentTripulantes.GetComponentsInChildren<Tripulante>().Length * 2;
    }

    private void Update()
    {
        if(players.Length > 1)
        {
            if(!isReadyGame)
            {
                isReadyGame = true;
                SetStartGame();
            }
        }
    }
    private void FixedUpdate()
    {
        if(isSpeedUp)
        {
            if (timeElapsedSpeed < 5)
            {
                goSpeed.GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, timeElapsedSpeed / 5);
                timeElapsedSpeed += Time.deltaTime;
            }
           // goSpeed.GetComponent<Image>().fillAmount = Mathf.Lerp(goSpeed.GetComponent<Image>().fillAmount, 1, Time.deltaTime / 5);
           else
            {
                goSpeed.GetComponent<Image>().fillAmount = 0;
                timeElapsedSpeed = 0;
                isSpeedUp = false;
            }
        }
        if (isDmgUp)
        {
            if (timeElapsedDMG < 10)
            {
                goDmg.GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, timeElapsedDMG / 10);
                timeElapsedDMG += Time.deltaTime;
            }
            else
            {
                goDmg.GetComponent<Image>().fillAmount = 0;
                timeElapsedDMG = 0;
                isDmgUp = false;
            }
            //  goDmg.GetComponent<Image>().fillAmount = Mathf.Lerp(goDmg.GetComponent<Image>().fillAmount, 1, Time.deltaTime / 10);
        }
        if (isAtkSpeedUp)
        {
            if (timeElapsedATKFire < 10)
            {
                goAtkSpeed.GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, timeElapsedATKFire / 10);
                timeElapsedATKFire += Time.deltaTime;
            }
            else
            {
                goAtkSpeed.GetComponent<Image>().fillAmount = 0;
                timeElapsedATKFire = 0;
                isAtkSpeedUp = false;
            }
            // goAtkSpeed.GetComponent<Image>().fillAmount = Mathf.Lerp(goAtkSpeed.GetComponent<Image>().fillAmount, 1, Time.deltaTime / 10);
        }
    }

    void WaitForFind()
    {
        goSpeed = players[0].GetComponent<Player>().goSpeed;
        goDmg = players[0].GetComponent<Player>().goDmg;
        goAtkSpeed = players[0].GetComponent<Player>().goAtkSpeed;
    }

    void SetStartGame()
    {
        photonView = this.GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            Invoke("WaitForFind", 3f);
        }

    }

    public IEnumerator HiddenCanvas()
    {
        yield return new WaitForSeconds(6f);
        players = GameObject.FindGameObjectsWithTag("Player");

        NetWorkSystem = GameObject.Find("NetworkManager");

    }



    public void LootEnemy(Transform lootPosition)
    {
        print("Lootenemy chamou");

        int rdChance = Random.Range(0, 10);

        if(rdChance >= 3)
        {
            int rd = Random.Range(0, Loots.Length);
            GameObject tmp = Instantiate(Loots[rd], lootPosition.position, Quaternion.identity);
            iTween.RotateAdd(tmp, iTween.Hash("amount", Vector3.up * 360, "time", 2, "looptype", "loop", "easetype", "linear"));
            Vector3 newPos = new Vector3(tmp.transform.position.x, tmp.transform.position.y + 1f, tmp.transform.position.z);
            iTween.MoveTo(tmp, iTween.Hash("position", newPos, "time", 1.5f, "easetype", "easeOutBack"));
            iTween.ScaleTo(tmp, iTween.Hash("scale", Vector3.one * 3.6f, "time", 1.5f, "looptype", "pingPong", "easetype", "linear"));
        }
    }

    public static void ShuffleList<T>(IList<T> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    public void ShowOnUI(string tagUI)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            switch (tagUI)
            {
                case "SPEED":
                    {
                        isSpeedUp = true;
                        goSpeed.GetComponent<Image>().fillAmount = 0;
                        timeElapsedSpeed = 0;
                        break;
                    }

                case "DAMAGE":
                    {
                        isDmgUp = true;
                        goDmg.GetComponent<Image>().fillAmount = 0;
                        timeElapsedDMG = 0;
                        break;
                    }

                case "ATKSPEED":
                    {
                        isAtkSpeedUp = true;
                        goAtkSpeed.GetComponent<Image>().fillAmount = 0;
                        timeElapsedATKFire = 0;
                        break;
                    }
            }
        }

    }

    [PunRPC]
    void AddPontos()
    {
        indexTripulantes++;
        if (indexTripulantes >= tripulantesTotal)
        {
            print("Finish!!");
            players[0].GetComponent<Player>().showPopUp("ENCONTRE O PAINEL CENTRAL E ATIVE, PARA ESCAPAR", Color.green, true);
            if (players.Length > 1)
            {
                players[1].GetComponent<Player>().showPopUp("ENCONTRE O PAINEL CENTRAL E ATIVE, PARA ESCAPAR", Color.green, true);
            }

            PainelCentralActive.GetComponent<PainelCentral>().enabled = true;

        }
    }

    IEnumerator ResetStatusUI(float timer, bool status, GameObject tmpUI)
    {
        yield return new WaitForSeconds(timer);
        status = false;
        tmpUI.GetComponent<Image>().fillAmount = 0;
    }


}
