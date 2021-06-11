using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnumCollectables
{
        LIFE,
        DAMAGE,
        SPEED,
        ATKSPEED
}
public class Collectable : MonoBehaviour
{

    public EnumCollectables myEnum = EnumCollectables.LIFE;
   

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            switch(myEnum)
            {
                case EnumCollectables.LIFE:
                    {
                        if(other.GetComponent<Player>().maxLife >= ( other.GetComponent<Player>().currentLife - 15 ))
                        {
                            other.GetComponent<Player>().currentLife += 15;
                            other.GetComponent<Player>().slider_Health.value += 15; 
                        } 
                        else
                        {
                            other.GetComponent<Player>().currentLife = other.GetComponent<Player>().maxLife;
                            other.GetComponent<Player>().slider_Health.value = other.GetComponent<Player>().maxLife;
                        }
                        break;
                    }

                case EnumCollectables.SPEED:
                    {
                        GameManager.manager.ShowOnUI("SPEED");
                        other.GetComponent<PlayerMovement>().NormalizeStatus();
                        break;
                    }


                case EnumCollectables.DAMAGE:
                    {
                        GameManager.manager.ShowOnUI("DAMAGE");
                        other.GetComponentInChildren<Gun>().BuffDamageGun();
                        break;
                    }

                case EnumCollectables.ATKSPEED:
                    {
                        GameManager.manager.ShowOnUI("ATKSPEED");
                        other.GetComponentInChildren<Gun>().BuffAttackSpeedGun();
                        break;
                    }

            }

            Destroy(this.gameObject, .1f);

           
        }
    }

    


}
