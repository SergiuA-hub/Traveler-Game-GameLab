using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private HUDScript hud;
    private void Awake()
    {
        hud = transform.parent.GetComponent<HUDScript>();
    }
    public void ReturnToGame()
    {
       
        hud.player.UnRoot();
        hud.isGamePaused = false;
        transform.gameObject.SetActive(false);
        Time.timeScale = 1f;



    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        //Main Menu Scene needs to be set
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
