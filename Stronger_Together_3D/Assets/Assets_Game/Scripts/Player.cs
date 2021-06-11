using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using TMPro;

public class Player : MonoBehaviour
{

    [SerializeField] public Slider slider_Health;
    [SerializeField] private Slider slider_Exp;
    [SerializeField] private TextMeshProUGUI txt_level;
    //public Player_SO statsSO;

    public GameObject PopUP;
    public GameObject SpotPopUp;
    public GameObject goSpeed, goDmg, goAtkSpeed;

    public GameObject levelUpFX;

    PhotonView photonView;

    Animator anim;
    public bool isDead = false;

    public float currentLife;
    public float currentExp;

    public float maxLife;
    public float maxExp;

    public int currentLevel;

    
    void Start()
    {
        photonView = this.GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            return;
        }
        else
        {

            this.maxLife = 100;
            this.slider_Health.maxValue = this.maxLife;
            this.currentLife = this.maxLife;
            this.slider_Health.value = this.currentLife;

            this.currentLevel = 1;

            this.maxExp = (this.currentLevel * 500);

            this.slider_Exp.maxValue = this.maxExp;
            this.slider_Exp.value = this.currentExp;
            txt_level.text = "Nivel: " + currentLevel.ToString();

            showPopUp("ENCONTRE TODOS TRIPULANTES E LIGUE O PAINEL CENTRAL", new Color32(255, 215, 0, 255), true);

            anim = GetComponent<Animator>();
            Invoke("showRawImage", 5f);
            Instantiate(levelUpFX, transform.position, Quaternion.Euler(Vector3.right * 90), this.gameObject.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            if (collision.collider.CompareTag("BulletEnemy"))
            {
                if (this.currentLife <= 0 && !isDead)
                {
                    isDead = true;
                    this.anim.SetBool("isDead", true);
                    photonView.RPC("Die", RpcTarget.AllBuffered);
                }
                else
                {
                    anim.SetBool("hiting", true);
                    float damage = collision.collider.GetComponent<Bullet>().damage;
                    this.currentLife -= damage;
                    showPopUp(damage, new Color32(240, 128, 128, 255));
                    this.slider_Health.value = this.currentLife;
                    Invoke("StopHit", .5f);
                }
            }
        }
    }

    public void SendExp(float exp) // +100 Exp
    {
        showPopUp("+" + exp + " Exp", new Color32(255, 140, 0, 255));
        currentExp += exp;
        slider_Exp.value = currentExp;
        if(currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currentLevel++;
        //statsSO.level = currentLevel;
        maxExp = (currentLevel * 500);
        slider_Exp.maxValue = maxExp;
        currentExp = 0;
        slider_Exp.value = 0;

        showPopUp("Level UP!!!", new Color32(255, 215, 0, 255));
        txt_level.text = "Nivel: " + currentLevel.ToString();
        iTween.PunchScale(txt_level.gameObject, Vector3.one * 1.2f, 1f);
        Instantiate(levelUpFX, transform.position, Quaternion.Euler(Vector3.right * 90), this.gameObject.transform);

        if(currentLevel == 4)
        {
            showPopUp("NOVA ARMA DESBLOQUEADA!", new Color32(255, 215, 0, 255), true);
        }
    }


    public void showPopUp(float value, Color32 color)
    {
        GameObject tmpPopUP = Instantiate(PopUP, SpotPopUp.transform.position, SpotPopUp.transform.rotation, SpotPopUp.transform);
        iTween.PunchScale(tmpPopUP, Vector3.one * 1.2f, 1f);
        iTween.MoveTo(tmpPopUP, iTween.Hash("position", new Vector3(SpotPopUp.transform.position.x + (int)Random.Range(-2, 3), SpotPopUp.transform.position.y - 2), "time", 1.5f, "easetype", "easeInBack"));
        iTween.PunchScale(this.gameObject, Vector3.one * 1.5f, 1f);
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().color = color;
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().text = value.ToString();
        Destroy(tmpPopUP, 1.5f);
    }

    public void showPopUp(string value, Color32 color)
    {
        GameObject tmpPopUP = Instantiate(PopUP, SpotPopUp.transform.position, SpotPopUp.transform.rotation, SpotPopUp.transform);
        iTween.PunchScale(tmpPopUP, Vector3.one * 1.2f, 1f);
        iTween.MoveTo(tmpPopUP, iTween.Hash("position", new Vector3(SpotPopUp.transform.position.x + (int)Random.Range(-2, 3), SpotPopUp.transform.position.y - 2), "time", 1.5f, "easetype", "easeInBack"));
        iTween.PunchScale(this.gameObject, Vector3.one * 1.5f, 1f);
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().color = color;
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().text = value;
        Destroy(tmpPopUP, 1.5f);
    }
    public void showPopUp(string value, Color32 color, bool isDestroy)
    {
        Vector3 tmpSpot = new Vector3(SpotPopUp.transform.position.x, SpotPopUp.transform.position.y + 1, SpotPopUp.transform.position.z);
        GameObject tmpPopUP = Instantiate(PopUP, tmpSpot, SpotPopUp.transform.rotation, SpotPopUp.transform);
        iTween.ShakeScale(tmpPopUP, Vector3.right/6, 1f);
      
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().color = color;
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().text = value;
       
        Destroy(tmpPopUP, 5.5f);
        
    }

    public GameObject showPopUp(string value, Color32 color, bool isDestroy, bool isReturnedValue)
    {
        Vector3 tmpSpot = new Vector3(SpotPopUp.transform.position.x, SpotPopUp.transform.position.y + 1, SpotPopUp.transform.position.z);
        GameObject tmpPopUP = Instantiate(PopUP, tmpSpot, SpotPopUp.transform.rotation, SpotPopUp.transform);
        iTween.ShakeScale(tmpPopUP, Vector3.right / 6, 1f);
      
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().color = color;
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().text = value;
        if (!isDestroy)
        {
            Destroy(tmpPopUP, 1.5f);
        }
        return tmpPopUP;
    }

    public void StopHit()
    {
        anim.SetBool("hiting", false);
    }

    void showRawImage()
    {
        this.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
    }

    [PunRPC]
    public void Die()
    {
        this.gameObject.GetComponent<PlayerMovement>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        this.gameObject.tag = "IsDead";
        GameManager.manager.indexTripulantes += GameManager.manager.parentTripulantes.GetComponentsInChildren<Tripulante>().Length;
        //GameManager.manager.tripulantesTotal = GameManager.manager.tripulantesTotal / 2;

        StartCoroutine(GetComponent<PlayerMovement>().SpecPlayer());
    }


}


[System.Serializable]
public class EventDetectEnemy : UnityEvent <GameObject> { }

public class EventArgs : MonoBehaviour
{
    public GameObject player;
}
