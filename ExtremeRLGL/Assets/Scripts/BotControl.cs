using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BotControl : MonoBehaviour
{
    public float speed;
    public double moveChance;
    public float moveDistanceMean;
    public float moveDistanceStd;
    private Collider collider;
    private Vector3 goal;

    // Start is called before the first frame update
    private void Awake()
    {
        collider = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider>();
        goal = new Vector3(
            Random.Range(collider.bounds.min.x, collider.bounds.max.x),
            Random.Range(collider.bounds.min.y, collider.bounds.max.y),
            Random.Range(collider.bounds.min.z, collider.bounds.max.z)
        );
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameStage == GameStage.Playing)
        {
            Vector3 direction = goal - transform.position;
            if (LightManager.RedlightAllOn())
            {
                System.Random random = new System.Random();
                if (random.NextDouble() > moveChance)
                {
                    transform.Translate(direction * GetRandomDistance(moveDistanceMean, moveDistanceStd));
                }
            }
            else
            {
                transform.Translate(direction * Time.deltaTime * speed);
            }
        }
    }

    private float GetRandomDistance(double mean, double stdDev)
    {
        System.Random rand = new System.Random(); //reuse this if you are generating many
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                     System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (float)randNormal;
    }
}
