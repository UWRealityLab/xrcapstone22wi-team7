using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class BotControl : MonoBehaviour
{
    public float speed;
    public double moveChance;
    public float moveDistanceMean;
    public float moveDistanceStd;
    private Collider goalCollider;
    private Vector3 goal;
    private bool moved;
    private PhotonView photonView;
    private PlayerInteraction playerInteraction;
    private Collider startLine;

    // Start is called before the first frame update
    private void Awake()
    {
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        goalCollider = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider>();
        if (goalCollider != null)
        {
            goal = new Vector3(
                Random.Range(goalCollider.bounds.min.x, goalCollider.bounds.max.x),
                Random.Range(goalCollider.bounds.min.y, goalCollider.bounds.max.y),
                Random.Range(goalCollider.bounds.min.z, goalCollider.bounds.max.z)
            );
        }
        moved = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startLine == null)
        {
            startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<Collider>();
            Debug.Log("Tried to find start line");
        }

        if (goalCollider == null)
        {
            goalCollider = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider>();
            if (goalCollider != null)
            {
                goal = new Vector3(
                    Random.Range(goalCollider.bounds.min.x, goalCollider.bounds.max.x),
                    Random.Range(goalCollider.bounds.min.y, goalCollider.bounds.max.y),
                    Random.Range(goalCollider.bounds.min.z, goalCollider.bounds.max.z)
                );
            }
        }
        else if (photonView.IsMine && GameManager.gameStage == GameStage.Playing)
        {
            Vector3 direction = goal - transform.position;
            if (LightManager.RedlightAllOn())
            {
                System.Random random = new System.Random();
                double currentP = random.NextDouble();
                if (currentP < moveChance && !moved)
                {
                    Debug.Log("Random Number we got is, " + currentP);
                    OnMoved();
                }
                moved = true;
            }
            else if (!playerInteraction.stopped)
            {
                transform.Translate(direction * Time.deltaTime * speed);
                moved = false;
            }
            else
            {
                moved = false;
            }

        }
    }

    private void OnMoved()
    {
        playerInteraction.stopped = true;
        StartCoroutine(MovePlayer());
        Debug.Log("Robot moved!");
    }

    IEnumerator MovePlayer()
    {
        yield return new WaitForSeconds(3.0f);
        Vector3 randomPoint = new Vector3(
            Random.Range(startLine.bounds.min.x, startLine.bounds.max.x),
            Random.Range(startLine.bounds.min.y, startLine.bounds.max.y),
            Random.Range(startLine.bounds.min.z, startLine.bounds.max.z)
        );
        playerInteraction.stopped = false;
        gameObject.transform.position = randomPoint;
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
