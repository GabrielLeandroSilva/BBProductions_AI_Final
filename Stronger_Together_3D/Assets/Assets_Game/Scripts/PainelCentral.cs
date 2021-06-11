using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainelCentral : MonoBehaviour
{
    bool isEnabled, isPressed;
    GameObject tmp, tmpText;

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.E) && isEnabled)
        {
            if(!isPressed)
            {
                tmpText = tmp.GetComponent<Player>().showPopUp("10", Color.yellow, true, true);
                StartCoroutine(countDown());
                isPressed = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isEnabled = true;
            tmp = other.gameObject;
            tmp.GetComponent<Player>().showPopUp(" E ", Color.cyan, true);
        }
    }

    IEnumerator countDown()
    {
        for (int i = 10 - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
            tmpText.GetComponent<TMPro.TextMeshProUGUI>().text = i.ToString();
        }

        GameManager.manager.LuzesAlarm.SetActive(true);
        GameManager.manager.PainelReturnedHome.GetComponent<PlayersWinner>().isReady = true;
        GameObject tmpReturn = tmp.GetComponent<Player>().showPopUp("RETORNE PARA O INICIO E ESCAPE COM VIDA!", Color.yellow, true, true);
        Destroy(tmpText, 1f);
        Destroy(tmpReturn, 8f);
        EnemyManager.manager.cooldown = 2f;
    }

}
