using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public Player_M player;
    public bool isTesting;

    public GameObject mainHUDUI;
    public GameObject welcomeScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            Debug.LogError("PLAYER NOT FOUND");

        mainHUDUI.SetActive(true);
        disableWelcomeScreen();
    }

    public void disableWelcomeScreen()
    {
        if (isTesting)
        {
            welcomeScreen.SetActive(false);
            player.UnRoot();
        }
        else
        {
            welcomeScreen.SetActive(true);
            player.Root();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
