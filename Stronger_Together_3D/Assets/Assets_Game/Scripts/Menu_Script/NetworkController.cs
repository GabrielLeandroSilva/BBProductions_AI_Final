using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController network;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (!network)
        {
            network = this;
        }
    }



    [Header("LOGIN")]
    public TMP_InputField playerNome;
    string tmpNomePlayer;
    public string playerNick;


    [Header("LOBBY")]
    public TMP_InputField roomNome;
    string tmpRoomNome;

    [Header("PAINEIS")]
    public GameObject Painel_central;
    public GameObject Painel_login;
    public GameObject Painel_lobby;
    public GameObject Painel_NomePlayer;
    public GameObject Painel_NomeRoom;
    public TextMeshProUGUI[] lst_txt_salas;

    [Header("Player")]
    public GameObject PlayerMultiplayer;
    public GameObject PlayerSpawn;


    PhotonView photonView1;
    bool isReset;
    bool isResetWinner;
    // Start is called before the first frame update
    void Start()
    {
        Painel_central.SetActive(true);
        photonView1 = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
        PhotonNetwork.ConnectUsingSettings();
        if(playerNome.text != "")
        {
            PhotonNetwork.NickName = playerNome.text;
        }
        else
        {
            PhotonNetwork.NickName = tmpNomePlayer;
        }

        Painel_central.SetActive(false);
        Painel_lobby.SetActive(true);
        Painel_login.SetActive(false);
        Painel_NomePlayer.SetActive(false);
        Painel_NomeRoom.SetActive(false);

    }

    public void QuickSearch()
    {
        PhotonNetwork.JoinRandomRoom();
        
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom(roomNome.text, roomOptions, TypedLobby.Default);

        photonView.RPC("SetRoomList", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void SetRoomList()
    {
        foreach (var item in lst_txt_salas)
        {
            if(item.text != string.Empty)
            {
                item.text = roomNome.text;
                print(item.text);
                return;
            }
        }
    }

    public void PlaySingle()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        StartCoroutine(StartScene());
    }

    public void IniciarGame()
    {
        btnPaineisVoltar(2);
    }

    public void IniciarLogin()
    {
        Painel_central.SetActive(false);
        Painel_lobby.SetActive(false);
        Painel_login.SetActive(false);
        Painel_NomePlayer.SetActive(true);
        Painel_NomeRoom.SetActive(false);

        tmpRoomNome = "Planeta" + Random.Range(0, 150);
        roomNome.text = tmpRoomNome;

        tmpNomePlayer = "Tripulante" + Random.Range(0, 1000);
        playerNome.text = tmpNomePlayer;
    }

    public void IniciarCriacaoSala()
    {
        Painel_central.SetActive(false);
        Painel_lobby.SetActive(false);
        Painel_login.SetActive(false);
        Painel_NomePlayer.SetActive(false);
        Painel_NomeRoom.SetActive(true);

        
        
    }

    public void btnPaineisVoltar(int value)
    {
        switch(value)
        {
            case 1:
                {
                    Painel_central.SetActive(true);
                    Painel_lobby.SetActive(false);
                    Painel_login.SetActive(false);
                    Painel_NomePlayer.SetActive(false);
                    Painel_NomeRoom.SetActive(false);
                    break;
                }
            case 2:
                {
                    Painel_central.SetActive(false);
                    Painel_lobby.SetActive(false);
                    Painel_login.SetActive(true);
                    Painel_NomePlayer.SetActive(false);
                    Painel_NomeRoom.SetActive(false);
                    break;
                }
            case 3:
                {
                    Painel_central.SetActive(false);
                    Painel_lobby.SetActive(false);
                    Painel_login.SetActive(false);
                    Painel_NomePlayer.SetActive(true);
                    Painel_NomeRoom.SetActive(false);

                    PhotonNetwork.Disconnect();
                    break;
                }
            case 4:
                {
                    Painel_central.SetActive(false);
                    Painel_lobby.SetActive(true);
                    Painel_login.SetActive(false);
                    Painel_NomePlayer.SetActive(false);
                    Painel_NomeRoom.SetActive(false);
                    break;
                }
        }
       
    }


    //############# PUN Callbacks ##################

    public override void OnConnected()
    {
        Invoke("LobbyWaiting", 1f);
    }


    void LobbyWaiting()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnectedToMaster()
    {
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
        for (int i = 0; i < roomList.Count; i++)
        {
            lst_txt_salas[i].text = roomList[i].Name;
        }
        

    }



    public override void OnJoinedLobby()
    {
        print("OnJoinedLooby");

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(tmpRoomNome);
        
    }

    public override void OnJoinedRoom()
    {
        
        SceneManager.LoadScene("MainScene");

        StartCoroutine(StartScene());
    }

     IEnumerator StartScene()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Procurando Cena");

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            playerNick = playerNome.text;
            StopCoroutine(StartScene());
            StartCoroutine(CheckPlayerScene());
        }
        else
        {
            StartCoroutine(StartScene());
        }
      
    }


    IEnumerator CheckPlayerScene()
    {
        yield return new WaitForSeconds(1f);
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.Instantiate(PlayerMultiplayer.name, new Vector3(PlayerSpawn.transform.position.x + Random.Range(-2,2), PlayerSpawn.transform.position.y, PlayerSpawn.transform.position.z + Random.Range(-2, 2)), PlayerMultiplayer.transform.rotation, 0);
            StartCoroutine(GameManager.manager.HiddenCanvas());
            GameObject tmpPainel = GameObject.Find("pnWaiting");
            tmpPainel.SetActive(false);
            StopCoroutine(CheckPlayerScene());
            InvokeRepeating("GamerOver", 10f, 2f);
        }
        else
        {
            StartCoroutine(CheckPlayerScene());

        }
    }

    public void GamerOver()
    {
        GameObject[] tmpList = GameObject.FindGameObjectsWithTag("IsDead");
        GameObject[] winnerList = GameObject.FindGameObjectsWithTag("Finish");
        if (tmpList.Length == 2 || isReset)
        {
            print("Foi game over");

            photonView.RPC("ResetGame", RpcTarget.AllViaServer);
            print("GameOver");
            
            GameManager.manager.players[0].GetComponent<Player>().showPopUp("MISSÃO FRACASSADA...", Color.red, true);
            if (GameManager.manager.players.Length > 1)
            {
                GameManager.manager.players[1].GetComponent<Player>().showPopUp("MISSÃO FRACASSADA...", Color.red, true);
            }
        }
        if (winnerList.Length == 2 || isResetWinner || winnerList.Length == 1 && tmpList.Length == 1)
        {
            print("Reset Winners");

            photonView.RPC("ResetGameWinner", RpcTarget.AllViaServer);          

        }

    }

    [PunRPC]
    void ResetGame()
    {
        isReset = true;
        StartCoroutine(LeaveGame());
    }
    [PunRPC]
    void ResetGameWinner()
    {
        isResetWinner = true;
        StartCoroutine(LeaveGame());
    }

    IEnumerator LeaveGame()
    {
        yield return new WaitForSeconds(6f);
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        Destroy(this.gameObject);
        
        SceneManager.LoadScene("MenuScene");
    }

}
