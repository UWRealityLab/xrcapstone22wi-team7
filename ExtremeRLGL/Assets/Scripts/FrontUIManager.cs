using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrontUIManager : MonoBehaviour
{

    public static FrontUIManager frontUIManager;
    [SerializeField]
    public TextMeshProUGUI warning;
    [SerializeField]
    public TextMeshProUGUI timeLeft;

    private void Awake()
    {
        frontUIManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        warning.gameObject.SetActive(false);
        timeLeft.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft.text = (int)GameManager.timeLeft + "s";
    }

    public void ShowTimer()
    {
        timeLeft.gameObject.SetActive(true);
        Debug.Log("Showing w");
    }

    public void ShowWarning()
    {
        warning.gameObject.SetActive(true);
        Debug.Log("Showing warning");
    }

    public void SetWarning(string t)
    {
        warning.text = t;
    }

    public void HideTimer()
    {
        timeLeft.gameObject.SetActive(false);
    }

    public void HideWarning()
    {
        warning.gameObject.SetActive(false);
    }

  
}
