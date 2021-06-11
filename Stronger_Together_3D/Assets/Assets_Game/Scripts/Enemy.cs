using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;


//Maquina de estados: { PATROL = Patrulha, PERSUIT = perseguir, ATTACK = Ataque, FLEE = Fuga, DIE = Morte }
public enum EnumBehaviours
{
    PATROL,
    PERSUIT,
    ATTACK,
    FLEE,
    DIE
}

public class Enemy : MonoBehaviour
{
    //Variavel para utilizar a maquina de estados
    public EnumBehaviours enumBehaviours;

    //Movimentação do inimigo
    NavMeshAgent nav;

    //Evento da animação de ataque
    public EventDetectEnemy EventDetect;

    //Prefab para exibir texto no canvas
    public GameObject PopUP;

    //Posição do objeto para exibir texto
    public GameObject SpotPopUp;

    //Scriptable object para o dano, range e velocidade de movimento
    [SerializeField] private EnemyShooting_SO itemSO;
    [SerializeField] private Enemy_SO enemySO;

    //Prefab do tiro do inimigo
    public GameObject SpotBullet;

    //Prefab Fx
    public GameObject DeathFx;
    public GameObject SpawnFx;

    //Objeto de referencia do player
    GameObject tmpPlayer;
    //Variavel para animação do inimigo
    Animator anim;

    //Variavel de photon view para multiplayer
    PhotonView photonView;

    //Flag de estados (auxiliar)
    bool isDead = false;
    bool isSpot = false;
    bool isFlee = false;

    //Posição inicial do inimigo
    Vector3 startPosition;
    //Ponto para o novo destino de navegação
    Vector3 newPosition;
    
    //Tempo para o proximo tiro
    float nextFire;

    //Vida maxima do inimigo
    float lifeMax;

    //Vida atual do inimigo
    public float currentLife;


    void Start()
    {
        photonView = this.GetComponent<PhotonView>();

        //Metodo para instanciar o evento da animação
        if (EventDetect == null)
        {
            EventDetect = new EventDetectEnemy();
        }

        //Obter os parametros do navmesh
        nav = GetComponent<NavMeshAgent>();
        //Obter os parametros do animator
        anim = GetComponent<Animator>();

        nav.speed = enemySO.enemySpeed;

        lifeMax = enemySO.enemyLifeMax;
        currentLife = lifeMax;

        //Escutar o evento de ataque
        EventDetect.AddListener(DetectPlayer);
        enumBehaviours = EnumBehaviours.PATROL;

        startPosition = transform.position;
        StartCoroutine(Patrol());

        Instantiate(SpawnFx, transform.position, Quaternion.Euler(Vector3.right * 90), this.gameObject.transform);
    }


    void Update()
    {
        // metodo de gerenciamento de estado (Maquina de Estado)
        switch(enumBehaviours)
        {
            case EnumBehaviours.PERSUIT: //Estado de perseguição, se o player aproximar ao certo Range, o Inimigo começa a perseguir
                {
                    
                    if (Vector3.Distance(this.transform.position, tmpPlayer.transform.position) < 10)
                    {
                        if(!isSpot)
                        {
                            isSpot = true;
                            showPopUp("!");
                        }

                        //O destino do inimigo é a posição do player
                        nav.SetDestination(tmpPlayer.transform.position);
                        if (Vector3.Distance(this.transform.position, tmpPlayer.transform.position) <= enemySO.enemyAttackRange)
                        {
                            enumBehaviours = EnumBehaviours.ATTACK;
                        }
                    }
                    else
                    {
                        //Caso o player saia do campo de visão, o inimigo volta para o estado de patrulha
                        isSpot = false;
                        enumBehaviours = EnumBehaviours.PATROL;
                        anim.SetBool("walking", true);
                        StartCoroutine(Patrol());
                    }
                    break;
                }

            case EnumBehaviours.ATTACK:  //Estado de ataque, ao chegar proximo do player, o inimigo inicia a animação e através de um evento executa o ataque
                {
                    if (!isDead)
                    {
                        //Caso o Player seja abatido, o inimigo volta para o estado de patrulha
                        if(tmpPlayer.GetComponent<Player>().isDead)
                        {
                            enumBehaviours = EnumBehaviours.PATROL;
                            anim.SetBool("attack", false);
                            return;
                        }

                        //Rotaciona em direção ao player e executa o ataque
                        anim.SetBool("attack", true);
                        Vector3 pointToLook = tmpPlayer.transform.position;
                        this.transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));

                        //Tempo de cooldown entre ataques
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + itemSO.gunCooldown;
                        }

                        //Caso o player fique longe do Range de ataque, o inimigo volta para o estagio de perseguição
                        if (Vector3.Distance(this.transform.position, tmpPlayer.transform.position) >= enemySO.enemyAttackRange)
                        {
                            enumBehaviours = EnumBehaviours.PERSUIT;
                            anim.SetBool("attack", false);
                        }

                    }
                    break;
                }


            case EnumBehaviours.PATROL: //Estado de Patrulha, inimigo recebe pontos aleatorios dentro de um perimetro para percorrer
                {
                    anim.SetBool("walking", true);
                    nav.SetDestination(newPosition);
                    break;
                }

            case EnumBehaviours.FLEE: // Estado de Fuga, Quando estiver em fuga, o inimigo recebera o destino para ficar longe do player (Repelir)
                {
                    
                    nav.SetDestination(-tmpPlayer.transform.position * 2);
                    if (Vector3.Distance(this.transform.position, tmpPlayer.transform.position) > 15)
                    {
                        enumBehaviours = EnumBehaviours.PATROL;
                        StartCoroutine(Patrol());
                    }

                        break;
                }

            case EnumBehaviours.DIE: //Estado de morte, ao ser abatido, o inimigo envia experiencia para os jogadores e pode deixar power ups (chance aleatoria)
                {
                    nav.isStopped = true;

                    if (!isDead)
                    {
                        isDead = true;
                        anim.SetBool("attack", false);
                        Instantiate(DeathFx, transform.position, Quaternion.identity);
                        this.enabled = false;
                        GameManager.manager.LootEnemy(this.transform);
                        GameManager.manager.players[0].GetComponent<Player>().SendExp(100);
                        if(GameManager.manager.players.Length > 1)
                        {
                            GameManager.manager.players[1].GetComponent<Player>().SendExp(100);
                        }

                       EnemyManager.manager.StartSpawn(startPosition);
                       this.gameObject.GetComponent<AudioSource>().Play();
                       Destroy(this.gameObject, .5f);
                    }

                    break;
                }
        }

       

    }

    //Metodo para obter o evento da animação e executar o ataque
    public void testAttack()
    {
        StartCoroutine(Attack());
    }

    
    //Metodo para detectação de players no jogo
    public void DetectPlayer(GameObject player)
    {
        enumBehaviours = EnumBehaviours.PERSUIT;
        tmpPlayer = player;
        StopCoroutine(Patrol());
    }

    
    //Metodo para geração de pontos para o estado de patrulha
    IEnumerator Patrol()
    {
        yield return new WaitForSeconds(1f);
        float rdx = Random.Range(-5, 5);
        float rdz = Random.Range(-5, 5);
        Vector3 newPos = new Vector3(startPosition.x + rdx, startPosition.y, startPosition.z + rdz);
        newPosition = newPos;
        StartCoroutine(Patrol());
    }

    //Metodo para instanciar o ataque no player (Bullet)
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0);
        GameObject spotTmp = this.gameObject.GetComponentInChildren<SpotGun>().gameObject;

        GameObject tmp = Instantiate(itemSO.prefabShoot, spotTmp.transform.position, spotTmp.transform.rotation);
        tmp.GetComponent<Rigidbody>().AddForce(transform.forward * itemSO.gunBulletSpeed);
        Destroy(tmp, 2f);
                       
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Se houver colisão com o tiro do player, será removido pontos de vida
        if(collision.collider.CompareTag("Bullet")) {
            float damage = collision.collider.GetComponent<Bullet>().damage + (int)Random.Range(-2, 3);

            currentLife -= damage;

            //Exibir o dano recebido
            if(!enumBehaviours.Equals(EnumBehaviours.DIE))
            {
                showPopUp(damage);
            }
             
            //Verifica se a vida foi zerada ou está negativa, para a troca de estado para morte
            if (currentLife <= 0)
            {
                enumBehaviours = EnumBehaviours.DIE;
            }

            //Change de mudar o estado para fuga quando atingir em 20% da vida
            else if(currentLife <= (lifeMax * .2f))
            {
                
                float changeFlee = Random.Range(0, 4);
                if(changeFlee == 0 && !isFlee)
                {
                    enumBehaviours = EnumBehaviours.FLEE;
                    showPopUp("Arrh");
                    isFlee = true;
                }

            }

        }

        
    }

    //Metodo para exibir no canvas texto/valor
    public void showPopUp(float value)
    {
        GameObject tmpPopUP = Instantiate(PopUP, SpotPopUp.transform.position, SpotPopUp.transform.rotation, SpotPopUp.transform);
        iTween.PunchScale(tmpPopUP, Vector3.one * 1.2f, 1f);
        iTween.MoveTo(tmpPopUP, iTween.Hash("position", new Vector3(SpotPopUp.transform.position.x + (int)Random.Range(-2, 3), SpotPopUp.transform.position.y - 2), "time", 1.5f, "easetype", "easeInBack"));
        iTween.PunchScale(this.gameObject, Vector3.one * 1.5f, 1f);

        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().text = value.ToString();
        Destroy(tmpPopUP, 1.5f);
    }


    public void showPopUp(string value)
    {
        GameObject tmpPopUP = Instantiate(PopUP, SpotPopUp.transform.position, SpotPopUp.transform.rotation, SpotPopUp.transform);

        iTween.PunchScale(tmpPopUP, Vector3.one * 1.2f, 1f);
        iTween.MoveTo(tmpPopUP, iTween.Hash("position", new Vector3(SpotPopUp.transform.position.x, SpotPopUp.transform.position.y - 2), "time", 1.5f, "easetype", "easeInBack"));

        iTween.PunchScale(this.gameObject, Vector3.one * 1.5f, 1f);

        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().text = value.ToString();
        tmpPopUP.GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(200, 0, 0, 255);
        Destroy(tmpPopUP, 1.5f);
    }

  



}
