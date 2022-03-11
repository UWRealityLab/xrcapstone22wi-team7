using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class BotControl : MonoBehaviour
{
    public float speedMean;
    public float speedStd;
    private float speed;
    public double moveChance;
    public float moveDistanceMean;
    public float moveDistanceStd;
    private Collider targetCollider;
    private Collider goalCollider;
    private Collider[] checkpointColliders;
    public int checkpointIdx = 0;
    private Vector3 target;
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
        checkpointIdx = 0;
        speed = GetRandomDistance(speedMean, speedStd);
        goalCollider = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider>();
        checkpointColliders = GameObject.Find("Checkpoints").GetComponentsInChildren<Collider>();
        Debug.Log("Coliders lenght = " + checkpointColliders.Length);
        moved = false;
    }

    /*
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
        checkpointColliders = GameObject.Find("Checkpoints").GetComponentsInChildren<Collider>();
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!");
        Debug.Log("Lenght = " + checkpointColliders.Length);
        moved = false;
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (startLine == null)
        {
            startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<Collider>();
            Debug.Log("Tried to find start line");
        }


        if (checkpointColliders == null || goalCollider == null)
        {
            goalCollider = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider>();
            checkpointColliders = GameObject.Find("Checkpoints").GetComponentsInChildren<Collider>();
            
        }
        else if (PhotonNetwork.IsMasterClient && GameManager.gameStage == GameStage.Playing)
        {
            targetCollider = checkpointIdx < checkpointColliders.Length ? checkpointColliders[checkpointIdx] : goalCollider;
            target = new Vector3(
                Random.Range(targetCollider.bounds.min.x, targetCollider.bounds.max.x),
                Random.Range(targetCollider.bounds.min.y, targetCollider.bounds.max.y),
                Random.Range(targetCollider.bounds.min.z, targetCollider.bounds.max.z)
            );

            Vector3 direction = target - transform.position;
            direction.Normalize();
            if (LightManager.RedlightAllOn())
            {
                System.Random random = new System.Random(gameObject.GetInstanceID());
                double currentP = random.NextDouble();
                if (currentP < moveChance && !moved)
                {
                    Debug.Log("Robot moved");
                    OnMoved();
                }
                moved = true;
            }
            else if (!playerInteraction.stopped)
            {
                transform.Translate(direction * Time.deltaTime * speed);
                if (checkpointIdx == 0)
                {
                    System.Random random = new System.Random(gameObject.GetInstanceID());
                    double currentP = random.NextDouble();
                    if (currentP < 0.005)
                    {
                        GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
                    }
                }
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
        checkpointIdx = 0;
    }

    private float GetRandomDistance(double mean, double stdDev)
    {
        System.Random rand = new System.Random(gameObject.GetInstanceID()); //reuse this if you are generating many
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                     System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (float)randNormal;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.transform.CompareTag("Checkpoint")) 
        {
            checkpointIdx += 1;
            if (checkpointIdx == 1)
                GetComponent<Rigidbody>().useGravity = false;
            if (checkpointIdx == 3)
                GetComponent<Rigidbody>().useGravity = true;
            Debug.Log("checkpoint change triggered!");
        }
    }
}
