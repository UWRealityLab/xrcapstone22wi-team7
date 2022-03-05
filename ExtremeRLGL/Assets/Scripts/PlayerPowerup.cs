using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PowerUpType
{
    SPEED_UP,
    UNSTOPPABLE,
    INVINCIBLE
}

public class PlayerPowerup : MonoBehaviour
{
    Dictionary<PowerUpType, PowerUpState> powerupStates;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        if (photonView.IsMine)
        {
            powerupStates = new Dictionary<PowerUpType, PowerUpState>();
            powerupStates.Add(PowerUpType.SPEED_UP, new PowerUpState(PowerUpType.SPEED_UP));
            powerupStates.Add(PowerUpType.UNSTOPPABLE, new PowerUpState(PowerUpType.UNSTOPPABLE));
            powerupStates.Add(PowerUpType.INVINCIBLE, new PowerUpState(PowerUpType.INVINCIBLE));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPowerUp(PowerUpType type)
    {
        if (photonView.IsMine)
        {
            powerupStates[type].StartEffect(this);
        }
    }

    public void ClearPowerUps(PowerUpType type) 
    {
        if (photonView.IsMine)
        {
            foreach (PowerUpType pair in powerupStates.Keys)
            {
                powerupStates[pair].StopEffect(this);
            }
        }
    }


    public bool isActivate(PowerUpType type)
    {
        if (photonView.IsMine)
        {
            return powerupStates[type].activate;
        }
        else
        {
            return false;
        }
    }


    private class PowerUpState 
    {
        PowerUpType type;
        public bool activate;
        public float time;
        private IEnumerator thread;
        public PowerUpState(PowerUpType type)
        {
            this.type = type;
            switch (type)
            {
                case PowerUpType.SPEED_UP:
                    time = 5.0f;
                    break;
                case PowerUpType.UNSTOPPABLE:
                    time = 20.0f;
                    break;
                case PowerUpType.INVINCIBLE:
                    time = 10.0f;
                    break;
            }
            thread = EffectDying(time);
            Debug.Log("PowerUpState Initialized");
            Debug.Log(thread);
        }

        public void StartEffect(PlayerPowerup script)
        {
            script.StopCoroutine(thread);
            activate = true;
            script.StartCoroutine(thread);
            Debug.Log("Powerup: " + type + " start taking effect");
        }

        public void StopEffect(PlayerPowerup script)
        {
            script.StopCoroutine(thread);
            activate = false;
            Debug.Log("Powerup: cleaned.");
        }

        IEnumerator EffectDying(float time)
        {
            yield return new WaitForSeconds(time);
            activate = false;
            Debug.Log("Powerup: " + type + " stop taking effect");
        }
    }

}
