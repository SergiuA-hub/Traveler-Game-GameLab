using UnityEngine;

public class InternalTesting : MonoBehaviour
{
    public GameObject WinzardIntro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameServices.Instance.internalTesting)
        {
            WinzardIntro.SetActive(false);
        }
        else
        {
            WinzardIntro.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
