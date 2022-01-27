using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStage 
{
    Running0,
    Climbing,
    Running1,
    Jumping,
    Running2,
    Rowing,
    Running3,
    Finished
}

public class PlayerState : MonoBehaviour
{
    public int ID;
    public string username;
    public PlayerStage stage;
    public bool joined;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
