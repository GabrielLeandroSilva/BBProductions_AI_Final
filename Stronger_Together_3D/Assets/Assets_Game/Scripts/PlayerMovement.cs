using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody rb;
    Animator anim;

    public float speed;
    public float running;

    public float startSpeed;
    public float startRunning;

    public float offSetMouseRotation;

    bool isRunning;

    public bool MasterClient;

    CinemachineVirtualCamera vcam;
    PhotonView photonView;

    [SerializeField] GameObject vcamPrefab;

    public GameObject PopUP;
    public GameObject SpotPopUp;


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
            GameObject tmpCam = Instantiate(vcamPrefab);
            this.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color32(138, 43, 226, 255);
            photonView.RPC("NamePlayer", RpcTarget.All, NetworkController.network.playerNick);

            vcam = tmpCam.GetComponent<CinemachineVirtualCamera>();
            vcam.Follow = this.gameObject.transform;
            vcam.LookAt = this.gameObject.transform;


            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startRunning = running;
            startSpeed = speed;

            GetComponentInChildren<Mask>(true).gameObject.SetActive(true);

            if(!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SyncPlayers", RpcTarget.AllBuffered);
               
            }
            
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            if (!this.gameObject.GetComponent<Player>().isDead)
            {

                if (!isRunning)
                {
                    walikng(hor, ver);
                }


                Run(hor, ver);
                RotationByMouse();
            }
        }

        
    }

    void walikng(float hor, float ver)
    {
        this.rb.AddForce(new Vector3(hor, 0, ver) * speed * Time.deltaTime);
        

        if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            anim.SetFloat("walking", ver);
        }
        else if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            anim.SetFloat("walking", ver);
        }
        else
        {
            anim.SetFloat("walking", hor + ver);
        }

    }

    private void Run(float hor, float ver)
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
            this.rb.AddForce(new Vector3(hor, 0, ver) * running * Time.deltaTime);
            anim.SetFloat("walking", 0);

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                anim.SetFloat("run", ver);
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
            {
                anim.SetFloat("run", ver);
            }
            else
            {
                anim.SetFloat("run", hor + ver);
            }

        }
        else
        {
            isRunning = false;
            anim.SetFloat("run", 0);
        }
    }

    /**
     * Metodo para realizar a rotação do personagem conforme o mouse
     */
    void RotationByMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;



        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            this.transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            this.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + offSetMouseRotation, 0);
            
        }
    }

    [PunRPC]
    void NamePlayer(string name)
    {
        this.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
    }
       

    public void NormalizeStatus()
    {
        StartCoroutine(NormalizeSpeed());
    }

    IEnumerator NormalizeSpeed()
    {
        running = 35000;
        speed = 16000;
        yield return new WaitForSeconds(5f);

        speed = startSpeed;
        running = startRunning;
    }

    public IEnumerator SpecPlayer()
    {
        yield return new WaitForSeconds(5f);
        var _mycam = vcam;
        GameObject tmpPlayer = GameObject.FindGameObjectWithTag("Player");
        if (tmpPlayer != null)
        {
            _mycam.Follow = tmpPlayer.transform;
            _mycam.LookAt = tmpPlayer.transform;
        }
    }

    [PunRPC]
    public void SyncPlayers()
    {
        GameManager.manager.lstPlayers.Add(this.gameObject);
    }

}
